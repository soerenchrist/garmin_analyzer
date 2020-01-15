using System.Collections.Generic;
using System.IO;
using System.Linq;
using GarminAnalyzer.Models;
using GarminAnalyzer.Repositories.Abstractions;
using GarminAnalyzer.Services.Abstractions;
using Newtonsoft.Json;

namespace GarminAnalyzer.Repositories
{
    public class WayRepository : IWayRepository
    {
        private readonly IEnumerable<Way> _ways;

        public WayRepository()
        {
            var textPistes = File.ReadAllText("pistes");
            _ways = JsonConvert.DeserializeObject<List<Way>>(textPistes);
        }
        
        public IEnumerable<Way> GetAllWays()
        {
            return _ways;
        }

        public IEnumerable<Way> GetSlopes()
        {
            return _ways.Where(w => w.Type == "downhill");
        }

        public IEnumerable<Way> GetLifts()
        {
            return _ways.Where(w => w.Type != "downhill");
        }
        
        public int GetTotalLiftCount()
        {
            var pistes = GetAllWays();
            return pistes.Count(p => p.Type != "downhill" && p.Type != "skitour" && p.Type != "sled");
        }

        public int GetGondolaLiftCount()
        {
            var pistes = GetAllWays();
            return pistes.Count(p => p.Type == "cable_car" || p.Type == "gondola");
        }

        public int GetChairLiftCount()
        {
            var pistes = GetAllWays();
            return pistes.Count(p => p.Type == "chair_lift");
        }
    }
}