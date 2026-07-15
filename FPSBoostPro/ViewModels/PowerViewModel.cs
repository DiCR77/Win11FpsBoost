using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FPSBoostPro.ViewModels
{
    public partial class PowerViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _powerStatus = "Prêt à optimiser l'alimentation.";

        [RelayCommand]
        private void OptimizePower()
        {
            PowerStatus = "Configuration de l'alimentation...";
            try
            {
                // 1. Activer le plan Ultimate Performance
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powercfg",
                    Arguments = "-duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };
                using (Process? process = Process.Start(psi)) { process?.WaitForExit(); }

                // 2. Désactiver la suspension sélective USB (Sur secteur)
                RunPowercfg("-setacvalueindex SCHEME_CURRENT SUB_USB 48e6d7a6-bc4d-4039-a546-f1363401c0c9 0");

                // 3. Désactiver la gestion d'alimentation PCIe Link State (Sur secteur)
                RunPowercfg("-setacvalueindex SCHEME_CURRENT SUB_PCIEXPRESS ee12f58e-cb17-4ac4-813c-8a0112706477 0");

                // Appliquer les changements immédiatement
                RunPowercfg("-setactive SCHEME_CURRENT");

                PowerStatus = "Plan d'alimentation débridé au maximum !";
            }
            catch (Exception ex)
            {
                PowerStatus = $"Erreur : {ex.Message}";
            }
        }

        private void RunPowercfg(string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powercfg",
                Arguments = args,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            using (Process? process = Process.Start(psi)) { process?.WaitForExit(); }
        }
    }
}