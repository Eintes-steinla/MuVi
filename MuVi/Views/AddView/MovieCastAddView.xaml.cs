using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels.UCViewModel;
using System;
using System.Windows;

namespace MuVi.Views.AddView
{
    public partial class MovieCastAddView : Window
    {
        private MovieCastAddViewModel _viewModel;

        /// <summary>
        /// Constructor cho chế độ Add (thêm mới)
        /// </summary>
        public MovieCastAddView()
        {
            InitializeComponent();

            _viewModel = new MovieCastAddViewModel();
            DataContext = _viewModel;

            Title = "Thêm diễn viên vào phim";
        }

        /// <summary>
        /// Constructor cho chế độ Edit (chỉnh sửa)
        /// </summary>
        public MovieCastAddView(MovieCastDTO existingCast)
        {
            InitializeComponent();

            _viewModel = new MovieCastAddViewModel(existingCast);
            DataContext = _viewModel;

            Title = "Chỉnh sửa thông tin vai diễn";
        }

        /// <summary>
        /// Lưu thông tin
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

                var castBLL = new MovieCastBLL();
                bool success;
                string message;

                if (_viewModel.IsAddMode)
                {
                    // Thêm mới
                    var newCast = new MovieCastDTO
                    {
                        MovieID = _viewModel.SelectedMovie!.MovieID,
                        ActorID = _viewModel.SelectedActor!.ActorID,
                        RoleName = _viewModel.RoleName?.Trim(),
                        Order = _viewModel.Order
                    };

                    success = castBLL.AddMovieCast(newCast, out message);
                }
                else
                {
                    // Cập nhật
                    success = castBLL.UpdateMovieCast(_viewModel.Cast, out message);
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