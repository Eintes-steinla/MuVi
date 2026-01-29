using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MuVi.Views.AddView;

namespace MuVi.ViewModels.UCViewModel
{
    public class MovieAddViewModel : BaseViewModel
    {
        #region Fields
        private MovieDTO _movie;
        private MovieBLL _movieBLL = new MovieBLL();
        private bool _isAddMode = true;
        private BitmapImage _previewPoster;
        private string _tempPosterPath; // Đường dẫn tạm của ảnh đã chọn
        private string _tempVideoPath; // Đường dẫn tạm của video đã chọn
        #endregion

        #region Properties

        public MovieDTO Movie
        {
            get => _movie;
            set => SetProperty(ref _movie, value);
        }

        public ObservableCollection<CountryDTO> CountryList { get; set; }
        public ObservableCollection<GenreDTO> GenreList { get; set; }
        public ObservableCollection<GenreDTO> SelectedGenres { get; set; }

        public string Title
        {
            get => _movie?.Title;
            set
            {
                if (_movie != null)
                {
                    _movie.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Description
        {
            get => _movie?.Description;
            set
            {
                if (_movie != null)
                {
                    _movie.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string PosterPath
        {
            get => _movie?.PosterPath;
            set
            {
                if (_movie != null)
                {
                    _movie.PosterPath = value;
                    OnPropertyChanged(nameof(PosterPath));
                }
            }
        }

        public string TrailerURL
        {
            get => _movie?.TrailerURL;
            set
            {
                if (_movie != null)
                {
                    _movie.TrailerURL = value;
                    OnPropertyChanged(nameof(TrailerURL));
                }
            }
        }

        public string VideoPath
        {
            get => _movie?.VideoPath;
            set
            {
                if (_movie != null)
                {
                    _movie.VideoPath = value;
                    OnPropertyChanged(nameof(VideoPath));
                    OnPropertyChanged(nameof(VideoFileName));
                }
            }
        }

        // Hiển thị tên file video (không hiển thị đường dẫn dài)
        public string VideoFileName
        {
            get
            {
                if (string.IsNullOrEmpty(VideoPath))
                    return "Chưa chọn video";

                // Nếu là URL (http/https)
                if (VideoPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    VideoPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return "Video từ cloud";
                }

                // Nếu là file local
                return Path.GetFileName(VideoPath);
            }
        }

        public int? ReleaseYear
        {
            get => _movie?.ReleaseYear;
            set
            {
                if (_movie != null)
                {
                    _movie.ReleaseYear = value;
                    OnPropertyChanged(nameof(ReleaseYear));
                }
            }
        }

        public int? TotalEpisodes
        {
            get => _movie?.TotalEpisodes;
            set
            {
                if (_movie != null)
                {
                    _movie.TotalEpisodes = value;
                    OnPropertyChanged(nameof(TotalEpisodes));
                }
            }
        }

        public int? Duration
        {
            get => _movie?.Duration;
            set
            {
                if (_movie != null)
                {
                    _movie.Duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        public string Director
        {
            get => _movie?.Director;
            set
            {
                if (_movie != null)
                {
                    _movie.Director = value;
                    OnPropertyChanged(nameof(Director));
                }
            }
        }

        public decimal? Rating
        {
            get => _movie?.Rating;
            set
            {
                if (_movie != null)
                {
                    _movie.Rating = value;
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }

        public int? ViewCount
        {
            get => _movie?.ViewCount;
            set
            {
                if (_movie != null)
                {
                    _movie.ViewCount = value;
                    OnPropertyChanged(nameof(ViewCount));
                }
            }
        }

        private CountryDTO? _selectedCountry;
        public CountryDTO? SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
                if (_movie != null && value != null)
                {
                    _movie.CountryID = value.CountryID;
                }
            }
        }

        public string Status
        {
            get => _movie?.Status ?? "Đang chiếu";
            set
            {
                if (_movie != null)
                {
                    _movie.Status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public string MovieType
        {
            get => _movie?.MovieType ?? "Phim lẻ";
            set
            {
                if (_movie != null)
                {
                    _movie.MovieType = value;
                    OnPropertyChanged(nameof(MovieType));
                }
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
            set => SetProperty(ref _isAddMode, value);
        }

        public BitmapImage PreviewPoster
        {
            get => _previewPoster;
            set => SetProperty(ref _previewPoster, value);
        }

        #endregion

        #region Commands

        public ICommand UploadPosterCommand { get; }
        public ICommand UploadVideoCommand { get; }
        public ICommand ClearVideoCommand { get; }

        #endregion

        #region Constructor

        public MovieAddViewModel(MovieDTO existingMovie = null)
        {
            CountryList = new ObservableCollection<CountryDTO>();
            GenreList = new ObservableCollection<GenreDTO>();
            SelectedGenres = new ObservableCollection<GenreDTO>();

            LoadCountries();
            LoadGenres();

            if (existingMovie != null)
            {
                // Edit mode
                _movie = new MovieDTO
                {
                    MovieID = existingMovie.MovieID,
                    Title = existingMovie.Title,
                    Description = existingMovie.Description,
                    PosterPath = existingMovie.PosterPath,
                    TrailerURL = existingMovie.TrailerURL,
                    VideoPath = existingMovie.VideoPath,
                    ReleaseYear = existingMovie.ReleaseYear,
                    TotalEpisodes = existingMovie.TotalEpisodes,
                    Duration = existingMovie.Duration,
                    Director = existingMovie.Director,
                    Rating = existingMovie.Rating,
                    ViewCount = existingMovie.ViewCount,
                    CountryID = existingMovie.CountryID,
                    Status = existingMovie.Status,
                    MovieType = existingMovie.MovieType
                };
                IsAddMode = false;

                // Set selected country
                if (_movie.CountryID.HasValue)
                {
                    SelectedCountry = CountryList.FirstOrDefault(c => c.CountryID == _movie.CountryID.Value);
                }

                // Load genres của phim (nếu đang edit)
                LoadMovieGenres(existingMovie.MovieID);

                // Load poster nếu có
                if (!string.IsNullOrEmpty(_movie.PosterPath) && File.Exists(_movie.PosterPath))
                {
                    LoadPosterFromPath(_movie.PosterPath);
                }
                else
                {
                    LoadDefaultPoster();
                }
            }
            else
            {
                // Add mode
                _movie = new MovieDTO
                {
                    Status = "Đang chiếu",
                    MovieType = "Phim lẻ",
                    ViewCount = 0,
                    Rating = 0
                };
                IsAddMode = true;
                LoadDefaultPoster();
            }

            UploadPosterCommand = new RelayCommand(
                execute: _ => ExecuteUploadPoster(),
                canExecute: _ => true
            );

            UploadVideoCommand = new RelayCommand(
                execute: _ => ExecuteUploadVideo(),
                canExecute: _ => true
            );

            ClearVideoCommand = new RelayCommand(
                execute: _ => ExecuteClearVideo(),
                canExecute: _ => !string.IsNullOrEmpty(VideoPath)
            );
        }

        #endregion

        #region Methods

        private void LoadGenres()
        {
            var genres = _movieBLL.GetAllGenres();
            GenreList.Clear();
            foreach (var g in genres)
            {
                GenreList.Add(g);
            }
        }

        private void LoadMovieGenres(int movieId)
        {
            var movie = _movieBLL.GetMovieById(movieId);
            if (movie?.Genres != null)
            {
                SelectedGenres.Clear();
                foreach (var genre in movie.Genres)
                {
                    SelectedGenres.Add(genre);
                }
            }
        }

        public List<int> GetSelectedGenreIds()
        {
            return SelectedGenres.Select(g => g.GenreID).ToList();
        }

        private void LoadCountries()
        {
            var countries = _movieBLL.GetAllCountries();
            CountryList.Clear();
            foreach (var c in countries)
            {
                CountryList.Add(c);
            }
        }

        /// <summary>
        /// Load ảnh mặc định
        /// </summary>
        private void LoadDefaultPoster()
        {
            try
            {
                var defaultPosterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Images", "Posters", "default-poster.png");

                if (File.Exists(defaultPosterPath))
                {
                    LoadPosterFromPath(defaultPosterPath);
                }
                else
                {
                    PreviewPoster = null;
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
        private void LoadPosterFromPath(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();
                PreviewPoster = bitmap;
            }
            catch
            {
                PreviewPoster = null;
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
        /// Validate đường dẫn file không chứa tiếng Việt hoặc ký tự đặc biệt
        /// </summary>
        private bool ValidateFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            try
            {
                string fileName = Path.GetFileName(filePath);
                string directory = Path.GetDirectoryName(filePath);

                // Kiểm tra tên file
                if (Regex.IsMatch(fileName, @"[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]", RegexOptions.IgnoreCase))
                {
                    MessageBox.Show("Tên file chứa ký tự tiếng Việt có dấu. Vui lòng chọn file khác hoặc đổi tên file.",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Kiểm tra đường dẫn
                if (Regex.IsMatch(directory, @"[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ]", RegexOptions.IgnoreCase))
                {
                    MessageBox.Show("Đường dẫn file chứa ký tự tiếng Việt có dấu. Vui lòng di chuyển file đến thư mục khác.",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Upload poster
        /// </summary>
        private void ExecuteUploadPoster()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif",
                    Title = "Chọn ảnh poster"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Validate đường dẫn
                    if (!ValidateFilePath(openFileDialog.FileName))
                    {
                        return;
                    }

                    // Lưu đường dẫn tạm
                    _tempPosterPath = openFileDialog.FileName;

                    // Preview ảnh
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(_tempPosterPath);
                    bitmap.EndInit();

                    PreviewPoster = bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn ảnh: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Upload video hoặc nhập URL
        /// </summary>
        private void ExecuteUploadVideo()
        {
            try
            {
                // Hiển thị dialog cho user chọn: Upload file hoặc nhập URL
                var choice = MessageBox.Show(
                    "Chọn 'Yes' để tải video từ máy tính\nChọn 'No' để nhập đường dẫn video từ cloud",
                    "Chọn nguồn video",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (choice == MessageBoxResult.Yes)
                {
                    // Upload file video từ máy
                    var openFileDialog = new OpenFileDialog
                    {
                        Filter = "Video files (*.mp4, *.avi, *.mkv, *.mov, *.wmv)|*.mp4;*.avi;*.mkv;*.mov;*.wmv|All files (*.*)|*.*",
                        Title = "Chọn file video"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        // Validate đường dẫn
                        if (!ValidateFilePath(openFileDialog.FileName))
                        {
                            return;
                        }

                        _tempVideoPath = openFileDialog.FileName;
                        VideoPath = _tempVideoPath; // Tạm thời hiển thị đường dẫn
                    }
                }
                else if (choice == MessageBoxResult.No)
                {
                    // Nhập URL từ cloud
                    var inputWindow = new VideoUrlInputWindow();
                    if (inputWindow.ShowDialog() == true)
                    {
                        VideoPath = inputWindow.VideoUrl;
                        _tempVideoPath = null; // Không cần lưu file
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn video: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xóa video đã chọn
        /// </summary>
        private void ExecuteClearVideo()
        {
            VideoPath = null;
            _tempVideoPath = null;
        }

        /// <summary>
        /// Lưu ảnh vào thư mục Assets/Images/Posters và trả về đường dẫn
        /// </summary>
        public string SavePoster()
        {
            try
            {
                // Nếu không có ảnh tạm (không chọn ảnh mới)
                if (string.IsNullOrEmpty(_tempPosterPath))
                {
                    return _movie?.PosterPath; // Trả về poster cũ
                }

                // Tạo thư mục Posters nếu chưa có
                var posterDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Images", "Posters");

                if (!Directory.Exists(posterDirectory))
                {
                    Directory.CreateDirectory(posterDirectory);
                }

                // Tạo tên file unique và an toàn
                var fileExtension = Path.GetExtension(_tempPosterPath);
                var safeTitle = ConvertToSafeFileName(_movie?.Title ?? "movie");
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileName = $"poster_{safeTitle}_{timestamp}{fileExtension}";
                var destinationPath = Path.Combine(posterDirectory, fileName);

                // Copy file
                File.Copy(_tempPosterPath, destinationPath, true);

                // Xóa ảnh cũ nếu có (chế độ edit và không phải default poster)
                if (!IsAddMode && !string.IsNullOrEmpty(_movie?.PosterPath) &&
                    File.Exists(_movie.PosterPath) &&
                    !_movie.PosterPath.Contains("default-poster"))
                {
                    try
                    {
                        File.Delete(_movie.PosterPath);
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
                return _movie?.PosterPath;
            }
        }

        /// <summary>
        /// Lưu video vào thư mục Assets/Videos và trả về đường dẫn
        /// </summary>
        public string SaveVideo()
        {
            try
            {
                // Nếu không có video
                if (string.IsNullOrEmpty(VideoPath))
                {
                    return null;
                }

                // Nếu là URL (cloud), trả về luôn
                if (VideoPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    VideoPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return VideoPath;
                }

                // Nếu không chọn video mới (edit mode và giữ nguyên video cũ)
                if (string.IsNullOrEmpty(_tempVideoPath))
                {
                    return _movie?.VideoPath;
                }

                // Tạo thư mục Videos nếu chưa có
                var videoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Videos");

                if (!Directory.Exists(videoDirectory))
                {
                    Directory.CreateDirectory(videoDirectory);
                }

                // Tạo tên file unique và an toàn
                var fileExtension = Path.GetExtension(_tempVideoPath);
                var safeTitle = ConvertToSafeFileName(_movie?.Title ?? "movie");
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileName = $"video_{safeTitle}_{timestamp}{fileExtension}";
                var destinationPath = Path.Combine(videoDirectory, fileName);

                // Copy file (có thể mất thời gian nếu file lớn)
                File.Copy(_tempVideoPath, destinationPath, true);

                // Xóa video cũ nếu có
                if (!IsAddMode && !string.IsNullOrEmpty(_movie?.VideoPath) &&
                    File.Exists(_movie.VideoPath))
                {
                    try
                    {
                        File.Delete(_movie.VideoPath);
                    }
                    catch
                    {
                        // Ignore
                    }
                }

                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu video: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return _movie?.VideoPath;
            }
        }

        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề phim!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ReleaseYear.HasValue && (ReleaseYear < 1800 || ReleaseYear > DateTime.Now.Year + 5))
            {
                MessageBox.Show("Năm phát hành không hợp lệ!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Rating.HasValue && (Rating < 0 || Rating > 10))
            {
                MessageBox.Show("Đánh giá phải từ 0 đến 10!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Duration.HasValue && Duration < 0)
            {
                MessageBox.Show("Thời lượng không hợp lệ!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (TotalEpisodes.HasValue && TotalEpisodes < 0)
            {
                MessageBox.Show("Số tập không hợp lệ!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}