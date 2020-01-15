using System.Collections.Generic;

namespace GarminAnalyzer.Models
{
    public class Way
    {
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public List<Position> Nodes { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Grooming { get; set; }
        public string Bubble { get; set; }
        public string Heating { get; set; }
        public int Occupancy { get; set; }
        public string Ref { get; set; }
        public bool PistMade { get; set; }
    }
}