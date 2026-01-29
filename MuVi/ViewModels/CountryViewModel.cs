using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace MuVi.ViewModels
{
    public class CountryViewModel : BaseViewModel
    {
        private readonly CountryBLL _countryBLL = new CountryBLL();

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
                        foreach (var country in CountryList)
                        {
                            country.IsSelected = value.Value;
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
                _countryBLL.SetSearchKeyword(value);
                LoadCountries();
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

        public CountryViewModel()
        {
            CountryList = new ObservableCollection<CountryDTO>();

            // Initialize commands
            RefreshCommand = new RelayCommand(p => LoadCountries());
            ClearFilterCommand = new RelayCommand(p => ClearFilters());
            DeleteSelectedCommand = new RelayCommand(p => DeleteSelectedCountries());

            _countryBLL.ClearFilters();
            LoadCountries();
        }

        public void LoadCountries()
        {
            var countries = _countryBLL.GetCountries();

            CountryList.Clear();
            foreach (var c in countries)
            {
                c.PropertyChanged += Country_PropertyChanged;
                CountryList.Add(c);
            }

            UpdatePageInfo();
            UpdateSelectAllState();
        }

        private void Country_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CountryDTO.IsSelected))
            {
                UpdateSelectAllState();
            }
        }

        private void UpdateSelectAllState()
        {
            if (CountryList == null || !CountryList.Any())
            {
                _isAllSelected = false;
            }
            else if (CountryList.All(c => c.IsSelected))
            {
                _isAllSelected = true;
            }
            else if (CountryList.All(c => !c.IsSelected))
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
            int currentPage = _countryBLL.GetCurrentPage();
            int totalPages = _countryBLL.GetTotalPages();
            PageInfo = $"Trang {currentPage}/{totalPages}";
        }

        public void NextPage()
        {
            _countryBLL.NextPage();
            LoadCountries();
        }

        public void PreviousPage()
        {
            _countryBLL.PreviousPage();
            LoadCountries();
        }

        public void FirstPage()
        {
            _countryBLL.FirstPage();
            LoadCountries();
        }

        public void LastPage()
        {
            _countryBLL.LastPage();
            LoadCountries();
        }

        private void ClearFilters()
        {
            SearchKeyword = "";
            _countryBLL.ClearFilters();
            LoadCountries();
        }

        private void DeleteSelectedCountries()
        {
            var selectedCountries = CountryList.Where(c => c.IsSelected).ToList();

            if (!selectedCountries.Any())
            {
                MessageBox.Show("Vui lòng chọn quốc gia cần xóa", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedCountries.Count} quốc gia đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var countryIds = selectedCountries.Select(c => c.CountryID).ToList();
                bool success = _countryBLL.DeleteMultipleCountries(countryIds, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadCountries();
                }
            }
        }

        public List<CountryDTO> GetSelectedCountries()
        {
            return CountryList.Where(c => c.IsSelected).ToList();
        }
    }
}