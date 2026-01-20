using System;
using System.Windows;
using System.Windows.Input;
using MuVi.Commands;
// using BLL; // Uncomment khi đã có BLL

namespace MuVi.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Fields
        private string _username;
        private string _password;
        private bool _rememberMe;
        private bool _isLoading;
        private string _errorMessage;
        #endregion

        #region Properties

        /// <summary>
        /// Username hoặc Email của người dùng
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    // Clear error khi user bắt đầu nhập
                    ErrorMessage = string.Empty;
                    // Update trạng thái nút Login
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Mật khẩu của người dùng
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ErrorMessage = string.Empty;
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Có ghi nhớ đăng nhập không
        /// </summary>
        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        /// <summary>
        /// Trạng thái đang loading
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Thông báo lỗi
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; }

        #endregion

        #region Constructor

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(
                execute: _ => ExecuteLogin(),
                canExecute: _ => CanExecuteLogin()
            );
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Kiểm tra xem có thể thực hiện đăng nhập không
        /// </summary>
        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Username)
                   && !string.IsNullOrWhiteSpace(Password)
                   && !IsLoading;
        }

        /// <summary>
        /// Thực hiện đăng nhập
        /// </summary>
        private async void ExecuteLogin()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // TODO: Kết nối với BLL để xác thực
                // Ví dụ:
                // var userBLL = new UserBLL();
                // var result = await userBLL.LoginAsync(Username, Password);

                // Simulate API call
                await System.Threading.Tasks.Task.Delay(1500);

                // Mock validation - Thay thế bằng logic thực tế từ BLL
                if (Username.ToLower() == "admin" && Password == "123456")
                {
                    // Đăng nhập thành công
                    MessageBox.Show(
                        "Đăng nhập thành công!",
                        "Thông báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    // TODO: Navigate to main window
                    // var mainWindow = new MainWindow();
                    // mainWindow.Show();
                    // CloseCurrentWindow();
                }
                else
                {
                    // Đăng nhập thất bại
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                    MessageBox.Show(
                        ErrorMessage,
                        "Lỗi đăng nhập",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
                MessageBox.Show(
                    ErrorMessage,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Validate input data
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Vui lòng nhập tên đăng nhập hoặc email!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu!";
                return false;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự!";
                return false;
            }

            return true;
        }

        #endregion
    }
}