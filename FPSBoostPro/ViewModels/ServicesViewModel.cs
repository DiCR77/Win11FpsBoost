using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FPSBoostPro.Models;

namespace FPSBoostPro.ViewModels
{
    public partial class ServicesViewModel : ObservableObject
    {
        public ObservableCollection<ServiceItem> ServicesList { get; set; }

        private bool _isOptimizing;
        public bool IsOptimizing
        {
            get => _isOptimizing;
            set => SetProperty(ref _isOptimizing, value);
        }

        private string _statusMessage = "Prêt à optimiser les services";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public event Action<string>? OnLogReceived;

        public ServicesViewModel()
        {
            ServicesList = new ObservableCollection<ServiceItem>
            {
                new ServiceItem { DisplayName = "SysMain (Superfetch)", ServiceNames = ["SysMain"], Description = "Précharge la RAM. À désactiver si vous avez un SSD." },
                new ServiceItem { DisplayName = "Windows Search", ServiceNames = ["WSearch"], Description = "Indexation des fichiers. Soulage le disque si désactivé." },
                new ServiceItem { DisplayName = "Spooler d'impression", ServiceNames = ["Spooler"], Description = "Désactiver si vous n'avez pas d'imprimante." },
                new ServiceItem { DisplayName = "Service Fax", ServiceNames = ["Fax"], Description = "Totalement inutile aujourd'hui." },
                new ServiceItem { DisplayName = "Bluetooth", ServiceNames = ["bthserv"], Description = "Désactiver si vous n'utilisez aucun appareil Bluetooth." },
                new ServiceItem { DisplayName = "Registre à distance", ServiceNames = ["RemoteRegistry"], Description = "Sécurité : empêche la modification réseau du registre." },
                new ServiceItem { DisplayName = "Rapports d'erreurs", ServiceNames = ["WerSvc"], Description = "Évite l'envoi de rapports lourds lors de crashs." },
                new ServiceItem { DisplayName = "Télémétrie Windows", ServiceNames = ["DiagTrack"], Description = "Désactive la collecte de données Microsoft." },
                new ServiceItem { DisplayName = "Services Xbox", ServiceNames = ["XblAuthManager", "XblGameSave", "XboxNetApiSvc", "XboxGipSvc"], Description = "Désactiver si vous ne jouez pas aux jeux du Xbox Store." },
                new ServiceItem { DisplayName = "Assistant Compatibilité", ServiceNames = ["PcaSvc"], Description = "Surveille les vieux programmes. Souvent inutile." }
            };
        }

        [RelayCommand]
        public async Task OptimizeSelectedServicesAsync()
        {
            if (IsOptimizing) return;

            IsOptimizing = true;
            StatusMessage = "Désactivation en cours...";
            Log("Début de l'optimisation des services...");

            await Task.Run(() =>
            {
                foreach (var item in ServicesList)
                {
                    if (item.IsSelected)
                    {
                        foreach (var service in item.ServiceNames)
                        {
                            Log($"Désactivation de {service}...");
                            ExecuteCommand("sc", $"config \"{service}\" start= disabled");
                            ExecuteCommand("sc", $"stop \"{service}\"");
                        }
                    }
                }
                Log("✅ Services sélectionnés optimisés !");
            });

            StatusMessage = "Terminé avec succès !";
            IsOptimizing = false;
        }

        private void ExecuteCommand(string filename, string arguments)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process? process = Process.Start(startInfo))
                {
                    process?.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Log($"Erreur ({filename}) : {ex.Message}");
            }
        }

        private void Log(string message) => OnLogReceived?.Invoke($"[Services] {message}");
    }
}