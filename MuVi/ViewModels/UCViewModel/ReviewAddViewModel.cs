using MuVi.BLL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MuVi.ViewModels.UCViewModel
{
    public class ReviewAddViewModel : BaseViewModel
    {
        #region Fields
        private ReviewDTO _review;
        private ReviewBLL _reviewBLL = new ReviewBLL();
        private bool _isAddMode = true;
        #endregion

        #region Properties

        public ReviewDTO Review
        {
            get => _review;
            set => SetProperty(ref _review, value);
        }

        public ObservableCollection<UserDTO> UserList { get; set; }
        public ObservableCollection<MovieDTO> MovieList { get; set; }

        public int ReviewID
        {
            get => _review?.ReviewID ?? 0;
        }

        public int? Rating
        {
            get => _review?.Rating;
            set
            {
                if (_review != null)
                {
                    _review.Rating = value;
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }

        public string Comment
        {
            get => _review?.Comment;
            set
            {
                if (_review != null)
                {
                    _review.Comment = value;
                    OnPropertyChanged(nameof(Comment));
                }
            }
        }

        private UserDTO? _selectedUser;
        public UserDTO? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                if (_review != null && value != null)
                {
                    _review.UserID = value.UserID;
                }
            }
        }

        private MovieDTO? _selectedMovie;
        public MovieDTO? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged(nameof(SelectedMovie));
                if (_review != null && value != null)
                {
                    _review.MovieID = value.MovieID;
                }
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
            set => SetProperty(ref _isAddMode, value);
        }

        #endregion

        #region Constructor

        public ReviewAddViewModel(ReviewDTO existingReview = null)
        {
            UserList = new ObservableCollection<UserDTO>();
            MovieList = new ObservableCollection<MovieDTO>();

            LoadUsers();
            LoadMovies();

            if (existingReview != null)
            {
                // Edit mode
                _review = new ReviewDTO
                {
                    ReviewID = existingReview.ReviewID,
                    MovieID = existingReview.MovieID,
                    UserID = existingReview.UserID,
                    Rating = existingReview.Rating,
                    Comment = existingReview.Comment,
                    LikeCount = existingReview.LikeCount
                };
                IsAddMode = false;

                // Set selected user
                SelectedUser = UserList.FirstOrDefault(u => u.UserID == _review.UserID);

                // Set selected movie
                SelectedMovie = MovieList.FirstOrDefault(m => m.MovieID == _review.MovieID);
            }
            else
            {
                // Add mode
                _review = new ReviewDTO
                {
                    Rating = 5,  // Mặc định 5 điểm
                    LikeCount = 0
                };
                IsAddMode = true;
            }
        }

        #endregion

        #region Methods

        private void LoadUsers()
        {
            var users = _reviewBLL.GetAllUsers();
            UserList.Clear();
            foreach (var u in users)
            {
                UserList.Add(u);
            }
        }

        private void LoadMovies()
        {
            var movies = _reviewBLL.GetAllMovies();
            MovieList.Clear();
            foreach (var m in movies)
            {
                MovieList.Add(m);
            }
        }

        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        public bool Validate()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn người dùng!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedMovie == null)
            {
                MessageBox.Show("Vui lòng chọn phim!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!Rating.HasValue || Rating < 1 || Rating > 10)
            {
                MessageBox.Show("Đánh giá phải từ 1 đến 10!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Comment))
            {
                MessageBox.Show("Vui lòng nhập bình luận!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}