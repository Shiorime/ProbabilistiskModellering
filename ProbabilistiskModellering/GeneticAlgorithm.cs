using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Commands;

namespace ProbabilistiskModellering
{
    public class GeneticAlgorithm<T>
    {
        string sumoOutputFilePath = "./SUMOFiles/out";
        public List<DNA<T>> population { get; private set; }
        public int generation { get; private set; }
        public double bestFitness { get; private set; }
        public T[] bestGenes { get; private set; }

        public float mutationRate;
        private Random random;

        private List<DNA<T>> newPopulation;
        private List<DNA<T>> elitists;
        private double fitnessSum;
        public int dnaSize;
        private Func<T> GetRandomGene;

        private int portNumber = 1000;
        private int numberOfInstances;

        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> GetRandomGene, float mutationRate = 0.01f)
        {
            generation = 1;
            this.mutationRate = mutationRate;
            population = new List<DNA<T>>(populationSize);
            newPopulation = new List<DNA<T>>(populationSize);
            this.random = random;
            this.dnaSize = dnaSize;
            this.GetRandomGene = GetRandomGene;
            numberOfInstances = populationSize;


            bestGenes = new T[dnaSize];

            for (int i = 0; i < populationSize; i++)
            {
                population.Add(new DNA<T>(dnaSize, GetRandomGene, true));
            }
        }

        // this method starts and updates the genetic algortihm as it runs. 
        public async Task StartGAAsync()
        {
            bestFitness = 0.5;
            // one call to start the simulation and run it is required to start the genetic algorithm
            await RunSimulationAsync();
            bool shouldStop = false;

            // should stop is used as a flag as to when the genetic algorithm stops 
            while (shouldStop == false)
            {
                // this if statement is the stop condition for the program. When this is met, the flag will be set to true
                // and the program will stop.
                if (bestFitness >= 0.85 || generation >= 100)
                {
                    shouldStop = true;
                }
                else
                {
                    NewGeneration(); // new generation gets generated based upon the old one
                    Console.WriteLine($"Best fitness of generation {generation} is: {bestFitness}");
                    ++generation;
                    await RunSimulationAsync(); // call to start simulation is made again
                }
            }
        }

        // method for running simulation 
        private async Task RunSimulationAsync()
        {
            List<TraCIClient> listOfClients = new List<TraCIClient>();
            List<SimulationCommands> listOfSimulations = new List<SimulationCommands>();
            List<TrafficLightCommands> listOfTrafficLights = new List<TrafficLightCommands>();

            //initialize clients, simulationCommands and trafficlightCommands used for controlling sumo
            for (int i = 0; i < numberOfInstances; ++i)
            {
                listOfClients.Add(new TraCIClient());
                listOfSimulations.Add(new SimulationCommands(listOfClients[i]));
                listOfTrafficLights.Add(new TrafficLightCommands(listOfClients[i]));
            }

            //open SUMO clients
            for (int i = 0; i < numberOfInstances; ++i)
            {
                OpenSumo(portNumber, sumoOutputFilePath + $"{i}.xml");
                await Task.Delay(100);
                await listOfClients[i].ConnectAsync("127.0.0.1", portNumber);
                ++portNumber;
            }


            // control trafficlights in simulation
            for (int i = 0; i < dnaSize; ++i)
            {
                Parallel.For(0, numberOfInstances, j =>
               {
                   listOfTrafficLights[j].SetRedYellowGreenState("n0", $"{population[j].genes[i]}");
                   listOfClients[j].Control.SimStep();
               });
            }


            // close clients, hence close ports, so they can be used again for the next round of simulations
            for (int i = 0; i < listOfClients.Count; ++i)
            {
                listOfClients[i].Control.Close();
            }

            // clear initialized lists to ensure that a null reference exception is avoided
            listOfClients.Clear();
            listOfSimulations.Clear();
            listOfTrafficLights.Clear();

            // task delay has been inserted, since SUMO is slow at outputting .xml files
            // this is done to avoid "file already in use" exception
            await Task.Delay(2000);
        }

        // compare DNA method for sorting the list of individuals in the population based upon their fitness
        private int CompareDNA(DNA<T> a, DNA<T> b)
        {
            if (a.fitness > b.fitness)
                return -1;
            else if (a.fitness < b.fitness)
                return 1;
            else
                return 0;
        }

        // method for generating a new generation. 
        public void NewGeneration()
        {
            if (population.Count <= 0) // just to ensure, that in case the population has not been initialized this method will stop
            {
                return;
            }

            CalculateFitness();
            population.Sort(CompareDNA);
            newPopulation.Clear();

            for (int i = 0; i < population.Count; i++)
            {
                DNA<T> parent1 = ChooseParent(); // Currently the selection chooses the two fittest individuals to create a new population. 
                DNA<T> parent2 = ChooseParent();
                

                DNA<T> child = parent1.CrossOver(parent2); // call to crossover function

                child.Mutate(mutationRate);

                newPopulation.Add(child);
            }
<<<<<<< HEAD

=======
>>>>>>> 4eff395210053eca3373428f72b8132b6f193c92
            List<DNA<T>> tmpList = population;
            population = newPopulation; // population gets set to the newly generation population
            newPopulation = tmpList;
        }

        public void CalculateFitness()
        {
            fitnessSum = 0; // fitnessSum currently has no use, but might be used for roulettewheel selection
            DNA<T> best = population[0];

            for (int i = 0; i < population.Count; i++)
            {

                fitnessSum += population[i].CalculateFitnessIndividual("tripinfo", "timeLoss", sumoOutputFilePath + $"{i}.xml");

                if (population[i].fitness > best.fitness)
                {
                    best = population[i];
                }
            }

            bestFitness = best.fitness;
            best.genes.CopyTo(bestGenes, 0);

        }

        //need rework, is weighted roulette wheel selection, however, it is unsure whether or not it works optimally
        private DNA<T> ChooseParent()
        {
            // https://stackoverflow.com/questions/56692/random-weighted-choice

            double randomNumber = random.NextDouble() * fitnessSum;

            for (int i = 0; i < population.Count; i++)
            {
                if (randomNumber < population[i].fitness)
                {
                    return population[i];
                }
                randomNumber -= population[i].fitness;
            }
            return null;


        }

        // method for opening sumo with desired command line arguments
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

        public void SaveBestGenesToXMLFile()
        {
            int size = bestGenes.Count();

            XDocument doc = new XDocument(new XElement("bestGenes"));
            for (int i = 0; i < size; ++i)
            {
                doc.Root.Add(new XElement("element", bestGenes[i]));
            }
            doc.Save("./output.xml");
        }
    }
}

