using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class PowerViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isOptimizing;
        [ObservableProperty] private string _statusMessage = "Prêt à optimiser (Plan Ultime, USB, Disques...)";
        [ObservableProperty] private string _auditLog = "";

        [RelayCommand]
        private async Task OptimizePower()
        {
            if (IsOptimizing) return;

            IsOptimizing = true;
            StatusMessage = "Optimisation de l'alimentation en cours...";
            AuditLog = "";

            Log("Début de l'optimisation...");
            Log("--------------------------------------------------");

            await Task.Run(() => ApplyPowerTweaks());

            StatusMessage = "Optimisation terminée !";
            IsOptimizing = false;
        }

        private void ApplyPowerTweaks()
        {
            try
            {
                // 1. Mode Performances Ultimes
                Log("Activation du mode Performances Ultimes...");
                bool plan1 = ExecuteCommand("powercfg", "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                bool plan2 = ExecuteCommand("powercfg", "-setactive e9a42b02-d5df-448d-aa00-03f14749eb61");
                Log(plan1 || plan2 ? "-> [SUCCÈS] Mode activé\n" : "-> [ERREUR] Impossible d'activer le mode\n");

                // 2. Suspension USB
                Log("Désactivation de la suspension USB sélective...");
                bool usb = ExecuteCommand("powercfg", "-setacvalueindex SCHEME_CURRENT 2a737441-1930-4402-8d77-b2bea5845741 48e6b7a6-50f5-4782-a5d4-53bb8f07e226 0");
                Log(usb ? "-> [SUCCÈS] Suspension USB désactivée\n" : "-> [ERREUR] Modification impossible\n");

                // 3. Disques durs
                Log("Désactivation de la mise en veille des disques durs...");
                bool disk = ExecuteCommand("powercfg", "-setacvalueindex SCHEME_CURRENT 0012ee47-9041-4b5d-9b77-535fba8b1442 6738e2c4-e8a5-4a42-b16a-e040e769756e 0");
                Log(disk ? "-> [SUCCÈS] Mise en veille désactivée\n" : "-> [ERREUR] Modification impossible\n");

                // Appliquer et sauvegarder
                Log("Sauvegarde des nouveaux paramètres d'alimentation...");
                bool save = ExecuteCommand("powercfg", "-S SCHEME_CURRENT");
                Log(save ? "-> [SUCCÈS] Paramètres appliqués" : "-> [ERREUR] Échec de l'application");

                Log("--------------------------------------------------");
                Log("🏁 Opération terminée !");
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
            catch
            {
                return false;
            }
        }

        private void Log(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AuditLog += $"[Power] {message}\n";
            });
        }
    }
}