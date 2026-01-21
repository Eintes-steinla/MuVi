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
        }

        /// <summary>
        /// Đóng window
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Xử lý khi password thay đổi
        /// </summary>
        private void pwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = pwd.Password;
            }
        }

        /// <summary>
        /// Chuyển đến màn hình đăng ký
        /// </summary>
        private void txtbRegisterLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var registerView = new RegisterView();
            registerView.Show();
            Close();
        }

        /// <summary>
        /// Xử lý quên mật khẩu
        /// </summary>
        private void lblForgotPwd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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