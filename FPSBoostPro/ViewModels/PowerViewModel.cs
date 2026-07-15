using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    [SupportedOSPlatform("windows")]
    public partial class PowerViewModel : ObservableObject
    {
        [ObservableProperty] private string _powerStatus = "Prêt à optimiser.";
        [ObservableProperty] private string _auditLog = "En attente...\n";

        [RelayCommand]
        private async Task OptimizePower()
        {
            PowerStatus = "Optimisation en cours...";
            AuditLog = "=== DÉBUT DE L'AUDIT D'ALIMENTATION ===\n\n";

            // Étape 1 : Débloquer le schéma de Performances Optimales (Ultimate Performance)
            await RunPowerCfgStep(
                "⏳ Déblocage du schéma 'Performances Optimales'...",
                "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61"
            );

            // Étape 2 : Activer le schéma de Performances Optimales
            await RunPowerCfgStep(
                "⏳ Activation du schéma 'Performances Optimales' comme profil actif...",
                "-setactive e9a42b02-d5df-448d-aa00-03f14749eb61"
            );

            // Étape 3 : Désactiver la suspension sélective USB (AC)
            await RunPowerCfgStep(
                "⏳ Configuration : Désactivation de la suspension sélective USB...",
                "-setacvalueindex SCHEME_CURRENT 2a84c312-ed34-40c2-9e77-841d2547514a d696105a-1923-441d-9e2c-772c86b60655 0"
            );

            // Étape 4 : Désactiver la gestion d'alimentation Link State du PCI Express (AC)
            await RunPowerCfgStep(
                "⏳ Configuration : PCI Express en Performances Maximales (LSPM Off)...",
                "-setacvalueindex SCHEME_CURRENT 501a4d13-42af-4429-9fd1-a8218c268e20 ee12f2c1-9844-474d-917e-f404d620220a 0"
            );

            // Étape 5 : Désactiver la mise en veille des disques durs (AC)
            await RunPowerCfgStep(
                "⏳ Configuration : Désactivation de la mise en veille des disques durs...",
                "-change -disk-timeout-ac 0"
            );

            // Étape finale : Forcer la mise à jour active des paramètres système
            await RunPowerCfgStep(
                "⏳ Application globale des nouveaux paramètres d'alimentation...",
                "-setactive SCHEME_CURRENT"
            );

            AuditLog += "=== AUDIT TERMINÉ ===";
            PowerStatus = "✓ Alimentation optimisée !";
        }

        private async Task RunPowerCfgStep(string message, string args)
        {
            AuditLog += message + "\n";
            string errorOutput = "";

            bool success = await Task.Run(() => {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "powercfg",
                        Arguments = args,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true
                    };
                    using (Process? p = Process.Start(psi))
                    {
                        errorOutput = p?.StandardError.ReadToEnd() ?? "";
                        p?.WaitForExit();
                        return p != null && p.ExitCode == 0;
                    }
                }
                catch (Exception ex)
                {
                    errorOutput = ex.Message;
                    return false;
                }
            });

            if (!success)
            {
                if (!string.IsNullOrWhiteSpace(errorOutput))
                {
                    errorOutput = errorOutput.Split('\n')[0].Trim();
                }
                else
                {
                    errorOutput = "Commande refusée par Windows.";
                }

                if (errorOutput.Contains("0x80070005") || errorOutput.ToLower().Contains("refus") || errorOutput.ToLower().Contains("denied"))
                {
                    errorOutput += " (Note : Droits Administrateur requis)";
                }
                AuditLog += $"❌ ÉCHEC : {errorOutput}\n\n";
            }
            else
            {
                AuditLog += "✅ SUCCÈS\n\n";
            }
            await Task.Delay(400);
        }
    }
}