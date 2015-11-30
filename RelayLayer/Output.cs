using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RelayLayer.AzureWebService;
using RelayLayer.Model;

namespace RelayLayer
{
    static class Output
    {
        /// <summary>
        /// Sends the data average of a second to the Web Service on the Cloud
        /// The web service then updates the information in the database.
        /// </summary>
        /// <param name="secData">Holds the average data for each second, the time when it's gotten and the name of sensor</param>
        public static void SendToWebService(DataModel[] secData)
        {
            foreach (DataModel model in secData)
            {
                //Check to see if lightResults are valid (0 - 300)
                //Writes a Trace message and doesn't send the data
                //TODO: LOG THIS EVENT
                if (model.Light < 0 || model.Light > 300)
                {
                    Trace.Write(DateTime.Now.ToString() + $"Invalid data received: {model.Light}");
                    return;
                }

                //Check to see if tempResult are valid (-10 [180] - 50 [250])
                //Writes a Trace message and doesn't send the data
                //TODO: LOG THIS EVENT
                if (model.Temperature < 190 || model.Temperature > 250)
                {
                    Trace.Write(DateTime.Now.ToString() + $"Invalid data received: {model.Temperature}");
                    return;
                }
            }

            Console.WriteLine("Data sent: " + secData[0].ToString());
            Console.WriteLine("Data sent: " + secData[1].ToString());
            Service1Client client = new Service1Client();
            AzureWebService.DataModel data = new AzureWebService.DataModel() { SensorName = secData[1].SensorName, Light = secData[1].Light, Temperature = secData[1].Temperature, TimeOfData = secData[1].TimeOfData };
            client.SaveData(data);
        }

        /// <summary>
        /// Sends the data average of a second to the Web Service on the Cloud
        /// The web service then updates the information in the database.
        /// </summary>
        /// <param name="secData">Holds the average data for each second, the time when it's gotten and the name of sensor</param>
        public static void SendToWebService(DataModel secData)
        {
            Console.WriteLine("Data sent: " + secData.ToString());
            Service1Client client = new Service1Client();
            //client.SaveData(ovenSensorToSend);
        }
    }
}
