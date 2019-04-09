using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProbabilistiskModellering;
using System.Globalization;
using System.Threading;

namespace ProbabilistiskModellering
{
    public class DNA<T>
    {
        // Array for genes for every individual
        public T[] genes { get; private set; }

        // Fitness, that will calculate fitness in fitnessFunction
        public float fitness { get; private set; }
        private Random random = new Random();
        private Func<T> GetRandomGene;
        public double FitnessFunction;

        public DNA(int size, Func<T> GetRandomGene, bool shouldInitializeGenes = true)
        {
            genes = new T[size];
            this.GetRandomGene = GetRandomGene;

            if (shouldInitializeGenes)
            {
                for (int i = 0; i < genes.Length; i++)
                {
                    genes[i] = GetRandomGene();
                }
            }
        }

        public DNA(int size, Func<T> GetRandomGene, double FitnessFunction, bool shouldInitializeGenes = true)
        {
            genes = new T[size];
            this.GetRandomGene = GetRandomGene;
            this.FitnessFunction = FitnessFunction;
            
            if(shouldInitializeGenes)
            {
                for (int i = 0; i < genes.Length; i++)
                {
                    genes[i] = GetRandomGene();
                }
            } 
        }

        // Calculate fitness for natural selection
        public float CalculateFitness(int index, string attribute, string filePath)
        {
            // TODO
            //fitness = FitnessFunction(index);
            return fitness;
        }

        public DNA<T> CrossOver(DNA <T> otherParent)
        {
            // Child initializes
            DNA<T> child = new DNA<T>(genes.Length, GetRandomGene, FitnessFunction ,false);

            for(int i = 0; i < genes.Length; i++)
            {
                // When genes split between 2 parents, then it is random who survives.
                // This is why we use random. If it is less than 0.5 then it is genes from the first parent, else it is the other parent.
                child.genes[i] = random.NextDouble() < 0.5 ? (genes[i]) : otherParent.genes[i]; 
            }
            return child;
        }

        // should work lmao
        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < genes.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    genes[i] = GetRandomGene();
                }
            }
        }
    }
}
