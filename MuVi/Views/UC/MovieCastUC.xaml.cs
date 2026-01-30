using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels;
using MuVi.Views.AddView;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class MovieCastUC : UserControl
    {
        private MovieCastViewModel _viewModel;

        public MovieCastUC()
        {
            InitializeComponent();
            _viewModel = new MovieCastViewModel();
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
            var addEditWindow = new MovieCastAddView();
            if (addEditWindow.ShowDialog() == true)
            {
                _viewModel.LoadCasts();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MovieCastDTO cast)
            {
                var addEditWindow = new MovieCastAddView(cast);
                if (addEditWindow.ShowDialog() == true)
                {
                    _viewModel.LoadCasts();
                }
            }
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MovieCastDTO cast)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa diễn viên '{cast.ActorName}' khỏi phim '{cast.MovieTitle}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var bll = new MovieCastBLL();
                    bool success = bll.DeleteMovieCast(cast.MovieID, cast.ActorID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadCasts();
                    }
                }
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng xuất Excel đang được phát triển", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}