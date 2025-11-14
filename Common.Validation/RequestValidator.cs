using Common.Models.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Validator.Service
{
    public class RequestValidator: IRequestValidator
    {
        public ReqValidationResult ProbCalcValidate(ProbCalculationRequest request)
        {
            var result = new ReqValidationResult { IsValid = true };

            if (request == null)
            {
                result.IsValid = false;
                result.Errors.Add("Request cannot be null");
                return result;
            }

            // Validate ProbabilityA
            if (!IsValidProbability(request.ProbabilityA))
            {
                result.IsValid = false;
                result.Errors.Add("ProbabilityA must be between 0 and 1 (inclusive)");
            }

            // Validate ProbabilityB
            if (!IsValidProbability(request.ProbabilityB))
            {
                result.IsValid = false;
                result.Errors.Add("ProbabilityB must be between 0 and 1 (inclusive)");
            }

            // Check for NaN or Infinity
            if (double.IsNaN(request.ProbabilityA) || double.IsInfinity(request.ProbabilityA))
            {
                result.IsValid = false;
                result.Errors.Add("ProbabilityA must be a valid number");
            }

            if (double.IsNaN(request.ProbabilityB) || double.IsInfinity(request.ProbabilityB))
            {
                result.IsValid = false;
                result.Errors.Add("ProbabilityB must be a valid number");
            }

            return result;
        }

        private bool IsValidProbability(double value)
        {
            return value >= 0.0 && value <= 1.0;
        }
    }
}
