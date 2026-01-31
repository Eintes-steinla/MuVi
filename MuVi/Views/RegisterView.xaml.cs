using MuVi.Resources.Themes;
using MuVi.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MuVi.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : ModernWindowBase
    {
        private RegisterViewModel _viewModel;

        public RegisterView()
        {
            InitializeComponent();
            _viewModel = new RegisterViewModel();
            DataContext = _viewModel;
        }

        /// <summary>
        /// Xử lý khi password thay đổi
        /// </summary>
        private void pwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.Password = pwd.Password;
            }
        }

        /// <summary>
        /// Xử lý khi confirm password thay đổi
        /// </summary>
        private void pwdConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.ConfirmPassword = pwdConfirm.Password;
            }
        }

        /// <summary>
        /// Chuyển đến màn hình đăng nhập
        /// </summary>
        private void txtbLoginLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();
            Close();
        }
    }
}