using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using GarminAnalyzer.Models;
using GarminAnalyzer.Repositories.Abstractions;
using GarminAnalyzer.Services.Abstractions;
using GarminAnalyzer.Util;
using GarminAnalyzer.ViewModels.Base;
using LiveCharts;
using LiveCharts.Wpf;
using MapControl;

namespace GarminAnalyzer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region ViewModels
        public DisplayOptions DisplayOptions { get; set; } = new DisplayOptions();
        public Statistics Statistics { get; set; } = new Statistics();
        public LiftStatistics LiftStatistics { get; set; } = new LiftStatistics();
        public MapItems MapItems { get; set; } = new MapItems();
        public MapLayers MapLayers { get; } = new MapLayers();


        #endregion

        #region Properties

        public Lap SelectedLap { get; set; }
        public ObservableCollection<Lap> Laps { get; set; }
        public Way SelectedLift { get; set; }
        public ObservableCollection<Way> TakenLifts { get; set; } = new ObservableCollection<Way>();
        
        #endregion

        #region Fields
        
        private readonly ILiftFinder _liftFinder;
        private readonly IWayRepository _wayRepository;
        private readonly ISlopeNodeChecker _slopeNodeChecker;
        private readonly IStatsService _statsService;

        #endregion
        


        public MainViewModel(IWayRepository wayRepository, ILiftFinder liftFinder,
            IActivityRepository activityRepository, ISlopeNodeChecker slopeNodeChecker,
            IStatsService statsService)
        {
            _wayRepository = wayRepository;
            _slopeNodeChecker = slopeNodeChecker;
            _statsService = statsService;
            _liftFinder = liftFinder;

            PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(SelectedLift):
                        ShowSelectedLift();
                        break;
                    case nameof(SelectedLap):
                        ShowSelectedLap();
                        break;
                }
            };

            // Changed displayOptions -> Update views accordingly
            DisplayOptions.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != null && args.PropertyName.StartsWith("Show"))
                {
                    TakenLifts = new ObservableCollection<Way>();
                    
                    MapItems.Reset();
                    
                    Laps = new ObservableCollection<Lap>();
                    Statistics = new Statistics();


                    var allActivities = new List<Lap>();
                    if (DisplayOptions.ShowTuesday)
                    {
                        allActivities.AddRange(activityRepository.GetSingleDay(Day.Tuesday));
                    }

                    if (DisplayOptions.ShowWednesday)
                    {
                        allActivities.AddRange(activityRepository.GetSingleDay(Day.Wednesday));
                    }

                    if (DisplayOptions.ShowThursday)
                    {
                        allActivities.AddRange(activityRepository.GetSingleDay(Day.Thursday));
                    }

                    if (DisplayOptions.ShowFriday)
                    {
                        allActivities.AddRange(activityRepository.GetSingleDay(Day.Friday));
                    }

                    if (DisplayOptions.ShowSaturday)
                    {
                        allActivities.AddRange(activityRepository.GetSingleDay(Day.Saturday));
                    }

                    var allTrackingPoints = allActivities.SelectMany(a => a.TrackingPoints);

                    CalculateStats(allTrackingPoints);

                    foreach (var lap in allActivities)
                    {
                        Laps.Add(lap);
                        var firstPoint = lap.TrackingPoints.FirstOrDefault();
                        ShowLapStartFlags(firstPoint, lap.Number);

                        CheckTakenLifts(firstPoint);
                        ShowTrackingPoints(lap);
                        ShowSpeedLine(lap);
                        ShowHeartRateLine(lap);
                    }
                }

                LiftStatistics = _statsService.CalculateLiftStatistics(TakenLifts);
                ShowLiftNamePins();
            };

            DrawMadeSlopes();
            DrawSlopes();
            DrawLifts();
        }

        private void ShowLiftNamePins()
        {
            var lifts = _wayRepository.GetLifts();
            if (!DisplayOptions.ShowLiftNames) return;

            foreach (var lift in lifts)
            {
                var firstNode = lift.Nodes.First();
                MapItems.Pushpins.Add(new PointItem
                {
                    Location = firstNode.ToLocation(),
                    Name = lift.Name
                });
            }
        }

        private void ShowSpeedLine(Lap lap)
        {
            if (!DisplayOptions.ShowSpeeds) return;
            for (var i = 1; i < lap.TrackingPoints.Count; i++)
            {
                var currentPoint = lap.TrackingPoints[i];
                var predecessor = lap.TrackingPoints[i - 1];

                if (predecessor.Altitude < currentPoint.Altitude) continue;

                if (currentPoint.Position == null || predecessor.Position == null) continue;

                var speed = currentPoint.Speed * 3.6;

                var color = ColorHelper.NumberToColor(speed);

                var locationPath =
                    $"{predecessor.Position.Latitude.ToString(CultureInfo.InvariantCulture)},{predecessor.Position.Longitude.ToString(CultureInfo.InvariantCulture)} ";
                locationPath +=
                    $"{currentPoint.Position.Latitude.ToString(CultureInfo.InvariantCulture)},{currentPoint.Position.Longitude.ToString(CultureInfo.InvariantCulture)}";

                MapItems.SpeedLine.Add(new Polyline
                {
                    Locations = LocationCollection.Parse(locationPath),
                    Color = color
                });
            }
        }

        private void ShowHeartRateLine(Lap lap)
        {
            if (!DisplayOptions.ShowHeartRates) return;
            for (var i = 1; i < lap.TrackingPoints.Count; i++)
            {
                var currentPoint = lap.TrackingPoints[i];
                var predecessor = lap.TrackingPoints[i - 1];

                if (predecessor.Altitude < currentPoint.Altitude) continue;

                if (currentPoint.Position == null || predecessor.Position == null) continue;

                var heartRate = currentPoint.HeartRate - 50;

                var color = ColorHelper.NumberToColor(heartRate);

                var locationPath =
                    $"{predecessor.Position.Latitude.ToString(CultureInfo.InvariantCulture)},{predecessor.Position.Longitude.ToString(CultureInfo.InvariantCulture)} ";
                locationPath +=
                    $"{currentPoint.Position.Latitude.ToString(CultureInfo.InvariantCulture)},{currentPoint.Position.Longitude.ToString(CultureInfo.InvariantCulture)}";

                MapItems.SpeedLine.Add(new Polyline
                {
                    Locations = LocationCollection.Parse(locationPath),
                    Color = color
                });
            }
        }

        private void CalculateStats(IEnumerable<TrackingPoint> allTrackingPoints)
        {
            Statistics = _statsService.CalculateStatistics(allTrackingPoints);
        }

        private void ShowTrackingPoints(Lap lap)
        {
            if (!DisplayOptions.ShowTrackpoints) return;
            var counter = 0;
            foreach (var node in lap.TrackingPoints)
            {
                if (node.Position == null) continue;

                counter++;
                if (counter % 2 == 0) continue;

                var pointItem = new PointItem
                {
                    Location = node.Position.ToLocation(),
                    Name = node.ToString(),
                };
                switch (node.DistanceConnection.NearestWay.Difficulty)
                {
                    case "intermediate":
                        MapItems.IntermediatePoints.Add(pointItem);
                        break;
                    case "easy":
                        MapItems.EasyPoints.Add(pointItem);
                        break;
                    case "advanced":
                        MapItems.AdvancedPoints.Add(pointItem);
                        break;
                }
            }
        }

        // Check which lift has been taken
        private void CheckTakenLifts(TrackingPoint firstPoint)
        {
            if (firstPoint?.Position == null) return;
            var takenLift = _liftFinder.GetLiftForStartPoint(firstPoint.Position);

            TakenLifts.Add(takenLift);
        }

        private void ShowLapStartFlags(TrackingPoint firstPoint, int lapCounter)
        {
            if (!DisplayOptions.ShowLapStarts) return;
            
            if (firstPoint?.Position != null)
            {
                MapItems.Pushpins.Add(new PointItem
                {
                    Location = firstPoint.Position.ToLocation(),
                    Name = lapCounter + " Start"
                });
            }
        }

        private void ShowSelectedLift()
        {
            if (SelectedLift == null) return;
            
            MapItems.SelectedLiftPolyLine = new ObservableCollection<Polyline>();
            var locationPath = "";
            foreach (var node in SelectedLift.Nodes)
            {
                locationPath +=
                    $"{node.Latitude.ToString(CultureInfo.InvariantCulture)},{node.Longitude.ToString(CultureInfo.InvariantCulture)} ";
            }

            MapItems.SelectedLiftPolyLine.Add(new Polyline
            {
                Locations = LocationCollection.Parse(locationPath),
                Color = Brushes.Yellow
            });
        }

        private void ShowSelectedLap()
        {
            if (SelectedLap == null) return;
            
            MapItems.SeriesCollection = new SeriesCollection();
            MapItems.SelectedLapPolyLine = new ObservableCollection<Polyline>();
            MapItems.SelectedLiftPolyLine = new ObservableCollection<Polyline>();

            var lineSeries = new LineSeries();
            var chartValues = new ChartValues<double>();

            var locationPath = "";
            foreach (var trackingPoint in SelectedLap.TrackingPoints.Take(SelectedLap.TrackingPoints.Count - 4))
            {
                chartValues.Add(trackingPoint.Altitude);
                locationPath +=
                    $"{trackingPoint.Position.Latitude.ToString(CultureInfo.InvariantCulture)},{trackingPoint.Position.Longitude.ToString(CultureInfo.InvariantCulture)} ";
            }

            lineSeries.Values = chartValues;
            MapItems.SeriesCollection.Add(lineSeries);
            RaisePropertyChanged(nameof(SeriesCollection));

            MapItems.SelectedLapPolyLine.Add(new Polyline
            {
                Locations = LocationCollection.Parse(locationPath),
                Color = Brushes.Chartreuse
            });
        }

        private void DrawSlopes()
        {
            MapItems.Slopes = new ObservableCollection<Polyline>();

            var pistes = _wayRepository.GetSlopes();
            foreach (var slope in pistes)
            {
                var locationPath = "";

                foreach (var node in slope.Nodes)
                {
                    locationPath +=
                        $"{node.Latitude.ToString(CultureInfo.InvariantCulture)},{node.Longitude.ToString(CultureInfo.InvariantCulture)} ";
                }


                MapItems.Slopes.Add(new Polyline
                {
                    Locations = LocationCollection.Parse(locationPath),
                    Color = slope.Difficulty == "easy" ? Brushes.Blue :
                        slope.Difficulty == "advanced" ? Brushes.Black : Brushes.Red
                });
            }
        }

        private void DrawLifts()
        {
            var lifts = _wayRepository.GetLifts();
            foreach (var lift in lifts.Where(p => p.Type != "downhill"))
            {
                if (DisplayOptions.ShowLiftNames)
                {
                    var firstNode = lift.Nodes.First();
                    MapItems.Pushpins.Add(new PointItem
                    {
                        Location = firstNode.ToLocation(),
                        Name = lift.Name
                    });
                }

                var locationPath = "";
                foreach (var node in lift.Nodes)
                {
                    locationPath +=
                        $"{node.Latitude.ToString(CultureInfo.InvariantCulture)},{node.Longitude.ToString(CultureInfo.InvariantCulture)} ";
                }

                MapItems.Lifts.Add(new Polyline
                {
                    Locations = LocationCollection.Parse(locationPath),
                });
            }
        }
        
        
        private void DrawMadeSlopes()
        {
            if (!DisplayOptions.ShowMadeNodes) return;

            MapItems.MadeNodes = new ObservableCollection<PointItem>();
            var pistes = _wayRepository.GetSlopes();
            foreach (var slope in pistes)
            {
                foreach (var node in slope.Nodes)
                {
                    var trackingPoint = _slopeNodeChecker.Check(node);
                    if (trackingPoint == null)
                    {
                        MapItems.MadeNodes.Add(new PointItem
                        {
                            Location = node.ToLocation()
                        });
                    }
                    else
                    {
                        MapItems.NotMadeNodes.Add(new PointItem
                        {
                            Location = node.ToLocation()
                        });
                    }
                }
            }
        }

    }
}