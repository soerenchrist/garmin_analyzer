using System;
using System.Collections.Generic;

namespace GarminAnalyzer.Models
{
    public class Lap
    {
        public string Day { get; set; }
        public int Number { get; set; }
        public DateTime StartTime { get; set; }
        public double TotalTimeSeconds { get; set; }
        public double TotalDistance { get; set; }
        public string FormattedTime
        {
            get
            {
                var minutes = (int)TotalTimeSeconds / 60;
                var remainingSeconds = TotalTimeSeconds % 60;
                return $"{minutes}:{remainingSeconds} min";
            }
        }

        public string FormattedDistance => $"{Math.Round(TotalDistance/1000, 2)} km";
        public double MaximumSpeed { get; set; }
        public int Calories { get; set; }
        public int AverageHeartRate { get; set; }
        public int MaximumHeartRate { get; set; }
        
        public List<TrackingPoint> TrackingPoints { get; set; }
        public string SlopeNames { get; set; }
    }
}