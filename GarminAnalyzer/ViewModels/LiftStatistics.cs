using GarminAnalyzer.ViewModels.Base;

namespace GarminAnalyzer.ViewModels
{
    public class LiftStatistics : ViewModelBase
    {
        public int TotalLiftCount { get; set; }
        public int ChairLiftCount { get; set; }
        public int GondolaLiftCount { get; set; }
        
        public int TakenLiftCount { get; set; }

        public string FormattedLiftCount => $"{TotalLiftCount} / {ChairLiftCount} / {GondolaLiftCount}";
    }
}