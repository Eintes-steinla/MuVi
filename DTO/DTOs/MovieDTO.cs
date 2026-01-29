using System.ComponentModel;

namespace MuVi.DTO.DTOs
{
    public class MovieDTO
    {
        // Main properties
        public int MovieID { get; set; }
        public string Title { get; set; } = "";
        public string MovieType { get; set; } = "Phim lẻ";
        public int? CountryID { get; set; }
        public string? Director { get; set; }
        public int? ReleaseYear { get; set; }
        public string? Description { get; set; }
        public string? PosterPath { get; set; }
        public string? TrailerURL { get; set; }
        public string? VideoPath { get; set; }
        public int? Duration { get; set; }
        public int? TotalEpisodes { get; set; }
        public decimal? Rating { get; set; }
        public int? ViewCount { get; set; }
        public string Status { get; set; } = "Đang chiếu";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Display property (not in database)
        public string? CountryName { get; set; }

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