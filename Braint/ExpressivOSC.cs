using SharpOSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emotiv;
using System.Threading;

namespace Braint
{
    public class ExpressivOSC
    {

        public class ExpressivState
        {

            public static string EXPRESSIV_STATE_ADDR = "/emoengine/expressiv";

            public string lowerFaceAction { get; set; }
            public float lowerFaceActionPower { get; set; }

            public string upperFaceAction { get; set; }
            public float upperFaceActionPower { get; set; }

            // EYES
            public float xEyelidState { get; set; }
            public float yEyelidState { get; set; }
            public float posXEyeLocation { get; set; }
            public float posYEyeLocation { get; set; }
            public bool isBlink { get; set; }
            public bool areEyesOpen { get; set; }
            public bool isLeftWink { get; set; }
            public bool isRightWink { get; set; }
            public bool isLookingLeft { get; set; }
            public bool isLookingRight { get; set; }
            public bool isLookingDown { get; set; }
            public bool isLookingUp { get; set; }

            public float emotivTimeStamp { get; set; }

            public OscMessage toOSCMesseage()
            {
                string header = "EmoState_Timestamp;" +
                    "LowerFaceAction;LowerFaceActionPower;UpperFaceAction;UpperFaceActionPower;" +
                    " ExpressivEyelidStateX;ExpressivEyelidStateY;ExpressivEyeLocationX;ExpressivEyeLocationY;" +
                    "IsBlink;AreEyesOpen;IsLeftWink;IsRightWink;IsLookingLeft;IsLookingRight;IsLookingDown;IsLookingUp";

                OscMessage expressivMSG = new OscMessage(
                    ExpressivState.EXPRESSIV_STATE_ADDR, 
                    header, 
                    emotivTimeStamp, 
                    lowerFaceAction, 
                    lowerFaceActionPower, 
                    upperFaceAction,
                    upperFaceActionPower,
                    xEyelidState,
                    yEyelidState,
                    posXEyeLocation,
                    posYEyeLocation,
                    isBlink ? 1 : 0,
                    areEyesOpen ? 1 : 0,
                    isLeftWink ? 1 : 0,
                    isRightWink ? 1 : 0,
                    isLookingLeft ? 1 : 0,
                    isLookingRight ? 1 : 0,
                    isLookingDown ? 1 : 0,
                    isLookingUp ? 1 : 0
                );

                return expressivMSG;
            }



        }


            UDPSender sender;
        public ExpressivOSC(String address, int port)
        {
            sender = new SharpOSC.UDPSender("127.0.0.1", 12000);

        }


        private double lasteTimeStamp = 0;
        public void sendExpressivOSC(ExpressivState exprState, bool isSimulated) {

            sender.Send(exprState.toOSCMesseage());

            if (isSimulated)
            {
                double waitTimeInSeconds;

                if(lasteTimeStamp == 0)
                    lasteTimeStamp = exprState.emotivTimeStamp;
                else if (lasteTimeStamp != exprState.emotivTimeStamp)
                {
                    waitTimeInSeconds = exprState.emotivTimeStamp - lasteTimeStamp;
                    lasteTimeStamp = exprState.emotivTimeStamp;
                    double timeInMS = waitTimeInSeconds * 1000.0;

                    int sleepTime = Convert.ToInt32(timeInMS);

                    Thread.Sleep(sleepTime);

                }

            }
        }



    }
}
