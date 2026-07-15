using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    [SupportedOSPlatform("windows")]
    public partial class GamingViewModel : ObservableObject
    {
        [ObservableProperty] private string _gamingStatus = "Prêt à optimiser.";
        [ObservableProperty] private string _auditLog = "En attente...\n";

        [ObservableProperty] private bool _applyGameMode = true;
        [ObservableProperty] private bool _applyDisableDVR = true;
        [ObservableProperty] private bool _applyMouseTweak = false;
        [ObservableProperty] private bool _applyTrim = true;
        [ObservableProperty] private bool _applyDisableHibernation = false;

        [RelayCommand]
        private async Task OptimizeGaming()
        {
            GamingStatus = "Optimisation en cours...";
            AuditLog = "=== DÉBUT DE L'AUDIT GAMING ===\n\n";

            if (ApplyGameMode) await RunRegStep("⏳ Game Mode...", () => {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\GameBar", true))
                    key?.SetValue("AllowAutoGameMode", 1, RegistryValueKind.DWord);
            });

            if (ApplyDisableDVR) await RunRegStep("⏳ Désactivation Game DVR...", () => {
                using (RegistryKey? key1 = Registry.CurrentUser.OpenSubKey(@"System\GameConfigStore", true))
                    key1?.SetValue("GameDVR_Enabled", 0, RegistryValueKind.DWord);
                using (RegistryKey? key2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\GameDVR", true))
                    key2?.SetValue("AllowGameDVR", 0, RegistryValueKind.DWord);
            });

            if (ApplyMouseTweak) await RunRegStep("⏳ Optimisation Souris...", () => {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Mouse", true))
                {
                    key?.SetValue("MouseSpeed", "0", RegistryValueKind.String);
                    key?.SetValue("MouseThreshold1", "0", RegistryValueKind.String);
                    key?.SetValue("MouseThreshold2", "0", RegistryValueKind.String);
                }
            });

            if (ApplyTrim) await RunCmdStep("⏳ TRIM SSD...", "powershell.exe", "-Command \"Optimize-Volume -DriveLetter C -Defrag\"");
            if (ApplyDisableHibernation) await RunCmdStep("⏳ Désactivation Hibernation...", "powercfg", "-h off");

            AuditLog += "=== AUDIT TERMINÉ ===";
            GamingStatus = "✓ Gaming optimisé !";
        }

        private async Task RunRegStep(string message, Action action)
        {
            AuditLog += message + "\n";
            string errorMsg = "";
            bool success = await Task.Run(() => {
                try { action(); return true; }
                catch (Exception ex) { errorMsg = ex.Message; return false; }
            });

            if (!success)
            {
                if (errorMsg.Contains("0x80070005") || errorMsg.ToLower().Contains("refus") || errorMsg.ToLower().Contains("denied"))
                {
                    errorMsg += " (Note : Privilèges Administrateur requis)";
                }
                AuditLog += $"❌ ÉCHEC : {errorMsg}\n\n";
            }
            else
            {
                AuditLog += "✅ SUCCÈS\n\n";
            }
            await Task.Delay(400);
        }

        private async Task RunCmdStep(string message, string cmd, string args)
        {
            AuditLog += message + "\n";
            string errorOutput = "";

            bool success = await Task.Run(() => {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = cmd,
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

            if (!string.IsNullOrWhiteSpace(errorOutput))
            {
                errorOutput = errorOutput.Split('\n')[0].Trim();
            }
            else if (!success)
            {
                errorOutput = "Erreur système inconnue.";
            }

            if (!success)
            {
                if (errorOutput.Contains("0x80070005") || errorOutput.ToLower().Contains("refus") || errorOutput.ToLower().Contains("denied"))
                {
                    errorOutput += " (Note : Lancez l'application en tant qu'Administrateur)";
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