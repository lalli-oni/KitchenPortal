﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Edm;

namespace RelayLayer
{
    class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Controls: 'F' for fake data, 'X' to quit");
            string cmdInput = "";
            while (cmdInput.ToLower() != "x")
            {
                cmdInput = Console.ReadLine();
                switch (cmdInput.ToLower())
                {
                    case "f":
                        Task.Run(() => StartFaking());
                        break;
                }
            }
        }

        /// <summary>
        /// Fakes data input, averages each second and sends to Web Service
        /// </summary>
        private static void StartFaking()
        {
            Input inp = new Input();
            while (true)
            {
                List<DataModel[]> dataSet = new List<DataModel[]>();
                DateTime startSecond = new DateTime(1990, 1, 1);
                bool newSecond = false;
                while (!newSecond)
                {
                    //Gets fake data
                    DataModel[] sensorData = inp.FakeData();

                    //Checks to see if any data has been collected in this second
                    if (!dataSet.Any())
                    {
                        //Sets the start of the second
                        startSecond = sensorData[0].TimeOfData;
                    }

                    //Checks if the dataset has enough data to average out and if one second has passed
                    if (dataSet.Count > 1 && DateTime.Compare(startSecond.AddSeconds(1), sensorData[0].TimeOfData) < 0)
                    {
                        //Averages the whole data set for oven and room data seperately
                        DataModel[] dataToSend = AverageDataSet(dataSet);
                        SendToWebService(dataToSend);
                        newSecond = true;
                    }
                    dataSet.Add(sensorData);
                }
            }
        }

        /// <summary>
        /// Sends the data average of a second to the Web Service on the Cloud
        /// The web service then updates the information in the database.
        /// </summary>
        /// <param name="secData">Holds the average data for each second, the time when it's gotten and the name of sensor</param>
        private static void SendToWebService(DataModel[] secData)
        {
            Console.WriteLine("Data sent: " + secData[0].ToString());
            //Console.WriteLine("Data sent: " + secData[1].ToString());
        }

        public static DataModel[] AverageDataSet(List<DataModel[]> dataSet)
        {
            int tempOvenSum = 0;

            int tempRoomSum = 0;
            int lightRoomSum = 0;

            int nrOfData = dataSet.Count;

            foreach (DataModel[] datas in dataSet)
            {
                tempOvenSum += datas[0].Temperature;

                tempRoomSum += datas[1].Temperature;
                lightRoomSum += datas[1].Light;
            }

            int avgOvenTemp = tempOvenSum / nrOfData;

            int avgRoomTemp = tempRoomSum / nrOfData;
            int avgRoomLight = lightRoomSum / nrOfData;

            DataModel averagedOvenData = new DataModel()
            {
                Light = 0,
                Temperature = avgOvenTemp,
                SensorName = dataSet[0][0].SensorName,
                TimeOfData = dataSet[0][0].TimeOfData
            };
            DataModel averagedRoomData = new DataModel()
            {
                Light = avgRoomTemp,
                Temperature = avgRoomLight,
                SensorName = dataSet[0][1].SensorName,
                TimeOfData = dataSet[0][1].TimeOfData
            };

            DataModel[] averagedData = new DataModel[2] {averagedOvenData, averagedRoomData};

            return averagedData;
        }
    }
}
