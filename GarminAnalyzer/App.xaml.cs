using System.Windows;
using GarminAnalyzer.Bootstrap;
using GarminAnalyzer.ViewModels;

namespace GarminAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppContainer.RegisterDependencies();

            var viewModel = AppContainer.Resolve<MainViewModel>();
            var window = new MainWindow();

            window.DataContext = viewModel;
            
            window.Show();
        }
    }
}