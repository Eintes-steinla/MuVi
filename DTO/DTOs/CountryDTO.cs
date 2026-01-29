using System.ComponentModel;

namespace MuVi.DTO.DTOs
{
    public class CountryDTO : INotifyPropertyChanged
    {
        // Main properties
        public int CountryID { get; set; }
        public string CountryName { get; set; } = "";
        public string? CountryCode { get; set; }

        // Property for checkbox selection
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return CountryName ?? "";
        }
    }
}