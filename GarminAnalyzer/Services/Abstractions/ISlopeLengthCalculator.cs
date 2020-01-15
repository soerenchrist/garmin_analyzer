using System.Threading.Tasks;

namespace GarminAnalyzer.Services.Abstractions
{
    public interface ISlopeLengthCalculator
    {
        Task<double> CalculateLength();
    }
}