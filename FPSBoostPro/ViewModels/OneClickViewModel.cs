using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    [SupportedOSPlatform("windows")]
    public partial class OneClickViewModel : ObservableObject
    {
        [ObservableProperty] private string _statusMessage = "Système prêt.";
        [ObservableProperty] private string _auditLog = "En attente...\n";

        [RelayCommand]
        private async Task Optimize()
        {
            StatusMessage = "Optimisation globale en cours...";
            AuditLog = "=== DÉBUT DU NETTOYAGE ET DEBLOAT ===\n\n";

            AuditLog += "⏳ Création Point de Restauration...\n";
            string restoreError = "";
            bool restoreSuccess = await Task.Run(() => {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = "-Command \"Checkpoint-Computer -Description 'FPSBoost' -RestorePointType 'MODIFY_SETTINGS'\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true
                    };
                    using (Process? p = Process.Start(psi))
                    {
                        restoreError = p?.StandardError.ReadToEnd() ?? "";
                        p?.WaitForExit();
                        return p != null && p.ExitCode == 0;
                    }
                }
                catch (Exception ex)
                {
                    restoreError = ex.Message;
                    return false;
                }
            });

            if (restoreSuccess)
            {
                AuditLog += "✅ SUCCÈS\n\n";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(restoreError))
                {
                    restoreError = restoreError.Split('\n')[0].Trim();
                }
                else
                {
                    restoreError = "La protection système est désactivée sur ce PC ou cette VM.";
                }

                if (restoreError.Contains("0x80070005") || restoreError.ToLower().Contains("refus") || restoreError.ToLower().Contains("denied"))
                {
                    restoreError += " (Note : Relancez FPSBoostPro en tant qu'Administrateur)";
                }

                AuditLog += $"❌ ÉCHEC : {restoreError}\n\n";
            }
            await Task.Delay(400);

            AuditLog += "⏳ Nettoyage %TEMP%...\n";
            long tempBytes = await Task.Run(() => CleanDirectory(Path.GetTempPath()));
            AuditLog += $"✅ SUCCÈS ({tempBytes / 1024 / 1024} Mo libérés)\n\n";
            await Task.Delay(400);

            AuditLog += "⏳ Nettoyage Windows Temp...\n";
            long winTempBytes = await Task.Run(() => CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp")));
            AuditLog += $"✅ SUCCÈS ({winTempBytes / 1024 / 1024} Mo libérés)\n\n";
            await Task.Delay(400);

            AuditLog += "⏳ Désactivation de Cortana, Widgets et Astuces...\n";
            bool debloatSuccess = await Task.Run(() => ApplyDebloatTweaks());
            AuditLog += debloatSuccess ? "✅ SUCCÈS\n\n" : "⚠️ ÉCHEC (Droits administrateur requis)\n\n";
            await Task.Delay(400);

            AuditLog += "⏳ Suppression du délai de démarrage...\n";
            bool startupSuccess = await Task.Run(() => DisableStartupDelay());
            AuditLog += startupSuccess ? "✅ SUCCÈS\n\n" : "⚠️ ÉCHEC\n\n";
            await Task.Delay(400);

            AuditLog += "⏳ Arrêt et désinstallation de OneDrive...\n";
            bool oneDriveSuccess = await Task.Run(() => RemoveOneDrive());
            AuditLog += oneDriveSuccess ? "✅ SUCCÈS\n\n" : "⚠️ ÉCHEC\n\n";
            await Task.Delay(400);

            AuditLog += "=== AUDIT TERMINÉ ===";
            StatusMessage = "✓ Système nettoyé et allégé !";
        }

        private long CleanDirectory(string path)
        {
            long bytesSaved = 0;
            if (!Directory.Exists(path)) return 0;
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles()) { try { bytesSaved += file.Length; file.Delete(); } catch { } }
            foreach (DirectoryInfo dir in di.GetDirectories()) { try { dir.Delete(true); } catch { } }
            return bytesSaved;
        }

        private bool ApplyDebloatTweaks()
        {
            try
            {
                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Windows Search"))
                {
                    key?.SetValue("AllowCortana", 0, RegistryValueKind.DWord);
                }

                using (RegistryKey? key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Dsh"))
                {
                    key?.SetValue("AllowNewsAndInterests", 0, RegistryValueKind.DWord);
                }

                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"))
                {
                    key?.SetValue("SubscribedContent-338389Enabled", 0, RegistryValueKind.DWord);
                    key?.SetValue("SubscribedContent-353698Enabled", 0, RegistryValueKind.DWord);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool DisableStartupDelay()
        {
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize"))
                {
                    key?.SetValue("StartupDelayInMSec", 0, RegistryValueKind.DWord);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool RemoveOneDrive()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c taskkill /f /im OneDrive.exe & %SystemRoot%\\SysWOW64\\OneDriveSetup.exe /uninstall",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process? process = Process.Start(startInfo))
                {
                    process?.WaitForExit();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}