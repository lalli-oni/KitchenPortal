using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RelayLayer
{
    /// <summary>
    /// Works with all the Inputs (sensors and fake data)
    /// </summary>
    public class Input
    {
        //The port that the broadcast is on [hard-coded]
        private const int listenPort = 7000;

        //Sensors
        private bool _isOvenSensorOn;
        private bool _isOvenOn;
        private bool _isRoomSensorOn;

        //SensorValues
        private int _ovenSensorTemp;
        private object ovenTempLock = new object();
        private int _roomSensorTemp;
        private object roomTempLock = new object();
        private int _roomSensorLight;

        public int RoomSensorTemp
        {
            get {
                lock (roomTempLock)
                    {
                        return _roomSensorTemp;
                    }
                }
            set { _roomSensorTemp = value; }
        }

        public int OvenSensorTemp
        {
            get
            {
                lock (ovenTempLock)
                {
                    return _ovenSensorTemp;
                }
            }
            set { _ovenSensorTemp = value; }
        }

        /// <summary>
        /// Starts listening for the broadcast from the Rasberry Pi
        /// </summary>
        public DataModel StartRoomListener()
        {
            bool done = false;
            //Starts listening on the specific port
            UdpClient listener = new UdpClient(listenPort);
            //Finds any IPAdrress broadcasting on the specific port.
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            //Starts listening or catches any exeption and closes the listener.
            try
            {
                Console.WriteLine("Waiting for broadcast");
                //Starts the listening, stops here if nothing is received
                byte[] bytes = listener.Receive(ref groupEP);
                //Gets a string out of the ASCII bytes received
                string results = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                //Makes a regular expression matching any 3 number digits in a row.
                MatchCollection resultMatches = Regex.Matches(results, @"\d{3}");
                //Assigns the 2nd match to be the sensor light (1st is potentiometer)
                byte lightByteValue = Convert.ToByte(resultMatches[1].Value);
                int lightResult = Convert.ToInt16(lightByteValue);
                //Assigns the 3rd match to be the sensor temperature
                byte tempByteValue = Convert.ToByte(resultMatches[2].Value);
                int tempResult = Convert.ToInt16(tempByteValue);

                //Check to see if lightResults are valid (0 - 300)
                //Writes a Trace message
                //TODO: LOG THIS EVENT
                if (lightResult < 0 || lightResult > 300)
                {
                    Trace.Write(DateTime.Now.ToString() + $"Invalid data received: {lightResult}");
                }

                //Check to see if tempResult are valid (-10 - 50)
                //Writes a Trace message
                //TODO: LOG THIS EVENT
                if (tempResult < -10 || tempResult > 50)
                {
                    Trace.Write(DateTime.Now.ToString() + $"Invalid data received: {tempResult}");
                }

                //Puts the results into a DataModel for each Sensor Data
                DataModel sensorData = new DataModel()
                {
                    Light = lightResult,
                    SensorName = "Room",
                    Temperature = tempResult,
                    TimeOfData = DateTime.Now
                };
                return sensorData;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
            return null;
        }

        #region Faking
        /// <summary>
        /// Starts giving fake data, for use within development while sensor is not running
        /// </summary>
        public DataModel[] FakeData()
        {
            bool done = false;
            Task.Run(() => StartFakeRoomSensor());
            while (!done)
            {
                DateTime currTime = DateTime.Now;
                #region OvenFaking
                Random rng = new Random();
                int currLight = 0;
                int currTemp = rng.Next(200,240);
                DataModel ovenData = new DataModel()
                {
                    Light = currLight,
                    Temperature = currTemp,
                    TimeOfData = currTime,
                    SensorName = "Fake Oven Sensor"
                };
                #endregion
                #region RoomFaking
                DataModel roomData = new DataModel()
                {
                    Light = _roomSensorLight,
                    Temperature = RoomSensorTemp,
                    TimeOfData = currTime,
                    SensorName = "Fake Room Sensor"
                };
                #endregion

                DataModel[] datas = new DataModel[2] {ovenData, roomData};
                return datas;
            }
            return null;
        }

        /// <summary>
        /// Starts emulating reception of data from a sensor in an ordinary room.
        /// Has fluctuating room temperature (21.5°C average)
        /// </summary>
        public void StartFakeRoomSensor()
        {
            _isRoomSensorOn = true;
            const int avgTemp = 215;
            int avgLightOn = 230;
            int avgLightOff = 10;
            bool lightsOn = true;
            int currentTemp = avgTemp;
            int currentLight = avgLightOn;
            Random rng = new Random();
            while (_isRoomSensorOn)
            {
                int roll = rng.Next(1000);
                if (roll < 140)
                {
                    currentTemp += rng.Next(-5,5);
                }
                if (roll < 4)
                {
                    if (lightsOn)
                    {
                        currentLight = avgLightOff;
                        lightsOn = false;
                    }
                    else
                    {
                        currentLight = avgLightOn;
                        lightsOn = true;
                    }
                    currentLight += rng.Next(-3, 3);
                }
                RoomSensorTemp = currentTemp;
                _roomSensorLight = currentLight;
            }
        }

        /// <summary>
        /// Starts heating up the fake oven
        /// </summary>
        public void StartFakeOvenSensor()
        {
            _isOvenOn = true;
            const int avgTemp = 22;
            int light  = 1;
            bool lightsOn = true;
            OvenSensorTemp = avgTemp;
            int maximumHeat = 500;
            int interval = 9000;
            while (_isOvenOn)
            {
                OvenSensorTemp = OvenSensorTemp + 1;
                interval = OvenSensorTemp*(500/OvenSensorTemp);
                Thread.Sleep(interval);
            }
        }
        #endregion
    }
}
