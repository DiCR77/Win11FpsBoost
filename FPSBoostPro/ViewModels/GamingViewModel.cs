using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class GamingViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isOptimizing;
        [ObservableProperty] private string _gamingStatus = "Prêt à optimiser le système pour le jeu";
        [ObservableProperty] private string _auditLog = "";

        [ObservableProperty] private bool _applyGameMode = true;
        [ObservableProperty] private bool _applyDisableDVR = true;
        [ObservableProperty] private bool _applyMouseTweak = true;
        [ObservableProperty] private bool _applyTrim = true;
        [ObservableProperty] private bool _applyDisableHibernation = true;

        [RelayCommand]
        private async Task OptimizeGaming()
        {
            if (IsOptimizing) return;

            IsOptimizing = true;
            GamingStatus = "Optimisation globale en cours...";
            AuditLog = "";

            Log("Début de l'optimisation Gaming...");
            Log("--------------------------------------------------");

            await Task.Run(() => ApplySelectedTweaks());

            GamingStatus = "Système optimisé ! (Redémarrage requis)";
            IsOptimizing = false;
        }

        private void ApplySelectedTweaks()
        {
            try
            {
                if (ApplyGameMode)
                {
                    Log("Activation du Game Mode...");
                    bool res = TryWriteRegistryDword(Registry.CurrentUser, @"Software\Microsoft\GameBar", "AllowAutoGameMode", 1);
                    Log(res ? "-> [SUCCÈS] Game Mode activé\n" : "-> [ERREUR] Échec de l'activation\n");
                }

                if (ApplyDisableDVR)
                {
                    Log("Désactivation de Game DVR et des captures...");
                    bool res1 = TryWriteRegistryDword(Registry.CurrentUser, @"System\GameConfigStore", "GameDVR_Enabled", 0);
                    bool res2 = TryWriteRegistryDword(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0);
                    Log(res1 && res2 ? "-> [SUCCÈS] Game DVR désactivé\n" : "-> [ERREUR] Impossible de désactiver\n");
                }

                if (ApplyMouseTweak)
                {
                    Log("Désactivation de l'accélération souris...");
                    bool res1 = TryWriteRegistryString(Registry.CurrentUser, @"Control Panel\Mouse", "MouseSpeed", "0");
                    bool res2 = TryWriteRegistryString(Registry.CurrentUser, @"Control Panel\Mouse", "MouseThreshold1", "0");
                    bool res3 = TryWriteRegistryString(Registry.CurrentUser, @"Control Panel\Mouse", "MouseThreshold2", "0");
                    Log(res1 && res2 && res3 ? "-> [SUCCÈS] Accélération désactivée\n" : "-> [ERREUR] Modification impossible\n");
                }

                if (ApplyTrim)
                {
                    Log("Activation du TRIM pour les SSD...");
                    bool res = ExecuteCommand("fsutil", "behavior set DisableDeleteNotify 0");
                    Log(res ? "-> [SUCCÈS] TRIM activé\n" : "-> [ERREUR] Commande refusée\n");
                }

                if (ApplyDisableHibernation)
                {
                    Log("Désactivation de l'hibernation...");
                    bool res = ExecuteCommand("powercfg", "-h off");
                    Log(res ? "-> [SUCCÈS] Hibernation désactivée et espace libéré\n" : "-> [ERREUR] Impossible de désactiver\n");
                }

                Log("--------------------------------------------------");
                Log("✅ Registre et paramètres optimisés avec succès !");
            }
            catch (Exception ex)
            {
                Log($"⚠️ Erreur critique : {ex.Message}");
            }
        }

        private bool TryWriteRegistryDword(RegistryKey baseKey, string subKeyPath, string valueName, int value)
        {
            try
            {
                using (RegistryKey? key = baseKey.CreateSubKey(subKeyPath))
                {
                    if (key == null) return false;
                    key.SetValue(valueName, value, RegistryValueKind.DWord);
                    return true;
                }
            }
            catch { return false; }
        }

        private bool TryWriteRegistryString(RegistryKey baseKey, string subKeyPath, string valueName, string value)
        {
            try
            {
                using (RegistryKey? key = baseKey.CreateSubKey(subKeyPath))
                {
                    if (key == null) return false;
                    key.SetValue(valueName, value, RegistryValueKind.String);
                    return true;
                }
            }
            catch { return false; }
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
                AuditLog += $"[Gaming] {message}\n";
            });
        }
    }
}