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
    public class HistoryViewModel : BaseViewModel
    {
        private readonly ViewHistoryBLL _historyBLL = new ViewHistoryBLL();

        public ObservableCollection<ViewHistoryDTO> HistoryList { get; set; }
        public ObservableCollection<UserDTO> UserList { get; set; }

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
                        foreach (var history in HistoryList)
                        {
                            history.IsSelected = value.Value;
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
                _historyBLL.SetSearchKeyword(value);
                LoadHistories();
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
                _historyBLL.SetStatusFilter(value);
                LoadHistories();
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
                _historyBLL.SetMovieTypeFilter(value);
                LoadHistories();
            }
        }

        // User filter
        private UserDTO? _selectedUser;
        public UserDTO? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                _historyBLL.SetUserFilter(value?.UserID);
                LoadHistories();
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

        public HistoryViewModel()
        {
            HistoryList = new ObservableCollection<ViewHistoryDTO>();
            UserList = new ObservableCollection<UserDTO>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadHistories());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedHistories());

            LoadUsers();
            _historyBLL.ClearFilters();
            LoadHistories();
        }

        public void LoadUsers()
        {
            var users = _historyBLL.GetAllUsers();

            UserList.Clear();
            UserList.Add(new UserDTO { UserID = 0, Username = "Tất cả" });
            foreach (var u in users)
            {
                UserList.Add(u);
            }

            SelectedUser = UserList.FirstOrDefault();
        }

        public void LoadHistories()
        {
            var histories = _historyBLL.GetHistories();

            HistoryList.Clear();
            foreach (var h in histories)
            {
                h.PropertyChanged += History_PropertyChanged;
                HistoryList.Add(h);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void History_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewHistoryDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (HistoryList == null || !HistoryList.Any())
            {
                _isAllSelected = false;
            }
            else if (HistoryList.All(h => h.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (HistoryList.All(h => !h.IsSelected))
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
            int currentPage = _historyBLL.GetCurrentPage();
            int totalPages = _historyBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _historyBLL.NextPage();
            LoadHistories();
        }

        public void PreviousPage()
        {
            _historyBLL.PreviousPage();
            LoadHistories();
        }

        public void FirstPage()
        {
            _historyBLL.FirstPage();
            LoadHistories();
        }

        public void LastPage()
        {
            _historyBLL.LastPage();
            LoadHistories();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            SelectedStatus = "Tất cả";
            SelectedMovieType = "Tất cả";
            SelectedUser = UserList.FirstOrDefault();
            _historyBLL.ClearFilters();
            LoadHistories();
        }

        private void DeleteSelectedHistories()
        {
            var selectedHistories = HistoryList.Where(h => h.IsSelected).ToList();

            if (!selectedHistories.Any())
            {
                MessageBox.Show("Vui lòng chọn lịch sử cần xóa", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedHistories.Count} lịch sử đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var historyIds = selectedHistories.Select(h => h.HistoryID).ToList();
                bool success = _historyBLL.DeleteMultipleHistories(historyIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadHistories();
                }
            }
        }
    }
}