using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCFServiceWebRole1.Model
{
    public class ReminderModel
    {
        private int _desiredTemperature;
        private DateTime _timeOfStart;
        private string _userId;

        public int DesiredTemperature
        {
            get { return _desiredTemperature; }
            set { _desiredTemperature = value; }
        }

        public DateTime TimeOfStart
        {
            get { return _timeOfStart; }
            set { _timeOfStart = value; }
        }

        //Multiple users not yet implemented
        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
    }
}