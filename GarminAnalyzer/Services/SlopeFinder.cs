using System.Device.Location;
using GarminAnalyzer.Models;
using GarminAnalyzer.Repositories.Abstractions;
using GarminAnalyzer.Services.Abstractions;

namespace GarminAnalyzer.Services
{
    public class SlopeFinder : ISlopeFinder
    {
        private readonly IWayRepository _wayRepository;

        public SlopeFinder(IWayRepository wayRepository)
        {
            _wayRepository = wayRepository;
        }

        public Distance FindPisteForTrackingPoint(TrackingPoint trackingPoint)
        {
            var slopes = _wayRepository.GetSlopes();

            Way nearestWay = null;
            var maxDistance = double.MaxValue;
            if (trackingPoint.Position == null) return null;
            foreach (var way in slopes)
            foreach (var node in way.Nodes)
            {
                var nodeGeo = new GeoCoordinate(node.Latitude, node.Longitude);
                var trackingGeo = new GeoCoordinate(trackingPoint.Position.Latitude, trackingPoint.Position.Longitude);

                var distance = trackingGeo.GetDistanceTo(nodeGeo);
                if (distance < maxDistance)
                {
                    maxDistance = distance;
                    nearestWay = way;
                }
            }

            return new Distance
            {
                NearestWay = nearestWay,
                DistanceMeters = maxDistance
            };
        }
    }
}