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
    class GeneticAlgorithmTest
    {
        [TestMethod]

        public void GeneticAlgorithmConstructor_IfTheConstructorAssignsCorrectly_ReturnsList()
        {
            //arrange
            Program pg = new Program();
            Random random = new Random();
            int _dnaSize = 10;
            int _popSize = 2;

            //act
            GeneticAlgorithm<string> ga = new GeneticAlgorithm<string>(_popSize, _dnaSize, random, pg.GenerateRandomRedYellowGreenState, 0.05f);

            //assert
            Assert.AreEqual(_dnaSize, ga.dnaSize);
            Assert.AreEqual(_popSize, ga.population.Count);
        }

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
        public void CompareDNA_ExpectedFitnessFromPairOfDNAComparison_ReturnsInt()
        {
            //arrange
            _DNA a = new _DNA(1);
            _DNA b = new _DNA(2);
            int firstSmaller;
            int firstLarger;
            int firstEqual;

            //act
            firstSmaller = CompareDNAModified(a, b);
            firstLarger = CompareDNAModified(b, a);
            firstEqual = CompareDNAModified(a, a);

            //assert
            Assert.AreEqual(firstSmaller, 1);
            Assert.AreEqual(firstLarger, -1);
            Assert.AreEqual(firstEqual, 0);
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