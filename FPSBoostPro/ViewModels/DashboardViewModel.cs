using System;
using System.Diagnostics;
using System.Windows.Threading;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FPSBoostPro.ViewModels
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardViewModel : ObservableObject
    {
        private PerformanceCounter? _cpuCounter;
        private PerformanceCounter? _ramCounter;
        private readonly DispatcherTimer _timer;

        [ObservableProperty] private string _cpuUsage = "Chargement...";
        [ObservableProperty] private string _ramUsage = "Chargement...";
        [ObservableProperty] private double _cpuUsageValue;
        [ObservableProperty] private double _ramUsageValue;

        public DashboardViewModel()
        {
            // 1. On configure le Timer immédiatement
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            // 2. On charge les compteurs en tâche de fond (asynchrone) pour ne pas bloquer l'UI au démarrage
            Task.Run(() =>
            {
                try
                {
                    _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");

                    // On force une première mesure en tâche de fond (la première NextValue renvoie toujours 0)
                    _cpuCounter.NextValue();

                    // Une fois prêts, on lance le Timer sur le thread principal de l'interface
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _timer.Start();
                        UpdateMetrics();
                    });
                }
                catch
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        CpuUsage = "N/A";
                        RamUsage = "N/A";
                    });
                }
            });
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateMetrics();
        }

        private void UpdateMetrics()
        {
            if (_cpuCounter == null || _ramCounter == null) return;

            try
            {
                double cpuVal = Math.Round(_cpuCounter.NextValue(), 0);
                double ramVal = Math.Round(_ramCounter.NextValue(), 0);

                CpuUsageValue = cpuVal;
                RamUsageValue = ramVal;

                CpuUsage = $"{cpuVal}%";
                RamUsage = $"{ramVal}%";
            }
            catch
            {
                CpuUsage = "N/A";
                RamUsage = "N/A";
            }
        }
    }
}