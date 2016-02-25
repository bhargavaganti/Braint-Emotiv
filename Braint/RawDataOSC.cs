using Emotiv;
using SharpOSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Braint
{
    public class RawDataOSC
    {



        private double minSmaple = 4115 - 150;
        private double maxSamlpe = 4115 + 150;
        private double minSCale = 0.0f;
        private double maxScale = 1.0f;


        UDPSender sender;




        public RawDataOSC(String address, int port)
        {
            sender = new SharpOSC.UDPSender("127.0.0.1", 12000);
        }


        //public void sendRawData(Dictionary<EdkDll.EE_DataChannel_t, double[]> data)
        //{
        //    int _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;






        //    for (int i = 0; i < _bufferSize; i++)
        //    {

        //        sending line by line
        //        OscMessage msg = new OscMessage("/rawEEG", "");
        //        msg.Arguments.Clear();
        //       // now write the data
        //        foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
        //        {

        //            msg.Arguments.Add(data[channel][i]);
        //        }
        //        sender.Send(msg);
        //    }





        //}
        /// <summary>
        /// Sends the raw data for each sample ("line by line") in the data object. The each sample is delayed according to the time stamps of the samples.
        /// </summary>
        /// <param name="data"></param>
        public void sendRawDataSimple(Dictionary<EdkDll.EE_DataChannel_t, double[]> data, string sendToOSCAdress)
        {
            int _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;



            double lastTime;


            for (int i = 0; i < _bufferSize; i++)
            {

                // sending line by line
                OscMessage msg = new OscMessage(sendToOSCAdress, "");
                msg.Arguments.Clear();
                // now write the data
                foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
                {

                    msg.Arguments.Add(data[channel][i]);
                }

                // try to send measseag with appropiate time intervals
                // to simulate it as real input as good as possible

                sender.Send(msg);

                if (i < _bufferSize - 1)
                {
                    double waitTimeInSeconds = data[EdkDll.EE_DataChannel_t.TIMESTAMP][i + 1] - data[EdkDll.EE_DataChannel_t.TIMESTAMP][i];


                    double timeInMS = waitTimeInSeconds * 1000.0;

                    int sleepTime = Convert.ToInt32(timeInMS);

                    Thread.Sleep(sleepTime);


                }

            }





        }

        public void sendRawDataNormalized(Dictionary<EdkDll.EE_DataChannel_t, double[]> data, string sendToOSCAdress)
        {

            int _bufferSize = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;

            for (int i = 0; i < _bufferSize; i++)
            {

                // sending line by line
                OscMessage msg = new OscMessage(sendToOSCAdress, "");
                msg.Arguments.Clear();
                // now write the data
                foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
                {


                    if (!channel.Equals(EdkDll.EE_DataChannel_t.COUNTER)
                        && !channel.Equals(EdkDll.EE_DataChannel_t.ES_TIMESTAMP)
                        && !channel.Equals(EdkDll.EE_DataChannel_t.FUNC_ID)
                         && !channel.Equals(EdkDll.EE_DataChannel_t.FUNC_VALUE)
                         && !channel.Equals(EdkDll.EE_DataChannel_t.GYROX)
                          && !channel.Equals(EdkDll.EE_DataChannel_t.GYROY)
                           && !channel.Equals(EdkDll.EE_DataChannel_t.INTERPOLATED)
                            && !channel.Equals(EdkDll.EE_DataChannel_t.MARKER)
                             && !channel.Equals(EdkDll.EE_DataChannel_t.RAW_CQ)
                              && !channel.Equals(EdkDll.EE_DataChannel_t.SYNC_SIGNAL)
                               && !channel.Equals(EdkDll.EE_DataChannel_t.TIMESTAMP))
                    {
                        double currentValue = data[channel][i];
                        //if (minSmaple > currentValue)
                        //    minSmaple = currentValue;
                        //else if (maxSamlpe < currentValue)
                        //    maxSamlpe = currentValue;

                        double normalized = getNormalizedValue(currentValue, maxSamlpe, minSmaple, maxScale, minSCale);
                        msg.Arguments.Add(normalized);
                    }
                    else {

                        msg.Arguments.Add(data[channel][i]);
                    }
                }

                // try to send measseag with appropiate time intervals
                // to simulate it as real input as good as possible

                sender.Send(msg);

                if (i < _bufferSize - 1)
                {
                    double waitTimeInSeconds = data[EdkDll.EE_DataChannel_t.TIMESTAMP][i + 1] - data[EdkDll.EE_DataChannel_t.TIMESTAMP][i];


                    double timeInMS = waitTimeInSeconds * 1000.0;

                    int sleepTime = Convert.ToInt32(timeInMS);

                    Thread.Sleep(sleepTime);


                }

            }

        }

        static public double getNormalizedValue(double value, double max, double min, double maxScaled, double minScaled)
        {
            return minScaled + (value - min) * (maxScaled - minScaled) / (max - min);
        }
    }
}
