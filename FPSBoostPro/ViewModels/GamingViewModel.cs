using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class GamingViewModel : ObservableObject
    {
        private bool _isOptimizing;
        public bool IsOptimizing
        {
            get => _isOptimizing;
            set => SetProperty(ref _isOptimizing, value);
        }

        private string _statusMessage = "Prêt à optimiser le système pour le jeu";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public event Action<string>? OnLogReceived;

        [RelayCommand]
        public async Task OptimizeGamingAndVisualsAsync()
        {
            if (IsOptimizing) return;

            IsOptimizing = true;
            StatusMessage = "Optimisation globale en cours...";
            Log("Début des optimisations Gaming et Interface...");

            await Task.Run(() =>
            {
                ApplyAllRegistryTweaks();
                ApplyAdvancedGamingTweaks();
            });

            StatusMessage = "Système optimisé pour le jeu ! (Redémarrage requis)";
            IsOptimizing = false;
        }

        private void ApplyAllRegistryTweaks()
        {
            try
            {
                // --- PARTIE 1 : GAMING (XBOX, DVR, GAME MODE) ---
                Log("Désactivation de Game DVR et des captures...");
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"System\GameConfigStore"))
                {
                    key?.SetValue("GameDVR_Enabled", 0, RegistryValueKind.DWord);
                    key?.SetValue("GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);
                }

                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\GameDVR"))
                {
                    key?.SetValue("AppCaptureEnabled", 0, RegistryValueKind.DWord);
                }

                Log("Désactivation Game Bar et activation Game Mode...");
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\GameBar"))
                {
                    key?.SetValue("ShowStartupPanel", 0, RegistryValueKind.DWord);
                    key?.SetValue("UseNexusForGameBarEnabled", 0, RegistryValueKind.DWord);
                    key?.SetValue("AllowAutoGameMode", 1, RegistryValueKind.DWord);
                }

                // --- PARTIE 2 : INTERFACE ET SOURIS (VISUELS) ---
                Log("Désactivation de la transparence Windows...");
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    key?.SetValue("EnableTransparency", 0, RegistryValueKind.DWord);
                }

                Log("Mode 'Ajuster pour de meilleures performances'...");
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects"))
                {
                    key?.SetValue("VisualFXSetting", 2, RegistryValueKind.DWord);
                }

                Log("Désactivation animations et accélération menus...");
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Control Panel\Desktop"))
                {
                    key?.SetValue("MenuShowDelay", "0", RegistryValueKind.String);
                    key?.SetValue("MinAnimate", "0", RegistryValueKind.String);
                }

                Log("Désactivation accélération souris...");
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Control Panel\Mouse"))
                {
                    key?.SetValue("MouseSpeed", "0", RegistryValueKind.String);
                    key?.SetValue("MouseThreshold1", "0", RegistryValueKind.String);
                    key?.SetValue("MouseThreshold2", "0", RegistryValueKind.String);
                }

                // --- NOUVEAUTÉS : HAGS ET VBS ---
                Log("Activation Hardware-Accelerated GPU Scheduling (HAGS)...");
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers"))
                {
                    key?.SetValue("HwSchMode", 2, RegistryValueKind.DWord);
                }

                Log("Désactivation VBS (Attention: Option avancée qui réduit la sécurité)...");
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"System\CurrentControlSet\Control\DeviceGuard"))
                {
                    key?.SetValue("EnableVirtualizationBasedSecurity", 0, RegistryValueKind.DWord);
                }

                Log("✅ Registre optimisé avec succès !");
            }
            catch (UnauthorizedAccessException)
            {
                Log("⚠️ ÉCHEC : Droits administrateur requis.");
            }
            catch (Exception ex)
            {
                Log($"⚠️ ÉCHEC Registre : {ex.Message}");
            }
        }

        private void ApplyAdvancedGamingTweaks()
        {
            Log("Désactivation du HPET et Hypervisor...");
            // Désactiver VBS via BCD
            ExecuteCommand("bcdedit", "/set hypervisorlaunchtype off");
            // Désactiver HPET
            ExecuteCommand("bcdedit", "/deletevalue useplatformclock");
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
            catch { }
        }

        private void Log(string message) => OnLogReceived?.Invoke($"[Gaming] {message}");
    }
}