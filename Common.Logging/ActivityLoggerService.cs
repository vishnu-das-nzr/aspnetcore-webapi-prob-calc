using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging.Service
{
    public class ActivityLoggerService: IActivityLoggerService
    {
        private readonly string _logFilePath;

        public ActivityLoggerService(IConfiguration configuration)
        {
            // Read path from appsettings.json
            var configuredPath = configuration["ActivityLogging:ProbabilityLogFilePath"];

            // Use configured path OR default fallback path
            _logFilePath = !string.IsNullOrWhiteSpace(configuredPath)
                ? configuredPath
                : Path.Combine(AppContext.BaseDirectory, "probability_activity.txt");

            // Ensure directory exists
            var directory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        public async Task LogActivityAsync(string calculationType, string inputAndResultDetails)
        {
            try
            {
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff UTC");
                // Construct the log entry by combining timestamp, type, and the provided details string
                string logEntry = $"[{timestamp}] | Op: {calculationType} | Details: {inputAndResultDetails}";
                
                // Append the log entry string representation to the file
                await File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging activity to file: {ex.Message}");
            }
        }
    }
}
