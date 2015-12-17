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
            Service1Client client = new Service1Client();
            AzureWebService.DataModel ovenData = new AzureWebService.DataModel() { SensorName = secData[0].SensorName, Light = secData[0].Light, Temperature = secData[0].Temperature, TimeOfData = secData[0].TimeOfData };
            client.SaveData(ovenData);
            Console.WriteLine("Data sent: " + secData[0].ToString());
            AzureWebService.DataModel roomData = new AzureWebService.DataModel() { SensorName = secData[1].SensorName, Light = secData[1].Light, Temperature = secData[1].Temperature, TimeOfData = secData[1].TimeOfData };
            client.SaveData(roomData);
            Console.WriteLine("Data sent: " + secData[1].ToString());
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
            AzureWebService.DataModel data = new AzureWebService.DataModel() { SensorName = secData.SensorName, Light = secData.Light, Temperature = secData.Temperature, TimeOfData = secData.TimeOfData };
            client.SaveData(data);
        }
    }
}
