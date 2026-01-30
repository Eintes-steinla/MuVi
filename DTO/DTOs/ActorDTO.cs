using System.ComponentModel;

namespace MuVi.DTO.DTOs
{
    public class ActorDTO : INotifyPropertyChanged
    {
        // Main properties
        public int ActorID { get; set; }
        public string ActorName { get; set; } = "";
        public string? Bio { get; set; }
        public string? PhotoPath { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public DateTime? CreatedAt { get; set; }

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
            return ActorName ?? "";
        }
    }
}