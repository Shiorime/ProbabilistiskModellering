using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProbabilistiskModellering;
using System.Globalization;
using System.Threading;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Commands;

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
            fitness = timeLossSum / cars;
            // return timeLossSum / cars - min / max - min;
            return timeLossSum / cars;
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
            DNA<T> child = new DNA<T>(genes.Length, GetRandomGene, FitnessFunction , false);

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
