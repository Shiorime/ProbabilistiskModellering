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
    public class ProgramTest
    { 
        [TestMethod]
        /* Testing the generation of a random RYG-state */
        /* consider DataRows / Random Seeds */
        public void GenerateRandomRedYellowGreenState_ExpectedRes1OrRes2Value_ReturnsString()
        {
            //arrange
            Program testProgram = new Program();
            string res1 = "GGGrrrrGGrr";
            string expected = "rrrGGGGrrGG";

            //act
            string testString = testProgram.GenerateRandomRedYellowGreenState();
            if (testString.Equals(res1))
                testString = expected;

            //assert
            Assert.AreEqual(testString, expected);

        }
    }
}