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


        private String connectionString =
            //  "Server=tcp:kitchenportaldb.database.windows.net,1433;Database=KitchenPortalDb;User ID=tomas@kitchenportaldb;Password={Password18};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            "Data Source=kitchenportaldb.database.windows.net;Initial Catalog=KitchenPortalDb;User ID=tomas;Password=Password18";
        public void InsertData(DataModel data)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

            string command = "INSERT INTO SensorData (sensorName, timeOfData, light, temperature) VALUES (@sensorName, @timeOfData, @light, @temperature)";
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            
                sqlCommand.Parameters.AddWithValue("@sensorName", data.SensorName);
                sqlCommand.Parameters.AddWithValue("@timeOfData", data.TimeOfData);
                sqlCommand.Parameters.AddWithValue("@light", data.Light);
                sqlCommand.Parameters.AddWithValue("@temperature", data.Temperature);
            try
            {
               connection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            connection.Close();
            }

        }

        public bool CheckOvenTemp(int temperature)
        {
            DateTime date = DateTime.Now;
            //Creates a timespan to find yesterday and tomorrow
            TimeSpan dateSpan = new TimeSpan(1, 0, 0, 0);
            DateTime yesterday = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            yesterday =  yesterday.Subtract(dateSpan);
            string yesterdaystring = yesterday.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime tomorrow = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
            tomorrow = tomorrow.AddDays(1);
            string tomorrowstring =  tomorrow.ToString("yyyy-MM-dd HH:mm:ss");
            //The SQL command sent to the database manager
            string command = "SELECT TOP 1 temperature from SensorData WHERE sensorName = 'OVEN' AND  timeOfData >= '" + yesterdaystring + "' AND timeOfData < '" + tomorrowstring +  "' ORDER BY timeOfData DESC";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(command,connection);
                //TODO: Error handling (timeout, cooling?...)

                connection.Open();
                //While the connection is open it checks if the temperature is reached
                //When it is reached it closes the connection and returns true
                while (connection.State == ConnectionState.Open)
                {
                    try
                    {
                        int sqlResult = (int)sqlCommand.ExecuteScalar();

                        if (sqlResult >= temperature)
                        {
                            connection.Close();
                        }
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        return false;
                    }
                    Thread.Sleep(100);
                }
                return true;
            }
        }
    }
}