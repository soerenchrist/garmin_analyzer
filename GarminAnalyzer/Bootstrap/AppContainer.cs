using Autofac;
using GarminAnalyzer.FileHandling;
using GarminAnalyzer.FileHandling.Abstractions;
using GarminAnalyzer.Repositories;
using GarminAnalyzer.Repositories.Abstractions;
using GarminAnalyzer.Services;
using GarminAnalyzer.Services.Abstractions;
using GarminAnalyzer.ViewModels;

namespace GarminAnalyzer.Bootstrap
{
    public static class AppContainer
    {
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainViewModel>();

            builder.RegisterType<ActivityRepository>().As<IActivityRepository>().SingleInstance();
            builder.RegisterType<WayRepository>().As<IWayRepository>().SingleInstance();
            
            
            builder.RegisterType<OsmParser>().As<IOsmParser>();
            builder.RegisterType<TcxParser>().As<IFileParser>();
            
            builder.RegisterType<LiftFinder>().As<ILiftFinder>();
            builder.RegisterType<SlopeFinder>().As<ISlopeFinder>();
            builder.RegisterType<SlopeNodeChecker>().As<ISlopeNodeChecker>();
            builder.RegisterType<StatsService>().As<IStatsService>();

            _container = builder.Build();
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}