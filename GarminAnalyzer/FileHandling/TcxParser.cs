using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using GarminAnalyzer.FileHandling.Abstractions;
using GarminAnalyzer.Models;

namespace GarminAnalyzer.FileHandling
{
    public class TcxParser : IFileParser
    {
        public Task<List<Lap>> ParseFile(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"File {filename} does not exist");
            }

            if (Path.GetExtension(filename)?.ToUpper() != ".TCX")
            {
                throw new ArgumentException("Only files with extension .tcx possible");
            }
            
            var results = new List<Lap>();
            
            var document = new XmlDocument();
            document.Load(filename);

            var activityNodes = document.DocumentElement?.FirstChild;
            if (activityNodes == null) throw new FileFormatException();

            var activity = activityNodes.FirstChild;
            if (activity == null) throw new FileFormatException();

            foreach (XmlNode lapNode in activity.ChildNodes)
            {
                if (lapNode.Name != "Lap") continue;
                var lap = new Lap();
                lap.StartTime = DateTime.Parse(lapNode.Attributes?["StartTime"].InnerText);
                foreach (XmlNode childNode in lapNode.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "TotalTimeSeconds":
                            lap.TotalTimeSeconds = double.Parse(childNode.InnerText, CultureInfo.InvariantCulture);
                            break;
                        case "DistanceMeters":
                            lap.TotalDistance = double.Parse(childNode.InnerText, CultureInfo.InvariantCulture);
                            break;
                        case "MaximumSpeed":
                            lap.MaximumSpeed = double.Parse(childNode.InnerText, CultureInfo.InvariantCulture);
                            break;
                        case "Calories":
                            lap.Calories = int.Parse(childNode.InnerText);
                            break;
                        case "AverageHeartRateBpm":
                            lap.AverageHeartRate = int.Parse(childNode.FirstChild.InnerText);
                            break;
                        case "MaximumHeartRateBpm":
                            lap.MaximumHeartRate = int.Parse(childNode.FirstChild.InnerText);
                            break;
                        case "Track":
                            lap.TrackingPoints = new List<TrackingPoint>();
                            foreach (XmlNode trackNode in childNode.ChildNodes)
                            {
                                var trackPoint = new TrackingPoint();
                                foreach (XmlNode attributeNode in trackNode.ChildNodes)
                                {
                                    switch (attributeNode.Name)
                                    {
                                        case "Time":
                                            trackPoint.Time = DateTime.Parse(attributeNode.InnerText);
                                            break;
                                        case "Position":
                                            trackPoint.Position = new Position();
                                            trackPoint.Position.Latitude =
                                                double.Parse(attributeNode.FirstChild.InnerText, CultureInfo.InvariantCulture);
                                            trackPoint.Position.Longitude =
                                                double.Parse(attributeNode.LastChild.InnerText, CultureInfo.InvariantCulture);
                                            break;
                                        case "AltitudeMeters":
                                            trackPoint.Altitude = double.Parse(attributeNode.InnerText,
                                                CultureInfo.InvariantCulture);
                                            break;
                                        case "DistanceMeters":
                                            trackPoint.Distance = double.Parse(attributeNode.InnerText,
                                                CultureInfo.InvariantCulture);
                                            break;
                                        case "HeartRateBpm":
                                            trackPoint.HeartRate = int.Parse(attributeNode.FirstChild.InnerText);
                                            break;
                                        case "Extensions":
                                            trackPoint.Speed =
                                                double.Parse(attributeNode.FirstChild?.FirstChild?.InnerText ?? "0.0",
                                                    CultureInfo.InvariantCulture);
                                            break;
                                    }
                                }
                                lap.TrackingPoints.Add(trackPoint);
                            }
                            break;
                    }
                }
                results.Add(lap);
            }

            return Task.FromResult(results);
        }
    }
}