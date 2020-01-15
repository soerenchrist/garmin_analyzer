using System.Collections.ObjectModel;
using GarminAnalyzer.ViewModels.Base;
using LiveCharts;
using MapControl;

namespace GarminAnalyzer.ViewModels
{
    public class MapItems : ViewModelBase
    {
        public Location MapCenter { get; set; } = new Location(47.0006, 10.297);
        public ObservableCollection<PointItem> MadeNodes { get; set; } = new ObservableCollection<PointItem>();
        public ObservableCollection<PointItem> NotMadeNodes { get; set; } = new ObservableCollection<PointItem>();
        public ObservableCollection<PointItem> IntermediatePoints { get; set; } = new ObservableCollection<PointItem>();
        public ObservableCollection<PointItem> AdvancedPoints { get; set; } = new ObservableCollection<PointItem>();
        public ObservableCollection<PointItem> EasyPoints { get; set; } = new ObservableCollection<PointItem>();
        public ObservableCollection<PointItem> Pushpins { get; set; } = new ObservableCollection<PointItem>();
        public ObservableCollection<Polyline> Lifts { get; } = new ObservableCollection<Polyline>();
        public ObservableCollection<Polyline> Slopes { get; set; } = new ObservableCollection<Polyline>();
        public ObservableCollection<Polyline> Laps { get; } = new ObservableCollection<Polyline>();
        public ObservableCollection<Polyline> SelectedLiftPolyLine { get; set; } = new ObservableCollection<Polyline>();
        public ObservableCollection<Polyline> SelectedLapPolyLine { get; set; } = new ObservableCollection<Polyline>();
        public ObservableCollection<Polyline> SpeedLine { get; set; } = new ObservableCollection<Polyline>();
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();


        public void Reset()
        {
            IntermediatePoints = new ObservableCollection<PointItem>();
            EasyPoints = new ObservableCollection<PointItem>();
            AdvancedPoints = new ObservableCollection<PointItem>();
            SpeedLine = new ObservableCollection<Polyline>();
            Pushpins = new ObservableCollection<PointItem>();
        }
    }
}