using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace MuVi.ViewModels
{
    public class GenreViewModel : BaseViewModel
    {
        private readonly GenreBLL _genreBLL = new GenreBLL();

        public ObservableCollection<GenreDTO> GenreList { get; set; }

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
                        foreach (var genre in GenreList)
                        {
                            genre.IsSelected = value.Value;
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
                _genreBLL.SetSearchKeyword(value);
                LoadGenres();
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

        public GenreViewModel()
        {
            GenreList = new ObservableCollection<GenreDTO>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadGenres());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedGenres());

            _genreBLL.ClearFilters();
            LoadGenres();
        }

        public void LoadGenres()
        {
            var genres = _genreBLL.GetGenres();

            GenreList.Clear();
            foreach (var g in genres)
            {
                g.PropertyChanged += Genre_PropertyChanged;
                GenreList.Add(g);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void Genre_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GenreDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (GenreList == null || !GenreList.Any())
            {
                _isAllSelected = false;
            }
            else if (GenreList.All(g => g.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (GenreList.All(g => !g.IsSelected))
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
            int currentPage = _genreBLL.GetCurrentPage();
            int totalPages = _genreBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _genreBLL.NextPage();
            LoadGenres();
        }

        public void PreviousPage()
        {
            _genreBLL.PreviousPage();
            LoadGenres();
        }

        public void FirstPage()
        {
            _genreBLL.FirstPage();
            LoadGenres();
        }

        public void LastPage()
        {
            _genreBLL.LastPage();
            LoadGenres();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            _genreBLL.ClearFilters();
            LoadGenres();
        }

        private void DeleteSelectedGenres()
        {
            var selectedGenres = GenreList.Where(g => g.IsSelected).ToList();

            if (!selectedGenres.Any())
            {
                MessageBox.Show("Vui lòng chọn thể loại cần xóa", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedGenres.Count} thể loại đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var genreIds = selectedGenres.Select(g => g.GenreID).ToList();
                bool success = _genreBLL.DeleteMultipleGenres(genreIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadGenres();
                }
            }
        }

        public List<GenreDTO> GetSelectedGenres()
        {
            return GenreList.Where(g => g.IsSelected).ToList();
        }
    }
}