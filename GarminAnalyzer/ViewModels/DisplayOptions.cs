using GarminAnalyzer.ViewModels.Base;

namespace GarminAnalyzer.ViewModels
{
    public class DisplayOptions : ViewModelBase
    {
        public bool ShowLapStarts { get; set; }
        public bool ShowTuesday { get; set; }
        public bool ShowWednesday { get; set; }
        public bool ShowThursday { get; set; }
        public bool ShowFriday { get; set; }
        public bool ShowSaturday { get; set; }
        public bool ShowTrackpoints { get; set; } = true;
        public bool ShowSpeeds { get; set; }
        public bool ShowMadeNodes { get; set; }
        public bool ShowHeartRates { get; set; }
        public bool ShowLiftNames { get; set; }
    }
}