using System.ComponentModel;
using System.Windows.Media;
using MapControl;

namespace GarminAnalyzer.ViewModels.Base
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyName == null) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class PointItem : ViewModelBase
    {
        public string Name { get; set; }

        public Location Location { get; set; }
    }

    public class Polyline
    {
        public LocationCollection Locations { get; set; }
        public Brush Color { get; set; }
    }
}