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
    /// ViewModel cho danh sách phim yêu thích của người dùng
    /// </summary>
    public class UserFavoritesViewModel : BaseViewModel
    {
        private readonly FavoriteBLL _favoriteBLL;
        private readonly MovieBLL _movieBLL;

        #region Properties

        // Danh sách phim yêu thích
        private ObservableCollection<MovieDTO> _favoriteMovies;
        public ObservableCollection<MovieDTO> FavoriteMovies
        {
            get => _favoriteMovies;
            set { _favoriteMovies = value; OnPropertyChanged(nameof(FavoriteMovies)); }
        }

        // Loading
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        // Tổng số phim yêu thích
        public int TotalFavorites => FavoriteMovies?.Count ?? 0;

        #endregion

        #region Commands

        public ICommand LoadDataCommand { get; }
        public ICommand ViewMovieDetailCommand { get; }
        public ICommand PlayMovieCommand { get; }
        public ICommand RemoveFavoriteCommand { get; }

        #endregion

        #region Constructor

        public UserFavoritesViewModel()
        {
            // Khởi tạo BLL
            _favoriteBLL = new FavoriteBLL();
            _movieBLL = new MovieBLL();

            // Khởi tạo collections
            FavoriteMovies = new ObservableCollection<MovieDTO>();

            // Khởi tạo commands
            LoadDataCommand = new RelayCommand(LoadData);
            ViewMovieDetailCommand = new RelayCommand(ViewMovieDetail);
            PlayMovieCommand = new RelayCommand(PlayMovie);
            RemoveFavoriteCommand = new RelayCommand(RemoveFavorite);

            // Load data
            LoadData(null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load danh sách phim yêu thích
        /// </summary>
        private void LoadData(object parameter)
        {
            if (AppSession.Instance.CurrentUser == null)
            {
                System.Windows.MessageBox.Show("Vui lòng đăng nhập để xem danh sách yêu thích!",
                    "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            IsLoading = true;

            try
            {
                // Lấy danh sách favorite của user
                var favorites = _favoriteBLL.GetFavoritesByUser(AppSession.Instance.CurrentUser.UserID, out string message);

                if (favorites != null && favorites.Count > 0)
                {
                    FavoriteMovies.Clear();

                    // Lấy thông tin chi tiết của từng phim
                    foreach (var favorite in favorites.OrderByDescending(f => f.AddedAt))
                    {
                        var movie = _movieBLL.GetMovieById(favorite.MovieID);
                        if (movie != null)
                        {
                            FavoriteMovies.Add(movie);
                        }
                    }
                }
                else
                {
                    FavoriteMovies.Clear();
                }

                OnPropertyChanged(nameof(TotalFavorites));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải danh sách yêu thích: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Xem chi tiết phim
        /// </summary>
        private void ViewMovieDetail(object parameter)
        {
            if (parameter is MovieDTO movie)
            {
                var detailWindow = new Views.UserMovieDetailView(movie);
                detailWindow.Show();
            }
        }

        /// <summary>
        /// Phát phim
        /// </summary>
        private void PlayMovie(object parameter)
        {
            if (parameter is MovieDTO movie)
            {
                var playerWindow = new Views.VideoPlayerView(movie);
                playerWindow.Show();
            }
        }

        /// <summary>
        /// Xóa khỏi danh sách yêu thích
        /// </summary>
        private void RemoveFavorite(object parameter)
        {
            if (parameter is MovieDTO movie)
            {
                var result = System.Windows.MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa '{movie.Title}' khỏi danh sách yêu thích?",
                    "Xác nhận",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {
                        // Tìm favorite record
                        var favorites = _favoriteBLL.GetFavoritesByUser(AppSession.Instance.CurrentUser.UserID, out string msg);
                        var favorite = favorites?.FirstOrDefault(f => f.MovieID == movie.MovieID);

                        if (favorite != null)
                        {
                            bool success = _favoriteBLL.DeleteFavorite(favorite.UserID, favorite.MovieID, out string message);

                            if (success)
                            {
                                FavoriteMovies.Remove(movie);
                                OnPropertyChanged(nameof(TotalFavorites));

                                System.Windows.MessageBox.Show("Đã xóa khỏi danh sách yêu thích!",
                                    "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                            }
                            else
                            {
                                System.Windows.MessageBox.Show(message,
                                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Lỗi khi xóa yêu thích: {ex.Message}",
                            "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }

        #endregion
    }
}