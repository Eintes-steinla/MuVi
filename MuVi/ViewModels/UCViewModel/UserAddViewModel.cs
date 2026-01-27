using MuVi.Commands;
using MuVi.DTO.DTOs;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MuVi.ViewModels.UCViewModel
{
    public class UserAddViewModel : BaseViewModel
    {
        #region Fields
        private UserDTO _user;
        private string _password;
        private bool _isAddMode = true;
        private BitmapImage _previewAvatar;
        private string _tempAvatarPath; // Đường dẫn tạm của ảnh đã chọn
        #endregion

        #region Properties

        public UserDTO User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public string Username
        {
            get => _user?.Username;
            set
            {
                if (_user != null)
                {
                    _user.Username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string Email
        {
            get => _user?.Email;
            set
            {
                if (_user != null)
                {
                    _user.Email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public DateTime? DateOfBirth
        {
            get => _user?.DateOfBirth;
            set
            {
                if (_user != null)
                {
                    _user.DateOfBirth = value;
                    OnPropertyChanged(nameof(DateOfBirth));
                }
            }
        }

        public string Avatar
        {
            get => _user?.Avatar;
            set
            {
                if (_user != null)
                {
                    _user.Avatar = value;
                    OnPropertyChanged(nameof(Avatar));
                }
            }
        }

        public string Role
        {
            get => _user?.Role ?? "User";
            set
            {
                if (_user != null)
                {
                    _user.Role = value;
                    OnPropertyChanged(nameof(Role));
                }
            }
        }

        public bool IsActive
        {
            get => _user?.IsActive ?? true;
            set
            {
                if (_user != null)
                {
                    _user.IsActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
            set => SetProperty(ref _isAddMode, value);
        }

        public BitmapImage PreviewAvatar
        {
            get => _previewAvatar;
            set => SetProperty(ref _previewAvatar, value);
        }

        #endregion

        #region Commands

        public ICommand UploadAvatarCommand { get; }

        #endregion

        #region Constructor

        public UserAddViewModel(UserDTO existingUser = null)
        {
            if (existingUser != null)
            {
                // Edit mode
                _user = new UserDTO
                {
                    UserID = existingUser.UserID,
                    Username = existingUser.Username,
                    Email = existingUser.Email,
                    Password = existingUser.Password,
                    DateOfBirth = existingUser.DateOfBirth,
                    Avatar = existingUser.Avatar,
                    Role = existingUser.Role,
                    IsActive = existingUser.IsActive
                };
                IsAddMode = false;

                // Load avatar nếu có
                if (!string.IsNullOrEmpty(_user.Avatar) && File.Exists(_user.Avatar))
                {
                    LoadAvatarFromPath(_user.Avatar);
                }
                else
                {
                    LoadDefaultAvatar();
                }
            }
            else
            {
                // Add mode
                _user = new UserDTO
                {
                    Role = "User",
                    IsActive = true
                };
                IsAddMode = true;
                LoadDefaultAvatar();
            }

            UploadAvatarCommand = new RelayCommand(
                execute: _ => ExecuteUploadAvatar(),
                canExecute: _ => true
            );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load ảnh mặc định
        /// </summary>
        private void LoadDefaultAvatar()
        {
            try
            {
                var defaultAvatarPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Avatars", "default-avatar.png");

                if (File.Exists(defaultAvatarPath))
                {
                    LoadAvatarFromPath(defaultAvatarPath);
                }
                else
                {
                    PreviewAvatar = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải ảnh mặc định: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load ảnh từ đường dẫn
        /// </summary>
        private void LoadAvatarFromPath(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
                PreviewAvatar = bitmap;
            }
            catch
            {
                PreviewAvatar = null;
            }
        }

        /// <summary>
        /// Upload avatar
        /// </summary>
        private void ExecuteUploadAvatar()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif",
                    Title = "Chọn ảnh đại diện"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Lưu đường dẫn tạm
                    _tempAvatarPath = openFileDialog.FileName;

                    // Preview ảnh
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(_tempAvatarPath);
                    bitmap.EndInit();

                    PreviewAvatar = bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn ảnh: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lưu ảnh vào thư mục Assets/Avatars và trả về đường dẫn
        /// </summary>
        public string SaveAvatar()
        {
            try
            {
                // Nếu không có ảnh tạm (không chọn ảnh mới)
                if (string.IsNullOrEmpty(_tempAvatarPath))
                {
                    return _user?.Avatar; // Trả về avatar cũ
                }

                // Tạo thư mục Avatars nếu chưa có
                var avatarDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Avatars");

                if (!Directory.Exists(avatarDirectory))
                {
                    Directory.CreateDirectory(avatarDirectory);
                }

                // Tạo tên file unique (username_timestamp.extension)
                var fileExtension = Path.GetExtension(_tempAvatarPath);
                var safeUsername = _user?.Username?.ToLower().Replace(" ", "_") ?? "user";
                var fileName = $"{safeUsername}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                var destinationPath = Path.Combine(avatarDirectory, fileName);

                // Copy file
                File.Copy(_tempAvatarPath, destinationPath, true);

                // Xóa ảnh cũ nếu có (chế độ edit và không phải default avatar)
                if (!IsAddMode && !string.IsNullOrEmpty(_user?.Avatar) &&
                    File.Exists(_user.Avatar) &&
                    !_user.Avatar.Contains("default-avatar"))
                {
                    try
                    {
                        File.Delete(_user.Avatar);
                    }
                    catch
                    {
                        // Ignore nếu không xóa được
                    }
                }

                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu ảnh: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return _user?.Avatar;
            }
        }

        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Vui lòng nhập email!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate email format
            if (!IsValidEmail(Email))
            {
                MessageBox.Show("Email không hợp lệ!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate password (chỉ khi Add)
            if (IsAddMode && string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (IsAddMode && Password.Length < 8)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 8 ký tự!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra email hợp lệ
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}