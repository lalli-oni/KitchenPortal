using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Edm;
using RelayLayer.AzureWebService;

namespace RelayLayer
{
    class Program
    {
        private static bool _running = false;
        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Controls: 'F' for fake data, 'S' for teacher sensor and 'X' to quit");
            string cmdInput = "";
            while (cmdInput.ToLower() != "x")
            {
                cmdInput = Console.ReadLine();
                switch (cmdInput.ToLower())
                {
                    case "f":
                        _running = true;
                        Task.Run(() => StartFaking());
                        break;
                    case "s":
                        _running = true;
                        Task.Run(() => StartListening());
                        break;
                }
            }
        }

        private static void StartListening()
        {
            Input inp = new Input();
            Task.Run(() =>
            {
                while (_running = true)
                {
                    DataModel[] datas = new DataModel[2];
                    datas[1] = inp.StartRoomListener();
                    datas[0] = new DataModel()
                    {
                        Light = 0,
                        Temperature = 0,
                        SensorName = "Oven",
                        TimeOfData = DateTime.Now
                    };
                    SendToWebService(datas);
                }
            });
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
            });

        /// <summary>
        /// Sends the data average of a second to the Web Service on the Cloud
        /// The web service then updates the information in the database.
        /// </summary>
        /// <param name="secData">Holds the average data for each second, the time when it's gotten and the name of sensor</param>
        private static void sendToWebService(DataModel secData)
            {
            Console.WriteLine("Data sent: " + secData.ToString());
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
            Console.WriteLine("Data sent: " + secData[1].ToString());
            Service1Client client = new Service1Client();
            SensorEntity ovenSensorToSend = new SensorEntity() { ETag = "*", PartitionKey = "Oven", RowKey = secData[0].TimeOfData.ToString(), light = secData[0].Light, teperature = secData[0].Temperature };
            SensorEntity roomSensorToSend = new SensorEntity() { ETag = "*", PartitionKey = "Room", RowKey = secData[1].TimeOfData.ToString(), light = secData[1].Light, teperature = secData[1].Temperature };
            client.SaveData(ovenSensorToSend);
            Console.ReadLine();
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
