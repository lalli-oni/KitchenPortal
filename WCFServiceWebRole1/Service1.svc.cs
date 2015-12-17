using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WCFServiceWebRole1.DBAImplementations;
using WCFServiceWebRole1.Model;
using DataModel = WCFServiceWebRole1.Model.DataModel;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        //Cache for the last data retrieved.
        private DataModel roomDataCache;
        //Time of cache creation
        private DateTime timeOfRoomCacheCreation;
        //Cache for the last data retrieved.
        private DataModel ovenDataCache;
        //Time of cache creation
        private DateTime timeOfOvenCacheCreation;

        /// <summary>
        /// Is not used. OperationContract is commented out.
        /// Only used for testing purposes
        /// </summary>
        /// <param name="data">Datamodel conforming to the format needed for the database</param>
        /// <returns></returns>
        public bool SaveDataAsync(DataModel data)
        {
            return SQLImplementation.GetInstance.InsertSensorData(data).Result;
        }
        

        /// <summary>
        /// Gets the latest Room temperature and light values
        /// </summary>
        /// <returns>[0] = Temperature, [1] = Light</returns>
        public int[] GetLastRoomData()
        {
                DataModel roomData = SQLImplementation.GetInstance.RetrieveLastRoomData();
                roomDataCache = roomData;
                int[] roomDataNumbers = { roomData.Temperature, roomData.Light };
                return roomDataNumbers;
        }

        /// <summary>
        /// Gets the latest Oven temperature value
        /// </summary>
        /// <returns>Latest Oven temperature</returns>
        public int GetLastOvenData()
        {
                DataModel ovenData = SQLImplementation.GetInstance.RetrieveLastOvenData();
                ovenDataCache = ovenData;
                int ovenDataNumbers = ovenData.Temperature;
                return ovenDataNumbers;
        }

    #region Start Reminder
        /// <summary>
        /// Starts a reminder that keeps on checking the last sensor data
        /// in the database to see if the desired temperature has been reached
        /// returns true when the temperature is reached
        /// NOTES: No time-out, is written on the assumption that there is only one reminder
        /// NOTES: Async implementation, is called by a method that uses these results in EndReminderAsync(result)
        /// </summary>
        /// <param name="temperature">The temperature that the user wants to be notified about if reached</param>
        /// <returns>True if the temperature is reached, false if something went wrong or the reminder is cancelled</returns>
        public IAsyncResult BeginStartReminder(int desiredTemperature, AsyncCallback callback, object asyncState)
        {
            ReminderModel activeReminder = new ReminderModel() { DesiredTemperature = desiredTemperature, TimeOfStart = DateTime.Now };
            CompletedAsyncResult<bool> result = new CompletedAsyncResult<bool>(SQLImplementation.GetInstance.CheckTemperatureReminder(desiredTemperature));
            return result;
        }

        public bool EndStartReminder(IAsyncResult r)
        {
            CompletedAsyncResult<bool> result = r as CompletedAsyncResult<bool>;
            return result.Data;
        }
    #endregion

    #region Stop Reminder
        /// <summary>
        /// Stops all reminders if there are any in the activeReminders
        /// in the database to see if the desired temperature has been reached
        /// returns true when the temperature is reached
        /// NOTES: No time-out, is written on the assumption that there is only one reminder
        /// NOTES: Async implementation, is called by a method that uses these results in EndReminderAsync(result)
        /// </summary>
        /// <returns>True if the temperature is reached, false if something went wrong or the reminder is cancelled</returns>
        public IAsyncResult BeginStopReminder(AsyncCallback callback, object asyncState)
        {
            string conString = SQLImplementation.GetInstance.ConnectionString;
            return new CompletedAsyncResult<bool>(SQLImplementation.GetInstance.RemindersClient.CancelReminder(conString));
        }

        public bool EndStopReminder(IAsyncResult r)
        {
            CompletedAsyncResult<bool> result = r as CompletedAsyncResult<bool>;
            return result.Data;
        }
    #endregion

        // Simple async result implementation.
        class CompletedAsyncResult<T> : IAsyncResult
        {
            T data;

            public CompletedAsyncResult(T data)
            { this.data = data; }

            public T Data
            { get { return data; } }

            #region IAsyncResult Members
            public object AsyncState
            { get { return (object)data; } }

            public WaitHandle AsyncWaitHandle
            { get { throw new Exception("The method or operation is not implemented."); } }

            public bool CompletedSynchronously
            { get { return true; } }

            public bool IsCompleted
            { get { return true; } }
            #endregion
        }
    }
}
