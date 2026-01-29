using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace MuVi.ViewModels
{
    public class MovieViewModel : BaseViewModel
    {
        private readonly MovieBLL _movieBLL = new MovieBLL();

        public ObservableCollection<MovieDTO> MovieList { get; set; }
        public ObservableCollection<CountryDTO> CountryList { get; set; }

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
                        foreach (var movie in MovieList)
                        {
                            movie.IsSelected = value.Value;
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
                _movieBLL.SetSearchKeyword(value);
                LoadMovies();
            }
        }

        // Status filter
        private string _selectedStatus = "Tất cả";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));
                _movieBLL.SetStatusFilter(value);
                LoadMovies();
            }
        }

        // Movie type filter
        private string _selectedMovieType = "Tất cả";
        public string SelectedMovieType
        {
            get => _selectedMovieType;
            set
            {
                _selectedMovieType = value;
                OnPropertyChanged(nameof(SelectedMovieType));
                _movieBLL.SetMovieTypeFilter(value);
                LoadMovies();
            }
        }

        // Year filter
        private string _selectedYear = "Tất cả";
        public string SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));

                if (value == "Tất cả")
                {
                    _movieBLL.SetYearFilter(null);
                }
                else if (int.TryParse(value, out int year))
                {
                    _movieBLL.SetYearFilter(year);
                }

                LoadMovies();
            }
        }

        // Country filter
        private CountryDTO? _selectedCountry;
        public CountryDTO? SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
                _movieBLL.SetCountryFilter(value?.CountryID);
                LoadMovies();
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

        public MovieViewModel()
        {
            MovieList = new ObservableCollection<MovieDTO>();
            CountryList = new ObservableCollection<CountryDTO>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadMovies());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedMovies());

            LoadCountries();
            _movieBLL.ClearFilters();
            LoadMovies();
        }

        public void LoadCountries()
        {
            var countries = _movieBLL.GetAllCountries();

            CountryList.Clear();
            CountryList.Add(new CountryDTO { CountryID = 0, CountryName = "Tất cả" });
            foreach (var c in countries)
            {
                CountryList.Add(c);
            }

            SelectedCountry = CountryList.FirstOrDefault();
        }

        public void LoadMovies()
        {
            var movies = _movieBLL.GetMovies();

            MovieList.Clear();
            foreach (var m in movies)
            {
                m.PropertyChanged += Movie_PropertyChanged;
                MovieList.Add(m);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void Movie_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MovieDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (MovieList == null || !MovieList.Any())
            {
                _isAllSelected = false;
            }
            else if (MovieList.All(m => m.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (MovieList.All(m => !m.IsSelected))
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
            int currentPage = _movieBLL.GetCurrentPage();
            int totalPages = _movieBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _movieBLL.NextPage();
            LoadMovies();
        }

        public void PreviousPage()
        {
            _movieBLL.PreviousPage();
            LoadMovies();
        }

        public void FirstPage()
        {
            _movieBLL.FirstPage();
            LoadMovies();
        }

        public void LastPage()
        {
            _movieBLL.LastPage();
            LoadMovies();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            SelectedStatus = "Tất cả";
            SelectedMovieType = "Tất cả";
            SelectedYear = "Tất cả";
            SelectedCountry = CountryList.FirstOrDefault();
            _movieBLL.ClearFilters();
            LoadMovies();
        }

        private void DeleteSelectedMovies()
        {
            var selectedMovies = MovieList.Where(m => m.IsSelected).ToList();

            if (!selectedMovies.Any())
            {
                MessageBox.Show("Vui lòng chọn phim cần xóa", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedMovies.Count} phim đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var movieIds = selectedMovies.Select(m => m.MovieID).ToList();
                bool success = _movieBLL.DeleteMultipleMovies(movieIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadMovies();
                }
            }
        }

        public List<MovieDTO> GetSelectedMovies()
        {
            return MovieList.Where(m => m.IsSelected).ToList();
        }
    }
}