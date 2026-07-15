using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.Versioning; // AJOUTER CETTE LIGNE

namespace FPSBoostPro.ViewModels
{
    [SupportedOSPlatform("windows")] // AJOUTER CETTE LIGNE
    public partial class NetworkViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _networkStatus = "Prêt à optimiser la latence.";

        [RelayCommand]
        private void OptimizeNetwork()
        {
            NetworkStatus = "Optimisation réseau en cours...";
            try
            {
                // 1. Clés de Registre existantes (Nagle's Algorithm & Throttling)
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", true))
                {
                    key?.SetValue("NetworkThrottlingIndex", 0xFFFFFFFF, RegistryValueKind.DWord);
                }

                // 2. Optimisations TCP via PowerShell / CMD
                RunCommand("netsh", "int tcp set global autotuninglevel=normal");
                RunCommand("netsh", "int tcp set global rss=enabled");

                // 3. Flush DNS & Reset IP
                RunCommand("ipconfig", "/flushdns");
                RunCommand("netsh", "winsock reset");

                NetworkStatus = "Réseau optimisé avec succès !";
            }
            catch (Exception ex)
            {
                NetworkStatus = $"Erreur : {ex.Message}";
            }
        }

        private void RunCommand(string fileName, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            using (Process? process = Process.Start(psi)) { process?.WaitForExit(); }
        }
    }
}