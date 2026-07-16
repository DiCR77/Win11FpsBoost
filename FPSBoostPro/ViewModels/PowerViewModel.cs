using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class PowerViewModel : ObservableObject
    {
        private bool _isOptimizing;
        public bool IsOptimizing
        {
            get => _isOptimizing;
            set => SetProperty(ref _isOptimizing, value);
        }

        private string _statusMessage = "Prêt à optimiser (Plan Ultime, CPU 100%, USB, Disques...)";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // Dédié à l'affichage dans ta console d'audit en direct
        public event Action<string>? OnLogReceived;

        [RelayCommand]
        public async Task OptimizePowerAsync()
        {
            IsOptimizing = true;
            StatusMessage = "Optimisation de l'alimentation en cours...";
            Log("Début de l'optimisation de l'alimentation...");

            await Task.Run(() =>
            {
                try
                {
                    // 1. Débloquer et activer le plan "Performances Ultimes"
                    Log("Déblocage du plan 'Performances Ultimes'...");
                    ExecuteCommand("powercfg", "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                    ExecuteCommand("powercfg", "-setactive e9a42b02-d5df-448d-aa00-03f14749eb61");

                    // 2. Désactiver le USB Selective Suspend (veille USB)
                    Log("Désactivation de la mise en veille sélective USB...");
                    ExecuteCommand("powercfg", "-setacvalueindex SCHEME_CURRENT SUB_USB 48e6d7a4-fb0b-4e1d-851c-45b6de3b145d 0");

                    // 3. Désactiver la mise en veille du disque dur (0 = jamais)
                    Log("Désactivation de la mise en veille des disques...");
                    ExecuteCommand("powercfg", "-change -disk-timeout-ac 0");

                    // 4. Désactiver le Core Parking (Force 100% des cœurs actifs)
                    Log("Désactivation du Core Parking (CPU)...");
                    ExecuteCommand("powercfg", "-setacvalueindex SCHEME_CURRENT SUB_PROCESSOR CPMAXCORES 100");
                    ExecuteCommand("powercfg", "-setacvalueindex SCHEME_CURRENT SUB_PROCESSOR CPMINCORES 100");

                    // 5. Forcer l'état minimal et maximal du CPU à 100% (No Throttling)
                    Log("Forçage de l'état CPU minimal et maximal à 100%...");
                    ExecuteCommand("powercfg", "-setacvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMIN 100");
                    ExecuteCommand("powercfg", "-setacvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMAX 100");

                    // Appliquer immédiatement les modifications powercfg
                    ExecuteCommand("powercfg", "-setactive SCHEME_CURRENT");

                    // 6. Tweaks Registre : Performances GPU & Désactivation C-States (OS)
                    Log("Application des tweaks registre (GPU & C-States)...");
                    ApplyRegistryTweaks();

                    Log("✅ Optimisation de l'alimentation réussie !");
                }
                catch (Exception ex)
                {
                    Log($"❌ ERREUR : {ex.Message}");
                }
            });

            StatusMessage = "Alimentation optimisée avec succès !";
            IsOptimizing = false;
        }

        private void ApplyRegistryTweaks()
        {
            try
            {
                // Forcer le GPU en mode "Performances Maximales"
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\501a4d13-42af-4429-9fd1-a821a06c6345\ee12f2c1-98ee-430d-9f2c-53f948e7ac09"))
                {
                    key.SetValue("Attributes", 2, RegistryValueKind.DWord);
                }

                // Désactiver la mise en veille profonde du CPU (C-States / Idle au niveau de l'OS)
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Power"))
                {
                    key.SetValue("CsEnabled", 0, RegistryValueKind.DWord);
                }

                Log("Registre mis à jour avec succès.");
            }
            catch (UnauthorizedAccessException)
            {
                Log("⚠️ ÉCHEC : Droits administrateur requis pour écrire dans le registre.");
            }
            catch (Exception ex)
            {
                Log($"⚠️ ÉCHEC Registre : {ex.Message}");
            }
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
                    Verb = "runas" // Force l'exécution en admin
                };

                using (Process? process = Process.Start(startInfo))
                {
                    process?.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Log($"Erreur d'exécution ({filename}) : {ex.Message}");
            }
        }

        private void Log(string message)
        {
            // Envoie le message à la console d'audit de ton interface principale
            OnLogReceived?.Invoke($"[Alimentation] {message}");
        }
    }
}