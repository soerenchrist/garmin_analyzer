using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using GarminAnalyzer.Models;
using GarminAnalyzer.Repositories.Abstractions;
using GarminAnalyzer.Services.Abstractions;
using GarminAnalyzer.Util;

namespace GarminAnalyzer.Services
{
    public class SlopeLengthCalculator : ISlopeLengthCalculator
    {
        private readonly IWayRepository _wayRepository;

        public SlopeLengthCalculator(IWayRepository wayRepository)
        {
            _wayRepository = wayRepository;
        }
        
        public async Task<double> CalculateLength()
        {
            const int taskCount = 4;
            var slopes = _wayRepository.GetSlopes().ToList();

            var slopeBatches = slopes.SplitList(taskCount);
            var tasks = new List<Task<double>>();
            foreach (var batch in slopeBatches)
            {
                tasks.Add(Task.Run(() =>CalculateLength(batch)));
            }

            var results = await Task.WhenAll(tasks);
            return results.Sum();
        }

        private double CalculateLength(List<Way> ways)
        {
            var totalLength = 0.0;
            foreach (var way in ways)
            {
                for (int i = 1; i < way.Nodes.Count; i++)
                {
                    var currentNode = way.Nodes[i];
                    var predecessor = way.Nodes[i - 1];

                    var currentGeo = new GeoCoordinate(currentNode.Latitude, currentNode.Longitude);
                    var preGeo = new GeoCoordinate(predecessor.Latitude, predecessor.Longitude);
                    totalLength += currentGeo.GetDistanceTo(preGeo);
                }
            }

            return totalLength;
        }
        
        
    }
}