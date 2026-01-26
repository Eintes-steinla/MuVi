using MuVi.ViewModels;
using MuVi.Views.AddView;
using MuVi.DTO.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace MuVi.Views.UC
{
    public partial class UserUC : UserControl
    {
        private UserViewModel _viewModel;

        public UserUC()
        {
            InitializeComponent();
            _viewModel = new UserViewModel();
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
            // Open Add/Edit window with no user (Add mode)
            var addEditWindow = new UserAddView();
            if (addEditWindow.ShowDialog() == true)
            {
                _viewModel.LoadUsers();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is UserDTO user)
            {
                // Open Add/Edit window with user data (Edit mode)
                var addEditWindow = new UserAddView(user);
                if (addEditWindow.ShowDialog() == true)
                {
                    _viewModel.LoadUsers();
                }
            }
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is UserDTO user)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa người dùng '{user.Username}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Call BLL to delete user
                    var bll = new MuVi.BLL.UserBLL();
                    bool success = bll.DeleteUser(user.UserID, out string message);

                    MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                        MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                    if (success)
                    {
                        _viewModel.LoadUsers();
                    }
                }
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is UserDTO user)
            {
                // !
                // Open view details window
                var viewWindow = new UserAddView(user);
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