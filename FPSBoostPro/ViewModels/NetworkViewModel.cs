using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class NetworkViewModel : ObservableObject
    {
        private bool _isOptimizing;
        public bool IsOptimizing
        {
            get => _isOptimizing;
            set => SetProperty(ref _isOptimizing, value);
        }

        private string _statusMessage = "Prêt à optimiser le réseau";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public event Action<string>? OnLogReceived;

        [RelayCommand]
        public async Task OptimizeNetworkAsync()
        {
            IsOptimizing = true;
            StatusMessage = "Optimisation du réseau en cours...";
            Log("Début de l'optimisation réseau...");

            await Task.Run(() =>
            {
                try
                {
                    // 1. Flush DNS
                    Log("Vidage du cache DNS...");
                    ExecuteCommand("ipconfig", "/flushdns");

                    // 2. Changer DNS (Cloudflare) via PowerShell
                    Log("Application des serveurs DNS Cloudflare (1.1.1.1)...");
                    ExecuteCommand("powershell", "-Command \"Set-DnsClientServerAddress -InterfaceAlias '*' -ServerAddresses '1.1.1.1','1.0.0.1'\"");

                    // 3. TCP Timestamps & Autotuning
                    Log("Désactivation des TCP Timestamps et configuration Autotuning...");
                    ExecuteCommand("netsh", "int tcp set global timestamps=disabled");
                    ExecuteCommand("netsh", "int tcp set global autotuninglevel=normal");
                    ExecuteCommand("netsh", "int tcp set global rss=enabled");

                    // 4. Désactiver IPv6 (si non utilisé)
                    Log("Désactivation de l'IPv6 sur les cartes réseau...");
                    ExecuteCommand("powershell", "-Command \"Disable-NetAdapterBinding -Name '*' -ComponentID ms_tcpip6\"");

                    // 5. Tweaks Registre (Nagle, Throttling, QoS, P2P)
                    Log("Application des tweaks registre réseau...");
                    ApplyRegistryTweaks();

                    Log("✅ Optimisation réseau terminée ! Redémarrage du PC recommandé.");
                }
                catch (Exception ex)
                {
                    Log($"❌ ERREUR : {ex.Message}");
                }
            });

            StatusMessage = "Réseau optimisé avec succès !";
            IsOptimizing = false;
        }

        private void ApplyRegistryTweaks()
        {
            try
            {
                // A. Network Throttling Index (Désactiver la limitation)
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile"))
                {
                    key?.SetValue("NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
                    key?.SetValue("SystemResponsiveness", 0, RegistryValueKind.DWord);
                }

                // B. QoS Bandwidth Limit (Désactiver la limite de 20%)
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Psched"))
                {
                    key?.SetValue("NonBestEffortLimit", 0, RegistryValueKind.DWord);
                }

                // C. Windows Update Delivery Optimization (Désactiver le P2P)
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DeliveryOptimization"))
                {
                    // 0 = Téléchargement HTTP uniquement (pas de P2P)
                    key?.SetValue("DODownloadMode", 0, RegistryValueKind.DWord);
                }

                // D. Algorithme de Nagle (Boucle sur toutes les interfaces réseau actives)
                using (RegistryKey? interfacesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces", true))
                {
                    if (interfacesKey != null)
                    {
                        foreach (string subKeyName in interfacesKey.GetSubKeyNames())
                        {
                            using (RegistryKey? subKey = interfacesKey.OpenSubKey(subKeyName, true))
                            {
                                // On cible uniquement les cartes réseau avec une adresse IP attribuée
                                if (subKey?.GetValue("DhcpIPAddress") != null || subKey?.GetValue("IPAddress") != null)
                                {
                                    subKey.SetValue("TcpAckFrequency", 1, RegistryValueKind.DWord);
                                    subKey.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);
                                    subKey.SetValue("TcpDelAckTicks", 0, RegistryValueKind.DWord);
                                }
                            }
                        }
                    }
                }

                Log("Registre réseau mis à jour avec succès.");
            }
            catch (UnauthorizedAccessException)
            {
                Log("⚠️ ÉCHEC : Droits administrateur requis pour le registre.");
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
                    Verb = "runas"
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
            OnLogReceived?.Invoke($"[Réseau] {message}");
        }
    }
}