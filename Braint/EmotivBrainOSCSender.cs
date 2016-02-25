using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emotiv;
using System.Data;
using Microsoft.VisualBasic;
using System.IO;
using System.Threading;

namespace Braint
{
    class EmotivBrainOSCSender
    {

        public EmoEngine engine = EmoEngine.Instance;
        static UInt32 userID = 999;


        public bool simulateWithCSV
        {
            get; set;
        }
        public bool emoComposerUsed
        {
            get; set;
        }

        public AffectiveOSC affectiveOSC
        {
            get; private set;
        }

        public RawDataOSC rawDataOSC
        {
            get; private set;
        }

        public ExpressivOSC expressivOSC
        {
            get; private set;
        }








        #region Braint Main Loop & Setup
        /// <summary>
        /// Iinitlaizes the needed event listerens and OSC objects for retrieving emotiv headset data and sending via OSC.
        /// OSC port used is 12001 on the local host.
        /// </summary>
        void SetUp()
        {

            if (!simulateWithCSV)
            {

                engine.EmoEngineConnected += new EmoEngine.EmoEngineConnectedEventHandler(engine_connected);
                engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);
                engine.EmoEngineDisconnected += new EmoEngine.EmoEngineDisconnectedEventHandler(engine_disconnected);
                engine.AffectivEmoStateUpdated += new EmoEngine.AffectivEmoStateUpdatedEventHandler(affectiveStateUpdate);
                engine.ExpressivEmoStateUpdated += new EmoEngine.ExpressivEmoStateUpdatedEventHandler(epressiveStateUdpate);

                try
                {
                    if (!emoComposerUsed)
                        engine.Connect();
                    else {
                        userID = 0;
                        engine.RemoteConnect("127.0.0.1", 1726);
                        
                    }
                }
                catch
                {
                    Console.WriteLine("Could not connect to EmoEngine or EmoComposer");
                    Console.WriteLine("\nPress any key to return to the main menu\n");
                    while (!Console.KeyAvailable)
                    {
                    }
                }
            }

            affectiveOSC = new AffectiveOSC("127.0.0.1", 12001);
            rawDataOSC = new RawDataOSC("127.0.0.1", 12001);
            expressivOSC = new ExpressivOSC("127.0.0.1", 12001);


        }


        void loopMe()
        {
            // TODO test this

            // get 1 second of data 
            engine.ProcessEvents(1000);
            // If the user has not yet connected, do not proceed
            // should not happen
            if (userID == 999)
                return;


            if(emoComposerUsed)
            {
                //Thread.Sleep(200);

            } else { 

            Dictionary<EdkDll.EE_DataChannel_t, double[]> data = engine.GetData(userID);


        


            if (data == null)
            {
                return;
            }

            // TODO KÖNNTE DAS HIER KAPUTT GEHEN, WENN SAMPLE ZU VIEL???
            rawDataOSC.sendRawDataNormalized(data, "/emoengine/rawEEG");
            }
        }



        #endregion

        #region Main Program 
        static void Main(string[] args)
        {

            string newFile = "";
            int option;
            EmotivBrainOSCSender braint = new EmotivBrainOSCSender();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===================================================================");
                Console.WriteLine("Simple program to transmit the data from the Emotiv Headset via OSC");
                Console.WriteLine("All data is send on the local machine on port 12001");
                Console.WriteLine("===================================================================");
                Console.WriteLine("Press '1' to start sending real time data gathered from the headset vis OSC (raw data, affective state data, expressive data ");
                Console.WriteLine("Press '2' to start sending raw EEG data from a CSV file");
                Console.WriteLine("Press '3' to start sending affective state data from a CSV file");
                Console.WriteLine("Press '4' to start sending expressiv state data from a CSV file");
                Console.WriteLine("Press '5' Connect to EmoComposer to simulate input");
                Console.WriteLine("Press '6' to exit													 ");
                Console.Write(">>");

                string input = Console.ReadLine();
                while (input == "")
                {
                    input = Console.ReadLine();
                }

                try
                {
                    option = int.Parse(input);
                }
                catch
                {
                    Console.WriteLine("Input invalid");
                    option = 0;
                }

                // option = Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case 1: // connect real (raw/affective/expressive )
                        {
                            Console.WriteLine("Trying to connect to Emo Engine");
                            braint.emoComposerUsed = false;
                            braint.simulateWithCSV = false;
                            break;
                        }
                    case 2: // raw csv
                        {
                            Console.WriteLine("\nEnter name for the raw EEG CSV file\n");
                            newFile = Console.ReadLine();
                            braint.emoComposerUsed = false;
                            braint.simulateWithCSV = true;

                            break;
                        }
                    case 3: // affective csv
                        {
                            Console.WriteLine("\nEnter name for the affective CSV file\n");
                            newFile = Console.ReadLine();
                            braint.emoComposerUsed = false;
                            braint.simulateWithCSV = true;
                            break;
                        }
                    case 4: // expressive csv
                        {
                            Console.WriteLine("\nEnter name for the expressive CSV file\n");
                            newFile = Console.ReadLine();
                            braint.emoComposerUsed = false;
                            braint.simulateWithCSV = true;
                            break;
                        }
                    case 5: // use composer
                        {
                            Console.WriteLine("Trying to connect to EmoComposer");
                            braint.emoComposerUsed = true;
                            braint.simulateWithCSV = false;
                            break;
                        }
                    case 6:
                        {
                            Console.WriteLine("\nGoodbye my friend!\n");
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("\nYou did not enter a valid option!\n");
                            Console.WriteLine("\nPress any key to return to the main menu\n");
                            while (!Console.KeyAvailable)
                            {
                            }
                            break;
                        }
                }



                if (option == 6)
                    break;

                if ((option != 1) && (option != 2) && (option != 3) && (option != 4) && (option != 5) && (option != 0)) continue;


                braint.SetUp();

                string theFile = "";
                if (option == 2 || option == 3 || option == 4)
                {
                    theFile = braint.checkFile(newFile);
                    if (theFile.Equals("INVALID"))
                    {
                        //Console.WriteLine("\nThe file >" +newFile+ "< could not opened\n");
                        Console.WriteLine("\nPress any key to return to the main menu\n");
                        while (!Console.KeyAvailable)
                        {
                        }
                        continue;
                    }
                }


                if (option == 2)
                    braint.rawEEGCSVParser(theFile);
                else if (option == 3)
                    braint.affectiveStateCSVParser(theFile);
                else if (option == 4)
                    braint.expressiveStateCSVParser(theFile);
                else {
                    while (!Console.KeyAvailable)
                    {
                        if (option == 1 || option == 5)
                            braint.loopMe();

                    }
                }


            }

            if (!braint.simulateWithCSV && option == 1 || option == 5)
                braint.engine.Disconnect();

        }

        private string checkFile(string newFile)
        {

            string path = newFile;
            bool res = false;
            if (!newFile.EndsWith(".csv"))
                path += ".csv";

            try
            {
                var reader = new StreamReader(File.OpenRead(path));
                res = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not open file:" + newFile);
                Console.WriteLine(e);
                path = "INVALID";
            }

            return path;
        }
        #endregion


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

        #region Affective & Expressive State Handlers
        void affectiveStateUpdate(object sender, EmoStateUpdatedEventArgs e)
        {

            EmoState es = e.emoState;
            // float lastUpdate = time;
            //float esTimeStamp = es.GetTimeFromStart();
            //string systemTimeStamp = DateTime.Now.ToString("hh.mm.ss.ffffff");

            // "Timestamp,EmoState_Timestamp,BoredomScore,ExcitementShortScore,FrustrationScore," +
            // " MediationScore,ValenceScore,ExcitementLongShort,"


            if (e.userId == userID)
            {
                AffectiveOSC.AffectiveState affectiveState = new AffectiveOSC.AffectiveState();
                affectiveState.emotivTimeStamp = es.GetTimeFromStart();
                affectiveState.boredom = es.AffectivGetEngagementBoredomScore();
                affectiveState.excitement = es.AffectivGetExcitementShortTermScore();
                affectiveState.frustration = es.AffectivGetFrustrationScore();
                affectiveState.mediation = es.AffectivGetMeditationScore();
                affectiveState.valence = es.AffectivGetValenceScore();
                affectiveState.excitementLongTerm = es.AffectivGetExcitementLongTermScore();
                affectiveOSC.sendAffectiveStateData(affectiveState, false);

                Console.WriteLine("Received Affective State Update");
            }



        }

        private void epressiveStateUdpate(object sender, EmoStateUpdatedEventArgs e)
        {

            if (e.userId == userID)
            {

                EmoState es = e.emoState;
                ExpressivOSC.ExpressivState exprState = new ExpressivOSC.ExpressivState();

                exprState.emotivTimeStamp = es.GetTimeFromStart();

                EdkDll.EE_ExpressivAlgo_t asd;
                int abc = (int) es.ExpressivGetLowerFaceAction();

                string lower = "" + es.ExpressivGetLowerFaceAction();
                exprState.lowerFaceAction = lower;
                exprState.lowerFaceActionPower = es.ExpressivGetLowerFaceActionPower();

                string upper = "" + es.ExpressivGetUpperFaceAction();
                exprState.upperFaceAction = upper;
                exprState.upperFaceActionPower = es.ExpressivGetUpperFaceActionPower();

                // EYES
                float x, y;
                es.ExpressivGetEyelidState(out x, out y);
                exprState.xEyelidState = x;
                exprState.yEyelidState = y;

                float posX, posY;
                es.ExpressivGetEyeLocation(out posX, out posY);
                exprState.posXEyeLocation = posX;
                exprState.posYEyeLocation = posY;

                exprState.isBlink = es.ExpressivIsBlink();
                exprState.areEyesOpen = es.ExpressivIsEyesOpen();

                exprState.isLeftWink = es.ExpressivIsLeftWink();
                exprState.isRightWink = es.ExpressivIsRightWink();

                exprState.isLookingLeft = es.ExpressivIsLookingLeft();
                exprState.isLookingRight = es.ExpressivIsLookingRight();
                exprState.isLookingDown = es.ExpressivIsLookingDown();
                exprState.isLookingUp = es.ExpressivIsLookingUp();

                expressivOSC.sendExpressivOSC(exprState, false);

                Console.WriteLine("Received Expressive State Update");
            }


        }
        #endregion


        #region Sending from CSV
        private void expressiveStateCSVParser(string path)
        {
            if(path == null || path == "")
            {
                Console.WriteLine("no path is specified");
                return;
            }


            try
            {

                var reader = new StreamReader(File.OpenRead(path));
   
                int count = 0;
                // read two lines: one for the header one for the sep=";" line
                reader.ReadLine(); reader.ReadLine();
                while (!reader.EndOfStream && !Console.KeyAvailable)
                {
                    var line = reader.ReadLine();
                    string[] values = line.Split(';');
                    ExpressivOSC.ExpressivState stateToSend = new ExpressivOSC.ExpressivState();

                    //string header = "EmoState_Timestamp;" +
                    //"LowerFaceAction;LowerFaceActionPower;UpperFaceAction;UpperFaceActionPower;" +
                    //" ExpressivEyelidStateX;ExpressivEyelidStateY;ExpressivEyeLocationX;ExpressivEyeLocationY" +
                    //"IsBlink;AreEyesOpen;IsLeftWink;IsRightWink;IsLookingLeft;IsLookingRight;IsLookingDown;IsLookingUp";
                    stateToSend.emotivTimeStamp = (float)Convert.ToDouble(values[0]);
                    stateToSend.lowerFaceAction = values[1];
                    stateToSend.lowerFaceActionPower = (float)Convert.ToDouble(values[2]);
                    stateToSend.upperFaceAction = values[3];
                    stateToSend.upperFaceActionPower = (float)Convert.ToDouble(values[4]);
                    stateToSend.xEyelidState = (float)Convert.ToDouble(values[5]);
                    stateToSend.yEyelidState = (float)Convert.ToDouble(values[6]);
                    stateToSend.posXEyeLocation = (float)Convert.ToDouble(values[7]);
                    stateToSend.posYEyeLocation = (float)Convert.ToDouble(values[8]);
                    stateToSend.isBlink = Convert.ToBoolean(values[9]);
                    stateToSend.areEyesOpen = Convert.ToBoolean(values[10]);
                    stateToSend.isLeftWink = Convert.ToBoolean(values[11]);
                    stateToSend.isRightWink = Convert.ToBoolean(values[12]);
                    stateToSend.isLookingLeft = Convert.ToBoolean(values[13]);
                    stateToSend.isLookingRight = Convert.ToBoolean(values[14]);
                    stateToSend.isLookingDown = Convert.ToBoolean(values[15]);
                    stateToSend.isLookingUp = Convert.ToBoolean(values[16]);


                    Console.WriteLine("Sending Package from CSV ..." + count);
                    Console.WriteLine(line);
                    expressivOSC.sendExpressivOSC(stateToSend, true);
                    count++;

                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Could not open file:" + e.FileName);
            }

        }



        // TODO is not in synv with time yet when sending
        private void affectiveStateCSVParser(string path)
        {
            if (path == null || path == "")
            {
                Console.WriteLine("no path is specified");
                return;
            }

            try
            {

                // TODO
                var reader = new StreamReader(File.OpenRead(path));


                int lines = 0;
                int count = 0;
                // read two lines: one for the header one for the sep=";" line
                reader.ReadLine(); reader.ReadLine();
                while (!reader.EndOfStream && !Console.KeyAvailable)
                {
                    var line = reader.ReadLine();
                    string[] values = line.Split(';');
                    AffectiveOSC.AffectiveState stateToSend = new AffectiveOSC.AffectiveState();

                    // Timestamp;EmoState_Timestamp;BoredomScore;ExcitementShortScore;
                    // FrustrationScore; MediationScore;ValenceScore;ExcitementLongShort;
                    stateToSend.emotivTimeStamp = (float)Convert.ToDouble(values[1]);
                    stateToSend.boredom = Convert.ToDouble(values[2]);
                    stateToSend.excitement = Convert.ToDouble(values[3]);
                    stateToSend.frustration = Convert.ToDouble(values[4]);
                    stateToSend.mediation = Convert.ToDouble(values[5]);
                    stateToSend.valence = Convert.ToDouble(values[6]);
                    stateToSend.excitementLongTerm = Convert.ToDouble(values[7]);
                    Console.WriteLine("Sending Package from CSV ..." + count);
                    Console.WriteLine(line);
                    affectiveOSC.sendAffectiveStateData(stateToSend,true);
                    count++;

                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Could not open file:" + e.FileName);
            }
        }
        /// <summary>
        /// Reads the data from a .csv from the given path and sends packages of 128 samples via OSC at the adress "/rawEEG".
        /// </summary>
        /// <param name="path"></param>
        public void rawEEGCSVParser(string path)
        {


            if (path == null || path == "")
            {
                Console.WriteLine("no path is specified");
                return;
            }

            Dictionary<EdkDll.EE_DataChannel_t, double[]> data = new Dictionary<EdkDll.EE_DataChannel_t, double[]>();

            // COUNTER INTERPOLATED	RAW_CQ	AF3	F7	F3	 FC5	 
            // T7	 P7	 O1	 O2	P8	 T8	 FC6	 F4	F8	 AF4	
            // GYROX	 GYROY	 TIMESTAMP	 ES_TIMESTAMPFUNC_ID	 
            // FUNC_VALUE	 MARKER	 SYNC_SIGNAL

            data.Add(EdkDll.EE_DataChannel_t.COUNTER, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.INTERPOLATED, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.RAW_CQ, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.AF3, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.F7, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.F3, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.FC5, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.T7, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.P7, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.O1, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.O2, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.P8, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.T8, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.FC6, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.F4, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.F8, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.AF4, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.GYROX, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.GYROY, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.TIMESTAMP, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.ES_TIMESTAMP, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.FUNC_ID, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.FUNC_VALUE, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.MARKER, new double[128]);
            data.Add(EdkDll.EE_DataChannel_t.SYNC_SIGNAL, new double[128]);


            try
            {

                //var reader = new StreamReader(File.OpenRead(@"C:\Users\piliib\Desktop\emotiv_data\pilleRawEEGData_01.csv"));
                var reader = new StreamReader(File.OpenRead(path));

                int lines = 0;
                int count = 0;
                // read two lines: one for the header one for the sep=";" line
                reader.ReadLine(); reader.ReadLine();
                while (!reader.EndOfStream && !Console.KeyAvailable)
                {
                    if (lines == 128)
                    { // send 128 samples at once
                        rawDataOSC.sendRawDataNormalized(data, "/emoengine/rawEEG");
                        Console.WriteLine("Sending Package from CSV ..." + count);
                        lines = 0;
                        count++;
                    }

                    var line = reader.ReadLine();
                    string[] values = line.Split(';');
                    if (!values[0].Equals("COUNTER"))
                    { // skip header

                        // double[] lineVals = values.Cast<double>();
                        //var linesVals = values.Cast<double>();

                        //double[] asd = linesVals.ToArray<double>();

                        int column = 0;
                        foreach (EdkDll.EE_DataChannel_t channel in data.Keys)
                        {
                            try
                            {
                                //Console.Write(column + ",");
                                data[channel][lines] = Convert.ToDouble(values[column]);
                            }
                            catch
                            {
                                data[channel][lines] = 0.0;
                            }
                            column++;
                        }

                        lines++;
                    }
                }

                if (lines < 128) // send remaining data
                    rawDataOSC.sendRawDataNormalized(data, "/emoengine/rawEEG");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Could not open file:" + e.FileName);
            }

        }
        #endregion
    }

}
