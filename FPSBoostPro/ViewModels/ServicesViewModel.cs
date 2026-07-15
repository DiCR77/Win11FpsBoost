using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    [SupportedOSPlatform("windows")]
    public partial class ServicesViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _serviceStatus = "Prêt à optimiser.";

        // Nouvelle propriété pour la console d'audit
        [ObservableProperty]
        private string _auditLog = "En attente du lancement...\n";

        [RelayCommand]
        private async Task DisableServices()
        {
            ServiceStatus = "Optimisation en cours...";
            AuditLog = "=== DÉBUT DE L'AUDIT DES SERVICES ===\n\n";
            int successCount = 0;

            string[] servicesToDisable = { "DiagTrack", "WSearch", "SysMain" };

            // On boucle de manière asynchrone pour mettre à jour l'interface à chaque étape
            foreach (string service in servicesToDisable)
            {
                AuditLog += $"⏳ Traitement du service : {service}...\n";

                // On exécute la commande PowerShell en arrière-plan
                bool success = await Task.Run(() =>
                {
                    try
                    {
                        string args = $"-Command \"Stop-Service -Name '{service}' -Force; Set-Service -Name '{service}' -StartupType Disabled\"";
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments = args,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            CreateNoWindow = true
                        };

                        using (Process? process = Process.Start(psi))
                        {
                            process?.WaitForExit();
                            return process != null && process.ExitCode == 0;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                });

                // On met à jour l'écran avec le résultat
                if (success)
                {
                    AuditLog += $"✅ SUCCÈS : {service} désactivé.\n\n";
                    successCount++;
                }
                else
                {
                    AuditLog += $"❌ ÉCHEC : Impossible de désactiver {service} (Droits Admin ?).\n\n";
                }

                // Petite pause visuelle pour bien voir le texte défiler
                await Task.Delay(400);
            }

            AuditLog += $"=== TERMINÉ : {successCount}/{servicesToDisable.Length} services optimisés ===";
            ServiceStatus = "✓ Audit terminé !";
        }
    }
}