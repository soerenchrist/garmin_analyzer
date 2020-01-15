using System.Collections.Generic;
using System.Threading.Tasks;
using GarminAnalyzer.Models;

namespace GarminAnalyzer.FileHandling.Abstractions
{
    public interface IFileParser
    {
        Task<List<Lap>> ParseFile(string filename);
    }
}