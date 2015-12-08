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
        private List<ReminderModel> _activeReminders;

        public List<ReminderModel> ActiveReminders
        {
            get { return _activeReminders; }
            set { _activeReminders = value; }
        }

        public Remindershandler()
        {
            ActiveReminders = new List<ReminderModel>();
        }

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
    }
}