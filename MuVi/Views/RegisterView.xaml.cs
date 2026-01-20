using System.Windows;
using System.Windows.Input;
using MuVi.ViewModels;

namespace MuVi.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Window
    {
        private RegisterViewModel _viewModel;

        public RegisterView()
        {
            InitializeComponent();
            _viewModel = new RegisterViewModel();
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
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }

        /// <summary>
        /// Xử lý khi confirm password thay đổi
        /// </summary>
        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }

        /// <summary>
        /// Chuyển đến màn hình đăng nhập
        /// </summary>
        private void LoginLink_Click(object sender, MouseButtonEventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();
            Close();
        }
    }
}