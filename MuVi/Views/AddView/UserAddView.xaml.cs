using MuVi.DTO.DTOs;
using MuVi.BLL;
using System;
using System.Windows;

namespace MuVi.Views.AddView
{
    public partial class UserAddView: Window
    {
        private UserDTO _user;
        private bool _isAddMode;
        private UserBLL _userBLL = new UserBLL();

        public string WindowTitle => _isAddMode ? "Thêm người dùng mới" : "Chỉnh sửa người dùng";
        public bool IsAddMode => _isAddMode;

        // Constructor for Add mode
        public UserAddView()
        {
            InitializeComponent();
            _isAddMode = true;
            _user = new UserDTO { IsActive = true, Role = "User" };
            DataContext = _user;
        }

        // Constructor for Edit mode
        public UserAddView(UserDTO user)
        {
            InitializeComponent();
            _isAddMode = false;
            _user = user;
            DataContext = _user;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập email", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isAddMode)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _user.Password = txtPassword.Password;
                bool success = _userBLL.AddUser(_user, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
            }
            else
            {
                // Update mode
                bool success = _userBLL.UpdateUser(_user, out string message);

                MessageBox.Show(message, success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}