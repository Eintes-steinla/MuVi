using MuVi.ViewModels;
using MuVi.Views.AddView;
using MuVi.DTO.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class ActorUC : UserControl
    {
        private ActorViewModel _viewModel;

        public ActorUC()
        {
            InitializeComponent();
            _viewModel = new ActorViewModel();
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
            // Open Add/Edit window with no actor (Add mode)
            var addEditWindow = new ActorAddView();
            if (addEditWindow.ShowDialog() == true)
            {
                _viewModel.LoadActors();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ActorDTO actor)
            {
                // Open Add/Edit window with actor data (Edit mode)
                var addEditWindow = new ActorAddView(actor);
                if (addEditWindow.ShowDialog() == true)
                {
                    _viewModel.LoadActors();
                }
            }
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ActorDTO actor)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa diễn viên '{actor.ActorName}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Call BLL to delete actor
                    var bll = new MuVi.BLL.ActorBLL();
                    bool success = bll.DeleteActor(actor.ActorID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadActors();
                    }
                }
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ActorDTO actor)
            {
                // Open view details window
                var viewWindow = new ActorAddView(actor);
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