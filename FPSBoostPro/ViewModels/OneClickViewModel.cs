using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class OneClickViewModel : ObservableObject
    {
        [ObservableProperty] private string _statusMessage = "Système prêt.";

        [RelayCommand]
        private void Optimize()
        {
            StatusMessage = "Sécurisation : Création d'un point de restauration...";

            try
            {
                // 1. Création du point de restauration via PowerShell
                string restoreCommand = "Checkpoint-Computer -Description 'FPSBoostPro_BeforeTweaks' -RestorePointType 'MODIFY_SETTINGS'";

                ProcessStartInfo psiRestore = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{restoreCommand}\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    Verb = "runas" // Force le mode admin si nécessaire
                };

                using (Process? process = Process.Start(psiRestore))
                {
                    process?.WaitForExit();
                }

                StatusMessage = "Point de restauration créé ! Nettoyage en cours...";

                // 2. Ton code de nettoyage existant ici...
                // (Ex: vider les dossiers temporaires, etc.)

                StatusMessage = "Optimisation terminée en toute sécurité !";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur : {ex.Message}";
            }
        }
    }
}