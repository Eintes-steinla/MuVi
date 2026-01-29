using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels.UCViewModel;
using System;
using System.Windows;

namespace MuVi.Views.AddView
{
    public partial class ReviewAddView : Window
    {
        private ReviewAddViewModel _viewModel;

        /// <summary>
        /// Constructor cho chế độ Add (thêm mới)
        /// </summary>
        public ReviewAddView()
        {
            InitializeComponent();

            _viewModel = new ReviewAddViewModel();
            DataContext = _viewModel;

            Title = "Thêm đánh giá";
        }

        /// <summary>
        /// Constructor cho chế độ Edit (chỉnh sửa)
        /// </summary>
        public ReviewAddView(ReviewDTO existingReview)
        {
            InitializeComponent();

            _viewModel = new ReviewAddViewModel(existingReview);
            DataContext = _viewModel;

            Title = "Chỉnh sửa đánh giá";
        }

        /// <summary>
        /// Lưu thông tin đánh giá
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

                var reviewBLL = new ReviewBLL();
                bool success;
                string message;

                if (_viewModel.IsAddMode)
                {
                    // Thêm đánh giá mới
                    var newReview = new ReviewDTO
                    {
                        UserID = _viewModel.SelectedUser!.UserID,
                        MovieID = _viewModel.SelectedMovie!.MovieID,
                        Rating = _viewModel.Rating,
                        Comment = _viewModel.Comment?.Trim()
                    };

                    success = reviewBLL.AddReview(newReview, out message);
                }
                else
                {
                    // Cập nhật đánh giá
                    success = reviewBLL.UpdateReview(_viewModel.Review, out message);
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