using System;
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
            string filePath = "./SUMOFiles/XMLTest.xml";
            string[] result = new string[1];

            // Act
            result = test.GetSpecificXMLAttributeFromFile("timeLoss", filePath);

            // Assert
            Assert.AreEqual(result[0], "6.78");

        }
        
    }
}
