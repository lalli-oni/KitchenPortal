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
        //Used to see if there are any reminders active so that multiple reminders are not created.
        private List<ReminderModel> activeReminders;
        //Cache for the last data retrieved.
        private DataModel roomDataCache;
        //Time of cache creation
        private DateTime timeOfRoomCacheCreation;
        //Cache for the last data retrieved.
        private DataModel ovenDataCache;
        //Time of cache creation
        private DateTime timeOfOvenCacheCreation;


        public Service1()
        {
            activeReminders = new List<ReminderModel>();
        }

        public bool SaveDataAsync(DataModel data)
        {
            return SQLImplementation.GetInstance.InsertSensorData(data).Result;
        }

        /// <summary>
        /// Cancels the set reminder (written with the assumption of only 1 reminder
        /// </summary>
        /// <returns>A boolean if the cancellation succeeded or not</returns>
        public async Task<bool> CancelReminderAsync()
        {
            if (activeReminders.Any())
            {
                try
                {
                    return await SQLImplementation.GetInstance.CancelReminder();
                }
                catch (Exception e)
                {
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the latest Room temperature and light values
        /// </summary>
        /// <returns>[0] = Temperature, [1] = Light</returns>
        public int[] GetLastRoomData()
        {
            //5 Seconds
            TimeSpan cacheDuration = new TimeSpan(0,0,5);
            DataModel roomData = null;
            if (roomDataCache == null)
            {
                roomDataCache = new DataModel();
            }

            if (roomDataCache.Temperature == null || DateTime.Now > timeOfRoomCacheCreation.Add(cacheDuration))
            {
                roomData = SQLImplementation.GetInstance.RetrieveLastRoomData();
                roomDataCache = roomData;
                timeOfRoomCacheCreation = DateTime.Now;
                int[] roomDataNumbers = { roomData.Temperature, roomData.Light };
                return roomDataNumbers;
            }
            int[] cachedRoomDataNumbers = { roomDataCache.Temperature, roomDataCache.Light };
            return cachedRoomDataNumbers;
        }

        /// <summary>
        /// Gets the latest Oven temperature value
        /// </summary>
        /// <returns>Latest Oven temperature</returns>
        public int GetLastOvenData()
        {
            //5 Seconds
            TimeSpan cacheDuration = new TimeSpan(0, 0, 5);
            DataModel ovenData = null;
            if (ovenDataCache == null)
            {
                ovenDataCache = new DataModel();
            }

            if (ovenDataCache.Temperature == null || DateTime.Now > timeOfOvenCacheCreation.Add(cacheDuration))
            {
                ovenData = SQLImplementation.GetInstance.RetrieveLastOvenData();
                ovenDataCache = ovenData;
                timeOfOvenCacheCreation = DateTime.Now;
                int ovenDataNumbers = ovenData.Temperature;
                return ovenDataNumbers;
            }
            int cachedOvenDataNumbers = ovenDataCache.Temperature;
            return cachedOvenDataNumbers;
        }


        /// <summary>
        /// Starts a reminder that keeps on checking the last sensor data
        /// in the database to see if the desired temperature has been reached
        /// returns true when the temperature is reached
        /// NOTES: No time-out, is written on the assumption that there is only one reminder
        /// NOTES: Async implementation, is called by a method that uses these results in EndReminderAsync(result)
        /// </summary>
        /// <param name="temperature">The temperature that the user wants to be notified about if reached</param>
        /// <returns>True if the temperature is reached, false if something went wrong or the reminder is cancelled</returns>
        public IAsyncResult BeginReminderAsync(int desiredTemperature, AsyncCallback callback, object asyncState)
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
                return new CompletedAsyncResult<bool>(false);
            }
            activeReminders.Add(new ReminderModel() { DesiredTemperature = desiredTemperature, TimeOfStart = DateTime.Now });
            CompletedAsyncResult<bool> result = new CompletedAsyncResult<bool>(SQLImplementation.GetInstance.CheckTemperatureReminder(desiredTemperature));
            return result;
        }

        public bool EndReminderAsync(IAsyncResult r)
        {
            CompletedAsyncResult<bool> result = r as CompletedAsyncResult<bool>;
            return result.Data;
        }

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
