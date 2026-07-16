using CommunityToolkit.Mvvm.ComponentModel;

namespace FPSBoostPro.Models
{
    public partial class ServiceItem : ObservableObject
    {
        public string DisplayName { get; set; } = string.Empty;
        public string[] ServiceNames { get; set; } = [];
        public string Description { get; set; } = string.Empty;

        private bool _isSelected = true;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}