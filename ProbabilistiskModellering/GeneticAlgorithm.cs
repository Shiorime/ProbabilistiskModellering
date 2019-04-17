using System;
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
        private double fitnessFunction;
        int portNumber = 1000;

        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, double fitnessFunction, float mutationRate = 0.01f)
        {
            Generation = 1;
            this.mutationRate = mutationRate;
            Population = new List<DNA<T>>(populationSize);
            newPopulation = new List<DNA<T>>(populationSize);
            this.random = random;
            this.dnaSize = dnaSize;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;
            

            BestGenes = new T[dnaSize];


            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new DNA<T>(dnaSize, getRandomGene, fitnessFunction, true));
            }
        }

        public void Start()
        {
            List<TraCIClient> listOfClients = new List<TraCIClient>();
            List<SimulationCommands> listOfSimulations = new List<SimulationCommands>();
            List<TrafficLightCommands> listOfTrafficLights = new List<TrafficLightCommands>();

            int numberOfInstancedClients = 10;

            for (int i = 0; i < numberOfInstancedClients; ++i)
            {
                listOfClients.Add(new TraCIClient());
                listOfSimulations.Add(new SimulationCommands(listOfClients[i]));
                listOfTrafficLights.Add(new TrafficLightCommands(listOfClients[i]));
            }
        }

        public void NewGeneration()
        {
            if (Population.Count <= 0)
            {
                return;
            }

            CalculateFitness();

            List<DNA<T>> newPopulation = new List<DNA<T>>();

            for(int i = 0; i < Population.Count; i++)
            {
                DNA<T> parent1 = ChooseParent();
                DNA<T> parent2 = ChooseParent();

                DNA<T> child = parent1.CrossOver(parent2);

                child.Mutate(mutationRate);

                newPopulation.Add(child);
            }

            Population = newPopulation;

            Generation++;
        }
                
        public void CalculateFitness()
        {
            fitnessSum = 0;
            DNA<T> best = Population[0];

            for(int i = 0; i < Population.Count; i ++)
            {
                fitnessSum += Population[i].CalculateFitness("timeLoss", sumoOutputFilePath + $"{0}.xml");

                if (Population[i].fitness > best.fitness)
                {
                    best = Population[i];
                }
            }

            BestFitness = best.fitness;
            best.genes.CopyTo(BestGenes, 0);
        }

        private DNA<T> ChooseParent()
        {
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

