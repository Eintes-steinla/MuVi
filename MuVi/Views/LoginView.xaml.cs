using Microsoft.Win32;
using MuVi.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MuVi.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;

            // Enable window dragging
            MouseLeftButtonDown += Window_MouseLeftButtonDown;
        }

        /// <summary>
        /// Cho phép kéo window
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Đóng window
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Xử lý khi password thay đổi
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }

        /// <summary>
        /// Chuyển đến màn hình đăng ký
        /// </summary>
        private void RegisterLink_Click(object sender, MouseButtonEventArgs e)
        {
            var registerView = new RegisterView();
            registerView.Show();
            Close();
        }

        /// <summary>
        /// Xử lý quên mật khẩu
        /// </summary>
        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(
                "Tính năng quên mật khẩu đang được phát triển!",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}