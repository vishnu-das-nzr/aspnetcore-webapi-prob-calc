using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging.Service.Tests
{
    public class ActivityLoggerServiceTests : IDisposable
    {
        private readonly string _testLogFile;

        public ActivityLoggerServiceTests()
        {
            // Setup: Create a unique test file path
            _testLogFile = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.txt");
        }

        public void Dispose()
        {
            // Cleanup: Delete test file after each test
            if (File.Exists(_testLogFile))
            {
                File.Delete(_testLogFile);
            }
        }

        private IConfiguration CreateTestConfiguration(string logPath)
        {
            var config = new Dictionary<string, string>
            {
                { "ActivityLogging:ProbabilityLogFilePath", logPath }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();
        }

        [Fact]
        public async Task LogActivityAsync_CreatesFileAndWritesLog()
        {
            // Arrange
            var config = CreateTestConfiguration(_testLogFile);
            var logger = new ActivityLoggerService(config);

            // Act
            await logger.LogActivityAsync("CombinedWith", "P(A)=0.5, P(B)=0.3, Result=0.15");

            // Assert
            Assert.True(File.Exists(_testLogFile));
            var content = await File.ReadAllTextAsync(_testLogFile);
            Assert.Contains("CombinedWith", content);
            Assert.Contains("P(A)=0.5", content);
            Assert.Contains("Result=0.15", content);
        }

        [Fact]
        public async Task LogActivityAsync_AppendsMultipleLogs()
        {
            // Arrange
            var config = CreateTestConfiguration(_testLogFile);
            var logger = new ActivityLoggerService(config);

            // Act
            await logger.LogActivityAsync("CombinedWith", "Entry 1");
            await logger.LogActivityAsync("Either", "Entry 2");

            // Assert
            var content = await File.ReadAllTextAsync(_testLogFile);
            Assert.Contains("Entry 1", content);
            Assert.Contains("Entry 2", content);
        }

        [Fact]
        public async Task LogActivityAsync_IncludesTimestamp()
        {
            // Arrange
            var config = CreateTestConfiguration(_testLogFile);
            var logger = new ActivityLoggerService(config);

            // Act
            await logger.LogActivityAsync("Either", "Test");

            // Assert
            var content = await File.ReadAllTextAsync(_testLogFile);
            Assert.Matches(@"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} UTC\]", content);
        }

        [Fact]
        public async Task LogActivityAsync_HasCorrectFormat()
        {
            // Arrange
            var config = CreateTestConfiguration(_testLogFile);
            var logger = new ActivityLoggerService(config);

            // Act
            await logger.LogActivityAsync("TestOp", "TestDetails");

            // Assert
            var content = await File.ReadAllTextAsync(_testLogFile);
            Assert.Contains("Op: TestOp", content);
            Assert.Contains("Details: TestDetails", content);
        }

        [Fact]
        public void Constructor_UsesFallbackPath_WhenConfigEmpty()
        {
            // Arrange
            var config = CreateTestConfiguration("");
            var fallbackPath = Path.Combine(AppContext.BaseDirectory, "probability_activity.txt");

            // Act
            var logger = new ActivityLoggerService(config);
            logger.LogActivityAsync("Test", "Data").Wait();

            // Assert
            Assert.True(File.Exists(fallbackPath));

            // Cleanup
            if (File.Exists(fallbackPath))
            {
                File.Delete(fallbackPath);
            }
        }

        [Fact]
        public void Constructor_CreatesDirectory_WhenNotExists()
        {
            // Arrange
            var newDir = Path.Combine(Path.GetTempPath(), $"test_dir_{Guid.NewGuid()}");
            var logPath = Path.Combine(newDir, "log.txt");
            var config = CreateTestConfiguration(logPath);

            // Act
            var logger = new ActivityLoggerService(config);
            logger.LogActivityAsync("Test", "Data").Wait();

            // Assert
            Assert.True(Directory.Exists(newDir));
            Assert.True(File.Exists(logPath));

            // Cleanup
            if (Directory.Exists(newDir))
            {
                Directory.Delete(newDir, true);
            }
        }
    }
}
