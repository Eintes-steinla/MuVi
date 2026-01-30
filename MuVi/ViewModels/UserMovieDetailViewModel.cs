using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using MuVi.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MuVi.ViewModels
{
    /// <summary>
    /// ViewModel cho chi tiết phim
    /// Chức năng: Hiển thị thông tin chi tiết, đánh giá, bình luận, yêu thích
    /// </summary>
    public class UserMovieDetailViewModel : BaseViewModel
    {
        private readonly MovieBLL _movieBLL;
        private readonly ReviewBLL _reviewBLL;
        private readonly FavoriteBLL _favoriteBLL;
        private readonly ViewHistoryBLL _viewHistoryBLL;
        private readonly MovieCastBLL _movieCastBLL;
        private readonly ActorBLL _actorBLL;
        private readonly EpisodeBLL _episodeBLL;

        #region Properties

        // Phim hiện tại
        private MovieDTO _currentMovie;
        public MovieDTO CurrentMovie
        {
            get => _currentMovie;
            set
            {
                _currentMovie = value;
                OnPropertyChanged(nameof(CurrentMovie));
                LoadMovieDetails();
            }
        }

        // Danh sách review/bình luận
        private ObservableCollection<ReviewDTO> _reviews;
        public ObservableCollection<ReviewDTO> Reviews
        {
            get => _reviews;
            set { _reviews = value; OnPropertyChanged(nameof(Reviews)); }
        }

        // Danh sách diễn viên
        private ObservableCollection<ActorDTO> _actors;
        public ObservableCollection<ActorDTO> Actors
        {
            get => _actors;
            set { _actors = value; OnPropertyChanged(nameof(Actors)); }
        }

        // Danh sách tập phim (nếu là phim bộ)
        private ObservableCollection<EpisodeDTO> _episodes;
        public ObservableCollection<EpisodeDTO> Episodes
        {
            get => _episodes;
            set { _episodes = value; OnPropertyChanged(nameof(Episodes)); }
        }

        // Rating của user hiện tại
        private int _userRating;
        public int UserRating
        {
            get => _userRating;
            set
            {
                _userRating = value;
                OnPropertyChanged(nameof(UserRating));
            }
        }

        // Bình luận của user
        private string _userComment;
        public string UserComment
        {
            get => _userComment;
            set
            {
                _userComment = value;
                OnPropertyChanged(nameof(UserComment));
            }
        }

        // Đã thích phim chưa
        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
                OnPropertyChanged(nameof(FavoriteButtonText));
            }
        }

        public string FavoriteButtonText => IsFavorite ? "Đã thích ❤" : "Yêu thích 🤍";

        // Loading
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        // Có phải phim bộ không
        public bool IsSeriesMovie => CurrentMovie?.MovieType == "Phim bộ";

        #endregion

        #region Commands

        public ICommand PlayMovieCommand { get; }
        public ICommand SubmitReviewCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand PlayEpisodeCommand { get; }
        public ICommand ViewTrailerCommand { get; }

        #endregion

        #region Constructor

        public UserMovieDetailViewModel(MovieDTO movie)
        {
            // Khởi tạo BLL
            _movieBLL = new MovieBLL();
            _reviewBLL = new ReviewBLL();
            _favoriteBLL = new FavoriteBLL();
            _viewHistoryBLL = new ViewHistoryBLL();
            _movieCastBLL = new MovieCastBLL();
            _actorBLL = new ActorBLL();
            _episodeBLL = new EpisodeBLL();

            // Khởi tạo collections
            Reviews = new ObservableCollection<ReviewDTO>();
            Actors = new ObservableCollection<ActorDTO>();
            Episodes = new ObservableCollection<EpisodeDTO>();

            // Khởi tạo commands
            PlayMovieCommand = new RelayCommand(PlayMovie);
            SubmitReviewCommand = new RelayCommand(SubmitReview);
            ToggleFavoriteCommand = new RelayCommand(ToggleFavorite);
            PlayEpisodeCommand = new RelayCommand(PlayEpisode);
            ViewTrailerCommand = new RelayCommand(ViewTrailer);

            // Set phim hiện tại
            CurrentMovie = movie;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load thông tin chi tiết phim
        /// </summary>
        private void LoadMovieDetails()
        {
            if (CurrentMovie == null) return;

            IsLoading = true;

            try
            {
                // Load reviews
                LoadReviews();

                // Load actors
                LoadActors();

                // Load episodes (nếu là phim bộ)
                if (IsSeriesMovie)
                {
                    LoadEpisodes();
                }

                // Check favorite status
                CheckFavoriteStatus();

                // Load user's existing review
                LoadUserReview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin phim: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Load danh sách review
        /// </summary>
        private void LoadReviews()
        {
            var reviews = _reviewBLL.GetReviewsByMovie(CurrentMovie.MovieID, out string message);
            if (reviews != null)
            {
                Reviews.Clear();
                foreach (var review in reviews.OrderByDescending(r => r.CreatedAt))
                {
                    Reviews.Add(review);
                }
            }
        }

        /// <summary>
        /// Load danh sách diễn viên
        /// </summary>
        private void LoadActors()
        {
            var movieCasts = _movieCastBLL.GetCastByMovie(CurrentMovie.MovieID, out string message);
            if (movieCasts != null)
            {
                Actors.Clear();
                foreach (var cast in movieCasts)
                {
                    var actor = _actorBLL.GetActorById(cast.ActorID, out string msg);
                    if (actor != null)
                    {
                        Actors.Add(actor);
                    }
                }
            }
        }

        /// <summary>
        /// Load danh sách tập phim
        /// </summary>
        private void LoadEpisodes()
        {
            var episodes = _episodeBLL.GetEpisodesByMovie(CurrentMovie.MovieID, out string message);
            if (episodes != null)
            {
                Episodes.Clear();
                foreach (var episode in episodes.OrderBy(e => e.EpisodeNumber))
                {
                    Episodes.Add(episode);
                }
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái yêu thích
        /// </summary>
        private void CheckFavoriteStatus()
        {
            if (AppSession.Instance.CurrentUser == null)
            {
                IsFavorite = false;
                return;
            }

            var favorites = _favoriteBLL.GetFavoritesByUser(AppSession.Instance.CurrentUser.UserID, out string message);
            if (favorites != null)
            {
                IsFavorite = favorites.Any(f => f.MovieID == CurrentMovie.MovieID);
            }
        }

        /// <summary>
        /// Load review của user hiện tại
        /// </summary>
        private void LoadUserReview()
        {
            if (AppSession.Instance.CurrentUser == null) return;

            var reviews = _reviewBLL.GetReviewsByMovie(CurrentMovie.MovieID, out string message);
            if (reviews != null)
            {
                var userReview = reviews.FirstOrDefault(r => r.UserID == AppSession.Instance.CurrentUser.UserID);
                if (userReview != null)
                {
                    UserRating = userReview.Rating ?? 0;
                    UserComment = userReview.Comment;
                }
            }
        }

        /// <summary>
        /// Phát phim
        /// </summary>
        private void PlayMovie(object parameter)
        {
            // Kiểm tra đăng nhập
            if (AppSession.Instance.CurrentUser == null)
            {
                MessageBox.Show("Vui lòng đăng nhập để xem phim!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Nếu là phim bộ, yêu cầu chọn tập
            if (IsSeriesMovie)
            {
                MessageBox.Show("Vui lòng chọn tập phim để xem!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Lưu lịch sử xem
            SaveViewHistory(null);

            // Mở video player
            var playerWindow = new Views.VideoPlayerView(CurrentMovie);
            playerWindow.Show();
        }

        /// <summary>
        /// Phát tập phim
        /// </summary>
        private void PlayEpisode(object parameter)
        {
            if (parameter is EpisodeDTO episode)
            {
                // Kiểm tra đăng nhập
                if (AppSession.Instance.CurrentUser == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập để xem phim!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Lưu lịch sử xem
                SaveViewHistory(episode.EpisodeID);

                // Mở video player với episode
                var playerWindow = new Views.VideoPlayerView(CurrentMovie, episode);
                playerWindow.Show();
            }
        }

        /// <summary>
        /// Lưu lịch sử xem
        /// </summary>
        private void SaveViewHistory(int? episodeID)
        {
            if (AppSession.Instance.CurrentUser == null) return;

            try
            {
                var viewHistory = new ViewHistoryDTO
                {
                    UserID = AppSession.Instance.CurrentUser.UserID,
                    MovieID = CurrentMovie.MovieID,
                    EpisodeID = episodeID,
                    WatchedAt = DateTime.Now
                };

                _viewHistoryBLL.AddViewHistory(viewHistory, out string message);

                // Cập nhật lượt xem phim
                CurrentMovie.ViewCount++;
                _movieBLL.UpdateMovie(CurrentMovie, null, out string msg);
            }
            catch (Exception ex)
            {
                // Log error but don't show to user
                System.Diagnostics.Debug.WriteLine($"Error saving view history: {ex.Message}");
            }
        }

        /// <summary>
        /// Gửi đánh giá
        /// </summary>
        private void SubmitReview(object parameter)
        {
            // Kiểm tra đăng nhập
            if (AppSession.Instance.CurrentUser == null)
            {
                MessageBox.Show("Vui lòng đăng nhập để đánh giá phim!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra rating
            if (UserRating < 1 || UserRating > 5)
            {
                MessageBox.Show("Vui lòng chọn rating từ 1 đến 5 sao!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Kiểm tra đã review chưa
                var existingReviews = _reviewBLL.GetReviewsByMovie(CurrentMovie.MovieID, out string msg);
                var existingReview = existingReviews?.FirstOrDefault(r => r.UserID == AppSession.Instance.CurrentUser.UserID);

                if (existingReview != null)
                {
                    // Update review
                    existingReview.Rating = UserRating;
                    existingReview.Comment = UserComment;
                    existingReview.CreatedAt = DateTime.Now;

                    bool success = _reviewBLL.UpdateReview(existingReview, out string message);
                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);
                }
                else
                {
                    // Add new review
                    var review = new ReviewDTO
                    {
                        UserID = AppSession.Instance.CurrentUser.UserID,
                        MovieID = CurrentMovie.MovieID,
                        Rating = UserRating,
                        Comment = UserComment,
                        CreatedAt = DateTime.Now
                    };

                    bool success = _reviewBLL.AddReview(review, out string message);
                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);
                }

                // Reload reviews
                LoadReviews();

                // Cập nhật rating trung bình của phim
                UpdateMovieRating();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi đánh giá: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cập nhật rating trung bình của phim
        /// </summary>
        private void UpdateMovieRating()
        {
            var reviews = _reviewBLL.GetReviewsByMovie(CurrentMovie.MovieID, out string message);
            if (reviews != null && reviews.Count > 0)
            {
                double avg = reviews.Average(r => (double)(r.Rating ?? 0));
                CurrentMovie.Rating = (decimal)avg;
                CurrentMovie.TotalReviews = reviews.Count;
                _movieBLL.UpdateMovie(CurrentMovie, null, out string msg);
            }
        }

        /// <summary>
        /// Thêm/xóa yêu thích
        /// </summary>
        private void ToggleFavorite(object parameter)
        {
            // Kiểm tra đăng nhập
            if (AppSession.Instance.CurrentUser == null)
            {
                MessageBox.Show("Vui lòng đăng nhập để sử dụng chức năng này!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsFavorite)
                {
                    // Xóa khỏi yêu thích
                    var favorites = _favoriteBLL.GetFavoritesByUser(AppSession.Instance.CurrentUser.UserID, out string msg);
                    var favorite = favorites?.FirstOrDefault(f => f.MovieID == CurrentMovie.MovieID);

                    if (favorite != null)
                    {
                        bool success = _favoriteBLL.DeleteFavorite(favorite.UserID, favorite.MovieID, out string message);
                        if (success)
                        {
                            IsFavorite = false;
                            MessageBox.Show("Đã xóa khỏi danh sách yêu thích!",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else
                {
                    // Thêm vào yêu thích
                    var favorite = new FavoriteDTO
                    {
                        UserID = AppSession.Instance.CurrentUser.UserID,
                        MovieID = CurrentMovie.MovieID,
                        AddedAt = DateTime.Now
                    };

                    // Sửa từ: _favoriteBLL.AddFavorite(favorite, out string message);
                    bool success = _favoriteBLL.AddFavorite(favorite.UserID, favorite.MovieID, out string message);
                    if (success)
                    {
                        IsFavorite = true;
                        MessageBox.Show("Đã thêm vào danh sách yêu thích!",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật yêu thích: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xem trailer
        /// </summary>
        private void ViewTrailer(object parameter)
        {
            if (!string.IsNullOrEmpty(CurrentMovie.TrailerURL))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = CurrentMovie.TrailerURL,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở trailer: {ex.Message}",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}