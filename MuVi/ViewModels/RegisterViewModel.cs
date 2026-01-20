using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MuVi.Commands;
// using BLL; // Uncomment khi đã có BLL
// using DTO; // Uncomment khi đã có DTO

namespace MuVi.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        #region Fields
        private string _fullName;
        private string _email;
        private string _username;
        private string _password;
        private string _confirmPassword;
        private bool _acceptTerms;
        private bool _isLoading;
        private string _errorMessage;
        #endregion

        #region Properties

        /// <summary>
        /// Họ và tên đầy đủ
        /// </summary>
        public string FullName
        {
            get => _fullName;
            set
            {
                if (SetProperty(ref _fullName, value))
                {
                    ErrorMessage = string.Empty;
                    (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Địa chỉ Email
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    ErrorMessage = string.Empty;
                    (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    ErrorMessage = string.Empty;
                    (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ErrorMessage = string.Empty;
                    (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Xác nhận mật khẩu
        /// </summary>
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                {
                    ErrorMessage = string.Empty;
                    (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Đồng ý điều khoản
        /// </summary>
        public bool AcceptTerms
        {
            get => _acceptTerms;
            set
            {
                if (SetProperty(ref _acceptTerms, value))
                {
                    (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
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
                    (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

        public ICommand RegisterCommand { get; }

        #endregion

        #region Constructor

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(
                execute: _ => ExecuteRegister(),
                canExecute: _ => CanExecuteRegister()
            );
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Kiểm tra xem có thể thực hiện đăng ký không
        /// </summary>
        private bool CanExecuteRegister()
        {
            return !string.IsNullOrWhiteSpace(FullName)
                   && !string.IsNullOrWhiteSpace(Email)
                   && !string.IsNullOrWhiteSpace(Username)
                   && !string.IsNullOrWhiteSpace(Password)
                   && !string.IsNullOrWhiteSpace(ConfirmPassword)
                   && AcceptTerms
                   && !IsLoading;
        }

        /// <summary>
        /// Thực hiện đăng ký
        /// </summary>
        private async void ExecuteRegister()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Validate input
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

                // TODO: Kết nối với BLL để đăng ký
                // Ví dụ:
                // var userDTO = new UserDTO
                // {
                //     FullName = FullName,
                //     Email = Email,
                //     Username = Username,
                //     Password = Password
                // };
                // var userBLL = new UserBLL();
                // var result = await userBLL.RegisterAsync(userDTO);

                // Simulate API call
                await System.Threading.Tasks.Task.Delay(2000);

                // Mock success - Thay thế bằng logic thực tế từ BLL
                var result = MessageBox.Show(
                    "Đăng ký thành công!\n\nBạn có muốn đăng nhập ngay không?",
                    "Thành công",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information
                );

                if (result == MessageBoxResult.Yes)
                {
                    // TODO: Navigate to Login
                    // var loginWindow = new LoginView();
                    // loginWindow.Show();
                    // CloseCurrentWindow();
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

        #region Validation Methods

        /// <summary>
        /// Validate tất cả input
        /// </summary>
        private bool ValidateInput()
        {
            // Validate Full Name
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Vui lòng nhập họ và tên!";
                return false;
            }

            if (FullName.Length < 3)
            {
                ErrorMessage = "Họ và tên phải có ít nhất 3 ký tự!";
                return false;
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Vui lòng nhập email!";
                return false;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Email không hợp lệ!";
                return false;
            }

            // Validate Username
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Vui lòng nhập tên đăng nhập!";
                return false;
            }

            if (Username.Length < 4)
            {
                ErrorMessage = "Tên đăng nhập phải có ít nhất 4 ký tự!";
                return false;
            }

            if (!IsValidUsername(Username))
            {
                ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới!";
                return false;
            }

            // Validate Password
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

            if (!IsStrongPassword(Password))
            {
                ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ hoa, một chữ thường và một số!";
                return false;
            }

            // Validate Confirm Password
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Vui lòng xác nhận mật khẩu!";
                return false;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp!";
                return false;
            }

            // Validate Terms
            if (!AcceptTerms)
            {
                ErrorMessage = "Bạn phải đồng ý với điều khoản để tiếp tục!";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra email có hợp lệ không
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra username có hợp lệ không
        /// </summary>
        private bool IsValidUsername(string username)
        {
            var regex = new Regex(@"^[a-zA-Z0-9_]+$");
            return regex.IsMatch(username);
        }

        /// <summary>
        /// Kiểm tra mật khẩu có đủ mạnh không
        /// </summary>
        private bool IsStrongPassword(string password)
        {
            // At least one uppercase, one lowercase, and one digit
            var hasUpperCase = new Regex(@"[A-Z]").IsMatch(password);
            var hasLowerCase = new Regex(@"[a-z]").IsMatch(password);
            var hasDigit = new Regex(@"[0-9]").IsMatch(password);

            return hasUpperCase && hasLowerCase && hasDigit;
        }

        #endregion
    }
}