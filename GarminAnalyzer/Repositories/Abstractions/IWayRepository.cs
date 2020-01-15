using System.Collections.Generic;
using GarminAnalyzer.Models;

namespace GarminAnalyzer.Repositories.Abstractions
{
    public interface IWayRepository
    {
        IEnumerable<Way> GetAllWays();
        IEnumerable<Way> GetSlopes();
        IEnumerable<Way> GetLifts();
        
        int GetTotalLiftCount();
        int GetGondolaLiftCount();
        int GetChairLiftCount();
    }
}