using Microsoft.Win32;
using Muvi.DAL;
using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MuVi.ViewModels.UCViewModel
{
    public class ActorAddViewModel : BaseViewModel
    {
        #region Fields
        private ActorDTO _actor;
        private ActorBLL _actorBLL = new ActorBLL();
        private bool _isAddMode = true;
        private BitmapImage _previewPhoto;
        private string _tempPhotoPath; // Đường dẫn tạm của ảnh đã chọn
        #endregion

        #region Properties

        public ActorDTO Actor
        {
            get => _actor;
            set => SetProperty(ref _actor, value);
        }

        public string ActorName
        {
            get => _actor?.ActorName;
            set
            {
                if (_actor != null)
                {
                    _actor.ActorName = value;
                    OnPropertyChanged(nameof(ActorName));
                }
            }
        }

        public string Bio
        {
            get => _actor?.Bio;
            set
            {
                if (_actor != null)
                {
                    _actor.Bio = value;
                    OnPropertyChanged(nameof(Bio));
                }
            }
        }

        public string PhotoPath
        {
            get => _actor?.PhotoPath;
            set
            {
                if (_actor != null)
                {
                    _actor.PhotoPath = value;
                    OnPropertyChanged(nameof(PhotoPath));
                }
            }
        }

        public DateTime? DateOfBirth
        {
            get => _actor?.DateOfBirth;
            set
            {
                if (_actor != null)
                {
                    _actor.DateOfBirth = value;
                    OnPropertyChanged(nameof(DateOfBirth));
                }
            }
        }

        public string Nationality
        {
            get => _actor?.Nationality;
            set
            {
                if (_actor != null)
                {
                    _actor.Nationality = value;
                    OnPropertyChanged(nameof(Nationality));
                }
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
            set => SetProperty(ref _isAddMode, value);
        }

        public BitmapImage PreviewPhoto
        {
            get => _previewPhoto;
            set => SetProperty(ref _previewPhoto, value);
        }

        #endregion

        #region Commands

        public ICommand UploadPhotoCommand { get; }

        #endregion

        #region Constructor

        public ActorAddViewModel(ActorDTO existingActor = null)
        {
            if (existingActor != null)
            {
                // Edit mode
                _actor = new ActorDTO
                {
                    ActorID = existingActor.ActorID,
                    ActorName = existingActor.ActorName,
                    Bio = existingActor.Bio,
                    PhotoPath = existingActor.PhotoPath,
                    DateOfBirth = existingActor.DateOfBirth,
                    Nationality = existingActor.Nationality
                };
                IsAddMode = false;

                // Load photo nếu có
                if (!string.IsNullOrEmpty(_actor.PhotoPath) && File.Exists(_actor.PhotoPath))
                {
                    LoadPhotoFromPath(_actor.PhotoPath);
                }
                else
                {
                    LoadDefaultPhoto();
                }
            }
            else
            {
                // Add mode
                _actor = new ActorDTO
                {
                    ActorName = "",
                    Nationality = ""
                };
                IsAddMode = true;
                LoadDefaultPhoto();
            }

            UploadPhotoCommand = new RelayCommand(
                execute: _ => ExecuteUploadPhoto(),
                canExecute: _ => true
            );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load ảnh mặc định
        /// </summary>
        private void LoadDefaultPhoto()
        {
            try
            {
                var defaultPhotoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Images", "Avatars", "default-actor.png");

                if (File.Exists(defaultPhotoPath))
                {
                    LoadPhotoFromPath(defaultPhotoPath);
                }
                else
                {
                    PreviewPhoto = null;
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
        private void LoadPhotoFromPath(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
                PreviewPhoto = bitmap;
            }
            catch
            {
                PreviewPhoto = null;
            }
        }

        /// <summary>
        /// Chuyển đổi chuỗi có dấu thành không dấu và loại bỏ ký tự đặc biệt
        /// </summary>
        private string ConvertToSafeFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "file";

            // Loại bỏ dấu tiếng Việt
            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) 
                    != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            string withoutDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            // Thay thế các ký tự đặc biệt và khoảng trắng
            withoutDiacritics = withoutDiacritics.ToLower();
            withoutDiacritics = Regex.Replace(withoutDiacritics, @"[đ]", "d");
            withoutDiacritics = Regex.Replace(withoutDiacritics, @"[Đ]", "D");
            
            // Chỉ giữ lại chữ cái, số và dấu gạch dưới
            withoutDiacritics = Regex.Replace(withoutDiacritics, @"[^a-z0-9_]", "_");
            
            // Loại bỏ các dấu gạch dưới liên tiếp
            withoutDiacritics = Regex.Replace(withoutDiacritics, @"_{2,}", "_");
            
            // Loại bỏ dấu gạch dưới ở đầu và cuối
            withoutDiacritics = withoutDiacritics.Trim('_');

            // Giới hạn độ dài
            if (withoutDiacritics.Length > 50)
            {
                withoutDiacritics = withoutDiacritics.Substring(0, 50).TrimEnd('_');
            }

            return string.IsNullOrWhiteSpace(withoutDiacritics) ? "file" : withoutDiacritics;
        }

        /// <summary>
        /// Upload photo
        /// </summary>
        private void ExecuteUploadPhoto()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif",
                    Title = "Chọn ảnh diễn viên"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Lưu đường dẫn tạm
                    _tempPhotoPath = openFileDialog.FileName;

                    // Preview ảnh
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(_tempPhotoPath);
                    bitmap.EndInit();

                    PreviewPhoto = bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn ảnh: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lưu ảnh vào thư mục Assets/Images/Avatars và trả về đường dẫn
        /// </summary>
        public string SavePhoto()
        {
            try
            {
                if (string.IsNullOrEmpty(_tempPhotoPath))
                    return _actor?.PhotoPath;

                var photoDirectory = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Images", "Avatars");

                if (!Directory.Exists(photoDirectory))
                    Directory.CreateDirectory(photoDirectory);

                string extension = Path.GetExtension(_tempPhotoPath);

                // 👉 Lấy tên file gốc (không path)
                string originalName = Path.GetFileNameWithoutExtension(_tempPhotoPath);

                // 👉 Convert tiếng Việt + ký tự đặc biệt
                string safeName = ConvertToSafeFileName(originalName);

                string fileName =
                    $"actor_{safeName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                string destinationPath = Path.Combine(photoDirectory, fileName);

                File.Copy(_tempPhotoPath, destinationPath, true);

                // Xóa ảnh cũ nếu là edit
                if (!IsAddMode &&
                    !string.IsNullOrEmpty(_actor?.PhotoPath) &&
                    File.Exists(_actor.PhotoPath) &&
                    !_actor.PhotoPath.Contains("default-actor"))
                {
                    try
                    {
                        File.Delete(_actor.PhotoPath);
                    }
                    catch { }
                }

                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi lưu ảnh: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return _actor?.PhotoPath;
            }
        }


        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(ActorName))
            {
                MessageBox.Show("Vui lòng nhập tên diễn viên!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DateOfBirth.HasValue && DateOfBirth > DateTime.Now)
            {
                MessageBox.Show("Ngày sinh không hợp lệ!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}