using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WCFServiceWebRole1.Model;

namespace WCFServiceWebRole1.DBAImplementations
{
    public class SQLImplementation : DBInterface.DBInterface
    {
        #region Singleton Implementation
        public static SQLImplementation _instance;
        
        /// <summary>
        /// Get accessor to this class.
        /// </summary>
        public static SQLImplementation GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SQLImplementation();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Private constructor, so that the only way to access this class is through GetInstance access field
        /// </summary>
        private SQLImplementation()
        {
            RemindersClient = new Remindershandler();
            ConnectionString =
                "Data Source=kitchenportaldb.database.windows.net;Initial Catalog=KitchenPortalDb;User ID=tomas;Password=Password18";
        }
        #endregion

        private Remindershandler _remindersClient;
        private string _connectionString;

        public Remindershandler RemindersClient
        {
            get { return _remindersClient; }
            set { _remindersClient = value; }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        

        public async Task<bool> InsertSensorData(DataModel data)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string command =
                    "INSERT INTO SensorData (sensorName, timeOfData, light, desiredTemp) VALUES (@sensorName, @timeOfData, @light, @desiredTemp)";
                SqlCommand sqlCommand = new SqlCommand(command, connection);

                sqlCommand.Parameters.AddWithValue("@sensorName", data.SensorName);
                sqlCommand.Parameters.AddWithValue("@timeOfData", data.TimeOfData);
                sqlCommand.Parameters.AddWithValue("@light", data.Light);
                sqlCommand.Parameters.AddWithValue("@desiredTemp", data.Temperature);
                try
                {
                    connection.Open();
                    await sqlCommand.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    return false;
                }
                connection.Close();
            }
            return true;
        }

        public DataModel RetrieveLastOvenData()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(SqlCommandBuilder.GetLastOvenTemperatureToday(), connection);
                //TODO: Error handling (timeout,no data, midnight, cooling?...)
                DataModel sqlOvenData = new DataModel();

                connection.Open();
                try
                {
                    object results = sqlCommand.ExecuteScalar();
                    sqlOvenData.Light = 0;
                    sqlOvenData.Temperature = Convert.ToInt16(results);
                    return sqlOvenData;
                }
                catch (Exception e)
                {
                    connection.Close();
                    return null;
                }
            }
        }

        public DataModel RetrieveLastRoomData()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(SqlCommandBuilder.GetLastRoomDataToday(), connection);
                //TODO: Error handling (timeout, cooling?...)
                DataModel sqlRoomData = new DataModel();

                connection.Open();
                try
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                sqlRoomData.Light = reader.GetInt32(3);
                                sqlRoomData.Temperature = reader.GetInt32(4);
                            }
                        }
                        else
                        {
                            throw new Exception("Nothing for today");
                        }
                    }
                    return sqlRoomData;
                }
                catch (Exception e)
                {
                    connection.Close();
                    return null;
                }
            }
        }

        /// <summary>
        /// Checks the latest temperature today until it reaches the desiredTemp
        /// or until the reminder in the database is changed to inactive by cancelReminder()
        /// </summary>
        /// <param name="desiredTemp">The temperature that will break the loop when reached</param>
        /// <returns></returns>
        public bool CheckTemperatureReminder(int desiredTemp)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                //While the connection is open it checks if the desiredTemp is reached
                //When it is reached it closes the connection and returns true
                //Runs once to create the Reminder in the database
                do
                {
                    if (i == 0)
                    {
                        ReminderModel newReminder = new ReminderModel() { DesiredTemperature = desiredTemp, TimeOfStart = DateTime.Now};
                        RemindersClient.CreateReminder(connection, newReminder);
                    }
                    //How long the thread is put to sleep at the end of the loop.
                    int checkInterval = 100;
                    try
                    {
                        SqlCommand checkTemperature = new SqlCommand(SqlCommandBuilder.GetLastOvenTemperatureToday(), connection);
                        object sqlResult = checkTemperature.ExecuteScalar();
                        if (sqlResult == null)
                        {
                            throw new Exception("No data for today");
                        }
                        int tempResult = -100;
                        tempResult = Convert.ToInt32(sqlResult);

                        if ((desiredTemp - tempResult) > 50)
                        {
                            checkInterval = 2000;
                        }
                        else if ((desiredTemp - tempResult) > 30)
                        {
                            checkInterval = 500;
                        }

                        if (tempResult >= desiredTemp)
                        {
                            connection.Close();
                            RemindersClient.CancelReminder(ConnectionString);
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        connection.Close();
                        RemindersClient.CancelReminder(ConnectionString);
                        return false;
                    }
                    Thread.Sleep(checkInterval);
                    i++;
                } while (connection.State == ConnectionState.Open && RemindersClient.CheckActiveReminder(connection));
                
                //TODO: Error handling (timeout, cooling?...)
                return false;
            }
        }
    }
}