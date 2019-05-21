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

                pg.AskPopulationSize();
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
                Console.ReadLine();
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

    }

}
