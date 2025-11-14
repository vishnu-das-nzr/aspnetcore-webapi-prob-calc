using Calculation.Core.Service.Probability;
using Common.Logging.Service;
using Moq;

namespace Calculation.Core.Service.Tests
{
    public class ProbCalculationServiceTests
    {
        [Fact]
        public async Task CombinedWithAsync_ComputesProduct_And_Logs()
        {
            // Arrange
            var loggerMock = new Mock<IActivityLoggerService>(MockBehavior.Strict);
            // Setup expectation: called once with operation "CombinedWith" and a message containing formatted values
            loggerMock
                .Setup(l => l.LogActivityAsync(
                    It.Is<string>(op => op == "CombinedWith"),
                    It.Is<string>(msg => msg.Contains("P(A)=0.5000") && msg.Contains("P(B)=0.5000") && msg.Contains("0.2500"))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var svc = new ProbCalculationService(loggerMock.Object);

            // Act
            var result = await svc.CombinedWithAsync(0.5, 0.5);

            // Assert
            Assert.Equal(0.25, result, 6);
            loggerMock.Verify(l => l.LogActivityAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            loggerMock.Verify(); // ensures the specific Setup expectation was met
        }

        [Fact]
        public async Task EitherAsync_ComputesUnion_And_Logs()
        {
            // Arrange
            var loggerMock = new Mock<IActivityLoggerService>(MockBehavior.Strict);
            // For pA=0.5, pB=0.5 result is 0.75 -> 0.7500 formatted
            loggerMock
                .Setup(l => l.LogActivityAsync(
                    It.Is<string>(op => op == "Either"),
                    It.Is<string>(msg => msg.Contains("P(A)=0.5000") && msg.Contains("P(B)=0.5000") && msg.Contains("0.7500"))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var svc = new ProbCalculationService(loggerMock.Object);

            // Act
            var result = await svc.EitherAsync(0.5, 0.5);

            // Assert
            Assert.Equal(0.75, result, 6);
            loggerMock.Verify(l => l.LogActivityAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            loggerMock.Verify();
        }

        [Theory]
        [InlineData(0.1234, 0.8766, 0.1082)] // combined: 0.1234*0.8766=0.10820044 -> 0.1082
        [InlineData(0.0, 1.0, 0.0)]
        public async Task CombinedWithAsync_FormatsResultToFourDecimals_InLog(double a, double b, double expected)
        {
            // Arrange
            var loggerMock = new Mock<IActivityLoggerService>();
            loggerMock
                .Setup(l => l.LogActivityAsync(
                    "CombinedWith",
                    It.Is<string>(s => s.Contains(expected.ToString("F4")))))
                .Returns(Task.CompletedTask);

            var svc = new ProbCalculationService(loggerMock.Object);

            // Act
            var result = await svc.CombinedWithAsync(a, b);

            // Assert numeric result (precision tolerance)
            Assert.Equal(a * b, result, 6);

            // Verify logger called
            loggerMock.Verify(l => l.LogActivityAsync("CombinedWith", It.IsAny<string>()), Times.Once);
        }
    }
}

