using System.ComponentModel;

namespace MuVi.DTO.DTOs
{
    public class EpisodeDTO : INotifyPropertyChanged
    {
        // Main properties
        public int EpisodeID { get; set; }
        public int MovieID { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public string? PosterPath { get; set; }
        public string? VideoPath { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? ViewCount { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Display properties (not in database)
        public string? MovieTitle { get; set; }
        public string? MovieType { get; set; }

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