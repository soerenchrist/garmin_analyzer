using GarminAnalyzer.Models;
using MapControl;

namespace GarminAnalyzer.Util
{
    public static class LocationUtil
    {
        public static Location ToLocation(this Position position)
        {
            return new Location(position.Latitude, position.Longitude);
        }
    }
}