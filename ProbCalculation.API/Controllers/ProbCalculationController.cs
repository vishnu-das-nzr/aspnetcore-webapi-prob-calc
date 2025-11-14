using Calculation.Core.Service.Probability;
using Common.Models.Service;
using Common.Validator.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ProbCalculation.API.Controllers
{
    [Route("api/probabilities")]
    [ApiController]
    public class ProbCalculationController : ControllerBase
    {
        private readonly IProbCalculationService _probCalculationService;
        private readonly IRequestValidator _requestValidator;
        private readonly ILogger<ProbCalculationController> _logger;
        
        public ProbCalculationController(IProbCalculationService probCalculationService, IRequestValidator requestValidator, ILogger<ProbCalculationController> logger)
        {
            _probCalculationService = probCalculationService;
            _requestValidator = requestValidator;
            _logger = logger;
        }

        [HttpPost("combinedwith")]
        public async Task<IActionResult> CombinedWith([FromBody] ProbCalculationRequest request)
        {
            var validationResult = _requestValidator.ProbCalcValidate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "errors", validationResult.Errors.ToArray() }
                }));
            }

            try
            {
                // Calculate result
                var result = await _probCalculationService.CombinedWithAsync(request.ProbabilityA, request.ProbabilityB);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during {{CombinedWith}} action execution.");

                return StatusCode(500, new
                {
                    error = "An unexpected error occurred during calculation."
                });
            }
        }

        [HttpPost("either")]
        public async Task<IActionResult> Either([FromBody] ProbCalculationRequest request)
        {
            var validationResult = _requestValidator.ProbCalcValidate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "errors", validationResult.Errors.ToArray() }
                }));
            }

            try
            {
                var result = await _probCalculationService.EitherAsync(request.ProbabilityA, request.ProbabilityB);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during {{Either}} action execution.");

                return StatusCode(500, new
                {
                    error = "An unexpected error occurred during calculation."
                });
            }
        }
    }

}
