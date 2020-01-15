using System.Device.Location;
using System.Linq;
using GarminAnalyzer.Models;
using GarminAnalyzer.Repositories.Abstractions;
using GarminAnalyzer.Services.Abstractions;

namespace GarminAnalyzer.Services
{
    public class LiftFinder : ILiftFinder
    {
        private readonly IWayRepository _wayRepository;

        public LiftFinder(IWayRepository wayRepository)
        {
            _wayRepository = wayRepository;
        }

        public Way GetLiftForStartPoint(Position position)
        {
            if (position == null) return null;

            var lifts = _wayRepository.GetLifts();

            var minDistance = double.MaxValue;
            Way minLift = null;
            var geoPosition = new GeoCoordinate(position.Latitude, position.Longitude);
            foreach (var lift in lifts)
            {
                var exit = lift.Nodes.LastOrDefault();
                if (exit == null) continue;

                var geoLift = new GeoCoordinate(exit.Latitude, exit.Longitude);
                var distance = geoLift.GetDistanceTo(geoPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minLift = lift;
                }
            }

            return minLift;
        }
    }
}