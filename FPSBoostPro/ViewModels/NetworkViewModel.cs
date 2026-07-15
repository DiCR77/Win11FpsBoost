using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace FPSBoostPro.ViewModels
{
    [SupportedOSPlatform("windows")]
    public partial class NetworkViewModel : ObservableObject
    {
        [ObservableProperty] private string _networkStatus = "Prêt à optimiser.";
        [ObservableProperty] private string _auditLog = "En attente...\n";

        [RelayCommand]
        private async Task OptimizeNetwork()
        {
            NetworkStatus = "Optimisation en cours...";
            AuditLog = "=== DÉBUT DE L'AUDIT RÉSEAU ===\n\n";

            AuditLog += "⏳ Registre : Index de limitation réseau...\n";
            bool regSuccess = await Task.Run(() => {
                try
                {
                    using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", true))
                    {
                        key?.SetValue("NetworkThrottlingIndex", -1, RegistryValueKind.DWord);
                        return true;
                    }
                }
                catch { return false; }
            });
            AuditLog += regSuccess ? "✅ SUCCÈS\n\n" : "❌ ÉCHEC\n\n";
            await Task.Delay(400);

            await RunStep("⏳ Optimisation TCP Autotuning...", "netsh", "int tcp set global autotuninglevel=normal");
            await RunStep("⏳ Activation RSS...", "netsh", "int tcp set global rss=enabled");
            await RunStep("⏳ Vidage DNS...", "ipconfig", "/flushdns");
            await RunStep("⏳ Reset Winsock...", "netsh", "winsock reset");

            AuditLog += "=== AUDIT TERMINÉ ===";
            NetworkStatus = "✓ Réseau optimisé !";
        }

        private async Task RunStep(string message, string cmd, string args)
        {
            AuditLog += message + "\n";
            bool success = await Task.Run(() => {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo { FileName = cmd, Arguments = args, WindowStyle = ProcessWindowStyle.Hidden, CreateNoWindow = true };
                    using (Process? p = Process.Start(psi)) { p?.WaitForExit(); return p != null && p.ExitCode == 0; }
                }
                catch { return false; }
            });
            AuditLog += success ? "✅ SUCCÈS\n\n" : "❌ ÉCHEC\n\n";
            await Task.Delay(400);
        }
    }
}