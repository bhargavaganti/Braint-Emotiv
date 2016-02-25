using Emotiv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressivStateLogger
{
    class ExpressivStateLogger
    {// EmoEngine engine; // the EmoEngine that connects to the head set
        static UInt32 userID = 0; // userID
        static float time;
        static string filename = "outfileExpressiv.csv"; //default output filename


        static void Main(string[] args)
        {

            string newFile = "";
            int option;

            EmoEngine engine = EmoEngine.Instance;
            engine.ExpressivEmoStateUpdated += new EmoEngine.ExpressivEmoStateUpdatedEventHandler(epressiveStateUdpate);
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(userAddedUpdate);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===================================================================");
                Console.WriteLine("Simple program to log ExpressivState Data");
                Console.WriteLine("then get emostates from this file without using the headset");
                Console.WriteLine("===================================================================");
                Console.WriteLine("Press '1' to start recording ExpressivState data into CSV file live from the headset ");
                Console.WriteLine("Press '2' to start recording ExpressivState data into CSV file using the EmoComposer (localhost, 1726)");
                Console.WriteLine("Press '3' to exit													 ");
                Console.Write(">>");

                string input = Console.ReadLine();
                while (input == "")
                {
                    input = Console.ReadLine();
                }

                option = int.Parse(input);

                // option = Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        {
                            Console.WriteLine("\nEnter name for the CSV file\n");
                            newFile = Console.ReadLine();
                            engine.Connect();
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("\nEnter name for the CSV file\n");
                            newFile = Console.ReadLine();
                            engine.RemoteConnect("127.0.0.1", 1726);
                            userID = 0;
                            break;
                        }
                    default:
                        break;
                }

                filename = newFile;

                if ((option != 1) && (option != 2)) break;
                writeHeader();
                Console.WriteLine("Start receiving ExpressivState data! Press any key to stop logging...\n");
                Console.WriteLine("ExpressivState data updating from user {0}", ExpressivStateLogger.userID);
                while (!Console.KeyAvailable)
                {
                    engine.ProcessEvents(100);
                }
                // TODOD

            }
            engine.Disconnect();

        }

        private static void epressiveStateUdpate(object sender, EmoStateUpdatedEventArgs e)
        {
            
            EmoState es = e.emoState;
            if (e.userId == userID)
            {
                string header = "EmoState_Timestamp;" +
                    "LowerFaceAction;LowerFaceActionPower;UpperFaceAction;UpperFaceActionPower;" +
                    " ExpressivEyelidStateX;ExpressivEyelidStateY;ExpressivEyeLocationX;ExpressivEyeLocationY;" +
                    "IsBlink;AreEyesOpen;IsLeftWink;IsRightWink;IsLookingLeft;IsLookingRight;IsLookingDown;IsLookingUp";
                // Wr;ite the data to a file
                TextWriter file = new StreamWriter(filename, true);

                file.Write(es.GetTimeFromStart());
                file.Write(";");

                EdkDll.EE_ExpressivAlgo_t lowerFaceAction = es.ExpressivGetLowerFaceAction();
                float lowerFaceActionPower = es.ExpressivGetLowerFaceActionPower();

                EdkDll.EE_ExpressivAlgo_t upperFaceAction = es.ExpressivGetUpperFaceAction();
                float upperFaceActionPower = es.ExpressivGetUpperFaceActionPower();

                file.Write(lowerFaceAction);
                file.Write(";");
                file.Write(lowerFaceActionPower);
                file.Write(";");

                file.Write(upperFaceAction);
                file.Write(";");
                file.Write(upperFaceActionPower);
                file.Write(";");

                // EYES
                float x, y;
                es.ExpressivGetEyelidState(out x, out y);

                file.Write(x);
                file.Write(";");
                file.Write(y);
                file.Write(";");



                float posX, posY;
                es.ExpressivGetEyeLocation(out posX, out posY);

                file.Write(posX);
                file.Write(";");
                file.Write(posY);
                file.Write(";");

                bool isBlink = es.ExpressivIsBlink();
                file.Write(isBlink);
                file.Write(";");

                bool areEyesOpen = es.ExpressivIsEyesOpen();
                file.Write(areEyesOpen);
                file.Write(";");

                bool isLeftWink = es.ExpressivIsLeftWink();
                bool isRightWink = es.ExpressivIsRightWink();
                file.Write(isLeftWink);
                file.Write(";");
                file.Write(isRightWink);
                file.Write(";");



                bool isLookingLeft = es.ExpressivIsLookingLeft();
                bool isLookingRight = es.ExpressivIsLookingRight();
                bool isLookingDown = es.ExpressivIsLookingDown();
                bool isLookingUp = es.ExpressivIsLookingUp();
                file.Write(isLookingLeft);
                file.Write(";");
                file.Write(isLookingRight);
                file.Write(";");
                file.Write(isLookingDown);
                file.Write(";");
                file.Write(isLookingUp);
                file.Write(";");

                file.WriteLine("");
                file.Close();

            }



        }

        static void userAddedUpdate(object sender, EmoEngineEventArgs e)
        {
            userID = e.userId;
        }

        static void writeHeader()
        {

            TextWriter file = new StreamWriter(filename, false);

            string header = "EmoState_Timestamp;" +
                    "LowerFaceAction;LowerFaceActionPower;UpperFaceAction;UpperFaceActionPower;" +
                    " ExpressivEyelidStateX;ExpressivEyelidStateY;ExpressivEyeLocationX;ExpressivEyeLocationY" +
                    "IsBlink;AreEyesOpen;IsLeftWink;IsRightWink;IsLookingLeft;IsLookingRight;IsLookingDown;IsLookingUp";
            file.WriteLine("sep=;");
            file.WriteLine(header);
            file.Close();

        }
    }
}
