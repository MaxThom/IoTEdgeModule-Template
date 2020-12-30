using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModuleTelemetry.IotHub.Test
{
    [TestClass]
    public class AzureIotHubServiceWorkerTest
    {
        [TestMethod]
        public void Square2_Result_NoError()
        {
            // Arrange
            int x = 5;

            // Act
            var y = AzureIotHubServiceWorker.Square(x);

            // Assert
            Assert.AreEqual(x * x, y);
        }
    }
}