using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProbabilistiskModellering;

namespace ProgramTest
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void MainAsync_TypesAreCorrect_ReturnsInt()
        {
            // Arrange
            var program = new Program();

            // Act
            var result = program.MainAsync(new string { StopwatchStart = true });


            // Assert

        }
        
    }
}
