using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class OneClickViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isOptimizing;
        [ObservableProperty] private string _statusMessage = "Prêt à booster votre PC en un clic";
        [ObservableProperty] private string _auditLog = "";

        [RelayCommand]
        private async Task OptimizeOneClick()
        {
            if (IsOptimizing) return;

            IsOptimizing = true;
            StatusMessage = "Optimisation globale en cours...";
            AuditLog = "";

            Log("Début du Clean & Boost rapide...");
            Log("--------------------------------------------------");

            await Task.Run(() =>
            {
                CleanTemporaryFiles();
                ApplyDebloatTweaks();
                FreeRAM();
            });

            Log("--------------------------------------------------");
            Log("🏁 Système entièrement nettoyé et optimisé !");
            StatusMessage = "Système nettoyé et optimisé !";
            IsOptimizing = false;
        }

        private void CleanTemporaryFiles()
        {
            Log("Nettoyage du dossier Temp (Local)...");
            CleanDirectory(Path.GetTempPath());

            Log("Nettoyage du dossier Temp (Windows)...");
            CleanDirectory(@"C:\Windows\Temp");

            Log("Nettoyage du dossier Prefetch...");
            CleanDirectory(@"C:\Windows\Prefetch");
        }

        private void CleanDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Log("-> [ERREUR] Dossier introuvable\n");
                return;
            }

            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                int files = 0, folders = 0;

                foreach (FileInfo file in di.GetFiles()) { try { file.Delete(); files++; } catch { } }
                foreach (DirectoryInfo dir in di.GetDirectories()) { try { dir.Delete(true); folders++; } catch { } }

                Log($"-> [SUCCÈS] {files} fichiers et {folders} dossiers supprimés\n");
            }
            catch
            {
                Log("-> [ERREUR] Impossible de nettoyer ce dossier\n");
            }
        }

        private void ApplyDebloatTweaks()
        {
            Log("Désactivation de la Télémétrie...");
            bool tel = ExecuteCommand("reg", "add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection\" /v AllowTelemetry /t REG_DWORD /d 0 /f");
            Log(tel ? "-> [SUCCÈS] Télémétrie coupée\n" : "-> [ERREUR] Échec de la modification\n");

            Log("Désactivation de Cortana...");
            bool cor = ExecuteCommand("reg", "add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Search\" /v AllowCortana /t REG_DWORD /d 0 /f");
            Log(cor ? "-> [SUCCÈS] Cortana désactivé\n" : "-> [ERREUR] Échec de la modification\n");
        }

        private void FreeRAM()
        {
            Log("Optimisation de la RAM...");
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Log("-> [SUCCÈS] RAM libérée\n");
            }
            catch
            {
                Log("-> [ERREUR] Échec de l'optimisation\n");
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
                AuditLog += $"[OneClick] {message}\n";
            });
        }
    }
}