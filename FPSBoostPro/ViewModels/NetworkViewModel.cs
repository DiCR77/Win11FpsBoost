using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class NetworkViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isOptimizing;
        [ObservableProperty] private string _networkStatus = "Prêt à optimiser le réseau";
        [ObservableProperty] private string _auditLog = "";

        [RelayCommand]
        private async Task OptimizeNetwork()
        {
            if (IsOptimizing) return;

            IsOptimizing = true;
            NetworkStatus = "Optimisation en cours...";
            AuditLog = "";

            Log("Début de l'optimisation Réseau...");
            Log("--------------------------------------------------");

            await Task.Run(() => ApplyNetworkTweaks());

            NetworkStatus = "Réseau optimisé avec succès !";
            IsOptimizing = false;
        }

        private void ApplyNetworkTweaks()
        {
            try
            {
                Log("Réinitialisation du catalogue Winsock...");
                bool winsock = ExecuteCommand("netsh", "winsock reset");
                Log(winsock ? "-> [SUCCÈS] Winsock réinitialisé\n" : "-> [ERREUR] Échec de la réinitialisation\n");

                Log("Vidage du cache DNS...");
                bool dns = ExecuteCommand("ipconfig", "/flushdns");
                Log(dns ? "-> [SUCCÈS] Cache DNS vidé avec succès\n" : "-> [ERREUR] Impossible de vider le cache\n");

                Log("Réinitialisation du protocole TCP/IP...");
                bool tcp = ExecuteCommand("netsh", "int ip reset");
                Log(tcp ? "-> [SUCCÈS] Protocole TCP/IP restauré\n" : "-> [ERREUR] Échec de la commande\n");

                Log("--------------------------------------------------");
                Log("✅ Optimisations réseau terminées !");
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
            catch { return false; }
        }

        private void Log(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AuditLog += $"[Réseau] {message}\n";
            });
        }
    }
}