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
                GeneticAlgorithm<string> ga = new GeneticAlgorithm<string>(2, 2400, pg.random, pg.GenerateRandomRedYellowGreenState, 0.05f);
                await ga.StartGAAsync();
                ga.NewGeneration();
                ga.SaveBestGenesToXMLFile();
                Console.WriteLine($"Best fitness of generation {ga.generation} is: {ga.bestFitness}");
                stopwatch.Stop();
                write(stopwatch.Elapsed.ToString());
                Console.ReadKey();
   

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
