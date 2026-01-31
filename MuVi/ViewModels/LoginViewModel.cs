using MuVi.BLL;
using MuVi.Commands;
using MuVi.Helpers;
using MuVi.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace MuVi.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Fields
        private string _usernameOrEmail;
        private string _password;
        private bool _rememberMe;
        private bool _isLoading;
        private string _errorMessage;
        #endregion

        #region Properties

        /// <summary>
        /// Username hoặc Email của người dùng
        /// </summary>
        public string UsernameOrEmail
        {
            get => _usernameOrEmail;
            set
            {
                // Kiểm tra xem giá trị mới(value) có khác giá trị cũ đang nằm trong _usernameOrEmail hay không
                // Nếu khác, nó sẽ gán _usernameOrEmail = value
                if (SetProperty(ref _usernameOrEmail, value))
                {
                    ErrorMessage = string.Empty;
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
            return !string.IsNullOrWhiteSpace(UsernameOrEmail)
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

                if (!ValidateInput())
                {
                    MessageBox.Show(
                        ErrorMessage,
                        "Lỗi xác thực",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                await System.Threading.Tasks.Task.Delay(500);

                var userBLL = new UserBLL();

                var user = userBLL.Login(UsernameOrEmail, Password, out string message);

                if (user != null)
                {
                    if (RememberMe)
                    {
                        AppSession.Instance.CurrentUser = user;
                        AppSession.Instance.LoginTime = DateTime.Now;
                    }

                    Task.Run(() => userBLL.UpdateLastLogin(user.UserID));
                    // ĐĂNG NHẬP THÀNH CÔNG
                    MessageBox.Show(
                        message,
                        "Thông báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    // Kiểm tra role
                    if (user.Role == "Admin")
                    {
                        // Mở AdminView
                        var adminWindow = new AdminView();
                        adminWindow.Show();
                    }
                    else
                    {
                        // Mở UserDashboardView cho người dùng thường
                        var userDashboard = new UserDashboardView();
                        userDashboard.Show();
                    }
                }
                else
                {
                    // ĐĂNG NHẬP THẤT BẠI
                    MessageBox.Show(
                        message,
                        "Thông báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi hệ thống: " + ex.Message;
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
            if (string.IsNullOrWhiteSpace(UsernameOrEmail))
            {
                ErrorMessage = "Vui lòng nhập tên đăng nhập hoặc email!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu!";
                return false;
            }

            // update mk mạnh hơn
            if (Password.Length < 8)
            {
                ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự!";
                return false;
            }

            return true;
        }

        #endregion
    }
}