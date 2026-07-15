using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using FPSBoostPro.ViewModels; // AJOUTE CETTE LIGNE

namespace FPSBoostPro.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // LIE LE VIEWMODEL À LA VUE (CRUCIAL POUR LES BOUTONS ET L'AFFICHAGE)
            this.DataContext = new MainViewModel();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
            }
            catch { }
            e.Handled = true;
        }
    }
}