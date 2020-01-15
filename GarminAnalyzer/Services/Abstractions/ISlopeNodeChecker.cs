using GarminAnalyzer.Models;

namespace GarminAnalyzer.Services.Abstractions
{
    /// <summary>
    ///     Try to find a TrackingPoint for a Node of a Slope within a few meters
    /// </summary>
    public interface ISlopeNodeChecker
    {
        TrackingPoint Check(Position node);
    }
}