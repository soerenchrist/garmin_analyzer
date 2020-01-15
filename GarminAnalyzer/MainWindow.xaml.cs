using System;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using GarminAnalyzer.Bootstrap;
using GarminAnalyzer.FileHandling.Abstractions;
using GarminAnalyzer.Services.Abstractions;
using MapControl;
using MapControl.Caching;
using Newtonsoft.Json;

namespace GarminAnalyzer
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            ImageLoader.HttpClient.DefaultRequestHeaders.Add("User-Agent", "GarminAnalyzer");
            TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheFolder);

            InitializeComponent();

            // Uncomment to preprocess osm and tcx files
            Preprocess();
        }

        // Preprocess OSM and TCX files to improve performance
        private async void Preprocess()
        {
            var parser = AppContainer.Resolve<IFileParser>();
            var slopeFinder = AppContainer.Resolve<ISlopeFinder>();


            var activity = await parser.ParseFile(@"C:\Users\chris\Downloads\dienstag.tcx");
            foreach (var lap in activity)
            foreach (var node in lap.TrackingPoints)
            {
                if (node.Position == null) continue;
                node.DistanceConnection =
                    slopeFinder.FindPisteForTrackingPoint(node);
            }

            var activityJson = JsonConvert.SerializeObject(activity);
            File.WriteAllText("dienstag", activityJson);

            activity = await parser.ParseFile(@"C:\Users\chris\Downloads\mittwoch.tcx");
            foreach (var lap in activity)
            foreach (var node in lap.TrackingPoints)
            {
                if (node.Position == null) continue;
                node.DistanceConnection =
                    slopeFinder.FindPisteForTrackingPoint(node);
            }

            activityJson = JsonConvert.SerializeObject(activity);
            File.WriteAllText("mittwoch", activityJson);


            activity = await parser.ParseFile(@"C:\Users\chris\Downloads\donnerstag.tcx");
            foreach (var lap in activity)
            foreach (var node in lap.TrackingPoints)
            {
                if (node.Position == null) continue;
                node.DistanceConnection = slopeFinder.FindPisteForTrackingPoint(node);
            }

            activityJson = JsonConvert.SerializeObject(activity);
            File.WriteAllText("donnerstag", activityJson);


            activity = await parser.ParseFile(@"C:\Users\chris\Downloads\freitag.tcx");
            foreach (var lap in activity)
            foreach (var node in lap.TrackingPoints)
            {
                if (node.Position == null) continue;
                node.DistanceConnection =
                    slopeFinder.FindPisteForTrackingPoint(node);
            }

            activityJson = JsonConvert.SerializeObject(activity);
            File.WriteAllText("freitag", activityJson);

            activity = await parser.ParseFile(@"C:\Users\chris\Downloads\samstag.tcx");
            foreach (var lap in activity)
            foreach (var node in lap.TrackingPoints)
            {
                if (node.Position == null) continue;
                node.DistanceConnection =
                    slopeFinder.FindPisteForTrackingPoint(node);
            }

            activityJson = JsonConvert.SerializeObject(activity);
            File.WriteAllText("samstag", activityJson);


            var osmParser = AppContainer.Resolve<IOsmParser>();

            var piste = await osmParser.ParseOsm(@"C:\Users\chris\Downloads\planet_pistes_1.osm", 3508010);

            var pisteJson = JsonConvert.SerializeObject(piste);
            File.WriteAllText("pistes", pisteJson);
        }

        private void MapItemTouchDown(object sender, TouchEventArgs e)
        {
            var mapItem = (MapItem) sender;
            mapItem.IsSelected = !mapItem.IsSelected;
            e.Handled = true;
        }

        private void MapMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) Map.TargetCenter = Map.ViewportPointToLocation(e.GetPosition(Map));
        }

        private void MapMouseLeave(object sender, MouseEventArgs e)
        {
            MouseLocation.Text = string.Empty;
        }

        private void MapMouseMove(object sender, MouseEventArgs e)
        {
            var location = Map.ViewportPointToLocation(e.GetPosition(Map));
            var latitude = (int) Math.Round(location.Latitude * 60000d);
            var longitude = (int) Math.Round(Location.NormalizeLongitude(location.Longitude) * 60000d);
            var latHemisphere = 'N';
            var lonHemisphere = 'E';

            if (latitude < 0)
            {
                latitude = -latitude;
                latHemisphere = 'S';
            }

            if (longitude < 0)
            {
                longitude = -longitude;
                lonHemisphere = 'W';
            }

            MouseLocation.Text = string.Format(CultureInfo.InvariantCulture,
                "{0}  {1:00} {2:00.000}\n{3} {4:000} {5:00.000}",
                latHemisphere, latitude / 60000, latitude % 60000 / 1000d,
                lonHemisphere, longitude / 60000, longitude % 60000 / 1000d);
        }

        private void MapManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.001;
        }
    }
}