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
        private readonly List<string> _filesToCleanup = new();

        public void Dispose()
        {
            foreach (var f in _filesToCleanup)
            {
                try
                {
                    if (File.Exists(f)) File.Delete(f);
                }
                catch { /* best-effort cleanup */ }
            }
        }

        [Fact]
        public async Task LogActivityAsync_WithConfiguredAbsolutePath_WritesExpectedLogEntry()
        {
            // Arrange - create temp folder and file path
            var tempDir = Path.Combine(Path.GetTempPath(), "ActivityLoggerTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var logFile = Path.Combine(tempDir, "my_probability_activity.txt");
            _filesToCleanup.Add(logFile);

            // Build configuration with the absolute path
            var inMemory = new Dictionary<string, string?>
            {
                ["ActivityLogging:ProbabilityLogFilePath"] = logFile
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            var service = new ActivityLoggerService(configuration);

            var operation = "CombinedWith";
            var details = "Input: { P(A)=0.5, P(B)=0.5 } , Result: { 0.2500 }";

            // Act
            await service.LogActivityAsync(operation, details);

            // Assert - file exists and contains expected information
            Assert.True(File.Exists(logFile), "Expected log file to be created at configured location.");

            var contents = await File.ReadAllTextAsync(logFile);
            Assert.Contains(operation, contents);
            Assert.Contains(details, contents);

            // basic timestamp pattern check (UTC with milliseconds)
            Assert.Matches(@"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} UTC\]", contents);
        }

        [Fact]
        public async Task LogActivityAsync_WhenConfigMissing_UsesFallbackPathInAppBaseDirectory()
        {
            // Arrange - no configuration key provided
            var inMemory = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            // compute expected fallback path (must match implementation)
            var fallbackPath = Path.Combine(AppContext.BaseDirectory, "probability_activity.txt");

            // ensure we clean up fallback file after test
            _filesToCleanup.Add(fallbackPath);

            // Remove existing fallback file if present (clean start)
            if (File.Exists(fallbackPath))
            {
                File.Delete(fallbackPath);
            }

            var service = new ActivityLoggerService(configuration);

            var operation = "Either";
            var details = "Input: { P(A)=0.2, P(B)=0.3 } , Result: { 0.4400 }";

            // Act
            await service.LogActivityAsync(operation, details);

            // Assert
            Assert.True(File.Exists(fallbackPath), "Expected fallback log file to be created in AppContext.BaseDirectory.");

            var contents = await File.ReadAllTextAsync(fallbackPath);
            Assert.Contains(operation, contents);
            Assert.Contains(details, contents);
        }
    }
}
