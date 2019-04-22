using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Commands;

namespace ProbabilistiskModellering
{
    delegate double FitnessFunction(Program pg, string attribute, string filePath);
    class Program 
    {
        Random random = new Random();

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            await Task.Run(async () =>
            {
                Program pg = new Program();
                Stopwatch stopwatch = new Stopwatch();
                Action<string> write = Console.WriteLine;

                stopwatch.Start();
                GeneticAlgorithm<string> ga = new GeneticAlgorithm<string>(10, 2400, pg.random, pg.GenerateRandomRedYellowGreenState, 10, 0.01f);
                await ga.StartGAAsync();
                stopwatch.Stop();
                write(stopwatch.Elapsed.ToString());
                Console.ReadKey();
   

                Console.WriteLine("Program complete");
                Console.ReadLine();
            });
        }


        public string GenerateRandomRedYellowGreenState()
        {
            string randomState = string.Empty;
            int result = 0;
            for (int i = 0; i < 11; i++)
            {
                result = random.Next(0, 2);
                if (result == 0)
                {
                    randomState = randomState + "r";
                }
                else if (result == 1)
                {
                    randomState = randomState + "G";
                }
            }
            return randomState;
        }

        //Denne funktion udregner Fitness funktionen, Denne funktion udregner den gennemsnitlige tid
        //som hver bil bruger uden at køre den maksimalt tilladt hastighed
        public double CalculateFitnessFunction(Program pg, string attribute, string filePath)
        {
            string[] timeLossArray = pg.GetSpecificXMLAttributeFromFile(attribute, filePath);
            int cars = timeLossArray.Count();
            double timeLossSum = 0.0;
            for (int i = 0; i < cars; i++)
            {
                timeLossSum += double.Parse(timeLossArray[i], CultureInfo.InvariantCulture);
            }
            File.Delete(filePath);
            return timeLossSum / cars;
        }

        // has been tested in seperate project, we will have to test if cmd.WaitForExit() causes conflict with the ending simulation
        public void OpenSumo(int portNumber, string outputFile)
        {
            //https://www.codeproject.com/Articles/25983/How-to-Execute-a-Command-in-C
            string yeet = $"sumo --remote-port {portNumber} -c SUMOFiles/cfg.sumocfg -W true --tripinfo-output {outputFile}";
            ProcessStartInfo sInfo = new ProcessStartInfo("cmd", "/c " + yeet);
            Process cmd = new Process();
            sInfo.FileName = "cmd.exe";
            cmd.StartInfo = sInfo;
            cmd.Start();
        }

        public string[] GetSpecificXMLAttributeFromFile(string attribute, string filePath)
        {
            //https://stackoverflow.com/questions/933687/read-xml-attribute-using-xmldocument
            XmlDocument xmlDoc = new XmlDocument(); /* Create new XmlDocument */
            xmlDoc.Load(filePath);                  /* Load the xml file from filePath */
            XmlNodeList list = xmlDoc.GetElementsByTagName("tripinfo"); /* Find elements with interval. Put it into an array/list */
            string[] finalArray = new string[list.Count];

            for(int i = 0; i < list.Count; ++i)
            {
                finalArray[i] = list[i].Attributes[$"{attribute}"].Value;
            }

            return finalArray;
        }
    }

}
