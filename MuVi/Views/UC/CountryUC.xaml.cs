using MuVi.ViewModels;
using MuVi.Views.AddView;
using MuVi.DTO.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class CountryUC : UserControl
    {
        private CountryViewModel _viewModel;

        public CountryUC()
        {
            InitializeComponent();
            _viewModel = new CountryViewModel();
            DataContext = _viewModel;
        }

        // Pagination events
        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PreviousPage();
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.NextPage();
        }

        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.FirstPage();
        }

        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LastPage();
        }

        // CRUD operations
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Open Add/Edit window with no country (Add mode)
            var addEditWindow = new CountryAddView();
            if (addEditWindow.ShowDialog() == true)
            {
                _viewModel.LoadCountries();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CountryDTO country)
            {
                // Open Add/Edit window with country data (Edit mode)
                var addEditWindow = new CountryAddView(country);
                if (addEditWindow.ShowDialog() == true)
                {
                    _viewModel.LoadCountries();
                }
            }
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CountryDTO country)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa quốc gia '{country.CountryName}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Call BLL to delete country
                    var bll = new MuVi.BLL.CountryBLL();
                    bool success = bll.DeleteCountry(country.CountryID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadCountries();
                    }
                }
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            // Implement Excel export functionality
            MessageBox.Show("Chức năng xuất Excel đang được phát triển", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}