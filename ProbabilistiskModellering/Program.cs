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
using System.Management;

namespace ProbabilistiskModellering
{
    delegate double FitnessFunction(Program pg, string attribute, string filePath);
    public class Program 
    {
        Random random = new Random();
        int population = 0;
        int genes = 0;
        int elitistNumber = 0;
        int generationStop = 0;
        double fitnessStop = 0.0;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            await Task.Run(async () =>
            {
                Program pg = new Program();

                Console.WriteLine(pg.CalculateFitnessIndividual("tripinfo", "timeLoss", "./sumo.xml"));
                Console.ReadLine();

                /*pg.AskPopulationSize();
                pg.AskGenePoolSize();
                pg.AskGenerationStopSize();
                pg.AskFitnessScoreStopSize();

                if (pg.population >= 10)
                    pg.elitistNumber = 5;
                else
                    pg.elitistNumber = pg.population / 2;

                GeneticAlgorithm<string> ga = new GeneticAlgorithm<string>(pg.population, pg.genes, pg.elitistNumber, pg.generationStop, pg.fitnessStop, pg.random, pg.GenerateRandomRedYellowGreenState, 0.05f);

                await ga.StartGAAsync();
                ga.NewGeneration();
                ga.SaveBestGenesToXMLFile();
                ga.SaveBestFitness();

                Console.WriteLine($"Best fitness of generation {ga.generation} is: {ga.bestFitness}");

                Console.WriteLine("Program complete");
                Console.ReadLine();*/
            });
        }

        // method for generating random gene for genome
        // two valid strings are available to ensure, that each opposite entrance to the crossing are green at the same time 
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
        
        public void AskPopulationSize()
        {
            Console.Write("Desired population count: ");
            try
            {
                population = int.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid. Please write a number. Press any key to close.");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            if (population <= 1)
            {
                Console.WriteLine("A population of one or less is not useful");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }

        public void AskGenePoolSize()
        {
            Console.Write("Desired gene length: ");
            try
            {
                genes = int.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid. Please write a number. Press any key to close.");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            if (genes <= 0)
            {
                Console.WriteLine("Not possible with a gene pool of 0 or less. Press any key to close.");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }

        public void AskGenerationStopSize()
        {
            Console.Write("Stop when reaching generation number: ");
            try
            {
                generationStop = int.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid. Please write a number. Press any key to close.");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            if (generationStop <= 0)
            {
                Console.WriteLine("Not possible with a generation count less than or equal to 0. Press any key to close.");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }

        public void AskFitnessScoreStopSize()
        {
            Console.Write("Stop when reaching fitness score: ");
            try
            {
                fitnessStop = double.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid. Please write a number. Press any key to close. Press any key to close.");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            if (fitnessStop > 0.0 && fitnessStop < 1.0)
            {
                Console.WriteLine("Not possible with fitness below 0 and above 1. Press any key to close.");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }

        public double CalculateFitnessIndividual(string element, string attribute, string filePath)
        {
            string[] timeLossArray = GetSpecificXMLAttributeFromFile(element, attribute, filePath);
            int cars = timeLossArray.Count();
            double timeLossSum = 0.0;
            for (int i = 0; i < cars; i++)
            {
                timeLossSum += double.Parse(timeLossArray[i], CultureInfo.InvariantCulture);
            }
            double timeLossAvg = timeLossSum / cars;
            // ide til anden matematisk model https://math.stackexchange.com/questions/384613/exponential-function-with-values-between-0-and-1-for-x-values-between-0-and-1

            return Math.Pow(2, -(0.1 * timeLossAvg));

        }

        public string[] GetSpecificXMLAttributeFromFile(string element, string attribute, string filePath)
        {
            //https://stackoverflow.com/questions/933687/read-xml-attribute-using-xmldocument
            XmlDocument xmlDoc = new XmlDocument(); /* Create new XmlDocument */
            xmlDoc.Load(filePath);                  /* Load the xml file from filePath */
            XmlNodeList list = xmlDoc.GetElementsByTagName($"{element}"); /* Find elements with interval. Put it into an array/list */
            string[] finalArray = new string[list.Count];

            for (int i = 0; i < list.Count; ++i)
            {
                finalArray[i] = list[i].Attributes[$"{attribute}"].Value;
            }

            return finalArray;
        }

    }

}
