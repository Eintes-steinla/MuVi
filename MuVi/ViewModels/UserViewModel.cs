using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace MuVi.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private readonly UserBLL _userBLL = new UserBLL();

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
                        foreach (var user in UserList)
                        {
                            user.IsSelected = value.Value;
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
                _userBLL.SetSearchKeyword(value);
                LoadUsers();
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
                _userBLL.SetStatusFilter(value);
                LoadUsers();
            }
        }

        // Role filter
        private string _selectedRole = "Tất cả";
        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged(nameof(SelectedRole));
                _userBLL.SetRoleFilter(value);
                LoadUsers();
            }
        }

        // Date of birth filter
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));

                _userBLL.SetDateFilter(value);

                LoadUsers();
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

        public UserViewModel()
        {
            UserList = new ObservableCollection<UserDTO>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadUsers());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedUsers());

            LoadUsers();
        }

        public void LoadUsers()
        {
            var users = _userBLL.GetUsers();

            UserList.Clear();
            foreach (var u in users)
            {
                u.PropertyChanged += User_PropertyChanged;
                UserList.Add(u);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void User_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (UserList == null || !UserList.Any())
            {
                _isAllSelected = false;
            }
            else if (UserList.All(u => u.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (UserList.All(u => !u.IsSelected))
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
            int currentPage = _userBLL.GetCurrentPage();
            int totalPages = _userBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _userBLL.NextPage();
            LoadUsers();
        }

        public void PreviousPage()
        {
            _userBLL.PreviousPage();
            LoadUsers();
        }

        public void FirstPage()
        {
            _userBLL.FirstPage();
            LoadUsers();
        }

        public void LastPage()
        {
            _userBLL.LastPage();
            LoadUsers();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            SelectedStatus = "Tất cả";
            SelectedRole = "Tất cả";
            SelectedDate = null;
            _userBLL.ClearFilters();
            LoadUsers();
        }

        private void DeleteSelectedUsers()
        {
            var selectedUsers = UserList.Where(u => u.IsSelected).ToList();

            if (!selectedUsers.Any())
            {
                MessageBox.Show("Vui lòng chọn người dùng cần xóa", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedUsers.Count} người dùng đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var userIds = selectedUsers.Select(u => u.UserID).ToList();
                bool success = _userBLL.DeleteMultipleUsers(userIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadUsers();
                }
            }
        }

        public List<UserDTO> GetSelectedUsers()
        {
            return UserList.Where(u => u.IsSelected).ToList();
        }
    }
}