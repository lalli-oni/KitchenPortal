using System;

namespace WCFServiceWebRole1.DBAImplementations
{
    public static class SqlCommandBuilder
    {
        public static string CreateSQLCommandGetLastOvenData()
        {
            DateTime date = DateTime.Now;
            //Creates a timespan to find yesterday and tomorrow
            TimeSpan dateSpan = new TimeSpan(1, 0, 0, 0);
            DateTime yesterday = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            yesterday = yesterday.Subtract(dateSpan);
            string yesterdaystring = yesterday.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime tomorrow = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
            tomorrow = tomorrow.AddDays(1);
            string tomorrowstring = tomorrow.ToString("yyyy-MM-dd HH:mm:ss");
            //The SQL command sent to the database manager
            return "SELECT * from SensorData WHERE sensorName = 'OVEN' ORDER BY timeOfData ASC";
        }

        public static string CreateSQLCommandGetLastOvenTemperatureToday()
        {
            DateTime date = DateTime.Now;
            //Creates a timespan to find yesterday and tomorrow
            TimeSpan dateSpan = new TimeSpan(1, 0, 0, 0);
            DateTime yesterday = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            yesterday = yesterday.Subtract(dateSpan);
            string yesterdaystring = yesterday.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime tomorrow = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
            tomorrow = tomorrow.AddDays(1);
            string tomorrowstring = tomorrow.ToString("yyyy-MM-dd HH:mm:ss");
            //The SQL command sent to the database manager
            return "SELECT TOP 1 temperature from SensorData WHERE sensorName = 'OVEN' AND  timeOfData >= '" + yesterdaystring + "' AND timeOfData < '" + tomorrowstring + "' ORDER BY timeOfData DESC";
        }

        public static string CreateSQLCommandGetLastRoomData()
        {
            DateTime date = DateTime.Now;
            //Creates a timespan to find yesterday and tomorrow
            TimeSpan dateSpan = new TimeSpan(1, 0, 0, 0);
            DateTime yesterday = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            yesterday = yesterday.Subtract(dateSpan);
            string yesterdaystring = yesterday.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime tomorrow = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
            tomorrow = tomorrow.AddDays(1);
            string tomorrowstring = tomorrow.ToString("yyyy-MM-dd HH:mm:ss");
            //The SQL command sent to the database manager
            return "SELECT * from SensorData WHERE sensorName = 'ROOM' ORDER BY timeOfData ASC";
        }
    }
}