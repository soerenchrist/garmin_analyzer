using System.Collections.Generic;
using System.IO;
using System.Linq;
using GarminAnalyzer.Models;
using GarminAnalyzer.Repositories.Abstractions;
using GarminAnalyzer.Services.Abstractions;
using Newtonsoft.Json;

namespace GarminAnalyzer.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly Dictionary<Day, IEnumerable<Lap>> _activities = new Dictionary<Day, IEnumerable<Lap>>();

        public ActivityRepository()
        {
            var filenames = new Dictionary<Day, string>
            {
                {Day.Tuesday, "dienstag"}, {Day.Wednesday, "mittwoch"}, {Day.Thursday, "donnerstag"},
                {Day.Friday, "freitag"}, {Day.Saturday, "samstag"}
            };

            foreach (var filename in filenames)
            {
                var text = File.ReadAllText(filename.Value);
                var activities = JsonConvert.DeserializeObject<IEnumerable<Lap>>(text);
                _activities.Add(filename.Key, activities);
            }
        }
        
        public IEnumerable<Lap> GetAllActivities()
        {
            return _activities.SelectMany(a => a.Value);
        }

        public IEnumerable<Lap> GetSingleDay(Day day)
        {
            var laps = _activities[day];
            var counter = 1;
            foreach (var lap in laps)
            {
                lap.Number = counter;
                lap.Day = day.ToString();
                
                var slopes = new List<string>();
                foreach (var trackingPoint in lap.TrackingPoints)
                {
                    var slopename = trackingPoint.DistanceConnection?.NearestWay?.Ref;
                    if (slopename == null)
                    {
                        continue;
                    }
                    if (!slopes.Contains(slopename))
                        slopes.Add(slopename);
                }
                lap.SlopeNames = string.Join(", ", slopes);
                counter++;
            }

            return laps;
        }
    }
}