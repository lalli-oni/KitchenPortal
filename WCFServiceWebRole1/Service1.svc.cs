using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataTableStorage;
using DataTableStorage.Model;
using WCFServiceWebRole1.DBAImplementations;
using WCFServiceWebRole1.Model;
using DataModel = WCFServiceWebRole1.Model.DataModel;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private List<ReminderModel> activeReminders;
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public bool SaveDataAsync(DataModel data)
        {                    
            SQLImplementation sqlClient = new SQLImplementation();
            return sqlClient.InsertData(data).Result;
        }


        

        public bool SetReminderAsync(int temperature)
        {
            if (activeReminders == null)
            {
                activeReminders = new List<ReminderModel>();
            }
            if (activeReminders.Any())
            {
                //If there are any active reminders it will return false.
                //This means that the web browser will throw an exception that the reminder failed
                //Even though the reminder might be running in another thread and will just discard it's return value
                activeReminders.Clear();
                return false;
            }
            activeReminders.Add(new ReminderModel() { DesiredTemperature = temperature, TimeOfStart = DateTime.Now});
            SQLImplementation sqlClient = new SQLImplementation();
            var result = sqlClient.CheckTemperatureReminder(temperature).Result;
            return result;
        }
    }
}
