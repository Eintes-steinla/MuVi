using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels;
using MuVi.ViewModels.UCViewModel;
using System;
using System.Windows;

namespace MuVi.Views.AddView
{
    /// <summary>
    /// Interaction logic for GenreAddView.xaml
    /// </summary>
    public partial class GenreAddView : Window
    {
        private GenreAddViewModel _viewModel;

        /// <summary>
        /// Constructor cho chế độ Add (thêm mới)
        /// </summary>
        public GenreAddView()
        {
            InitializeComponent();

            _viewModel = new GenreAddViewModel();
            DataContext = _viewModel;

            Title = "Thêm thể loại";
        }

        /// <summary>
        /// Constructor cho chế độ Edit (chỉnh sửa)
        /// </summary>
        public GenreAddView(GenreDTO existingGenre)
        {
            InitializeComponent();

            _viewModel = new GenreAddViewModel(existingGenre);
            DataContext = _viewModel;

            Title = "Chỉnh sửa thể loại";
        }

        /// <summary>
        /// Lưu thông tin thể loại
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

                var genreBLL = new GenreBLL();
                bool success;
                string message;

                if (_viewModel.IsAddMode)
                {
                    // Thêm thể loại mới
                    var newGenre = new GenreDTO
                    {
                        GenreName = _viewModel.GenreName?.Trim(),
                        Description = _viewModel.Description?.Trim()
                    };

                    success = genreBLL.AddGenre(newGenre, out message);
                }
                else
                {
                    // Cập nhật thể loại
                    success = genreBLL.UpdateGenre(_viewModel.Genre, out message);
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
            catch (Exception ex)
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