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
            string filePath = "./TestFiles/XMLTest.xml";
            string[] result = new string[1];

            // Act
            result = test.GetSpecificXMLAttributeFromFile("tripinfo", "timeLoss", filePath);

            // Assert
            Assert.AreEqual(result[0], "6.78");
        }

        [TestMethod]
        public void CalculateFitnessFunction_DoesCorrectCalculation_ReturnsDouble()
        {
            // Arrange
            DNA<string> test = new DNA<string>(0, TestString, 0.0, true);
            string filePath = null;
            string attribute = null;
            string element = null;
            string[] timeLossArray = test.GetSpecificXMLAttributeFromFile(element, attribute, filePath);
            int cars = timeLossArray.Count();
            double timeLossSum = 0.0;
            for (int i = 0; i < cars; i++)
            {
                timeLossSum += double.Parse(timeLossArray[i], CultureInfo.InvariantCulture);
            }
            double result = new double();

            // Act
            result = test.CalculateFitnessIndividual(element, attribute, filePath);

            // Assert
            Assert.
                
        }

        string TestString()
        {
            return "GGGGGGGGGGG";
        }

    }

}