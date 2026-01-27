using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels;
using MuVi.ViewModels.UCViewModel;
using System;
using System.Windows;

namespace MuVi.Views.AddView
{
    /// <summary>
    /// Interaction logic for UserAddView.xaml
    /// </summary>
    public partial class UserAddView : Window
    {
        private UserAddViewModel _viewModel;

        /// <summary>
        /// Constructor cho chế độ Add (thêm mới)
        /// </summary>
        public UserAddView()
        {
            InitializeComponent();

            _viewModel = new UserAddViewModel();
            DataContext = _viewModel;

            Title = "Thêm người dùng";
        }

        /// <summary>
        /// Constructor cho chế độ Edit (chỉnh sửa)
        /// </summary>
        public UserAddView(UserDTO existingUser)
        {
            InitializeComponent();

            _viewModel = new UserAddViewModel(existingUser);
            DataContext = _viewModel;

            Title = "Chỉnh sửa người dùng";

            // Ẩn password khi edit
            txtPassword.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Lưu thông tin user
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy password từ PasswordBox nếu là Add mode
                if (_viewModel.IsAddMode)
                {
                    _viewModel.Password = txtPassword.Password;
                }

                // Validate
                if (!_viewModel.Validate())
                {
                    return;
                }

                // Lưu ảnh và lấy đường dẫn
                var avatarPath = _viewModel.SaveAvatar();
                if (!string.IsNullOrEmpty(avatarPath))
                {
                    _viewModel.Avatar = avatarPath;
                }

                var userBLL = new UserBLL();
                bool success;
                string message;

                if (_viewModel.IsAddMode)
                {
                    // Thêm user mới
                    var newUser = new UserDTO
                    {
                        Username = _viewModel.Username.Trim(),
                        Password = _viewModel.Password,
                        Email = _viewModel.Email?.Trim(),
                        DateOfBirth = _viewModel.DateOfBirth,
                        Avatar = avatarPath,
                        Role = _viewModel.Role,
                        IsActive = _viewModel.IsActive
                    };

                    success = userBLL.AddUser(newUser, out message);
                }
                else
                {
                    // Cập nhật user
                    _viewModel.User.Avatar = avatarPath;
                    success = userBLL.UpdateUser(_viewModel.User, out message);
                }

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
            catch (Exception ex)
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