using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbabilistiskModellering
{
    public class DNA<T>
    {
        // Array for genes for every individual
        public T[] genes { get; private set; }

        // Fitness, that will calculate fitness in fitnessFunction
        public float fitness { get; private set; }
        private Random random;
        private Func<T> GetRandomGene;
        Func<int, float> FitnessFunction;
        string randomState;

        public DNA(int size, Random random, Func<T> GetRandomGene, Func<int, float> FitnessFunction, bool shouldInitializeGenes = true)
        {
            genes = new T[size];
            this.random = random;
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
        public float CalculateFitness(int index)
        {
            // TODO
            fitness = FitnessFunction(index);
            return fitness;
        }

        public DNA<T> CrossOver(DNA <T> otherParent)
        {
            // Child initializes
            DNA<T> child = new DNA<T>(genes.Length, random, GetRandomGene, FitnessFunction, shouldInitializeGenes: false);

            for(int i = 0; i < genes.Length; i++)
            {
                // When genes split between 2 parents, then it is random who survives.
                // This is why we use random. If it is less than 0.5 then it is genes from the first parent, else it is the other parent.

                child.genes[i] = random.NextDouble() < 0.5 ? (genes[i]) : otherParent.genes[i]; 
            }
            return child;
        }

        Random rand = new Random();
        public string GenerateRandomRedYellowGreenState()
        {
            randomState = string.Empty;
            int result = rand.Next(0, 2);
            for (int i = 0; i < 11; i++)
            {
                if (result == 0)
                {
                    randomState = randomState + "r";
                }
                else if (result == 1)
                {
                    randomState = randomState + "G";
                }
                result = rand.Next(0, 2);
            }
            return randomState;
        }

        public void Mutate(float mutationRate)
        {
            for(int i = 0; i < genes.Length; i++)
            {
                genes[i] = GetRandomGene();
            }
        }

    }
}
