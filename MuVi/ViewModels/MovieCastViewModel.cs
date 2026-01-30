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
    public class MovieCastViewModel : BaseViewModel
    {
        private readonly MovieCastBLL _castBLL = new MovieCastBLL();

        public ObservableCollection<MovieCastDTO> CastList { get; set; }
        public ObservableCollection<ActorDTO> ActorList { get; set; }
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
                        foreach (var cast in CastList)
                        {
                            cast.IsSelected = value.Value;
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
                _castBLL.SetSearchKeyword(value);
                LoadCasts();
            }
        }

        // Order filter (vai chính/phụ)
        private string _selectedOrder = "Tất cả";
        public string SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));

                if (value == "Tất cả")
                {
                    _castBLL.SetOrderFilter(null);
                }
                else if (value == "Vai chính")
                {
                    _castBLL.SetOrderFilter(1);
                }
                else if (value == "Vai phụ")
                {
                    _castBLL.SetOrderFilter(2);
                }

                LoadCasts();
            }
        }

        // Actor filter
        private ActorDTO? _selectedActor;
        public ActorDTO? SelectedActor
        {
            get => _selectedActor;
            set
            {
                _selectedActor = value;
                OnPropertyChanged(nameof(SelectedActor));
                _castBLL.SetActorFilter(value?.ActorID);
                LoadCasts();
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
                _castBLL.SetMovieFilter(value?.MovieID);
                LoadCasts();
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

        public MovieCastViewModel()
        {
            CastList = new ObservableCollection<MovieCastDTO>();
            ActorList = new ObservableCollection<ActorDTO>();
            MovieList = new ObservableCollection<MovieDTO>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadCasts());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedCasts());

            LoadActors();
            LoadMovies();
            _castBLL.ClearFilters();
            LoadCasts();
        }

        public void LoadActors()
        {
            var actors = _castBLL.GetAllActors();

            ActorList.Clear();
            ActorList.Add(new ActorDTO { ActorID = 0, ActorName = "Tất cả" });
            foreach (var a in actors)
            {
                ActorList.Add(a);
            }

            SelectedActor = ActorList.FirstOrDefault();
        }

        public void LoadMovies()
        {
            var movies = _castBLL.GetAllMovies();

            MovieList.Clear();
            MovieList.Add(new MovieDTO { MovieID = 0, Title = "Tất cả" });
            foreach (var m in movies)
            {
                MovieList.Add(m);
            }

            SelectedMovie = MovieList.FirstOrDefault();
        }

        public void LoadCasts()
        {
            var casts = _castBLL.GetMovieCasts();

            CastList.Clear();
            foreach (var c in casts)
            {
                c.PropertyChanged += Cast_PropertyChanged;
                CastList.Add(c);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void Cast_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MovieCastDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (CastList == null || !CastList.Any())
            {
                _isAllSelected = false;
            }
            else if (CastList.All(c => c.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (CastList.All(c => !c.IsSelected))
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
            int currentPage = _castBLL.GetCurrentPage();
            int totalPages = _castBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _castBLL.NextPage();
            LoadCasts();
        }

        public void PreviousPage()
        {
            _castBLL.PreviousPage();
            LoadCasts();
        }

        public void FirstPage()
        {
            _castBLL.FirstPage();
            LoadCasts();
        }

        public void LastPage()
        {
            _castBLL.LastPage();
            LoadCasts();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            SelectedOrder = "Tất cả";
            SelectedActor = ActorList.FirstOrDefault();
            SelectedMovie = MovieList.FirstOrDefault();
            _castBLL.ClearFilters();
            LoadCasts();
        }

        private void DeleteSelectedCasts()
        {
            var selectedCasts = CastList.Where(c => c.IsSelected).ToList();

            if (!selectedCasts.Any())
            {
                MessageBox.Show("Vui lòng chọn diễn viên cần xóa khỏi phim", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedCasts.Count} diễn viên khỏi phim?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var castIds = selectedCasts.Select(c => (c.MovieID, c.ActorID)).ToList();
                bool success = _castBLL.DeleteMultipleMovieCasts(castIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadCasts();
                }
            }
        }
    }
}