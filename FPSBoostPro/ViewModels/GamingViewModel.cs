using System;
using System.Diagnostics;
using System.Runtime.Versioning; // AJOUTER CETTE LIGNE
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    [SupportedOSPlatform("windows")] // AJOUTER CETTE LIGNE
    public partial class GamingViewModel : ObservableObject
    {
        [ObservableProperty] private string _gamingStatus = "Sélectionnez vos options puis cliquez sur optimiser.";

        // États des cases à cocher (cochées par défaut pour le confort de l'utilisateur)
        [ObservableProperty] private bool _applyGameMode = true;
        [ObservableProperty] private bool _applyDisableDVR = true;
        [ObservableProperty] private bool _applyMouseTweak = false; // Désactivé par défaut, choix utilisateur
        [ObservableProperty] private bool _applyTrim = true;
        [ObservableProperty] private bool _applyDisableHibernation = false;

        [RelayCommand]
        private void OptimizeGaming()
        {
            GamingStatus = "Optimisation en cours...";
            int count = 0;

            try
            {
                // 1. Game Mode
                if (ApplyGameMode)
                {
                    using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\GameBar", true))
                    {
                        key?.SetValue("AllowAutoGameMode", 1, RegistryValueKind.DWord);
                    }
                    count++;
                }

                // 2. Game DVR
                if (ApplyDisableDVR)
                {
                    using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"System\GameConfigStore", true))
                    {
                        key?.SetValue("GameDVR_Enabled", 0, RegistryValueKind.DWord);
                    }
                    using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\GameDVR", true))
                    {
                        key?.SetValue("AllowGameDVR", 0, RegistryValueKind.DWord);
                    }
                    count++;
                }

                // 3. Mouse Tweak
                if (ApplyMouseTweak)
                {
                    using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Mouse", true))
                    {
                        key?.SetValue("MouseSpeed", "0", RegistryValueKind.String);
                        key?.SetValue("MouseThreshold1", "0", RegistryValueKind.String);
                        key?.SetValue("MouseThreshold2", "0", RegistryValueKind.String);
                    }
                    count++;
                }

                // 4. TRIM SSD
                if (ApplyTrim)
                {
                    RunPowerShell("Optimize-Volume -DriveLetter C -Defrag -Verbose");
                    count++;
                }

                // 5. Disable Hibernation
                if (ApplyDisableHibernation)
                {
                    RunPowerShell("powercfg -h off");
                    count++;
                }

                GamingStatus = $"Succès ! {count} optimisation(s) appliquée(s). Redémarrez le PC.";
            }
            catch (UnauthorizedAccessException)
            {
                GamingStatus = "Erreur : Relancer l'application en tant qu'administrateur !";
            }
            catch (Exception ex)
            {
                GamingStatus = $"Erreur : {ex.Message}";
            }
        }

        // Méthode utilitaire pour exécuter du PowerShell proprement en tâche de fond
        private void RunPowerShell(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{command}\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            using (Process? process = Process.Start(psi))
            {
                process?.WaitForExit();
            }
        }
    }
}