using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emotiv;
using System.IO;

namespace AffectiveStateLogger
{
    /*
    * Simple programm to log affective state data to a CSV File.
    * Info: If connecting to the Emotiv Control Panel, use port 3008. If connecting to the EmoComposer, use port 1726.
    */
    class AffectiveStateLogger
    {

        // EmoEngine engine; // the EmoEngine that connects to the head set
        static UInt32 userID = 999; // userID
        static float time;
        static string filename = "outfile.csv"; //default output filename


        static void Main(string[] args)
        {

            string newFile = "";
            int option;

            EmoEngine engine = EmoEngine.Instance;
            engine.AffectivEmoStateUpdated += new EmoEngine.AffectivEmoStateUpdatedEventHandler(affectiveStateUpdate);
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(userAddedUpdate);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===================================================================");
                Console.WriteLine("Simple program to log AffectiveState Data");
                Console.WriteLine("then get emostates from this file without using the headset");
                Console.WriteLine("===================================================================");
                Console.WriteLine("Press '1' to start recording affective state data into CSV file live from the headset ");
                Console.WriteLine("Press '2' to start recording affective state data into CSV file using the EmoComposer (localhost, 1726)");
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
                Console.WriteLine("Start receiving affective data! Press any key to stop logging...\n");
                Console.WriteLine("Affective data updating from user {0}", AffectiveStateLogger.userID);
                while (!Console.KeyAvailable)
                {
                    engine.ProcessEvents(100);
                }
                // TODOD

            }
            engine.Disconnect();

        }

        static void affectiveStateUpdate(object sender, EmoStateUpdatedEventArgs e)
        {

            EmoState es = e.emoState;
            float lastUpdate = time;
            float esTimeStamp = es.GetTimeFromStart();
            string systemTimeStamp = DateTime.Now.ToString("hh.mm.ss.ffffff");
            // Write the data to a file
            TextWriter file = new StreamWriter(filename, true);

            // "Timestamp,EmoState_Timestamp,BoredomScore,ExcitementShortScore,FrustrationScore," +
            // " MediationScore,ValenceScore,ExcitementLongShort,"


            if (e.userId == userID)
            {
                file.Write(systemTimeStamp + ";");
                file.Write(Convert.ToString(esTimeStamp) + ";");
                file.Write(es.AffectivGetEngagementBoredomScore() + ";");
                file.Write(es.AffectivGetExcitementShortTermScore() + ";");
                file.Write(es.AffectivGetFrustrationScore() + ";");
                file.Write(es.AffectivGetMeditationScore() + ";");
                file.Write(es.AffectivGetValenceScore()+";");
                file.Write(es.AffectivGetExcitementLongTermScore() + ";");
                file.WriteLine("");

                

                Console.WriteLine("Receiveing affective update .....");

            }
            file.Close();

        }

        static void userAddedUpdate(object sender, EmoEngineEventArgs e)
        {
            userID = e.userId;
        }

        static void writeHeader()
        {

            TextWriter file = new StreamWriter(filename, false);

            string header = "Timestamp;EmoState_Timestamp;BoredomScore;ExcitementShortScore;FrustrationScore;" +
                " MediationScore;ValenceScore;ExcitementLongShort;";
            file.WriteLine("sep=;");
            file.WriteLine(header);
            file.Close();

        }






    }
}
