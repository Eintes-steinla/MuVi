using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace MuVi.ViewModels
{
    public class EpisodeViewModel : BaseViewModel
    {
        private readonly EpisodeBLL _episodeBLL = new EpisodeBLL();

        public ObservableCollection<EpisodeDTO> EpisodeList { get; set; }
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
                        foreach (var episode in EpisodeList)
                        {
                            episode.IsSelected = value.Value;
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
                _episodeBLL.SetSearchKeyword(value);
                LoadEpisodes();
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
                _episodeBLL.SetMovieFilter(value?.MovieID);
                LoadEpisodes();
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
                    _episodeBLL.SetYearFilter(null);
                }
                else if (int.TryParse(value, out int year))
                {
                    _episodeBLL.SetYearFilter(year);
                }

                LoadEpisodes();
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

        public EpisodeViewModel()
        {
            EpisodeList = new ObservableCollection<EpisodeDTO>();
            MovieList = new ObservableCollection<MovieDTO>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadEpisodes());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedEpisodes());

            LoadMovies();
            _episodeBLL.ClearFilters();
            LoadEpisodes();
        }

        public void LoadMovies()
        {
            var movies = _episodeBLL.GetSeriesMovies();

            MovieList.Clear();
            MovieList.Add(new MovieDTO { MovieID = 0, Title = "Tất cả" });
            foreach (var m in movies)
            {
                MovieList.Add(m);
            }

            SelectedMovie = MovieList.FirstOrDefault();
        }

        public void LoadEpisodes()
        {
            var episodes = _episodeBLL.GetEpisodes();

            EpisodeList.Clear();
            foreach (var e in episodes)
            {
                e.PropertyChanged += Episode_PropertyChanged;
                EpisodeList.Add(e);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void Episode_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EpisodeDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (EpisodeList == null || !EpisodeList.Any())
            {
                _isAllSelected = false;
            }
            else if (EpisodeList.All(e => e.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (EpisodeList.All(e => !e.IsSelected))
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
            int currentPage = _episodeBLL.GetCurrentPage();
            int totalPages = _episodeBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _episodeBLL.NextPage();
            LoadEpisodes();
        }

        public void PreviousPage()
        {
            _episodeBLL.PreviousPage();
            LoadEpisodes();
        }

        public void FirstPage()
        {
            _episodeBLL.FirstPage();
            LoadEpisodes();
        }

        public void LastPage()
        {
            _episodeBLL.LastPage();
            LoadEpisodes();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            SelectedYear = "Tất cả";
            SelectedMovie = MovieList.FirstOrDefault();
            _episodeBLL.ClearFilters();
            LoadEpisodes();
        }

        private void DeleteSelectedEpisodes()
        {
            var selectedEpisodes = EpisodeList.Where(e => e.IsSelected).ToList();

            if (!selectedEpisodes.Any())
            {
                MessageBox.Show("Vui lòng chọn tập phim cần xóa", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedEpisodes.Count} tập phim đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var episodeIds = selectedEpisodes.Select(e => e.EpisodeID).ToList();
                bool success = _episodeBLL.DeleteMultipleEpisodes(episodeIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadEpisodes();
                }
            }
        }

        public List<EpisodeDTO> GetSelectedEpisodes()
        {
            return EpisodeList.Where(e => e.IsSelected).ToList();
        }
    }
}