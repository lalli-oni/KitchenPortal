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
        public DataModel StartFakingData()
        {
            bool done = false;
            while (!done)
            {
                //The emulation of sensor telemetry
                //TODO: Make this more realistic
                Random rng = new Random();
                DateTime currTime = DateTime.Now;
                int currLight = rng.Next(200,240);
                int currTemp = rng.Next(200,240);

                var templateBroadcast = "RoomSensor Broadcasting\r\n" +
                                                 "Location: Fake room\r\n" +
                                                 "Platform: Linux-3.12.28+-armv6l-with-debian-7.6\r\n" +
                                                 "Machine: armv6l\r\n" +
                                                 "Potentiometer(8bit): 129\r\n" +
                                                 "Light Sensor(8bit): "+ currLight +"\r\n" +
                                                 "Temperature(8bit): " + currTemp + "\r\n" +
                                                 "Movement last detected: " + DateTime.Now + "\r\n";
                //Console.WriteLine("Faking data.");
                //Console.WriteLine("Received broadcast from {0} :\n {1}\n",
                //"1.1.1.1",
                //broadCastFromTeacherLounge);
                DataModel data = new DataModel()
                {
                    Light = currLight,
                    Temperature = currTemp,
                    TimeOfData = currTime,
                    SensorName = "Fake Sensor"
                };
                return data;
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
    }
}
