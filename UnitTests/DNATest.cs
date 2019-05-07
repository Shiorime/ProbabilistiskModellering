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
    public class DNATest
    {

        [TestMethod]
        public void GetSpecificXMLAttributeFromFile_ExpectedFirstArrayElement_ReturnsArray()
        {
            
            // Arrange
            DNA<string> test = new DNA<string>(0, TestString, 0.0, true);
            string filePath = "./../../../../TestFiles/XMLTest.xml";
            string[] result = new string[1];

            // Act
            result = test.GetSpecificXMLAttributeFromFile("tripinfo", "timeLoss", filePath);

            // Assert
            Assert.AreEqual(result[0], "6.78");
        }

        [TestMethod]
        public void CalculateFitnessIndividual_DoesCorrectCalculation_ReturnsDouble()
        {
            // Arrange
            DNA<string> test = new DNA<string>(0, TestString, 0.0, true);
            string filePath = "./../../../../TestFiles/XMLTest.xml";
            string element = "tripinfo";
            string attribute = "timeLoss";
            string[] timeLossArray = test.GetSpecificXMLAttributeFromFile("tripinfo", "timeLoss", filePath);
            int cars = timeLossArray.Count();
            double timeLossSum = 0.0;
            for (int i = 0; i < cars; i++)
            {
                timeLossSum += double.Parse(timeLossArray[i]);
            }
            double result = timeLossSum;

            double expected = 1 - ((6.78 / 1 - 5) / (25));

            // Act
            result = test.CalculateFitnessIndividual(element, attribute, filePath);

            // Assert
            Assert.AreEqual(result, expected, 0.1);
                
        }

        string TestString()
        {
            return "GGGGGGGGGGG";
        }

    }

}