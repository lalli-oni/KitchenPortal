using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFServiceWebRole1.Model
{
    public class DataModel
    {
        private DateTime _timeOfData;
        private int _temperature;
        private int _light;
        private string _sensorName;

        public string SensorName
        {
            get { return _sensorName; }
            set { _sensorName = value; }
        }

        public DateTime TimeOfData
        {
            get { return _timeOfData; }
            set { _timeOfData = value; }
        }

        public int Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        public int Light
        {
            get { return _light; }
            set { _light = value; }
        }

        public override string ToString()
        {
            return "Sensor Name: " + SensorName + " Time: " + TimeOfData + " Temperature: " + Temperature + " Light: " + Light;
        }
    }
}