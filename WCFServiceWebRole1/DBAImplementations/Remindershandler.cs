using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WCFServiceWebRole1.Model;

namespace WCFServiceWebRole1.DBAImplementations
{
    public class Remindershandler
    {
        /// <summary>
        /// Checks whether there is an active reminder in the table
        /// </summary>
        /// <param name="activeConnection">The open connection to the database</param>
        /// <returns>Whether there is an active reminder or not</returns>
        public bool CheckActiveReminder(SqlConnection activeConnection)
        {
            SqlCommand sqlCommand = new SqlCommand("SELECT * from Reminders WHERE IsActive != 0", activeConnection);
            object sqlResult = sqlCommand.ExecuteScalar();
            if (sqlResult == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks whether there is an active reminder in the table
        /// </summary>
        /// <param name="conString">The connection string for the database</param>
        /// <returns>Whether there is an active reminder or not</returns>
        public bool CheckActiveReminder(string conString)
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT * from Reminders WHERE IsActive != 0", connection);
                object sqlResult = sqlCommand.ExecuteScalar();
                if (sqlResult == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates an active reminder in the Reminders table
        /// </summary>
        /// <param name="activeConnection">The open connection to the database</param>
        /// <param name="newReminder">Model of the new reminder to put into the database</param>
        public void CreateReminder(SqlConnection activeConnection, ReminderModel newReminder)
        {
            if (CheckActiveReminder(activeConnection))
            {
                throw new Exception("Trying to create a reminder while there is an active reminder! Not being worked in in current user stories: Contect Lárus Þór. Bring faxe konde");
            }
            string command =
                    "INSERT INTO Reminders (IsActive, ReminderStart, DesiredTemperature) VALUES (@isActive, @timeOfStart, @desiredTemp)";
            SqlCommand sqlCommand = new SqlCommand(command, activeConnection);

            sqlCommand.Parameters.AddWithValue("@isActive", true);
            sqlCommand.Parameters.AddWithValue("@timeOfStart", newReminder.TimeOfStart);
            sqlCommand.Parameters.AddWithValue("@desiredTemp", newReminder.DesiredTemperature);
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create new Reminder in Reminder table");
            }
        }

        /// <summary>
        /// Goes into the reminder database table and changes every active reminder to false
        /// </summary>
        /// <param name="conString">The connection string for the database</param>
        /// <returns>Whether it executed, doesn't say if it changed anything or not</returns>
        public bool CancelReminder(string conString)
        {
            using (SqlConnection connection = new SqlConnection(conString))
            {
                try
                {
                    connection.Open();
                    int deactivedReminders = 1;
                    do
                    {
                        SqlCommand command = new SqlCommand("UPDATE Reminders SET IsActive = 0 WHERE IsActive = 1", connection);
                        deactivedReminders = command.ExecuteNonQuery();
                    } while (deactivedReminders < 0);
                    return true;
                }
                catch (Exception e)
                {
                    connection.Close();
                    return false;
                }
            }
        }
    }
}