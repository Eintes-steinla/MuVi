using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace MuVi.ViewModels
{
    public class FavoriteViewModel : BaseViewModel
    {
        private readonly FavoriteBLL _favoriteBLL = new FavoriteBLL();

        public ObservableCollection<FavoriteDTO> FavoriteList { get; set; }

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
                        foreach (var favorite in FavoriteList)
                        {
                            favorite.IsSelected = value.Value;
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
                _favoriteBLL.SetSearchKeyword(value);
                LoadFavorites();
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
                _favoriteBLL.SetMovieTypeFilter(value);
                LoadFavorites();
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
                    _favoriteBLL.SetYearFilter(null);
                }
                else if (int.TryParse(value, out int year))
                {
                    _favoriteBLL.SetYearFilter(year);
                }

                LoadFavorites();
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

        public FavoriteViewModel()
        {
            FavoriteList = new ObservableCollection<FavoriteDTO>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadFavorites());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedFavorites());

            _favoriteBLL.ClearFilters();
            LoadFavorites();
        }

        public void LoadFavorites()
        {
            var favorites = _favoriteBLL.GetFavorites();

            FavoriteList.Clear();
            foreach (var f in favorites)
            {
                f.PropertyChanged += Favorite_PropertyChanged;
                FavoriteList.Add(f);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void Favorite_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FavoriteDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (FavoriteList == null || !FavoriteList.Any())
            {
                _isAllSelected = false;
            }
            else if (FavoriteList.All(f => f.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (FavoriteList.All(f => !f.IsSelected))
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
            int currentPage = _favoriteBLL.GetCurrentPage();
            int totalPages = _favoriteBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _favoriteBLL.NextPage();
            LoadFavorites();
        }

        public void PreviousPage()
        {
            _favoriteBLL.PreviousPage();
            LoadFavorites();
        }

        public void FirstPage()
        {
            _favoriteBLL.FirstPage();
            LoadFavorites();
        }

        public void LastPage()
        {
            _favoriteBLL.LastPage();
            LoadFavorites();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            SelectedMovieType = "Tất cả";
            SelectedYear = "Tất cả";
            _favoriteBLL.ClearFilters();
            LoadFavorites();
        }

        private void DeleteSelectedFavorites()
        {
            var selectedFavorites = FavoriteList.Where(f => f.IsSelected).ToList();

            if (!selectedFavorites.Any())
            {
                MessageBox.Show("Vui lòng chọn mục yêu thích cần xóa", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedFavorites.Count} mục yêu thích đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var favoriteIds = selectedFavorites.Select(f => (f.UserID, f.MovieID)).ToList();
                bool success = _favoriteBLL.DeleteMultipleFavorites(favoriteIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadFavorites();
                }
            }
        }

        public List<FavoriteDTO> GetSelectedFavorites()
        {
            return FavoriteList.Where(f => f.IsSelected).ToList();
        }
    }
}