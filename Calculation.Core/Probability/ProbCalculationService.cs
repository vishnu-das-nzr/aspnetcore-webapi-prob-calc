using Common.Logging.Service;
using Common.Validator.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculation.Core.Service.Probability
{
    public class ProbCalculationService: IProbCalculationService
    {

        private readonly IActivityLoggerService _activityLoggerService;
        public ProbCalculationService(
            IActivityLoggerService activityLoggerService)
        {
            _activityLoggerService = activityLoggerService;
        }

        /// <summary>
        /// Calculates P(A) AND P(B): P(A) * P(B)
        /// </summary>
        public async Task<double> CombinedWithAsync(double pA, double pB)
        {
            double result = pA * pB;
            string logDetails = BuildLogDetails(pA, pB, result);

            // await logger asynchronously; ConfigureAwait(false) since this is library code
            await _activityLoggerService.LogActivityAsync("CombinedWith", logDetails).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Calculates P(A) OR P(B): P(A) + P(B) - P(A)P(B)
        /// </summary>
        public async Task<double> EitherAsync(double pA, double pB)
        {
            double result = pA + pB - pA * pB;
            string logDetails = BuildLogDetails(pA, pB, result);
            await _activityLoggerService.LogActivityAsync("Either", logDetails).ConfigureAwait(false);

            return result;
        }

        private static string BuildLogDetails(double pA, double pB, double result)
        {
            string a = pA.ToString("F4");
            string b = pB.ToString("F4");
            string r = result.ToString("F4");

            return $"{{ Input: [ P(A)={a}, P(B)={b} ] , Result: [ {r} ] }}";
        }

    }
}
