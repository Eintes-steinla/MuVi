using MuVi.BLL;
using MuVi.Commands;
using MuVi.Helpers;
using MuVi.DTO.DTOs;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MuVi.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Fields
        private string _searchText;
        private string _pageTitle;
        private string _username;
        private string _userRole;
        private string _userInitial;
        private ObservableCollection<MovieDTO> _trendingMovies;
        private ObservableCollection<MovieDTO> _newReleases;
        private ObservableCollection<MovieDTO> _topRatedMovies;
        private ObservableCollection<GenreDTO> _genres;
        private bool _isLoading;
        private string _currentSection;
        #endregion

        #region Properties

        /// <summary>
        /// Văn bản tìm kiếm
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    SearchCommand?.Execute(null);
                }
            }
        }

        /// <summary>
        /// Tiêu đề trang hiện tại
        /// </summary>
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        /// <summary>
        /// Vai trò người dùng
        /// </summary>
        public string UserRole
        {
            get => _userRole;
            set => SetProperty(ref _userRole, value);
        }

        /// <summary>
        /// Chữ cái đầu tên người dùng (dùng cho avatar)
        /// </summary>
        public string UserInitial
        {
            get => _userInitial;
            set => SetProperty(ref _userInitial, value);
        }

        /// <summary>
        /// Danh sách phim xu hướng
        /// </summary>
        public ObservableCollection<MovieDTO> TrendingMovies
        {
            get => _trendingMovies;
            set => SetProperty(ref _trendingMovies, value);
        }

        /// <summary>
        /// Danh sách phim mới phát hành
        /// </summary>
        public ObservableCollection<MovieDTO> NewReleases
        {
            get => _newReleases;
            set => SetProperty(ref _newReleases, value);
        }

        /// <summary>
        /// Danh sách phim đánh giá cao
        /// </summary>
        public ObservableCollection<MovieDTO> TopRatedMovies
        {
            get => _topRatedMovies;
            set => SetProperty(ref _topRatedMovies, value);
        }

        /// <summary>
        /// Danh sách thể loại
        /// </summary>
        public ObservableCollection<GenreDTO> Genres
        {
            get => _genres;
            set => SetProperty(ref _genres, value);
        }

        /// <summary>
        /// Trạng thái đang tải dữ liệu
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        /// Section hiện tại đang hiển thị
        /// </summary>
        public string CurrentSection
        {
            get => _currentSection;
            set => SetProperty(ref _currentSection, value);
        }

        #endregion

        #region Commands

        public ICommand LoadDataCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand NavigateToMovieCommand { get; }
        public ICommand AddToFavoritesCommand { get; }
        public ICommand FilterByGenreCommand { get; }

        #endregion

        #region Constructor

        public DashboardViewModel()
        {
            // Khởi tạo collections
            TrendingMovies = new ObservableCollection<MovieDTO>();
            NewReleases = new ObservableCollection<MovieDTO>();
            TopRatedMovies = new ObservableCollection<MovieDTO>();
            Genres = new ObservableCollection<GenreDTO>();

            // Khởi tạo commands
            LoadDataCommand = new RelayCommand(_ => LoadData());
            SearchCommand = new RelayCommand(_ => ExecuteSearch(), _ => !string.IsNullOrWhiteSpace(SearchText));
            NavigateToMovieCommand = new RelayCommand(param => NavigateToMovie((int)param));
            AddToFavoritesCommand = new RelayCommand(param => AddToFavorites((int)param));
            FilterByGenreCommand = new RelayCommand(param => FilterByGenre((int)param));

            // Thiết lập thông tin người dùng
            InitializeUserInfo();

            // Load dữ liệu ban đầu
            PageTitle = "Trang chủ";
            CurrentSection = "Home";
            LoadData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Khởi tạo thông tin người dùng từ session
        /// </summary>
        private void InitializeUserInfo()
        {
            var currentUser = AppSession.Instance.CurrentUser;

            if (currentUser != null)
            {
                Username = currentUser.Username;
                UserRole = currentUser.Role == "Admin" ? "Quản trị viên" : "Người dùng";
                UserInitial = string.IsNullOrEmpty(currentUser.Username)
                    ? "U"
                    : currentUser.Username.Substring(0, 1).ToUpper();
            }
            else
            {
                Username = "Guest";
                UserRole = "Khách";
                UserInitial = "G";
            }
        }

        /// <summary>
        /// Load dữ liệu phim từ database
        /// </summary>
        private async void LoadData()
        {
            try
            {
                IsLoading = true;

                await Task.Run(() =>
                {
                    var movieBLL = new MovieBLL();
                    var genreBLL = new GenreBLL();

                    // Load phim xu hướng (sắp xếp theo lượt xem)
                    //var trending = movieBLL.GetTopMoviesByViewCount(10);

                    // Load phim mới phát hành (sắp xếp theo năm phát hành)
                    //var newMovies = movieBLL.GetNewReleases(10);

                    // Load thể loại
                    var genres = genreBLL.GetAllGenres();

                    // Cập nhật UI trên UI thread
                    //App.Current.Dispatcher.Invoke(() =>
                    //{
                    //    TrendingMovies.Clear();
                    //    foreach (var movie in trending)
                    //    {
                    //        TrendingMovies.Add(movie);
                    //    }

                    //    NewReleases.Clear();
                    //    foreach (var movie in newMovies)
                    //    {
                    //        NewReleases.Add(movie);
                    //    }

                    //    Genres.Clear();
                    //    foreach (var genre in genres)
                    //    {
                    //        Genres.Add(genre);
                    //    }
                    //});
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi khi tải dữ liệu: {ex.Message}",
                    "Lỗi",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Thực hiện tìm kiếm phim
        /// </summary>
        private async void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return;

            try
            {
                IsLoading = true;
                PageTitle = $"Kết quả tìm kiếm: {SearchText}";

                await Task.Run(() =>
                {
                    var movieBLL = new MovieBLL();
                    //var searchResults = movieBLL.SearchMovies(SearchText);

                    //App.Current.Dispatcher.Invoke(() =>
                    //{
                    //    TrendingMovies.Clear();
                    //    foreach (var movie in searchResults.Take(10))
                    //    {
                    //        TrendingMovies.Add(movie);
                    //    }

                    //    NewReleases.Clear();
                    //});
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi khi tìm kiếm: {ex.Message}",
                    "Lỗi",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Điều hướng đến chi tiết phim
        /// </summary>
        private void NavigateToMovie(int movieId)
        {
            // TODO: Implement navigation to movie detail page
            System.Windows.MessageBox.Show(
                $"Điều hướng đến phim ID: {movieId}",
                "Thông báo",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Thêm phim vào danh sách yêu thích
        /// </summary>
        private void AddToFavorites(int movieId)
        {
            try
            {
                var currentUser = AppSession.Instance.CurrentUser;

                if (currentUser == null)
                {
                    System.Windows.MessageBox.Show(
                        "Vui lòng đăng nhập để thêm phim yêu thích!",
                        "Thông báo",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Warning
                    );
                    return;
                }

                var favoriteBLL = new FavoriteBLL();
                var result = favoriteBLL.AddToFavorites(currentUser.UserID, movieId);

                if (result)
                {
                    System.Windows.MessageBox.Show(
                        "Đã thêm vào danh sách yêu thích!",
                        "Thành công",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information
                    );
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        "Phim đã có trong danh sách yêu thích!",
                        "Thông báo",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi khi thêm yêu thích: {ex.Message}",
                    "Lỗi",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Lọc phim theo thể loại
        /// </summary>
        private async void FilterByGenre(int genreId)
        {
            try
            {
                IsLoading = true;
                var genre = Genres.FirstOrDefault(g => g.GenreID == genreId);
                PageTitle = genre != null ? $"Thể loại: {genre.GenreName}" : "Thể loại";

                await Task.Run(() =>
                {
                    var movieBLL = new MovieBLL();
                    //var filteredMovies = movieBLL.GetMoviesByGenre(genreId);

                    //App.Current.Dispatcher.Invoke(() =>
                    //{
                    //    TrendingMovies.Clear();
                    //    foreach (var movie in filteredMovies.Take(10))
                    //    {
                    //        TrendingMovies.Add(movie);
                    //    }

                    //    NewReleases.Clear();
                    //    foreach (var movie in filteredMovies.Skip(10).Take(10))
                    //    {
                    //        NewReleases.Add(movie);
                    //    }
                    //});
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi khi lọc phim: {ex.Message}",
                    "Lỗi",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load phim theo section cụ thể
        /// </summary>
        public async void LoadSection(string section)
        {
            CurrentSection = section;

            switch (section)
            {
                case "Home":
                    PageTitle = "Trang chủ";
                    LoadData();
                    break;

                case "Movies":
                    PageTitle = "Phim lẻ";
                    await LoadMoviesByType("Phim lẻ");
                    break;

                case "Series":
                    PageTitle = "Phim bộ";
                    await LoadMoviesByType("Phim bộ");
                    break;

                case "Favorites":
                    PageTitle = "Yêu thích";
                    await LoadFavorites();
                    break;

                case "History":
                    PageTitle = "Lịch sử xem";
                    await LoadHistory();
                    break;
            }
        }

        /// <summary>
        /// Load phim theo loại (phim lẻ/phim bộ)
        /// </summary>
        private async Task LoadMoviesByType(string movieType)
        {
            try
            {
                IsLoading = true;

                await Task.Run(() =>
                {
                    var movieBLL = new MovieBLL();
                    //var movies = movieBLL.GetMoviesByType(movieType);

                    //App.Current.Dispatcher.Invoke(() =>
                    //{
                    //    TrendingMovies.Clear();
                    //    foreach (var movie in movies.Take(10))
                    //    {
                    //        TrendingMovies.Add(movie);
                    //    }

                    //    NewReleases.Clear();
                    //    foreach (var movie in movies.Skip(10).Take(10))
                    //    {
                    //        NewReleases.Add(movie);
                    //    }
                    //});
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi khi tải phim: {ex.Message}",
                    "Lỗi",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Load danh sách phim yêu thích
        /// </summary>
        private async Task LoadFavorites()
        {
            try
            {
                IsLoading = true;
                var currentUser = AppSession.Instance.CurrentUser;

                if (currentUser == null)
                {
                    System.Windows.MessageBox.Show(
                        "Vui lòng đăng nhập để xem danh sách yêu thích!",
                        "Thông báo",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Warning
                    );
                    return;
                }

                await Task.Run(() =>
                {
                    var favoriteBLL = new FavoriteBLL();
                    var favorites = favoriteBLL.GetFavoritesByUser(currentUser.UserID);

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        TrendingMovies.Clear();
                        foreach (var movie in favorites)
                        {
                            TrendingMovies.Add(movie);
                        }

                        NewReleases.Clear();
                    });
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi khi tải danh sách yêu thích: {ex.Message}",
                    "Lỗi",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Load lịch sử xem
        /// </summary>
        private async Task LoadHistory()
        {
            try
            {
                IsLoading = true;
                var currentUser = AppSession.Instance.CurrentUser;

                if (currentUser == null)
                {
                    System.Windows.MessageBox.Show(
                        "Vui lòng đăng nhập để xem lịch sử!",
                        "Thông báo",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Warning
                    );
                    return;
                }

                await Task.Run(() =>
                {
                    var historyBLL = new ViewHistoryBLL();
                    var history = historyBLL.GetHistoryByUser(currentUser.UserID);

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        TrendingMovies.Clear();
                        foreach (var movie in history)
                        {
                            TrendingMovies.Add(movie);
                        }

                        NewReleases.Clear();
                    });
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi khi tải lịch sử: {ex.Message}",
                    "Lỗi",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}