using Emotiv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmoEngineExploration
{
    class Program
    {
        public static EmoEngine engine = EmoEngine.Instance;
        public uint userID;

        static void Main(string[] args)
        {
            Program p = new Program();
            
            engine.EmoEngineConnected += new EmoEngine.EmoEngineConnectedEventHandler(p.engine_connected);
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(p.engine_UserAdded_Event);
            engine.EmoEngineDisconnected += new EmoEngine.EmoEngineDisconnectedEventHandler(p.engine_disconnected);

            engine.ExpressivTrainingStarted += new EmoEngine.ExpressivTrainingStartedEventEventHandler(p.engine_expressiveTrainStarted);

            engine.InternalStateChanged += new EmoEngine.InternalStateChangedEventHandler(p.engine_internalStateChanged);

            engine.ExpressivTrainingSucceeded += new EmoEngine.ExpressivTrainingSucceededEventHandler(p.engine_expressiveSucceded);

            //engine.RemoteConnect("127.0.0.1", 1726);

            engine.Connect();
            engine.ProcessEvents(200);
            Thread.Sleep(200);
            engine.ExpressivSetTrainingAction(p.userID, EdkDll.EE_ExpressivAlgo_t.EXP_SMILE);

            engine.ExpressivSetTrainingControl(p.userID, EdkDll.EE_ExpressivTrainingControl_t.EXP_START);

            while (!Console.KeyAvailable) {
                engine.ProcessEvents(200);

                p.mainLoop();
                Thread.Sleep(200);
            }

        }

        private void engine_expressiveSucceded(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("sucess");
        }

        private void engine_internalStateChanged(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("hlleo");
        }

        private void engine_expressiveTrainStarted(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        private void mainLoop()
        {
            //Profile p = engine.GetUserProfile(userID);



            //Console.WriteLine(engine.EngineGetNumUser());





            //Console.WriteLine(engine.ExpressivGetThreshold(userID, EdkDll.EE_ExpressivAlgo_t.EXP_BLINK, EdkDll.EE_ExpressivThreshold_t.EXP_SENSITIVITY));

            //engine.ExpressivSetTrainingControl(userID, EdkDll.EE_ExpressivTrainingControl_t.;
            //engine.ex




        }













        #region Basic Emo Engine Events
        private void engine_disconnected(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("Disconnected from EmoEngine");
        }

        private void engine_connected(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("Connected to EmoEngine (User: " + e.userId + ")");
        }


        void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("User Added Event has occured");

            // record the user 
            userID = e.userId;

            // enable data aquisition for this user.
            engine.DataAcquisitionEnable(userID, true);

            // ask for up to 1 second of buffered data
            engine.EE_DataSetBufferSizeInSec(1);

        }
        #endregion
    }
}
