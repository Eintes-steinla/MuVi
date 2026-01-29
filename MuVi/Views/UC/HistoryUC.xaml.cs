using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class HistoryUC : UserControl
    {
        private HistoryViewModel _viewModel;

        public HistoryUC()
        {
            InitializeComponent();
            _viewModel = new HistoryViewModel();
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

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ViewHistoryDTO history)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa lịch sử xem '{history.DisplayName}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var bll = new ViewHistoryBLL();
                    bool success = bll.DeleteHistory(history.HistoryID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadHistories();
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