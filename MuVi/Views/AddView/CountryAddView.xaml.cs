using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels.UCViewModel;
using System.Windows;

namespace MuVi.Views.AddView
{
    /// <summary>
    /// Interaction logic for CountryAddView.xaml
    /// </summary>
    public partial class CountryAddView : Window
    {
        private CountryAddViewModel _viewModel;

        /// <summary>
        /// Constructor cho chế độ Add (thêm mới)
        /// </summary>
        public CountryAddView()
        {
            InitializeComponent();

            _viewModel = new CountryAddViewModel();
            DataContext = _viewModel;

            Title = "Thêm quốc gia";
        }

        /// <summary>
        /// Constructor cho chế độ Edit (chỉnh sửa)
        /// </summary>
        public CountryAddView(CountryDTO existingCountry)
        {
            InitializeComponent();

            _viewModel = new CountryAddViewModel(existingCountry);
            DataContext = _viewModel;

            Title = "Chỉnh sửa quốc gia";
        }

        /// <summary>
        /// Lưu thông tin quốc gia
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate
                if (!_viewModel.Validate())
                {
                    return;
                }

                var countryBLL = new CountryBLL();
                bool success;
                string message;

                if (_viewModel.IsAddMode)
                {
                    // Thêm quốc gia mới
                    var newCountry = new CountryDTO
                    {
                        CountryName = _viewModel.CountryName?.Trim(),
                        CountryCode = _viewModel.CountryCode?.Trim()
                    };

                    success = countryBLL.AddCountry(newCountry, out message);
                }
                else
                {
                    // Cập nhật quốc gia
                    success = countryBLL.UpdateCountry(_viewModel.Country, out message);
                }

                MessageBox.Show(message,
                    success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK,
                    success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Hủy bỏ
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}