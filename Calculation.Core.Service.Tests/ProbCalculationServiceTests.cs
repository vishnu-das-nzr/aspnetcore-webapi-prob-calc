using Calculation.Core.Service.Probability;
using Common.Logging.Service;
using Moq;

namespace Calculation.Core.Service.Tests
{
    public class ProbCalculationServiceTests
    {
        [Fact]
        public async Task CombinedWithAsync_ReturnsProduct_And_Logs()
        {
            // Arrange
            var mockLogger = new Mock<IActivityLoggerService>();
            mockLogger
                .Setup(l => l.LogActivityAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = new ProbCalculationService(mockLogger.Object);

            double pA = 0.5;
            double pB = 0.2;
            double expectedResult = pA * pB;
            // Build expected log text exactly as the service does (ToString("F4"))
            string expectedLog = $"{{ Input: [ P(A)={pA.ToString("F4")}, P(B)={pB.ToString("F4")} ] , Result: [ {expectedResult.ToString("F4")} ] }}";

            // Act
            var result = await service.CombinedWithAsync(pA, pB);

            // Assert result
            Assert.Equal(expectedResult, result, precision: 10);

            // Assert logger called with correct operation name and details
            mockLogger.Verify(
                l => l.LogActivityAsync("CombinedWith", expectedLog),
                Times.Once);
        }

        [Fact]
        public async Task EitherAsync_ReturnsUnion_And_Logs()
        {
            // Arrange
            var mockLogger = new Mock<IActivityLoggerService>();
            mockLogger
                .Setup(l => l.LogActivityAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = new ProbCalculationService(mockLogger.Object);

            double pA = 0.5;
            double pB = 0.3;
            double expectedResult = pA + pB - pA * pB;
            string expectedLog = $"{{ Input: [ P(A)={pA.ToString("F4")}, P(B)={pB.ToString("F4")} ] , Result: [ {expectedResult.ToString("F4")} ] }}";

            // Act
            var result = await service.EitherAsync(pA, pB);

            // Assert result
            Assert.Equal(expectedResult, result, precision: 10);

            // Assert logger called with correct operation name and details
            mockLogger.Verify(
                l => l.LogActivityAsync("Either", expectedLog),
                Times.Once);
        }
    }
}

