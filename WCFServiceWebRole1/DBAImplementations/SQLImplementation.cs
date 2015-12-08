using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
            remindersClient = new Remindershandler();
        }
        #endregion

        private Remindershandler remindersClient;
        private const string connectionString =
            //  "Server=tcp:kitchenportaldb.database.windows.net,1433;Database=KitchenPortalDb;User ID=tomas@kitchenportaldb;Password={Password18};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            "Data Source=kitchenportaldb.database.windows.net;Initial Catalog=KitchenPortalDb;User ID=tomas;Password=Password18";

        public async Task<bool> InsertSensorData(DataModel data)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(SqlCommandBuilder.GetLastOvenData(), connection);
                //TODO: Error handling (timeout, cooling?...)
                DataModel sqlOvenData = new DataModel();

                connection.Open();
                try
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                sqlOvenData.Light = reader.GetInt32(3);
                                sqlOvenData.Temperature = reader.GetInt32(4);
                            }
                        }
                        else
                        {
                            throw new Exception("No records.");
                        }
                    }
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(SqlCommandBuilder.GetLastRoomData(), connection);
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
                            throw new Exception("No records.");
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

        public bool CheckTemperatureReminder(int desiredTemp)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
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
                        remindersClient.CreateReminder(connection, newReminder);
                    }
                    //How long the thread is put to sleep at the end of the loop.
                    int checkInterval = 100;
                    try
                    {
                        SqlCommand checkTemperature = new SqlCommand(SqlCommandBuilder.GetLastOvenTemperatureToday(), connection);
                        object sqlResult = checkTemperature.ExecuteScalar();
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
                            return true;
                        }
                    }
                    catch (NullReferenceException nullE)
                    {
                        connection.Close();
                        return false;
                    }
                    catch (Exception e)
                    {
                        connection.Close();
                        return false;
                    }
                    Thread.Sleep(checkInterval);
                    i++;
                } while (connection.State == ConnectionState.Open && remindersClient.CheckActiveReminder(connection));
                
                //TODO: Error handling (timeout, cooling?...)
                return false;
            }
        }
        

        public async Task<bool> CancelReminder()
        {
            try
            {
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}