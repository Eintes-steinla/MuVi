using MuVi.ViewModels;
using MuVi.Views.AddView;
using MuVi.DTO.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class FavoriteUC : UserControl
    {
        private FavoriteViewModel _viewModel;

        public FavoriteUC()
        {
            InitializeComponent();
            _viewModel = new FavoriteViewModel();
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
            // Open Add window
            var addWindow = new FavoriteAddView();
            if (addWindow.ShowDialog() == true)
            {
                _viewModel.LoadFavorites();
            }
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is FavoriteDTO favorite)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa phim '{favorite.MovieTitle}' khỏi danh sách yêu thích của '{favorite.Username}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Call BLL to delete favorite
                    var bll = new MuVi.BLL.FavoriteBLL();
                    bool success = bll.DeleteFavorite(favorite.UserID, favorite.MovieID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadFavorites();
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