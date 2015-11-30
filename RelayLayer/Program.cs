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
                    Output.SendToWebService(datas);
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
                    DataModel data1 = new DataModel() {Light = 200, Temperature = 220, SensorName = "FAKEROOM", TimeOfData = DateTime.Now};
                    DataModel data2 = new DataModel() { Light = 200, Temperature = 220, SensorName = "FAKEROOM", TimeOfData = DateTime.Now };
                    DataModel[] sensorData = new DataModel[2] {data1, data2};
                    //DataModel[] sensorData = inp.FakeData();

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
                        DataModel[] dataToSend = DataProcessor.AverageDataSet(dataSet);
                        Output.SendToWebService(dataToSend);
                        newSecond = true;
                    }
                    dataSet.Add(sensorData);
                }
            }
        }
    }
}
