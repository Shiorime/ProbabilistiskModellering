using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbabilistiskModellering
{
    public class GeneticAlgorithm
    {
        Random rand = new Random();
        const string charsAllowed = "Gr";
        string randomState;
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
