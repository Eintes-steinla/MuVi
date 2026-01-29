using System;
using System.ComponentModel;

namespace MuVi.DTO.DTOs
{
    public class ReviewDTO : INotifyPropertyChanged
    {
        // Main properties
        public int ReviewID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }
        public int? Rating { get; set; }  // Điểm đánh giá 1-10
        public string? Comment { get; set; }
        public int? LikeCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Display properties (not in database)
        public string? MovieTitle { get; set; }
        public string? Username { get; set; }

        // Formatted display properties
        public string RatingFormatted
        {
            get => Rating.HasValue ? $"{Rating}/10" : "Chưa đánh giá";
        }

        public string CreatedAtFormatted
        {
            get => CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "";
        }

        public string UpdatedAtFormatted
        {
            get => UpdatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "";
        }

        public string CommentPreview
        {
            get
            {
                if (string.IsNullOrEmpty(Comment)) return "Không có bình luận";
                return Comment.Length > 50 ? Comment.Substring(0, 50) + "..." : Comment;
            }
        }

        public string LikeCountFormatted
        {
            get => LikeCount.HasValue ? $"{LikeCount} ❤" : "0 ❤";
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
            return MovieTitle ?? "";
        }
    }
}