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
                int population = 0;
                Program pg = new Program();

                Console.Write("Population count: ");
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

                if(population <= 1)
                {
                    Console.WriteLine("A population of one is not useful");
                    Console.ReadLine();
                    Environment.Exit(-1);
                }

                GeneticAlgorithm<string> ga = new GeneticAlgorithm<string>(population, 2400, 2, pg.random, pg.GenerateRandomRedYellowGreenState, 0.05f);
                
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

        
    }

}
