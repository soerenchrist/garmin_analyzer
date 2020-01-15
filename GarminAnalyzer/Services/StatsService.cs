using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarminAnalyzer.Models;
using GarminAnalyzer.Repositories.Abstractions;
using GarminAnalyzer.Services.Abstractions;
using GarminAnalyzer.ViewModels;

namespace GarminAnalyzer.Services
{
    public class StatsService : IStatsService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IWayRepository _wayRepository;
        private readonly ISlopeLengthCalculator _slopeLengthCalculator;

        public StatsService(IActivityRepository activityRepository,
            IWayRepository wayRepository,
            ISlopeLengthCalculator slopeLengthCalculator)
        {
            _activityRepository = activityRepository;
            _wayRepository = wayRepository;
            _slopeLengthCalculator = slopeLengthCalculator;
        }

        public async Task<Statistics> CalculateStatistics(IEnumerable<TrackingPoint> trackingPoints)
        {
            
            var allActivities = _activityRepository.GetAllActivities();
            var enumerable = trackingPoints as TrackingPoint[] ?? trackingPoints.ToArray();

            var countAll = enumerable.Length;
            var countEasy =
                enumerable.Count(a => a.DistanceConnection?.NearestWay?.Difficulty == "easy");
            var countIntermediate = enumerable.Count(a =>
                a.DistanceConnection?.NearestWay?.Difficulty == "intermediate");
            var countAdvanced = enumerable.Count(a =>
                a.DistanceConnection?.NearestWay?.Difficulty == "advanced");

            var slopeLength = await _slopeLengthCalculator.CalculateLength();
            
            var activities = allActivities as Lap[] ?? allActivities.ToArray();
            if (countAll == 0)
            {
                return new Statistics();
            }
            var statistics = new Statistics
            {
                MaxSpeed = enumerable.Max(t => t.Speed) * 3.6,
                MaxHeartRate = enumerable.Max(t => t.HeartRate),
                PercentageEasy = (double) countEasy / countAll * 100,
                PercentageAdvanced = (double) countAdvanced / countAll * 100,
                PercentageIntermediate = (double) countIntermediate / countAll * 100,
                TotalAltitude = activities.Sum(a => a.TotalDistance),
                TotalDistance = activities.SelectMany(a => a.TrackingPoints).Sum(a => a.Altitude) / 1000000,
                SlopeLength = slopeLength / 1000,
                Loading = false
            };

            return statistics;
        }

        public LiftStatistics CalculateLiftStatistics(IEnumerable<Way> takenLifts)
        {
            var statistics = new LiftStatistics
            {
                TotalLiftCount = _wayRepository.GetTotalLiftCount(),
                ChairLiftCount = _wayRepository.GetChairLiftCount(),
                GondolaLiftCount = _wayRepository.GetGondolaLiftCount(),

                TakenLiftCount = takenLifts.Distinct().Count()
            };
            return statistics;
        }
    }
}