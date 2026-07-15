using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class ServicesViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _serviceStatus = "Prêt à optimiser les services.";

        [RelayCommand]
        private void DisableServices()
        {
            ServiceStatus = "Optimisation des services en cours...";

            try
            {
                // On utilise PowerShell en arrière-plan pour désactiver proprement les services (nécessite d'être Admin)
                string[] servicesToDisable = { "DiagTrack", "WSearch", "SysMain" };
                int optimizedCount = 0;

                foreach (string service in servicesToDisable)
                {
                    // Commande PowerShell pour arrêter et désactiver le service
                    string args = $"-Command \"Stop-Service -Name '{service}' -Force; Set-Service -Name '{service}' -StartupType Disabled\"";

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = args,
                        Verb = "runas", // Force la demande de droits d'administrateur
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    };

                    using (Process? process = Process.Start(psi))
                    {
                        process?.WaitForExit();
                        optimizedCount++;
                    }
                }

                ServiceStatus = $"Optimisation terminée ! {optimizedCount} services gourmands ont été désactivés.";
            }
            catch (Exception ex)
            {
                ServiceStatus = $"Erreur lors de l'optimisation : {ex.Message}";
            }
        }
    }
}