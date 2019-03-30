using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbabilistiskModellering
{
    public class DNA<T>
    {
        // Array til gener til hvert individ // 
        public T[] Genes { get; private set; }
        // Fittness, som vil udregnes i en fitness funktion
        public float Fitness { get; private set; }
        private Random random;
        private Func<T> GetRandomGene;
        Func<float, int> FitnessFunction;

        public DNA(int size, Random random, Func<T> GetRandomGene, Func<float, int> FitnessFunction, bool ShouldInitializeGenes = true)
        {
            Genes = new T[size];
            this.random = random;
            this.GetRandomGene = GetRandomGene;
            this.FitnessFunction = FitnessFunction;
            
            if( ShouldInitializeGenes)
            {
                for (int i = 0; i < Genes.Length; i++)
                {
                    Genes[i] = GetRandomGene();
                }
            }
                     
        }

        // udregne fitness til naturlig selektion
        public float CalculateFitness(int index)
        {
            // ikke lavet
            Fitness = FitnessFunction(index);
            return Fitness;
        }

        

        public DNA<T> CrossOver(DNA <T> otherParent)
        {
            // barnet initialiseres
            DNA<T> child = new DNA<T>(Genes.Length, random, GetRandomGene, FitnessFunction, ShouldInitializeGenes: false);

            for(int i = 0; i < Genes.Length; i++)
            {
                // når gener deles mellem to forældre, så er det tilfældigt, hvilke der overlever
                // Derfor bruger vi random. Hvis den er mindre en 0.5 er det gener fra den første, ellers er det fra den anden

                child.Genes[i] = random.NextDouble() < 0.5 ? (Genes[i]) : otherParent.Genes[i]; 
            }
            return child;
        }

        public void Mutate(float mutationRate)
        {
            for(int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = GetRandomGene();
            }
        }

    }
}
