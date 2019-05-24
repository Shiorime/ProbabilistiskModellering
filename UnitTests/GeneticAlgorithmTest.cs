using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProbabilistiskModellering;

namespace UnitTests
{
    [TestClass]
    public class GeneticAlgorithmTest
    {
        [TestMethod]
        /* Testing the GeneticAlgorithm constructor, specifically if population count is being assigned. */
        public void GeneticAlgorithmConstructor_IfTheConstructorAssignsCorrectly_ReturnsList()
        {
            //arrange
            Program pg = new Program();
            Random random = new Random(1);
            int expected = 2;

            //act
            GeneticAlgorithm<string> ga = new GeneticAlgorithm<string>(2, 10, 2, 5, 0.5, random, pg.GenerateRandomRedYellowGreenState, 0.05f);

            //assert
            Assert.AreEqual(ga.population.Count, expected);
        }
        /* Simplified CompareDNA method in order to reduce arrangement requirements */
        private int CompareDNAModified(_DNA a, _DNA b)
        {
            if (a.fitness > b.fitness)
            {
                return -1;
            }
            else if (a.fitness < b.fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        [TestMethod]
        /* Testing the CompareDNA method for all three possible outcomes. */
        [DataRow(1,2,1)]
        [DataRow(2,1,-1)]
        [DataRow(1,1,0)]
        public void CompareDNA_ExpectedFitnessFromPairOfDNAComparison_ReturnsInt(int x, int y, int expected)
        {
            //arrange
            _DNA a = new _DNA(x);
            _DNA b = new _DNA(y);
            int c;

            //act
            c = CompareDNAModified(a, b);
            
            //assert
            Assert.AreEqual(c, expected);
        }

        [TestMethod]
        [DataRow(0.5,20)]
        [DataRow(0.1, 25)]
        [DataRow(0.9, 10)]
        [DataRow(2,9001)]
        public void ChooseParentTest_ExpectedFitnessScore_ReturnsDouble(double x, double y)
        {
            //Arrange
            double[] fitnessArray = new double[] { 25, 20, 15, 10, 5 };
            double randomFake = x;
            double randomNumber = randomFake * (75);
            double expected = y;
            double result;

            //Act
            result = ChooseParentModified(fitnessArray, randomNumber);

            //Assert
            Assert.AreEqual(result, expected);
        }

        public double ChooseParentModified(double[] fitnessArray, double randomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                if (randomNumber < fitnessArray[i])
                {
                    return fitnessArray[i];
                }
                randomNumber -= fitnessArray[i];
            }
            return 9001;
        }


    }
    





    public class _DNA
    {
        public int fitness;
        public _DNA(int fitness)
        {
            this.fitness = fitness;
        }
    }
}