using GarminAnalyzer.ViewModels.Base;

namespace GarminAnalyzer.ViewModels
{
    public class Statistics : ViewModelBase
    {
        public double PercentageEasy { get; set; }
        public double PercentageIntermediate { get; set; }
        public double PercentageAdvanced { get; set; }

        public double TotalDistance { get; set; }
        public double TotalAltitude { get; set; }

        public double MaxSpeed { get; set; }
        public double MaxHeartRate { get; set; }
    }
}