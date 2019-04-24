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
    public class Program 
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
                GeneticAlgorithm<string> ga = new GeneticAlgorithm<string>(3, 2400, pg.random, pg.GenerateRandomRedYellowGreenState, 3, 0.01f);
                await ga.StartGAAsync();
                ga.NewGeneration();
                Console.WriteLine($"Best fitness of generation {ga.Generation} is: {ga.BestFitness}");
                stopwatch.Stop();
                write(stopwatch.Elapsed.ToString());
                Console.ReadKey();
   

                Console.WriteLine("Program complete");
                Console.ReadLine();
            });
        }


        public string GenerateRandomRedYellowGreenState()
        {
            string validString1 = "GGGrrrrGGrr";
            string validString2 = "rrrGGGGrrGG";
            string randomState = string.Empty;
            int result = 0;
            for (int i = 0; i < 11; i++)
            {
                result = random.Next(0, 2);
                if (result == 0)
                {
                    randomState = validString1;
                }
                else if (result == 1)
                {
                    randomState = validString2;
                }
            }
            return randomState;
        }

        //This function calculates the average time, that each car spends not driving at the maximum allowed speed
        public double CalculateFitnessFunction(Program pg, string element, string attribute, string filePath)
        {
            string[] timeLossArray = pg.GetSpecificXMLAttributeFromFile(element, attribute, filePath);
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

        public string[] GetSpecificXMLAttributeFromFile(string element, string attribute, string filePath)
        {
            //https://stackoverflow.com/questions/933687/read-xml-attribute-using-xmldocument
            XmlDocument xmlDoc = new XmlDocument(); /* Create new XmlDocument */
            xmlDoc.Load(filePath);                  /* Load the xml file from filePath */
            XmlNodeList list = xmlDoc.GetElementsByTagName($"{element}"); /* Find elements with interval. Put it into an array/list */
            string[] finalArray = new string[list.Count];

            for(int i = 0; i < list.Count; ++i)
            {
                finalArray[i] = list[i].Attributes[$"{attribute}"].Value;
            }

            return finalArray;
        }
    }

}
