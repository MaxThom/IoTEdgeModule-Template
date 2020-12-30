using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModuleTelemetry.Test
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void Square_Result_NoError()
        {
            // Arrange
            int x = 5;

            // Act
            var y = Program.Square(x);

            // Assert
            Assert.AreEqual(x * x, y);
        }
    }
}