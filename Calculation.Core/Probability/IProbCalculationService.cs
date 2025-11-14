using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculation.Core.Service.Probability
{
    public interface IProbCalculationService
    {
        Task<double> CombinedWithAsync(double probA, double probB);
        Task<double> EitherAsync(double pA, double pB);
    }
}
