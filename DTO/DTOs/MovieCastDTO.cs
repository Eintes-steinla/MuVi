using System.ComponentModel;

namespace MuVi.DTO.DTOs
{
    public class MovieCastDTO : INotifyPropertyChanged
    {
        // Main properties
        public int MovieID { get; set; }
        public int ActorID { get; set; }
        public string? RoleName { get; set; }
        public int? Order { get; set; }  // Thứ tự ưu tiên (1=chính, 2=phụ...)

        // Display properties (not in database)
        public string? MovieTitle { get; set; }
        public string? ActorName { get; set; }

        // Formatted display properties
        public string OrderText
        {
            get
            {
                if (!Order.HasValue) return "Không xác định";
                return Order.Value switch
                {
                    1 => "Vai chính",
                    2 => "Vai phụ",
                    3 => "Vai phụ",
                    _ => $"Thứ {Order.Value}"
                };
            }
        }

        public string DisplayInfo
        {
            get => $"{ActorName} - {RoleName ?? "Chưa rõ vai"} ({OrderText})";
        }

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
            return DisplayInfo;
        }
    }
}