using MuVi.ViewModels;
using MuVi.Views.AddView;
using MuVi.DTO.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class EpisodeUC : UserControl
    {
        private EpisodeViewModel _viewModel;

        public EpisodeUC()
        {
            InitializeComponent();
            _viewModel = new EpisodeViewModel();
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
            // Open Add/Edit window with no episode (Add mode)
            var addEditWindow = new EpisodeAddView();
            if (addEditWindow.ShowDialog() == true)
            {
                _viewModel.LoadEpisodes();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is EpisodeDTO episode)
            {
                // Open Add/Edit window with episode data (Edit mode)
                var addEditWindow = new EpisodeAddView(episode);
                if (addEditWindow.ShowDialog() == true)
                {
                    _viewModel.LoadEpisodes();
                }
            }
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is EpisodeDTO episode)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa tập '{episode.EpisodeNumber} - {episode.Title}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Call BLL to delete episode
                    var bll = new MuVi.BLL.EpisodeBLL();
                    bool success = bll.DeleteEpisode(episode.EpisodeID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadEpisodes();
                    }
                }
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is EpisodeDTO episode)
            {
                // Open view details window
                var viewWindow = new EpisodeAddView(episode);
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