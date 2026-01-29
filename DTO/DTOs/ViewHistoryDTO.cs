using System;
using System.ComponentModel;

namespace MuVi.DTO.DTOs
{
    public class ViewHistoryDTO : INotifyPropertyChanged
    {
        // Main properties
        public int HistoryID { get; set; }
        public int UserID { get; set; }
        public int MovieID { get; set; }
        public int? EpisodeID { get; set; }
        public DateTime? WatchedAt { get; set; }
        public int? WatchDuration { get; set; }  // Thời gian đã xem (giây)
        public bool IsCompleted { get; set; }

        // Display properties (not in database)
        public string? Username { get; set; }
        public string? MovieTitle { get; set; }
        public string? EpisodeTitle { get; set; }
        public string? MovieType { get; set; }
        public int? EpisodeNumber { get; set; }

        // Formatted display properties
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(EpisodeTitle))
                    return MovieTitle ?? "";
                return $"{MovieTitle} - Tập {EpisodeNumber}: {EpisodeTitle}";
            }
        }

        public string WatchDurationFormatted
        {
            get
            {
                if (!WatchDuration.HasValue) return "0 phút";
                int minutes = WatchDuration.Value / 60;
                int seconds = WatchDuration.Value % 60;
                return $"{minutes}:{seconds:D2}";
            }
        }

        public string StatusText
        {
            get => IsCompleted ? "Đã xem hết" : "Chưa xem hết";
        }

        public string WatchedAtFormatted
        {
            get => WatchedAt?.ToString("dd/MM/yyyy HH:mm") ?? "";
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
    }
}