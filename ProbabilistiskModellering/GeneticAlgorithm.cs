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
        public float mutationRate;
        private Random random2;

        //contructor 
        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, Func<int, float> FitnessFunction, float mutationRate = 0.01f)
        {
            generation = 1;
            this.mutationRate = mutationRate;
            Population = new List<DNA<T>>();
            this.random2 = random;

            for( int i = 0; i < populationSize; i++)
            {
                Population.Add(new DNA<T>(dnaSize, random, getRandomGene, FitnessFunction, shouldInitializeGenes: true));
            }
        }

        

        // variables for generation random gene ie. trafficlightstate
        Random rand = new Random();
        string randomState;
        //function for generation random gene of size 11, will only be used for initializing, will get moved later
        public string GenerateRandomRedYellowGreenState()
        {
            for (int i = 0; i < 10; i++)
            {
                int result = rand.Next(0, 2);
                if (result == 0)
                {
                    randomState = randomState + "r";
                }
                if (rand.Next(0, 1 + 1) == 1)
                {
                    randomState = randomState + "G";
                }
            }
            return randomState;
        }
    }
}
