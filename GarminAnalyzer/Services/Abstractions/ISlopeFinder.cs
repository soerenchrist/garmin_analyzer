using System.Collections.Generic;
using GarminAnalyzer.Models;

namespace GarminAnalyzer.Services.Abstractions
{
    /// <summary>
    /// Try to find a Slope near a tracking point to determine which slope has been used
    /// </summary>
    public interface ISlopeFinder
    {
        Distance FindPisteForTrackingPoint(TrackingPoint trackingPoint);
    }
}