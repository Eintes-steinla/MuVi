using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace MuVi.ViewModels
{
    public class ActorViewModel : BaseViewModel
    {
        private readonly ActorBLL _actorBLL = new ActorBLL();

        public ObservableCollection<ActorDTO> ActorList { get; set; }
        public ObservableCollection<string> NationalityList { get; set; }

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
                        foreach (var actor in ActorList)
                        {
                            actor.IsSelected = value.Value;
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
                _actorBLL.SetSearchKeyword(value);
                LoadActors();
            }
        }

        // Nationality filter
        private string _selectedNationality = "Tất cả";
        public string SelectedNationality
        {
            get => _selectedNationality;
            set
            {
                _selectedNationality = value;
                OnPropertyChanged(nameof(SelectedNationality));
                _actorBLL.SetNationalityFilter(value == "Tất cả" ? null : value);
                LoadActors();
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

        public ActorViewModel()
        {
            ActorList = new ObservableCollection<ActorDTO>();
            NationalityList = new ObservableCollection<string>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadActors());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedActors());

            LoadNationalities();
            _actorBLL.ClearFilters();
            LoadActors();
        }

        public void LoadNationalities()
        {
            var nationalities = _actorBLL.GetDistinctNationalities();

            NationalityList.Clear();
            NationalityList.Add("Tất cả");
            foreach (var n in nationalities)
            {
                NationalityList.Add(n);
            }
        }

        public void LoadActors()
        {
            var actors = _actorBLL.GetActors();

            ActorList.Clear();
            foreach (var a in actors)
            {
                a.PropertyChanged += Actor_PropertyChanged;
                ActorList.Add(a);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void Actor_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ActorDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (ActorList == null || !ActorList.Any())
            {
                _isAllSelected = false;
            }
            else if (ActorList.All(a => a.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (ActorList.All(a => !a.IsSelected))
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
            int currentPage = _actorBLL.GetCurrentPage();
            int totalPages = _actorBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _actorBLL.NextPage();
            LoadActors();
        }

        public void PreviousPage()
        {
            _actorBLL.PreviousPage();
            LoadActors();
        }

        public void FirstPage()
        {
            _actorBLL.FirstPage();
            LoadActors();
        }

        public void LastPage()
        {
            _actorBLL.LastPage();
            LoadActors();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            SelectedNationality = "Tất cả";
            _actorBLL.ClearFilters();
            LoadActors();
        }

        private void DeleteSelectedActors()
        {
            var selectedActors = ActorList.Where(a => a.IsSelected).ToList();

            if (!selectedActors.Any())
            {
                MessageBox.Show("Vui lòng chọn diễn viên cần xóa", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedActors.Count} diễn viên đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var actorIds = selectedActors.Select(a => a.ActorID).ToList();
                bool success = _actorBLL.DeleteMultipleActors(actorIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadActors();
                }
            }
        }

        public List<ActorDTO> GetSelectedActors()
        {
            return ActorList.Where(a => a.IsSelected).ToList();
        }
    }
}