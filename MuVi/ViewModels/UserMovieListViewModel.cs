using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace MuVi.ViewModels
{
    /// <summary>
    /// ViewModel cho danh sách phim người dùng
    /// Hỗ trợ: Tìm kiếm, lọc theo thể loại, quốc gia, năm, phân trang
    /// </summary>
    public class UserMovieListViewModel : BaseViewModel
    {
        private readonly MovieBLL _movieBLL;
        private readonly GenreBLL _genreBLL;
        private readonly CountryBLL _countryBLL;

        #region Properties

        // Danh sách phim hiển thị
        private ObservableCollection<MovieDTO> _movies;
        public ObservableCollection<MovieDTO> Movies
        {
            get => _movies;
            set { _movies = value; OnPropertyChanged(nameof(Movies)); }
        }

        // Danh sách phim gốc (chưa filter)
        private ObservableCollection<MovieDTO> _allMovies;
        public ObservableCollection<MovieDTO> AllMovies
        {
            get => _allMovies;
            set { _allMovies = value; OnPropertyChanged(nameof(AllMovies)); }
        }

        // Danh sách thể loại để filter
        private ObservableCollection<GenreDTO> _genres;
        public ObservableCollection<GenreDTO> Genres
        {
            get => _genres;
            set { _genres = value; OnPropertyChanged(nameof(Genres)); }
        }

        // Danh sách quốc gia để filter
        private ObservableCollection<CountryDTO> _countries;
        public ObservableCollection<CountryDTO> Countries
        {
            get => _countries;
            set { _countries = value; OnPropertyChanged(nameof(Countries)); }
        }

        // Thể loại được chọn
        private GenreDTO _selectedGenre;
        public GenreDTO SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                _selectedGenre = value;
                OnPropertyChanged(nameof(SelectedGenre));
                ApplyFilters();
            }
        }

        // Quốc gia được chọn
        private CountryDTO _selectedCountry;
        public CountryDTO SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
                ApplyFilters();
            }
        }

        // Loại phim được chọn (Phim lẻ / Phim bộ)
        private string _selectedMovieType;
        public string SelectedMovieType
        {
            get => _selectedMovieType;
            set
            {
                _selectedMovieType = value;
                OnPropertyChanged(nameof(SelectedMovieType));
                ApplyFilters();
            }
        }

        // Năm phát hành được chọn
        private int? _selectedYear;
        public int? SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
                ApplyFilters();
            }
        }

        // Từ khóa tìm kiếm
        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
                ApplyFilters();
            }
        }

        // Sắp xếp theo
        private string _sortBy;
        public string SortBy
        {
            get => _sortBy;
            set
            {
                _sortBy = value;
                OnPropertyChanged(nameof(SortBy));
                ApplyFilters();
            }
        }

        // Pagination
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(PageInfo));
                LoadCurrentPageData();
            }
        }

        private int _itemsPerPage = 12;
        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                _itemsPerPage = value;
                OnPropertyChanged(nameof(ItemsPerPage));
                CurrentPage = 1;
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(PageInfo));
            }
        }

        public string PageInfo => $"Trang {CurrentPage} / {TotalPages}";

        // Loading
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
        public ICommand ClearFiltersCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand LastPageCommand { get; }

        #endregion

        #region Constructor

        public UserMovieListViewModel()
        {
            // Khởi tạo BLL
            _movieBLL = new MovieBLL();
            _genreBLL = new GenreBLL();
            _countryBLL = new CountryBLL();

            // Khởi tạo collections
            Movies = new ObservableCollection<MovieDTO>();
            AllMovies = new ObservableCollection<MovieDTO>();
            Genres = new ObservableCollection<GenreDTO>();
            Countries = new ObservableCollection<CountryDTO>();

            // Khởi tạo commands
            LoadDataCommand = new RelayCommand(LoadData);
            ViewMovieDetailCommand = new RelayCommand(ViewMovieDetail);
            PlayMovieCommand = new RelayCommand(PlayMovie);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            NextPageCommand = new RelayCommand(NextPage);
            FirstPageCommand = new RelayCommand(FirstPage);
            LastPageCommand = new RelayCommand(LastPage);

            // Thiết lập mặc định
            SortBy = "Mới nhất";

            // Load dữ liệu
            LoadData(null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load tất cả dữ liệu
        /// </summary>
        private void LoadData(object parameter)
        {
            IsLoading = true;

            try
            {
                // Load danh sách phim
                LoadMovies();

                // Load danh sách thể loại
                LoadGenres();

                // Load danh sách quốc gia
                LoadCountries();
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
        /// Load danh sách phim
        /// </summary>
        private void LoadMovies()
        {
            var movies = _movieBLL.GetAllMovies(out string message);
            if (movies != null)
            {
                AllMovies.Clear();
                foreach (var movie in movies)
                {
                    AllMovies.Add(movie);
                }

                ApplyFilters();
            }
        }

        /// <summary>
        /// Load danh sách thể loại
        /// </summary>
        private void LoadGenres()
        {
            var genres = _genreBLL.GetAllGenres(out string message);
            if (genres != null)
            {
                Genres.Clear();
                Genres.Add(new GenreDTO { GenreID = 0, GenreName = "Tất cả thể loại" });
                foreach (var genre in genres)
                {
                    Genres.Add(genre);
                }
            }
        }

        /// <summary>
        /// Load danh sách quốc gia
        /// </summary>
        private void LoadCountries()
        {
            var countries = _countryBLL.GetAllCountries(out string message);
            if (countries != null)
            {
                Countries.Clear();
                Countries.Add(new CountryDTO { CountryID = 0, CountryName = "Tất cả quốc gia" });
                foreach (var country in countries)
                {
                    Countries.Add(country);
                }
            }
        }

        /// <summary>
        /// Áp dụng các bộ lọc
        /// </summary>
        private void ApplyFilters()
        {
            if (AllMovies == null || AllMovies.Count == 0) return;

            var filtered = AllMovies.AsEnumerable();

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                filtered = filtered.Where(m =>
                    m.Title.ToLower().Contains(SearchKeyword.ToLower()) ||
                    (m.Director != null && m.Director.ToLower().Contains(SearchKeyword.ToLower())));
            }

            // Lọc theo thể loại
            if (SelectedGenre != null && SelectedGenre.GenreID > 0)
            {
                // Lọc phim có GenreID khớp
                // (Lưu ý: Cần join với bảng MovieGenres nếu có quan hệ nhiều-nhiều)
                filtered = filtered.Where(m => m.Genres != null && m.Genres.Any(g => g.GenreID == SelectedGenre.GenreID));
            }

            // Lọc theo quốc gia
            if (SelectedCountry != null && SelectedCountry.CountryID > 0)
            {
                filtered = filtered.Where(m => m.CountryID == SelectedCountry.CountryID);
            }

            // Lọc theo loại phim
            if (!string.IsNullOrEmpty(SelectedMovieType))
            {
                filtered = filtered.Where(m => m.MovieType == SelectedMovieType);
            }

            // Lọc theo năm
            if (SelectedYear.HasValue)
            {
                filtered = filtered.Where(m => m.ReleaseYear == SelectedYear.Value);
            }

            // Sắp xếp
            filtered = SortBy switch
            {
                "Mới nhất" => filtered.OrderByDescending(m => m.ReleaseYear).ThenByDescending(m => m.CreatedAt),
                "Cũ nhất" => filtered.OrderBy(m => m.ReleaseYear).ThenBy(m => m.CreatedAt),
                "Đánh giá cao" => filtered.OrderByDescending(m => m.AverageRating).ThenByDescending(m => m.TotalReviews),
                "Xem nhiều" => filtered.OrderByDescending(m => m.ViewCount),
                "Tên A-Z" => filtered.OrderBy(m => m.Title),
                "Tên Z-A" => filtered.OrderByDescending(m => m.Title),
                _ => filtered.OrderByDescending(m => m.ReleaseYear)
            };

            // Cập nhật Movies và phân trang
            var filteredList = filtered.ToList();
            TotalPages = (int)Math.Ceiling((double)filteredList.Count / ItemsPerPage);
            if (TotalPages == 0) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = 1;

            LoadCurrentPageData();
        }

        /// <summary>
        /// Load dữ liệu trang hiện tại
        /// </summary>
        private void LoadCurrentPageData()
        {
            if (AllMovies == null || AllMovies.Count == 0) return;

            var filtered = AllMovies.AsEnumerable();

            // Áp dụng lại các filter (code giống ApplyFilters)
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                filtered = filtered.Where(m =>
                    m.Title.ToLower().Contains(SearchKeyword.ToLower()) ||
                    (m.Director != null && m.Director.ToLower().Contains(SearchKeyword.ToLower())));
            }

            if (SelectedGenre != null && SelectedGenre.GenreID > 0)
            {
                filtered = filtered.Where(m => m.Genres != null && m.Genres.Any(g => g.GenreID == SelectedGenre.GenreID));
            }

            if (SelectedCountry != null && SelectedCountry.CountryID > 0)
            {
                filtered = filtered.Where(m => m.CountryID == SelectedCountry.CountryID);
            }

            if (!string.IsNullOrEmpty(SelectedMovieType))
            {
                filtered = filtered.Where(m => m.MovieType == SelectedMovieType);
            }

            if (SelectedYear.HasValue)
            {
                filtered = filtered.Where(m => m.ReleaseYear == SelectedYear.Value);
            }

            // Sắp xếp
            filtered = SortBy switch
            {
                "Mới nhất" => filtered.OrderByDescending(m => m.ReleaseYear).ThenByDescending(m => m.CreatedAt),
                "Cũ nhất" => filtered.OrderBy(m => m.ReleaseYear).ThenBy(m => m.CreatedAt),
                "Đánh giá cao" => filtered.OrderByDescending(m => m.AverageRating).ThenByDescending(m => m.TotalReviews),
                "Xem nhiều" => filtered.OrderByDescending(m => m.ViewCount),
                "Tên A-Z" => filtered.OrderBy(m => m.Title),
                "Tên Z-A" => filtered.OrderByDescending(m => m.Title),
                _ => filtered.OrderByDescending(m => m.ReleaseYear)
            };

            // Phân trang
            var pagedData = filtered
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            Movies.Clear();
            foreach (var movie in pagedData)
            {
                Movies.Add(movie);
            }
        }

        /// <summary>
        /// Xóa tất cả bộ lọc
        /// </summary>
        private void ClearFilters(object parameter)
        {
            SearchKeyword = string.Empty;
            SelectedGenre = null;
            SelectedCountry = null;
            SelectedMovieType = null;
            SelectedYear = null;
            SortBy = "Mới nhất";
            CurrentPage = 1;
            ApplyFilters();
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
        /// Trang trước
        /// </summary>
        public void PreviousPage(object parameter)
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        /// <summary>
        /// Trang sau
        /// </summary>
        public void NextPage(object parameter)
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }

        /// <summary>
        /// Trang đầu
        /// </summary>
        public void FirstPage(object parameter)
        {
            CurrentPage = 1;
        }

        /// <summary>
        /// Trang cuối
        /// </summary>
        public void LastPage(object parameter)
        {
            CurrentPage = TotalPages;
        }

        #endregion
    }
}