using System;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace ProbabilistiskModellering
{
    public class DNA<T>
    {
        // Array for genes for every individual
        public T[] genes { get; private set; }

        // Fitness, that will calculate fitness in fitnessFunction
        public double fitness { get; private set; }
        private Random random = new Random();
        private Func<T> GetRandomGene;
        public double fitnessFunction;

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

        public DNA(int size, Func<T> GetRandomGene, double fitnessFunction, bool shouldInitializeGenes = true)
        {
            genes = new T[size];
            this.GetRandomGene = GetRandomGene;
            this.fitnessFunction = fitnessFunction;
           
            if(shouldInitializeGenes)
            {
                for (int i = 0; i < genes.Length; i++)
                {
                    genes[i] = GetRandomGene();
                }
            } 
        }

        // method for calculating fitness for each individual 
        // fitness is currently the average time loss for each car
        public double CalculateFitnessIndividual(string element, string attribute, string filePath)
        {
            int min = 5;
            int max = 30;
            string[] timeLossArray = GetSpecificXMLAttributeFromFile(element, attribute, filePath);
            int cars = timeLossArray.Count();
            double timeLossSum = 0.0;
            for (int i = 0; i < cars; i++)
            {
                timeLossSum += double.Parse(timeLossArray[i], CultureInfo.InvariantCulture);
            }

            // ide til anden matematisk model https://math.stackexchange.com/questions/384613/exponential-function-with-values-between-0-and-1-for-x-values-between-0-and-1

            fitness = 1 - (timeLossSum / cars - min ) / (max - min);
            return fitness;

        }

        public string[] GetSpecificXMLAttributeFromFile(string element, string attribute, string filePath)
        {
            //https://stackoverflow.com/questions/933687/read-xml-attribute-using-xmldocument
            XmlDocument xmlDoc = new XmlDocument(); /* Create new XmlDocument */
            xmlDoc.Load(filePath);                  /* Load the xml file from filePath */
            XmlNodeList list = xmlDoc.GetElementsByTagName($"{element}"); /* Find elements with interval. Put it into an array/list */
            string[] finalArray = new string[list.Count];

            for (int i = 0; i < list.Count; ++i)
            {
                finalArray[i] = list[i].Attributes[$"{attribute}"].Value;
            }

            return finalArray;
        }

        public DNA<T> CrossOver(DNA <T> otherParent)
        {
            // Child initializes
            DNA<T> child = new DNA<T>(genes.Length, GetRandomGene, fitnessFunction , false);

            //for(int i = 0; i < genes.Length; i++)
            //{
            //    When genes split between 2 parents, then it is random who survives.
            //    This is why we use random. If it is less than 0.5 then it is genes from the first parent, else it is the other parent.
            //   child.genes[i] = random.NextDouble() < 0.5 ? (genes[i]) : otherParent.genes[i];
            //}

            int crossNumber = random.Next(0, genes.Length + 1);
            int i = 0;

            while(i - crossNumber < 0)
            {
                child.genes[i] = genes[i++];
            }

            while(i < genes.Length)
            {
                child.genes[i] = otherParent.genes[i++];
            }
            Console.WriteLine(child.genes[1200]);
            return child;
        }

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
