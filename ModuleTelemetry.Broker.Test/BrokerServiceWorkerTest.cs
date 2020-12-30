using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModuleTelemetry.Broker.Test
{
    [TestClass]
    public class BrokerServiceWorkerTest
    {
        [TestMethod]
        public void Square3_Result_NoError()
        {
            // Arrange
            int x = 5;

            // Act
            var y = BrokerServiceWorker.Square(x);

            // Assert
            Assert.AreEqual(x * x, y);
        }
    }
}