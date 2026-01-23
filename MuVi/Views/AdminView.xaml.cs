using MuVi.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MuVi.Views
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : Window
    {
        private AdminViewModel _viewModel;

        public AdminView()
        {
            InitializeComponent();
            _viewModel = new AdminViewModel();
            DataContext = _viewModel;

            // Enable window dragging (optional - nếu muốn kéo window)
            // MouseLeftButtonDown += Window_MouseLeftButtonDown;
        }

        #region Window Events

        /// <summary>
        /// Đóng window
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn đóng ứng dụng?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận đăng xuất",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                // TODO: Clear session
                // AppSession.Instance.Clear();

                // Navigate to login
                var loginView = new LoginView();
                loginView.Show();
                Close();
            }
        }

        /// <summary>
        /// Cho phép kéo window (nếu cần)
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #endregion

        #region Tab Control Events

        /// <summary>
        /// Xử lý khi chuyển tab
        /// </summary>
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                string tabTag = selectedTab.Tag?.ToString();
                if (!string.IsNullOrEmpty(tabTag))
                {
                    _viewModel.CurrentTab = tabTag;

                    // Update DataGrid columns based on tab
                    UpdateDataGridColumns(tabTag);
                }
            }
        }

        /// <summary>
        /// Cập nhật columns của DataGrid theo từng tab
        /// </summary>
        private void UpdateDataGridColumns(string tabTag)
        {
            // TODO: Customize columns for each tab
            // For now, we keep the default columns

            // Example:
            // dgData.Columns.Clear();
            // switch (tabTag)
            // {
            //     case "Movies":
            //         // Add movie-specific columns
            //         break;
            //     case "Genres":
            //         // Add genre-specific columns
            //         break;
            //     // ... other cases
            // }
        }

        #endregion

        #region Filter Events

        /// <summary>
        /// Xử lý khi tìm kiếm
        /// </summary>
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchQuery = txtSearch.Text;
        }

        /// <summary>
        /// Xử lý khi thay đổi filter trạng thái
        /// </summary>
        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cmbStatus.SelectedItem is ComboBoxItem selectedItem)
            //{
            //    _viewModel.StatusFilter = selectedItem.Content.ToString();
            //}
        }

        /// <summary>
        /// Xử lý khi thay đổi filter loại phim
        /// </summary>
        private void cmbMovieType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cmbMovieType.SelectedItem is ComboBoxItem selectedItem)
            //{
            //    _viewModel.TypeFilter = selectedItem.Content.ToString();
            //}
        }

        /// <summary>
        /// Xử lý khi thay đổi filter năm
        /// </summary>
        private void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cmbYear.SelectedItem is ComboBoxItem selectedItem)
            //{
            //    _viewModel.YearFilter = selectedItem.Content.ToString();
            //}
        }

        /// <summary>
        /// Xóa bộ lọc
        /// </summary>
        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearFilters();

            // Reset UI
            txtSearch.Text = string.Empty;
            cmbStatus.SelectedIndex = 0;
            cmbMovieType.SelectedIndex = 0;
            cmbYear.SelectedIndex = 0;
        }

        #endregion

        #region Toolbar Events

        /// <summary>
        /// Thêm mới
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string currentTab = _viewModel.CurrentTab;

            // TODO: Open appropriate add dialog based on current tab
            switch (currentTab)
            {
                case "Movies":
                    OpenAddMovieDialog();
                    break;
                case "Genres":
                    OpenAddGenreDialog();
                    break;
                case "Actors":
                    OpenAddActorDialog();
                    break;
                case "Countries":
                    OpenAddCountryDialog();
                    break;
                case "Episodes":
                    OpenAddEpisodeDialog();
                    break;
                case "Users":
                    OpenAddUserDialog();
                    break;
            }
        }

        /// <summary>
        /// Làm mới dữ liệu
        /// </summary>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadData();
            MessageBox.Show(
                "Dữ liệu đã được làm mới!",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Xóa các mục đã chọn
        /// </summary>
        private void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            //_viewModel.ExecuteDeleteSelected();
        }

        /// <summary>
        /// Xuất dữ liệu ra Excel
        /// </summary>
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Tính năng xuất Excel đang được phát triển!",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // TODO: Implement Excel export
            // var excelExporter = new ExcelExporter();
            // excelExporter.Export(_viewModel.CurrentItems, $"{_viewModel.CurrentTab}.xlsx");
        }

        #endregion

        #region DataGrid Events

        /// <summary>
        /// Double click vào row để xem chi tiết
        /// </summary>
        private void dgData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgData.SelectedItem is DataItemViewModel selectedItem)
            {
                OpenDetailDialog(selectedItem);
            }
        }

        /// <summary>
        /// Chỉnh sửa
        /// </summary>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is DataItemViewModel item)
            {
                OpenEditDialog(item);
            }
        }

        /// <summary>
        /// Xóa
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is DataItemViewModel item)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa '{item.Title}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    //_viewModel.ExecuteDelete(item);
                }
            }
        }

        /// <summary>
        /// Xem chi tiết
        /// </summary>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is DataItemViewModel item)
            {
                OpenDetailDialog(item);
            }
        }

        /// <summary>
        /// Select all checkbox
        /// </summary>
        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectAll(true);
        }

        /// <summary>
        /// Unselect all checkbox
        /// </summary>
        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectAll(false);
        }

        #endregion

        #region Pagination Events

        /// <summary>
        /// Chuyển đến trang đầu
        /// </summary>
        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GoToFirstPage();
            UpdatePageInfo();
        }

        /// <summary>
        /// Chuyển đến trang trước
        /// </summary>
        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GoToPreviousPage();
            UpdatePageInfo();
        }

        /// <summary>
        /// Chuyển đến trang sau
        /// </summary>
        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GoToNextPage();
            UpdatePageInfo();
        }

        /// <summary>
        /// Chuyển đến trang cuối
        /// </summary>
        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GoToLastPage();
            UpdatePageInfo();
        }

        /// <summary>
        /// Cập nhật thông tin trang
        /// </summary>
        private void UpdatePageInfo()
        {
            //txtPageInfo.Text = $"{_viewModel.CurrentPage} / {_viewModel.TotalPages}";

            // Update button states
            btnFirstPage.IsEnabled = _viewModel.CurrentPage > 1;
            btnPrevPage.IsEnabled = _viewModel.CurrentPage > 1;
            btnNextPage.IsEnabled = _viewModel.CurrentPage < _viewModel.TotalPages;
            btnLastPage.IsEnabled = _viewModel.CurrentPage < _viewModel.TotalPages;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Mở dialog thêm phim
        /// </summary>
        private void OpenAddMovieDialog()
        {
            // TODO: Create and open AddMovieDialog
            MessageBox.Show(
                "Dialog thêm phim đang được phát triển!",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // var dialog = new AddMovieDialog();
            // if (dialog.ShowDialog() == true)
            // {
            //     _viewModel.LoadData();
            // }
        }

        private void OpenAddGenreDialog()
        {
            MessageBox.Show("Dialog thêm thể loại đang được phát triển!", "Thông báo");
        }

        private void OpenAddActorDialog()
        {
            MessageBox.Show("Dialog thêm diễn viên đang được phát triển!", "Thông báo");
        }

        private void OpenAddCountryDialog()
        {
            MessageBox.Show("Dialog thêm quốc gia đang được phát triển!", "Thông báo");
        }

        private void OpenAddEpisodeDialog()
        {
            MessageBox.Show("Dialog thêm tập phim đang được phát triển!", "Thông báo");
        }

        private void OpenAddUserDialog()
        {
            MessageBox.Show("Dialog thêm người dùng đang được phát triển!", "Thông báo");
        }

        /// <summary>
        /// Mở dialog chỉnh sửa
        /// </summary>
        private void OpenEditDialog(DataItemViewModel item)
        {
            MessageBox.Show(
                $"Chỉnh sửa: {item.Title}\nID: {item.ID}",
                "Chỉnh sửa",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // TODO: Open edit dialog
            // var dialog = new EditMovieDialog(item.ID);
            // if (dialog.ShowDialog() == true)
            // {
            //     _viewModel.LoadData();
            // }
        }

        /// <summary>
        /// Mở dialog xem chi tiết
        /// </summary>
        private void OpenDetailDialog(DataItemViewModel item)
        {
            MessageBox.Show(
                $"Chi tiết:\n" +
                $"ID: {item.ID}\n" +
                $"Tiêu đề: {item.Title}\n" +
                $"Loại: {item.Type}\n" +
                $"Năm: {item.Year}\n" +
                $"Đánh giá: {item.Rating}\n" +
                $"Trạng thái: {item.Status}",
                "Chi tiết",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // TODO: Open detail dialog
            // var dialog = new MovieDetailDialog(item.ID);
            // dialog.ShowDialog();
        }

        #endregion
    }
}