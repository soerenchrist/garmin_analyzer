using System.Collections.Generic;
using GarminAnalyzer.Models;

namespace GarminAnalyzer.Repositories.Abstractions
{
    public interface IActivityRepository
    {
        IEnumerable<Lap> GetAllActivities();
        IEnumerable<Lap> GetSingleDay(Day day);
    }

    public enum Day
    {
        Tuesday, Wednesday, Thursday, Friday, Saturday
    }
}