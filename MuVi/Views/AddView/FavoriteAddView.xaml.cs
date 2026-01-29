using MuVi.BLL;
using MuVi.ViewModels.UCViewModel;
using System.Windows;

namespace MuVi.Views.AddView
{
    /// <summary>
    /// Interaction logic for FavoriteAddView.xaml
    /// </summary>
    public partial class FavoriteAddView : Window
    {
        private FavoriteAddViewModel _viewModel;

        public FavoriteAddView()
        {
            InitializeComponent();

            _viewModel = new FavoriteAddViewModel();
            DataContext = _viewModel;
        }

        /// <summary>
        /// Thêm vào yêu thích
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

                var favoriteBLL = new FavoriteBLL();
                bool success = favoriteBLL.AddFavorite(
                    _viewModel.GetSelectedUserId(),
                    _viewModel.GetSelectedMovieId(),
                    out string message);

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