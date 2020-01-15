using System.Collections.Generic;
using System.Threading.Tasks;
using GarminAnalyzer.Models;

namespace GarminAnalyzer.FileHandling.Abstractions
{
    public interface IOsmParser
    {
        Task<List<Way>> ParseOsm(string filename, int relationId);
    }
}