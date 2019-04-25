using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Commands;

namespace ProbabilistiskModellering
{
    public class GeneticAlgorithm<T>
    {
        string sumoOutputFilePath = "./SUMOFiles/out";
        string element = "tripinfo";
        public List<DNA<T>> Population { get; private set; }
        public int Generation { get; private set; }
        public double BestFitness { get; private set; }
        public T[] BestGenes { get; private set; }

        public float mutationRate;
        private Random random;

        private List<DNA<T>> newPopulation;
        private double fitnessSum;
        private int dnaSize;
        private Func<T> getRandomGene;

        private int portNumber = 1000;
        int numberOfInstances;

        

        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, int numberOfInstances, float mutationRate = 0.01f)
        {
            Generation = 1;
            this.mutationRate = mutationRate;
            Population = new List<DNA<T>>(populationSize);
            newPopulation = new List<DNA<T>>(populationSize);
            this.random = random;
            this.dnaSize = dnaSize;
            this.getRandomGene = getRandomGene;
            this.numberOfInstances = numberOfInstances;

           
            BestGenes = new T[dnaSize];

            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new DNA<T>(dnaSize, getRandomGene, true));
            }
        }

        // this method starts and updates the genetic algortihm as it runs. 
        public async Task StartGAAsync()
        {
            BestFitness = 0.5;
            // one call to start the simulation and run it is required to start the genetic algorithm
            await RunSimulationAsync();
            bool shouldStop = false;
         
            // should stop is used as a flag as to when the genetic algorithm stops 
            while (shouldStop == false)
            {
                // this if statement is the stop condition for the program. When this is met, the flag will be set to true
                // and the program will stop. 
                if (BestFitness >= 0.85)
                {
                    shouldStop = true;
                }
                else
                {
                    NewGeneration(); // new generation gets generated based upon the old one
                    Console.WriteLine($"Best fitness of generation {Generation} is: {BestFitness}");
                    ++Generation;
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
                await listOfClients[i].ConnectAsync("127.0.0.1", portNumber);
                ++portNumber;
            }

            // control trafficlights in simulation
            for (int i = 0; i < dnaSize; ++i)
            {
                for (int j = 0; j < numberOfInstances; j++)
                {
                    listOfTrafficLights[j].SetRedYellowGreenState("n0", $"{Population[j].genes[i]}");
                    listOfClients[j].Control.SimStep();
                }
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
            await Task.Delay(100);
        }

        // compare DNA method for sorting the list of individuals in the population based upon their fitness
        private int CompareDNA(DNA<T> a, DNA<T> b)
        {
            if (a.fitness > b.fitness)
            {
                return -1;
            }
            else if( a.fitness < b.fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        // method for generating a new generation. 
        public void NewGeneration()
        {
            if (Population.Count <= 0) // just to ensure, that in case the population has not been initialized this method will stop
            {
                return;
            }

            CalculateFitness();
            Population.Sort(CompareDNA);
            List<DNA<T>> newPopulation = new List<DNA<T>>(); // new population is declared and will be initialized in for-loop below

            for(int i = 0; i < Population.Count; i++)
            {
                DNA<T> parent1 = ChooseParent(); // Currently the selection chooses the two fittest individuals to create a new population. 
                DNA<T> parent2 = ChooseParent();

                DNA<T> child = parent1.CrossOver(parent2); // call to crossover function

                child.Mutate(mutationRate);

                newPopulation.Add(child);
            }

            Population = newPopulation; // population gets set to the newly generation population

        }
        
        public void CalculateFitness()
        {
            fitnessSum = 0; // fitnessSum currently has no use, but might be used for roulettewheel selection
            DNA<T> best = Population[0];

            for(int i = 0; i < Population.Count; i ++)
            {
                
                fitnessSum += Population[i].CalculateFitnessIndividual("tripinfo", "timeLoss", sumoOutputFilePath + $"{i}.xml");

                if (Population[i].fitness > best.fitness)
                {
                    best = Population[i];
                }
            }

            BestFitness = best.fitness;
            best.genes.CopyTo(BestGenes, 0);

        }

        //need rework, is weighted roulette wheel selection, however, it is unsure whether or not it works optimally
        private DNA<T> ChooseParent()
        {
            // https://stackoverflow.com/questions/56692/random-weighted-choice

            double randomNumber = random.NextDouble() * fitnessSum;

            for(int i = 0; i < Population.Count; i++)
            {
                if (randomNumber < Population[i].fitness)
                {
                    return Population[i];
                }
                randomNumber -= Population[i].fitness;
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
    }

}

