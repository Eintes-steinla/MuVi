using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using MuVi.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace MuVi.ViewModels
{
    /// <summary>
    /// ViewModel cho trang chủ người dùng (User Home)
    /// Hiển thị: Phim mới, phim nổi bật, phim xem nhiều, phim đề xuất
    /// </summary>
    public class UserHomeViewModel : BaseViewModel
    {
        private readonly MovieBLL _movieBLL;
        private readonly ReviewBLL _reviewBLL;
        private readonly ViewHistoryBLL _viewHistoryBLL;

        #region Properties

        // Danh sách phim mới nhất
        private ObservableCollection<MovieDTO> _newMovies;
        public ObservableCollection<MovieDTO> NewMovies
        {
            get => _newMovies;
            set { _newMovies = value; OnPropertyChanged(nameof(NewMovies)); }
        }

        // Danh sách phim nổi bật (rating cao)
        private ObservableCollection<MovieDTO> _featuredMovies;
        public ObservableCollection<MovieDTO> FeaturedMovies
        {
            get => _featuredMovies;
            set { _featuredMovies = value; OnPropertyChanged(nameof(FeaturedMovies)); }
        }

        // Danh sách phim xem nhiều nhất
        private ObservableCollection<MovieDTO> _popularMovies;
        public ObservableCollection<MovieDTO> PopularMovies
        {
            get => _popularMovies;
            set { _popularMovies = value; OnPropertyChanged(nameof(PopularMovies)); }
        }

        // Danh sách phim đề xuất (dựa trên lịch sử)
        private ObservableCollection<MovieDTO> _recommendedMovies;
        public ObservableCollection<MovieDTO> RecommendedMovies
        {
            get => _recommendedMovies;
            set { _recommendedMovies = value; OnPropertyChanged(nameof(RecommendedMovies)); }
        }

        // Phim đang được chọn
        private MovieDTO _selectedMovie;
        public MovieDTO SelectedMovie
        {
            get => _selectedMovie;
            set { _selectedMovie = value; OnPropertyChanged(nameof(SelectedMovie)); }
        }

        // Loading indicator
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        #endregion

        #region Commands

        public ICommand LoadDataCommand { get; }
        public ICommand ViewMovieDetailCommand { get; }
        public ICommand PlayMovieCommand { get; }

        #endregion

        #region Constructor

        public UserHomeViewModel()
        {
            // Khởi tạo BLL
            _movieBLL = new MovieBLL();
            _reviewBLL = new ReviewBLL();
            _viewHistoryBLL = new ViewHistoryBLL();

            // Khởi tạo collections
            NewMovies = new ObservableCollection<MovieDTO>();
            FeaturedMovies = new ObservableCollection<MovieDTO>();
            PopularMovies = new ObservableCollection<MovieDTO>();
            RecommendedMovies = new ObservableCollection<MovieDTO>();

            // Khởi tạo commands
            LoadDataCommand = new RelayCommand(LoadData);
            ViewMovieDetailCommand = new RelayCommand(ViewMovieDetail);
            PlayMovieCommand = new RelayCommand(PlayMovie);

            // Load dữ liệu ban đầu
            LoadData(null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load tất cả dữ liệu cho trang chủ
        /// </summary>
        private void LoadData(object parameter)
        {
            IsLoading = true;

            try
            {
                // Load phim mới (12 phim mới nhất theo năm phát hành)
                LoadNewMovies();

                // Load phim nổi bật (12 phim có rating cao nhất)
                LoadFeaturedMovies();

                // Load phim xem nhiều (12 phim có lượt xem nhiều nhất)
                LoadPopularMovies();

                // Load phim đề xuất (nếu user đã đăng nhập)
                if (AppSession.Instance.CurrentUser != null)
                {
                    LoadRecommendedMovies();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Load danh sách phim mới nhất
        /// </summary>
        private void LoadNewMovies()
        {
            var allMovies = _movieBLL.GetAllMovies(out string message);
            if (allMovies != null)
            {
                // Lấy 12 phim mới nhất (sắp xếp theo năm phát hành giảm dần)
                var newMovies = allMovies
                    .OrderByDescending(m => m.ReleaseYear)
                    .ThenByDescending(m => m.CreatedAt)
                    .Take(12)
                    .ToList();

                NewMovies.Clear();
                foreach (var movie in newMovies)
                {
                    NewMovies.Add(movie);
                }
            }
        }

        /// <summary>
        /// Load danh sách phim nổi bật (rating cao)
        /// </summary>
        private void LoadFeaturedMovies()
        {
            var allMovies = _movieBLL.GetAllMovies(out string message);
            if (allMovies != null)
            {
                // Lấy 12 phim có rating cao nhất
                var featuredMovies = allMovies
                    .OrderByDescending(m => m.AverageRating)
                    .ThenByDescending(m => m.TotalReviews)
                    .Take(12)
                    .ToList();

                FeaturedMovies.Clear();
                foreach (var movie in featuredMovies)
                {
                    FeaturedMovies.Add(movie);
                }
            }
        }

        /// <summary>
        /// Load danh sách phim xem nhiều nhất
        /// </summary>
        private void LoadPopularMovies()
        {
            var allMovies = _movieBLL.GetAllMovies(out string message);
            if (allMovies != null)
            {
                // Lấy 12 phim có lượt xem nhiều nhất
                var popularMovies = allMovies
                    .OrderByDescending(m => m.ViewCount)
                    .Take(12)
                    .ToList();

                PopularMovies.Clear();
                foreach (var movie in popularMovies)
                {
                    PopularMovies.Add(movie);
                }
            }
        }

        /// <summary>
        /// Load danh sách phim đề xuất dựa trên lịch sử xem
        /// </summary>
        private void LoadRecommendedMovies()
        {
            if (AppSession.Instance.CurrentUser == null) return;

            // Lấy lịch sử xem của user
            var history = _viewHistoryBLL.GetViewHistoryByUser(AppSession.Instance.CurrentUser.UserID, out string message);
            if (history != null && history.Count > 0)
            {
                // Lấy danh sách MovieID đã xem
                var watchedMovieIds = history.Select(h => h.MovieID).Distinct().ToList();

                // Lấy tất cả phim
                var allMovies = _movieBLL.GetAllMovies(out string msg);
                if (allMovies != null)
                {
                    // Lấy phim chưa xem, có cùng thể loại hoặc quốc gia với phim đã xem
                    var recommendedMovies = allMovies
                        .Where(m => !watchedMovieIds.Contains(m.MovieID))
                        .OrderByDescending(m => m.AverageRating)
                        .ThenByDescending(m => m.ViewCount)
                        .Take(12)
                        .ToList();

                    RecommendedMovies.Clear();
                    foreach (var movie in recommendedMovies)
                    {
                        RecommendedMovies.Add(movie);
                    }
                }
            }
            else
            {
                // Nếu chưa có lịch sử, hiển thị phim nổi bật
                var allMovies = _movieBLL.GetAllMovies(out string msg);
                if (allMovies != null)
                {
                    var recommendedMovies = allMovies
                        .OrderByDescending(m => m.AverageRating)
                        .Take(12)
                        .ToList();

                    RecommendedMovies.Clear();
                    foreach (var movie in recommendedMovies)
                    {
                        RecommendedMovies.Add(movie);
                    }
                }
            }
        }

        /// <summary>
        /// Xem chi tiết phim
        /// </summary>
        private void ViewMovieDetail(object parameter)
        {
            if (parameter is MovieDTO movie)
            {
                SelectedMovie = movie;
                // Mở window chi tiết phim
                var detailWindow = new Views.UserMovieDetailView(movie);
                detailWindow.Show();
            }
        }

        /// <summary>
        /// Phát phim ngay lập tức
        /// </summary>
        private void PlayMovie(object parameter)
        {
            if (parameter is MovieDTO movie)
            {
                // Kiểm tra user đã đăng nhập chưa
                if (AppSession.Instance.CurrentUser == null)
                {
                    System.Windows.MessageBox.Show("Vui lòng đăng nhập để xem phim!",
                        "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                // Mở video player
                var playerWindow = new Views.VideoPlayerView(movie);
                playerWindow.Show();
            }
        }

        #endregion
    }
}