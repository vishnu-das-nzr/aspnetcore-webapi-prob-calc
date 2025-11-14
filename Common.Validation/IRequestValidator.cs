using Common.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Validator.Service
{
    public interface IRequestValidator
    {
        ReqValidationResult ProbCalcValidate(ProbCalculationRequest request);
    }
}
