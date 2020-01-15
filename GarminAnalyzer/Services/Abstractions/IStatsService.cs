using System.Collections.Generic;
using System.Threading.Tasks;
using GarminAnalyzer.Models;
using GarminAnalyzer.ViewModels;

namespace GarminAnalyzer.Services.Abstractions
{
    public interface IStatsService
    {
        Task<Statistics> CalculateStatistics(IEnumerable<TrackingPoint> trackingPoints);

        LiftStatistics CalculateLiftStatistics(IEnumerable<Way> takenLifts);
    }
}