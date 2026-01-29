using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MuVi.ViewModels
{
    public class ReviewViewModel : BaseViewModel
    {
        private readonly ReviewBLL _reviewBLL = new ReviewBLL();

        public ObservableCollection<ReviewDTO> ReviewList { get; set; }
        public ObservableCollection<UserDTO> UserList { get; set; }
        public ObservableCollection<MovieDTO> MovieList { get; set; }

        // Select All Checkbox
        private bool? _isAllSelected;
        public bool? IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (_isAllSelected != value)
                {
                    _isAllSelected = value;
                    OnPropertyChanged(nameof(IsAllSelected));

                    if (value.HasValue)
                    {
                        foreach (var review in ReviewList)
                        {
                            review.IsSelected = value.Value;
                        }
                    }
                }
            }
        }

        // Search keyword
        private string _searchKeyword = "";
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
                _reviewBLL.SetSearchKeyword(value);
                LoadReviews();
            }
        }

        // Rating filter
        private string _selectedRating = "Tất cả";
        public string SelectedRating
        {
            get => _selectedRating;
            set
            {
                _selectedRating = value;
                OnPropertyChanged(nameof(SelectedRating));

                if (value == "Tất cả")
                {
                    _reviewBLL.SetRatingFilter(null);
                }
                else if (int.TryParse(value, out int rating))
                {
                    _reviewBLL.SetRatingFilter(rating);
                }

                LoadReviews();
            }
        }

        // User filter
        private UserDTO? _selectedUser;
        public UserDTO? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                _reviewBLL.SetUserFilter(value?.UserID);
                LoadReviews();
            }
        }

        // Movie filter
        private MovieDTO? _selectedMovie;
        public MovieDTO? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged(nameof(SelectedMovie));
                _reviewBLL.SetMovieFilter(value?.MovieID);
                LoadReviews();
            }
        }

        // Pagination info
        private string _pageInfo;
        public string PageInfo
        {
            get => _pageInfo;
            set
            {
                _pageInfo = value;
                OnPropertyChanged(nameof(PageInfo));
            }
        }

        // Commands
        public ICommand RefreshCommand { get; set; }
        public ICommand ClearFilterCommand { get; set; }
        public ICommand DeleteSelectedCommand { get; set; }

        public ReviewViewModel()
        {
            ReviewList = new ObservableCollection<ReviewDTO>();
            UserList = new ObservableCollection<UserDTO>();
            MovieList = new ObservableCollection<MovieDTO>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadReviews());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedReviews());

            LoadUsers();
            LoadMovies();
            _reviewBLL.ClearFilters();
            LoadReviews();
        }

        public void LoadUsers()
        {
            var users = _reviewBLL.GetAllUsers();

            UserList.Clear();
            UserList.Add(new UserDTO { UserID = 0, Username = "Tất cả" });
            foreach (var u in users)
            {
                UserList.Add(u);
            }

            SelectedUser = UserList.FirstOrDefault();
        }

        public void LoadMovies()
        {
            var movies = _reviewBLL.GetAllMovies();

            MovieList.Clear();
            MovieList.Add(new MovieDTO { MovieID = 0, Title = "Tất cả" });
            foreach (var m in movies)
            {
                MovieList.Add(m);
            }

            SelectedMovie = MovieList.FirstOrDefault();
        }

        public void LoadReviews()
        {
            var reviews = _reviewBLL.GetReviews();

            ReviewList.Clear();
            foreach (var r in reviews)
            {
                r.PropertyChanged += Review_PropertyChanged;
                ReviewList.Add(r);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void Review_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReviewDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (ReviewList == null || !ReviewList.Any())
            {
                _isAllSelected = false;
            }
            else if (ReviewList.All(r => r.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (ReviewList.All(r => !r.IsSelected))
            {
                _isAllSelected = false;
            }
            else
            {
                _isAllSelected = null; // Indeterminate
            }

            OnPropertyChanged(nameof(IsAllSelected));
        }

        private void UpdatePageInfo()
        {
            int currentPage = _reviewBLL.GetCurrentPage();
            int totalPages = _reviewBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _reviewBLL.NextPage();
            LoadReviews();
        }

        public void PreviousPage()
        {
            _reviewBLL.PreviousPage();
            LoadReviews();
        }

        public void FirstPage()
        {
            _reviewBLL.FirstPage();
            LoadReviews();
        }

        public void LastPage()
        {
            _reviewBLL.LastPage();
            LoadReviews();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            SelectedRating = "Tất cả";
            SelectedUser = UserList.FirstOrDefault();
            SelectedMovie = MovieList.FirstOrDefault();
            _reviewBLL.ClearFilters();
            LoadReviews();
        }

        private void DeleteSelectedReviews()
        {
            var selectedReviews = ReviewList.Where(r => r.IsSelected).ToList();

            if (!selectedReviews.Any())
            {
                MessageBox.Show("Vui lòng chọn đánh giá cần xóa", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedReviews.Count} đánh giá đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var reviewIds = selectedReviews.Select(r => r.ReviewID).ToList();
                bool success = _reviewBLL.DeleteMultipleReviews(reviewIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadReviews();
                }
            }
        }
    }
}