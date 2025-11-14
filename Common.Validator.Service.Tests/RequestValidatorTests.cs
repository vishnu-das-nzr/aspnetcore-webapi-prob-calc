using Common.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Validator.Service.Tests
{
    public class RequestValidatorTests
    {
        private readonly RequestValidator _validator = new RequestValidator();

        [Fact]
        public void ProbCalcValidate_RequestIsNull_ReturnsInvalidWithNullError()
        {
            // Act
            var result = _validator.ProbCalcValidate(null);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Request cannot be null", result.Errors);
        }

        [Fact]
        public void ProbCalcValidate_ValidProbabilities_ReturnsValid()
        {
            // Arrange
            var request = new ProbCalculationRequest
            {
                ProbabilityA = 0.25,
                ProbabilityB = 0.75
            };

            // Act
            var result = _validator.ProbCalcValidate(request);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData(-0.01, 0.5, "ProbabilityA must be between 0 and 1")]
        [InlineData(0.5, 1.01, "ProbabilityB must be between 0 and 1")]
        public void ProbCalcValidate_OutOfRangeProbabilities_ReturnsRangeErrors(double a, double b, string expectedErrorSnippet)
        {
            // Arrange
            var request = new ProbCalculationRequest
            {
                ProbabilityA = a,
                ProbabilityB = b
            };

            // Act
            var result = _validator.ProbCalcValidate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.IndexOf(expectedErrorSnippet, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        [Fact]
        public void ProbCalcValidate_NaNOrInfinity_ReturnsNumericErrors()
        {
            // Arrange
            var request = new ProbCalculationRequest
            {
                ProbabilityA = double.NaN,
                ProbabilityB = double.PositiveInfinity
            };

            // Act
            var result = _validator.ProbCalcValidate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.IndexOf("ProbabilityA must be a valid number", StringComparison.OrdinalIgnoreCase) >= 0);
            Assert.Contains(result.Errors, e => e.IndexOf("ProbabilityB must be a valid number", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        [Fact]
        public void ProbCalcValidate_MultipleProblems_AccumulatesAllErrors()
        {
            // Arrange: A is NaN and out of range, B is negative
            var request = new ProbCalculationRequest
            {
                ProbabilityA = double.NaN,    // triggers numeric error
                ProbabilityB = -0.5           // triggers range error
            };

            // Act
            var result = _validator.ProbCalcValidate(request);

            // Assert
            Assert.False(result.IsValid);

            // Expect at least two errors: one about ProbabilityA numeric validity and one about ProbabilityB range
            Assert.Contains(result.Errors, e => e.IndexOf("ProbabilityA must be a valid number", StringComparison.OrdinalIgnoreCase) >= 0
                                              || e.IndexOf("ProbabilityA must be between 0 and 1", StringComparison.OrdinalIgnoreCase) >= 0);

            Assert.Contains(result.Errors, e => e.IndexOf("ProbabilityB must be between 0 and 1", StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
