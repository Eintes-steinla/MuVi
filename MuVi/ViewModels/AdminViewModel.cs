using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MuVi.Commands;
// using MuVi.BLL;
// using MuVi.DTO;

namespace MuVi.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        #region Fields
        private ObservableCollection<DataItemViewModel> _currentItems;
        private ObservableCollection<DataItemViewModel> _allItems;
        private string _currentTab = "Movies";
        private int _totalItems;
        private int _currentPage = 1;
        private int _pageSize = 25;
        private int _totalPages;
        private string _searchQuery;
        private string _statusFilter = "Tất cả";
        private string _typeFilter = "Tất cả";
        private string _yearFilter = "Tất cả";
        #endregion

        #region Properties

        public ObservableCollection<DataItemViewModel> CurrentItems
        {
            get => _currentItems;
            set => SetProperty(ref _currentItems, value);
        }

        public ObservableCollection<DataItemViewModel> AllItems
        {
            get => _allItems;
            set => SetProperty(ref _allItems, value);
        }

        public string CurrentTab
        {
            get => _currentTab;
            set
            {
                if (SetProperty(ref _currentTab, value))
                {
                    LoadData();
                }
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            set => SetProperty(ref _totalItems, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    LoadCurrentPage();
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    CurrentPage = 1;
                    CalculateTotalPages();
                    LoadCurrentPage();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string StatusFilter
        {
            get => _statusFilter;
            set
            {
                if (SetProperty(ref _statusFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string TypeFilter
        {
            get => _typeFilter;
            set
            {
                if (SetProperty(ref _typeFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string YearFilter
        {
            get => _yearFilter;
            set
            {
                if (SetProperty(ref _yearFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DeleteSelectedCommand { get; }

        #endregion

        #region Constructor

        public AdminViewModel()
        {
            InitializeCommands();
            LoadData();
        }

        #endregion

        #region Initialization

        private void InitializeCommands()
        {
            //RefreshCommand = new RelayCommand(_ => LoadData());
            //AddCommand = new RelayCommand(_ => ExecuteAdd());
            //EditCommand = new RelayCommand(item => ExecuteEdit((DataItemViewModel)item));
            //DeleteCommand = new RelayCommand(item => ExecuteDelete((DataItemViewModel)item));
            //DeleteSelectedCommand = new RelayCommand(_ => ExecuteDeleteSelected());
        }

        #endregion

        #region Data Loading

        public void LoadData()
        {
            // TODO: Load data from BLL based on CurrentTab
            // Example:
            // switch (CurrentTab)
            // {
            //     case "Movies":
            //         var movieBLL = new MovieBLL();
            //         var movies = movieBLL.GetAllMovies();
            //         AllItems = new ObservableCollection<DataItemViewModel>(
            //             movies.Select(m => new DataItemViewModel
            //             {
            //                 ID = m.MovieID,
            //                 Title = m.Title,
            //                 Type = m.MovieType,
            //                 Year = m.ReleaseYear,
            //                 Rating = m.Rating,
            //                 Status = m.Status
            //             })
            //         );
            //         break;
            //     // ... other cases
            // }

            // Mock data for demonstration
            LoadMockData();
            ApplyFilters();
        }

        private void LoadMockData()
        {
            var mockData = new ObservableCollection<DataItemViewModel>();

            for (int i = 1; i <= 100; i++)
            {
                mockData.Add(new DataItemViewModel
                {
                    ID = i,
                    Title = $"Phim số {i}",
                    Type = i % 2 == 0 ? "Phim lẻ" : "Phim bộ",
                    Year = 2020 + (i % 5),
                    Rating = 7.0 + (i % 3),
                    Status = i % 3 == 0 ? "Đang chiếu" : i % 3 == 1 ? "Hoàn thành" : "Sắp chiếu"
                });
            }

            AllItems = mockData;
        }

        private void ApplyFilters()
        {
            if (AllItems == null) return;

            var filtered = AllItems.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filtered = filtered.Where(x =>
                    x.Title.ToLower().Contains(SearchQuery.ToLower()) ||
                    x.ID.ToString().Contains(SearchQuery)
                );
            }

            // Apply status filter
            if (StatusFilter != "Tất cả")
            {
                filtered = filtered.Where(x => x.Status == StatusFilter);
            }

            // Apply type filter
            if (TypeFilter != "Tất cả")
            {
                filtered = filtered.Where(x => x.Type == TypeFilter);
            }

            // Apply year filter
            if (YearFilter != "Tất cả" && int.TryParse(YearFilter, out int year))
            {
                filtered = filtered.Where(x => x.Year == year);
            }

            var filteredList = filtered.ToList();
            TotalItems = filteredList.Count;
            CalculateTotalPages();

            // Apply pagination
            CurrentPage = 1;
            LoadCurrentPage();
        }

        private void CalculateTotalPages()
        {
            if (AllItems == null || !AllItems.Any())
            {
                TotalPages = 0;
                return;
            }

            var filtered = ApplyFilterLogic();
            TotalPages = (int)Math.Ceiling((double)filtered.Count() / PageSize);
        }

        private IEnumerable<DataItemViewModel> ApplyFilterLogic()
        {
            var filtered = AllItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filtered = filtered.Where(x =>
                    x.Title.ToLower().Contains(SearchQuery.ToLower()) ||
                    x.ID.ToString().Contains(SearchQuery)
                );
            }

            if (StatusFilter != "Tất cả")
                filtered = filtered.Where(x => x.Status == StatusFilter);

            if (TypeFilter != "Tất cả")
                filtered = filtered.Where(x => x.Type == TypeFilter);

            if (YearFilter != "Tất cả" && int.TryParse(YearFilter, out int year))
                filtered = filtered.Where(x => x.Year == year);

            return filtered;
        }

        private void LoadCurrentPage()
        {
            if (AllItems == null) return;

            var filtered = ApplyFilterLogic();

            var pagedItems = filtered
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            CurrentItems = new ObservableCollection<DataItemViewModel>(pagedItems);
        }

        #endregion

        #region Pagination Methods

        public void GoToFirstPage()
        {
            CurrentPage = 1;
        }

        public void GoToPreviousPage()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        public void GoToNextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }

        public void GoToLastPage()
        {
            CurrentPage = TotalPages;
        }

        #endregion

        #region Command Methods

        private void ExecuteAdd()
        {
            // TODO: Open Add dialog
            // var addDialog = new AddMovieDialog();
            // if (addDialog.ShowDialog() == true)
            // {
            //     LoadData();
            // }
        }

        private void ExecuteEdit(DataItemViewModel item)
        {
            if (item == null) return;

            // TODO: Open Edit dialog
            // var editDialog = new EditMovieDialog(item.ID);
            // if (editDialog.ShowDialog() == true)
            // {
            //     LoadData();
            // }
        }

        private void ExecuteDelete(DataItemViewModel item)
        {
            if (item == null) return;

            // TODO: Confirm and delete
            // var result = MessageBox.Show(
            //     $"Bạn có chắc muốn xóa '{item.Title}'?",
            //     "Xác nhận xóa",
            //     MessageBoxButton.YesNo,
            //     MessageBoxImage.Question
            // );
            // 
            // if (result == MessageBoxResult.Yes)
            // {
            //     var movieBLL = new MovieBLL();
            //     if (movieBLL.Delete(item.ID))
            //     {
            //         LoadData();
            //         MessageBox.Show("Xóa thành công!");
            //     }
            // }
        }

        private void ExecuteDeleteSelected()
        {
            var selectedItems = CurrentItems?.Where(x => x.IsSelected).ToList();

            if (selectedItems == null || !selectedItems.Any())
            {
                System.Windows.MessageBox.Show(
                    "Vui lòng chọn ít nhất một mục để xóa!",
                    "Thông báo",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning
                );
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"Bạn có chắc muốn xóa {selectedItems.Count} mục đã chọn?",
                "Xác nhận xóa",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question
            );

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                // TODO: Delete selected items
                // foreach (var item in selectedItems)
                // {
                //     var movieBLL = new MovieBLL();
                //     movieBLL.Delete(item.ID);
                // }

                LoadData();
                System.Windows.MessageBox.Show("Xóa thành công!");
            }
        }

        public void ClearFilters()
        {
            SearchQuery = string.Empty;
            StatusFilter = "Tất cả";
            TypeFilter = "Tất cả";
            YearFilter = "Tất cả";
        }

        public void SelectAll(bool isSelected)
        {
            if (CurrentItems == null) return;

            foreach (var item in CurrentItems)
            {
                item.IsSelected = isSelected;
            }
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// ViewModel for DataGrid items
    /// </summary>
    public class DataItemViewModel : BaseViewModel
    {
        private bool _isSelected;

        public int ID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
        public double Rating { get; set; }
        public string Status { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    #endregion
}