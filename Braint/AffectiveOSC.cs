using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpOSC;
using System.Threading;

namespace Braint
{


    public class AffectiveOSC
    {

        UDPSender sender;

        public class AffectiveState
        {

            // "Timestamp,EmoState_Timestamp,BoredomScore,ExcitementShortScore,FrustrationScore," +
            // " MediationScore,ValenceScore,ExcitementLongShort,"

            public static string BOREDOM_ADDR = "/Affective/Boredom";
            public static string EXCITEMENT_ADDR = "/Affective/Excitement";
            public static string FRUSTRATION_ADDR = "/Affective/Frustration";
            public static string MEDIATION_ADDR = "/Affective/Mediation";
            public static string VALENCE_ADDR = "/Affective/Valence";
            public static string EXCITEMENT_LONG_ADDR = "/Affective/ExcitementLong";

            public static string AFFECTIVE_STATE_ADDR = "/emoengine/affective";

            // todo model params???
            public double boredom { get; set; }
            public double excitement { get; set; }
            public double frustration { get; set; }
            public double mediation { get; set; }
            public double valence { get; set; }
            public double excitementLongTerm { get; set; }

            public float emotivTimeStamp { get; set; }


            public OscMessage toOSCMesseage()
            {
                OscMessage affectiveMSG = new OscMessage(AffectiveState.AFFECTIVE_STATE_ADDR, "emotivTimeStamp, boredom, excitement, frustration, mediation, valence, excitementLongTerm",
                emotivTimeStamp,
                boredom,
                excitement,
                frustration,
                mediation,
                valence,
                excitementLongTerm);

                return affectiveMSG;
            }
        }


        public AffectiveOSC(String address, int port)
        {
            sender = new SharpOSC.UDPSender("127.0.0.1", 12000);

        }
        private double lasteTimeStamp = 0;


        public void sendAffectiveStateData(AffectiveState affectiveState, bool isSimulated)
        {

            sender.Send(affectiveState.toOSCMesseage());

            if (isSimulated)
            {
                double waitTimeInSeconds;
                if (lasteTimeStamp == 0)
                    lasteTimeStamp = affectiveState.emotivTimeStamp;
                else if (lasteTimeStamp != affectiveState.emotivTimeStamp)
                {
                    waitTimeInSeconds = affectiveState.emotivTimeStamp - lasteTimeStamp;
                    lasteTimeStamp = affectiveState.emotivTimeStamp;
                    double timeInMS = waitTimeInSeconds * 1000.0;

                    int sleepTime = Convert.ToInt32(timeInMS);

                    Thread.Sleep(sleepTime);

                }





            }

        }



    }
}
