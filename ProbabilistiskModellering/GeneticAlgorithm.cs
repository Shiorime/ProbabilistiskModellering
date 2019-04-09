using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbabilistiskModellering
{
    public class GeneticAlgorithm<T>
    {
        public List<DNA<T>> Population { get; private set; }
        public int generation { get; private set; }
        public float bestFitness { get; private set; }
        public T[] bestGenes { get; private set; }
        
        public float mutationRate;
        private Random random;

        private List<DNA<T>> newPopulation;
        private float fitnessSum;
        private int dnaSize;
        private Func<T> getRandomGene;
        private double fitnessFunction;

        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, double fitnessFunction, float mutationRate = 0.01f)
        {
            generation = 1;
            this.mutationRate = mutationRate;
            Population = new List<DNA<T>>(populationSize);
            newPopulation = new List<DNA<T>>(populationSize);
            this.random = random;
            this.dnaSize = dnaSize;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;

            bestGenes = new T[dnaSize];


            for( int i = 0; i < populationSize; i++)
            {
                Population.Add(new DNA<T>(dnaSize, getRandomGene, fitnessFunction, true));
            }
        }

        

      
    }
}
