using System;

namespace GarminAnalyzer.Models
{
    public class TrackingPoint
    {
        public DateTime Time { get; set; }
        public Position Position { get; set; }
        public double Altitude { get; set; }
        public double Distance { get; set; }
        public int HeartRate { get; set; }
        public double Speed { get; set; }
        public Distance DistanceConnection { get; set; }

        public override string ToString()
        {
            return $"Zeit: {Time.ToLongTimeString()} \nHöhe: {Altitude}m, \nHerzfrequenz: {HeartRate}, \nGeschwindigkeit: {Math.Round(Speed*3.6, 2)}km/h";
        }
    }
}