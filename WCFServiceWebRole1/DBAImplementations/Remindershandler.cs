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