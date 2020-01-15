using System.Device.Location;
using System.Linq;
using GarminAnalyzer.Models;
using GarminAnalyzer.Repositories;
using GarminAnalyzer.Repositories.Abstractions;
using GarminAnalyzer.Services.Abstractions;

namespace GarminAnalyzer.Services
{
    public class SlopeNodeChecker : ISlopeNodeChecker
    {
        private readonly IActivityRepository _activityRepository;

        public SlopeNodeChecker(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }
        
        public TrackingPoint Check(Position node)
        {
            const double threshold = 50;
            var allLaps = _activityRepository.GetAllActivities();
            var allTrackingPoints = allLaps.SelectMany(a => a.TrackingPoints);

            var geoNode = new GeoCoordinate(node.Latitude, node.Longitude);
            foreach (var trackingPoint in allTrackingPoints)
            {
                if (trackingPoint.Position == null) continue;
                var geoTracking = new GeoCoordinate(trackingPoint.Position.Latitude, trackingPoint.Position.Longitude);
                var distance = geoTracking.GetDistanceTo(geoNode);

                if (distance < threshold)
                {
                    return trackingPoint;
                }
            }

            return null;
        }

    }
}