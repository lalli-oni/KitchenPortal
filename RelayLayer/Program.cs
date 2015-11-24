using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RelayLayer
{
    class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            Input inp = new Input();
            Processor proc = new Processor();
            Output outp = new Output();
            string cmdInput = "";

            Task.Run(() =>
            {
                List<long[]> dataSet = new List<long[]>();
                while (true)
                {
                    long[] data = inp.StartFakingData().Result;
                    DateTime startSecond = DateTime.FromBinary(data[0]);
                    while (true)
                    {
                        DateTime currTime = DateTime.FromBinary(data[0]);
                        int i = DateTime.Compare(startSecond.AddSeconds(1), currTime);
                        if (dataSet.Count > 2 && DateTime.Compare(startSecond.AddSeconds(1), currTime) < 0)
                        {
                            long timeSum = 0;
                            long tempSum = 0;
                            long lightSum = 0;
                            int nrOfData = dataSet.Count;
                            foreach (long[] datas in dataSet)
                            {
                                timeSum += datas[0];
                                tempSum += datas[1];
                                lightSum += datas[2];
                            }
                            long[] second = { timeSum / nrOfData, tempSum / nrOfData, lightSum / nrOfData };

                            outp.sendToWebService(second);
                        }
                        dataSet.Add(data);
                    }
                }
            });

            while (cmdInput.ToLower() != "x")
            {
                Console.ReadLine();
            }
        }
    }
}
