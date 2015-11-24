using System;
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
                    DateTime startSecond = new DateTime(1990, 1, 1);
                    List<DataModel> dataSet = new List<DataModel>();
                    bool newSecond = false;
                    while (newSecond == false)
                    {
                        DataModel data = inp.StartFakingData();
                        if (dataSet.Count() == 0)
                        {
                            startSecond = data.TimeOfData;
                        }
                        if (dataSet.Count > 2 && DateTime.Compare(startSecond.AddSeconds(1), data.TimeOfData) < 0)
                        {
                            int tempSum = 0;
                            int lightSum = 0;
                            int nrOfData = dataSet.Count;
                            foreach (DataModel datas in dataSet)
                            {
                                tempSum += datas.Temperature;
                                lightSum += datas.Light;
                            }
                            DataModel second = new DataModel()
                            {
                                SensorName = data.SensorName,
                                TimeOfData = startSecond,
                                Temperature = tempSum/nrOfData,
                                Light = lightSum/nrOfData
                            };

                            sendToWebService(second);
                            newSecond = true;
                        }
                        dataSet.Add(data);
                    }
                }
        }

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
}
