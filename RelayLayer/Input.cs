using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private int _roomSensorTemp;
        private int _roomSensorLight;

        /// <summary>
        /// Starts listening for the broadcast from the Rasberry Pi
        /// </summary>
        public void StartListener()
        {
            bool done = false;
            //Starts listening on the specific port
            UdpClient listener = new UdpClient(listenPort);
            //Finds any IPAdrress broadcasting on the specific port.
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine("Received broadcast from {0} :\n {1}\n",
                        groupEP.ToString(),
                        Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }

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
                    Temperature = _roomSensorTemp,
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
        /// Starts emulating reception of data from a sensor in an oven.
        /// Just starts receiving from the sensor, use StartOven() to start "heating it up"
        /// </summary>
        public void StartOvenSensor()
        {
            throw new NotImplementedException("Talk to Lárus Þór. Bring him a candy, he might bite you if you don't.");
        }

        /// <summary>
        /// Starts "heating up" the virtual oven.
        /// Just starts "heating it up", use StartOvenSensor() to start receiving data.
        /// </summary>
        public void StartOven()
        {
            throw new NotImplementedException("Talk to Lárus Þór. Bring him a candy, he might bite you if you don't.");
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
                    currentLight += rng.Next(0, 2);
                }
                _roomSensorTemp = currentTemp;
                _roomSensorLight = currentLight;
            }
        }
    }
}
