using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels;
using MuVi.Views.AddView;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class ReviewUC : UserControl
    {
        private ReviewViewModel _viewModel;

        public ReviewUC()
        {
            InitializeComponent();
            _viewModel = new ReviewViewModel();
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
            var addEditWindow = new ReviewAddView();
            if (addEditWindow.ShowDialog() == true)
            {
                _viewModel.LoadReviews();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ReviewDTO review)
            {
                var addEditWindow = new ReviewAddView(review);
                if (addEditWindow.ShowDialog() == true)
                {
                    _viewModel.LoadReviews();
                }
            }
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ReviewDTO review)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa đánh giá của '{review.Username}' về phim '{review.MovieTitle}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var bll = new ReviewBLL();
                    bool success = bll.DeleteReview(review.ReviewID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadReviews();
                    }
                }
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ReviewDTO review)
            {
                var viewWindow = new ReviewAddView(review);
                viewWindow.ShowDialog();
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel đang được phát triển", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}