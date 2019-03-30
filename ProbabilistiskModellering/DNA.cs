using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C2.ProbabilistiskModellering.DNA
{
    public class DNA<T>
    {
        // Array for genes for every individual
        public T[] genes { get; private set; }

        // Fitness, that will calculate fitness in fitnessFunction
        public float fitness { get; private set; }
        private Random random;
        private Func<T> getRandomGene;
        Func<float, int> fitnessFunction;

        public DNA(int size, Random random, Func<T> getRandomGene, Func<float, int> fitnessFunction, bool shouldInitializeGenes = true)
        {
            genes = new T[size];
            this.random = random;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;
            
            if(shouldInitializeGenes)
            {
                for (int i = 0; i < genes.Length; i++)
                {
                    genes[i] = getRandomGene();
                }
            } 
        }

        // Calculate fitness for natural selection
        public float CalculateFitness(int index)
        {
            // TODO
            fitness = fitnessFunction(index);
            return fitness;
        }

        public DNA<T> CrossOver(DNA <T> otherParent)
        {
            // Child initializes
            DNA<T> child = new DNA<T>(genes.Length, random, getRandomGene, fitnessFunction, shouldInitializeGenes: false);

            for(int i = 0; i < genes.Length; i++)
            {
                // When genes split between 2 parents, then it is random who survives.
                // This is why we use random. If it is less than 0.5 then it is genes from the first parent, else it is the other parent.

                child.genes[i] = random.NextDouble() < 0.5 ? (genes[i]) : otherParent.genes[i]; 
            }
            return child;
        }

        public void Mutate(float mutationRate)
        {
            for(int i = 0; i < genes.Length; i++)
            {
                genes[i] = getRandomGene();
            }
        }

    }
}
