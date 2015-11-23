using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RelayLayer
{
    class Program
    {
        //The port that the broadcast is on [hard-coded]
        private const int listenPort = 7000;

        /// <summary>
        /// Starts listening for the broadcast from the Rasberry Pi
        /// </summary>
        private static void StartListener()
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
        /// Entry point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            StartListener();
            Console.ReadLine();
        }
    }
}
