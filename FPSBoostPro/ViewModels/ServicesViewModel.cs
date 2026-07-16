using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class ServicesViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isOptimizing;
        [ObservableProperty] private string _statusMessage = "Prêt à optimiser les services";
        [ObservableProperty] private string _auditLog = "";

        public ObservableCollection<ServiceItem> ServicesList { get; set; } = new();

        public ServicesViewModel()
        {
            ServicesList.Add(new ServiceItem { IsSelected = true, ServiceName = "DiagTrack", DisplayName = "Télémetrie Windows (DiagTrack)", Description = "Collecte et envoie vos données d'utilisation à Microsoft." });
            ServicesList.Add(new ServiceItem { IsSelected = true, ServiceName = "SysMain", DisplayName = "SysMain (Superfetch)", Description = "Précharge des fichiers en RAM. Cause souvent des saccades (stuttering) en jeu." });
            ServicesList.Add(new ServiceItem { IsSelected = true, ServiceName = "WerSvc", DisplayName = "Rapports d'erreurs Windows", Description = "Envoie des rapports de plantage en arrière-plan." });
            ServicesList.Add(new ServiceItem { IsSelected = true, ServiceName = "MapsBroker", DisplayName = "Gestionnaire de cartes téléchargées", Description = "Totalement inutile sur un PC de bureau." });
            ServicesList.Add(new ServiceItem { IsSelected = true, ServiceName = "TrkWks", DisplayName = "Suivi des liaisons distribuées", Description = "Utile uniquement pour les serveurs d'entreprise." });

            ServicesList.Add(new ServiceItem { IsSelected = false, ServiceName = "Spooler", DisplayName = "Spouleur d'impression", Description = "Cochez ceci si vous n'utilisez jamais d'imprimante." });
            ServicesList.Add(new ServiceItem { IsSelected = false, ServiceName = "WbioSrvc", DisplayName = "Service Biométrique", Description = "Gère les lecteurs d'empreintes/visage. Inutile si vous n'en avez pas." });
            ServicesList.Add(new ServiceItem { IsSelected = false, ServiceName = "WSearch", DisplayName = "Windows Search", Description = "Indexe les fichiers en arrière-plan. Peut utiliser beaucoup de disque/CPU." });
            ServicesList.Add(new ServiceItem { IsSelected = false, ServiceName = "XblAuthManager", DisplayName = "Xbox Live Auth Manager", Description = "Désactivez uniquement si vous ne jouez pas aux jeux Microsoft Store/Xbox." });
        }

        [RelayCommand]
        private async Task OptimizeSelectedServices()
        {
            if (IsOptimizing) return;

            IsOptimizing = true;
            StatusMessage = "Désactivation des services en cours...";
            AuditLog = "";

            Log("Début de l'optimisation des Services...");
            Log("--------------------------------------------------");

            await Task.Run(() => ApplyServicesTweaks());

            StatusMessage = "Services optimisés avec succès !";
            IsOptimizing = false;
        }

        private void ApplyServicesTweaks()
        {
            try
            {
                int count = 0;
                foreach (var service in ServicesList)
                {
                    if (service.IsSelected)
                    {
                        Log($"Configuration de : {service.DisplayName}...");

                        // Arrête le service
                        ExecuteCommand("sc", $"stop {service.ServiceName}");
                        // Désactive le démarrage automatique
                        bool success = ExecuteCommand("sc", $"config {service.ServiceName} start= disabled");

                        if (success)
                        {
                            Log("-> [SUCCÈS] Service désactivé\n");
                            count++;
                        }
                        else
                        {
                            Log("-> [ERREUR] Impossible de modifier la configuration\n");
                        }
                    }
                }

                Log("--------------------------------------------------");
                if (count == 0)
                {
                    Log("🏁 Aucun service n'a été modifié.");
                }
                else
                {
                    Log($"🏁 {count} service(s) désactivé(s) avec succès !");
                }
            }
            catch (Exception ex)
            {
                Log($"⚠️ Erreur critique : {ex.Message}");
            }
        }

        private bool ExecuteCommand(string filename, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = arguments,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas"
                };
                using (Process? p = Process.Start(psi))
                {
                    if (p == null) return false;
                    p.WaitForExit();
                    return p.ExitCode == 0;
                }
            }
            catch { return false; }
        }

        private void Log(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AuditLog += $"[Services] {message}\n";
            });
        }
    }

    public partial class ServiceItem : ObservableObject
    {
        [ObservableProperty] private bool _isSelected;
        [ObservableProperty] private string _serviceName = "";
        [ObservableProperty] private string _displayName = "";
        [ObservableProperty] private string _description = "";
    }
}