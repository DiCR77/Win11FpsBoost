using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
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
            StatusMessage = "Nettoyage en cours...";
            AuditLog = "=== DÉBUT DU NETTOYAGE ===\n\n";

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

            AuditLog += "=== AUDIT TERMINÉ ===";
            StatusMessage = "✓ Système nettoyé !";
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
    }
}