using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProbabilistiskModellering;

namespace ProgramTest
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void GetSpecificXMLAttributeFromFile_ExpectedFirstArrayElement_ReturnsArray()
        {
            // Arrange
            Program test = new Program();
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
            Program test = new Program();
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
            result = test.CalculateFitnessFunction(test, element, attribute, filePath);

            // Assert
            Assert.

        }
    }
}
