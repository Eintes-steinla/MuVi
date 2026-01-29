using MuVi.ViewModels;
using MuVi.Views.AddView;
using MuVi.DTO.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class MovieUC : UserControl
    {
        private MovieViewModel _viewModel;

        public MovieUC()
        {
            InitializeComponent();
            _viewModel = new MovieViewModel();
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
            // Open Add/Edit window with no movie (Add mode)
            var addEditWindow = new MovieAddView();
            if (addEditWindow.ShowDialog() == true)
            {
                _viewModel.LoadMovies();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MovieDTO movie)
            {
                // Open Add/Edit window with movie data (Edit mode)
                var addEditWindow = new MovieAddView(movie);
                if (addEditWindow.ShowDialog() == true)
                {
                    _viewModel.LoadMovies();
                }
            }
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MovieDTO movie)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa phim '{movie.Title}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Call BLL to delete movie
                    var bll = new MuVi.BLL.MovieBLL();
                    bool success = bll.DeleteMovie(movie.MovieID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadMovies();
                    }
                }
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MovieDTO movie)
            {
                // Open view details window
                var viewWindow = new MovieAddView(movie);
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