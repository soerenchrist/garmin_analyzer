using System.Collections.Generic;
using GarminAnalyzer.Models;
using GarminAnalyzer.ViewModels;

namespace GarminAnalyzer.Services.Abstractions
{
    public interface IStatsService
    {
        Statistics CalculateStatistics(IEnumerable<TrackingPoint> trackingPoints);

        LiftStatistics CalculateLiftStatistics(IEnumerable<Way> takenLifts);
    }
}