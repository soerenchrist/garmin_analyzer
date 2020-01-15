using GarminAnalyzer.Models;

namespace GarminAnalyzer.Services.Abstractions
{
    /// <summary>
    ///     Try to find the nearest lift for a position to check which lift has been taken
    /// </summary>
    public interface ILiftFinder
    {
        Way GetLiftForStartPoint(Position position);
    }
}