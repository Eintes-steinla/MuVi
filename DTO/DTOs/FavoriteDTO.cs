using System;
using System.ComponentModel;

namespace MuVi.DTO.DTOs
{
    public class FavoriteDTO : INotifyPropertyChanged
    {
        // Main properties
        public int UserID { get; set; }
        public int MovieID { get; set; }
        public DateTime? AddedAt { get; set; }

        // Display properties (not in database)
        public string? Username { get; set; }
        public string? MovieTitle { get; set; }
        public string? MovieType { get; set; }
        public int? ReleaseYear { get; set; }
        public string? PosterPath { get; set; }
        public decimal? Rating { get; set; }
        public string? GenreNames { get; set; }

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
            return $"{Username} - {MovieTitle}";
        }
    }
}