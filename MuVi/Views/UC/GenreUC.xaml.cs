using MuVi.ViewModels;
using MuVi.Views.AddView;
using MuVi.DTO.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class GenreUC : UserControl
    {
        private GenreViewModel _viewModel;

        public GenreUC()
        {
            InitializeComponent();
            _viewModel = new GenreViewModel();
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
            // Open Add/Edit window with no genre (Add mode)
            var addEditWindow = new GenreAddView();
            if (addEditWindow.ShowDialog() == true)
            {
                _viewModel.LoadGenres();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is GenreDTO genre)
            {
                // Open Add/Edit window with genre data (Edit mode)
                var addEditWindow = new GenreAddView(genre);
                if (addEditWindow.ShowDialog() == true)
                {
                    _viewModel.LoadGenres();
                }
            }
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is GenreDTO genre)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa thể loại '{genre.GenreName}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Call BLL to delete genre
                    var bll = new MuVi.BLL.GenreBLL();
                    bool success = bll.DeleteGenre(genre.GenreID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadGenres();
                    }
                }
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is GenreDTO genre)
            {
                // Open view details window
                var viewWindow = new GenreAddView(genre);
                viewWindow.ShowDialog();
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