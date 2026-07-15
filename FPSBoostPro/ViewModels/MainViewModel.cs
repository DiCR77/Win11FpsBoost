using System;
using System.Runtime.Versioning;
using System.Windows; // AJOUTÉ pour les MessageBox
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    [SupportedOSPlatform("windows")]
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? _currentView;

        public MainViewModel()
        {
            // On initialise l'application sur le tableau de bord
            var dash = new Views.DashboardView();
            dash.DataContext = new DashboardViewModel();
            CurrentView = dash;
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            try
            {
                switch (destination)
                {
                    case "Dashboard":
                        var dash = new Views.DashboardView();
                        dash.DataContext = new DashboardViewModel();
                        CurrentView = dash;
                        break;

                    case "OneClick":
                        var oneClick = new Views.OneClickView();
                        oneClick.DataContext = new OneClickViewModel();
                        CurrentView = oneClick;
                        break;

                    case "Network":
                        var netView = new Views.NetworkView();
                        netView.DataContext = new NetworkViewModel();
                        CurrentView = netView;
                        break;

                    case "Services":
                        var servView = new Views.ServicesView();
                        servView.DataContext = new ServicesViewModel();
                        CurrentView = servView;
                        break;

                    case "Gaming":
                        var gameView = new Views.GamingView();
                        gameView.DataContext = new GamingViewModel();
                        CurrentView = gameView;
                        break;

                    case "Power":
                        var powView = new Views.PowerView();
                        powView.DataContext = new PowerViewModel();
                        CurrentView = powView;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Impossible de charger la page {destination} :\n\n{ex.Message}",
                    "Erreur de Navigation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}