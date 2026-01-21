using System;
using System.Windows;
using System.Windows.Input;
using MuVi.Commands;
using MuVi.BLL;

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
                // 1. Bắt đầu trạng thái chờ và xóa lỗi cũ
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

                // Giả lập một khoảng trễ nhỏ để người dùng thấy hiệu ứng Loading (tùy chọn)
                await System.Threading.Tasks.Task.Delay(500);

                // 2. Khởi tạo lớp nghiệp vụ
                var userBLL = new UserBLL();

                // 3. Gọi hàm Login từ BLL (Sử dụng Task.Run để không làm treo UI khi truy vấn DB)
                // Chúng ta dùng biến 'message' làm tham số 'out' để nhận thông báo từ BLL
                //var user = await System.Threading.Tasks.Task.Run(() =>
                //    userBLL.Login(UsernameOrEmail, Password, out string message)
                //);
                var user = userBLL.Login(UsernameOrEmail, Password, out string message);

                // 4. Kiểm tra kết quả
                if (user != null)
                {
                    // ĐĂNG NHẬP THÀNH CÔNG
                    MessageBox.Show(
                        message,
                        "Thông báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    // TODO: Lưu thông tin User vào Session hoặc Global Variable nếu cần
                    // Ví dụ: App.CurrentUser = user;

                    // TODO: Điều hướng sang màn hình chính
                    // var mainWindow = new MainWindow();
                    // mainWindow.Show();
                    // CloseCurrentWindow(); // Bạn cần viết thêm hàm này để đóng cửa sổ Login
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
                // Xử lý lỗi hệ thống (mất kết nối DB, v.v.)
                ErrorMessage = "Lỗi hệ thống: " + ex.Message;
            }
            finally
            {
                // 5. Kết thúc trạng thái chờ (Luôn chạy dù thành công hay thất bại)
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