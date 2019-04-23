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

        public async Task StartGAAsync()
        {
            BestFitness = 100;
            await RunSimulationAsync();
            bool shouldStop = false;
         

            while (shouldStop == false)
            {
                if (BestFitness < 10 || Generation >= 2)
                {
                    shouldStop = true;
                }
                else
                {
                    NewGeneration();
                    Console.WriteLine($"Best fitness of generation {Generation} is: {BestFitness}");
                    ++Generation;
                    await RunSimulationAsync();
                }
            }
        }

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

            listOfClients.Clear();
            listOfSimulations.Clear();
            listOfTrafficLights.Clear();

            await Task.Delay(100);
        }

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

        public void NewGeneration()
        {
            if (Population.Count <= 0)
            {
                return;
            }

            CalculateFitness();
            Population.Sort(CompareDNA);
            List<DNA<T>> newPopulation = new List<DNA<T>>();

            for(int i = 0; i < Population.Count; i++)
            {
                DNA<T> parent1 = Population[0];
                DNA<T> parent2 = Population[1];

                DNA<T> child = parent1.CrossOver(parent2);

                child.Mutate(mutationRate);

                newPopulation.Add(child);
            }

            Population = newPopulation;

        }
                
        public void CalculateFitness()
        {
            fitnessSum = 0;
            DNA<T> best = Population[0];

            for(int i = 0; i < Population.Count; i ++)
            {
                
                fitnessSum += Population[i].CalculateFitnessIndividual("tripinfo", "timeLoss", sumoOutputFilePath + $"{i}.xml");

                if (Population[i].fitness < best.fitness)
                {
                    best = Population[i];
                }
            }

            BestFitness = best.fitness;
            best.genes.CopyTo(BestGenes, 0);

        }

        //need rework
        private DNA<T> ChooseParent()
        {
            https://stackoverflow.com/questions/56692/random-weighted-choice

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

