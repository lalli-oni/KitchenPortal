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


        private const string connectionString =
            //  "Server=tcp:kitchenportaldb.database.windows.net,1433;Database=KitchenPortalDb;User ID=tomas@kitchenportaldb;Password={Password18};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            "Data Source=kitchenportaldb.database.windows.net;Initial Catalog=KitchenPortalDb;User ID=tomas;Password=Password18";
        
        public async Task<bool> InsertData(DataModel data)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string command = "INSERT INTO SensorData (sensorName, timeOfData, light, desiredTemp) VALUES (@sensorName, @timeOfData, @light, @desiredTemp)";
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

        public async Task<bool> CheckTemperatureReminder(int desiredTemp)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(SqlCommandBuilder.CreateSQLCommandGetLastOvenTemperature(), connection);
                //TODO: Error handling (timeout, cooling?...)

                connection.Open();
                //While the connection is open it checks if the desiredTemp is reached
                //When it is reached it closes the connection and returns true
                while (connection.State == ConnectionState.Open)
                {
                    //How long the thread is put to sleep at the end of the loop.
                    int checkInterval = 100;
                    try
                    {
                        object sqlResult = await sqlCommand.ExecuteScalarAsync();
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
                }
                return true;
            }
        }

        public DataModel RetrieveLastData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(SqlCommandBuilder.CreateSQLCommandGetLastOvenDataModel(), connection);
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
    }
}