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
    public class EpisodeAddViewModel : BaseViewModel
    {
        #region Fields
        private EpisodeDTO _episode;
        private EpisodeBLL _episodeBLL = new EpisodeBLL();
        private bool _isAddMode = true;
        private BitmapImage _previewPoster;
        private string _tempPosterPath;
        private string _tempVideoPath;
        #endregion

        #region Properties

        public EpisodeDTO Episode
        {
            get => _episode;
            set => SetProperty(ref _episode, value);
        }

        public ObservableCollection<MovieDTO> MovieList { get; set; }

        public string Title
        {
            get => _episode?.Title;
            set
            {
                if (_episode != null)
                {
                    _episode.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Description
        {
            get => _episode?.Description;
            set
            {
                if (_episode != null)
                {
                    _episode.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string PosterPath
        {
            get => _episode?.PosterPath;
            set
            {
                if (_episode != null)
                {
                    _episode.PosterPath = value;
                    OnPropertyChanged(nameof(PosterPath));
                }
            }
        }

        public string VideoPath
        {
            get => _episode?.VideoPath;
            set
            {
                if (_episode != null)
                {
                    _episode.VideoPath = value;
                    OnPropertyChanged(nameof(VideoPath));
                    OnPropertyChanged(nameof(VideoFileName));
                }
            }
        }

        public string VideoFileName
        {
            get
            {
                if (string.IsNullOrEmpty(VideoPath))
                    return "Chưa chọn video";

                if (VideoPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    VideoPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return "Video từ cloud";
                }

                return Path.GetFileName(VideoPath);
            }
        }

        public int? Duration
        {
            get => _episode?.Duration;
            set
            {
                if (_episode != null)
                {
                    _episode.Duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        public int EpisodeNumber
        {
            get => _episode?.EpisodeNumber ?? 0;
            set
            {
                if (_episode != null)
                {
                    _episode.EpisodeNumber = value;
                    OnPropertyChanged(nameof(EpisodeNumber));
                }
            }
        }

        public DateTime? ReleaseDate
        {
            get => _episode?.ReleaseDate;
            set
            {
                if (_episode != null)
                {
                    _episode.ReleaseDate = value;
                    OnPropertyChanged(nameof(ReleaseDate));
                }
            }
        }

        public int? ViewCount
        {
            get => _episode?.ViewCount;
            set
            {
                if (_episode != null)
                {
                    _episode.ViewCount = value;
                    OnPropertyChanged(nameof(ViewCount));
                }
            }
        }

        private MovieDTO? _selectedMovie;
        public MovieDTO? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged(nameof(SelectedMovie));
                if (_episode != null && value != null)
                {
                    _episode.MovieID = value.MovieID;
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

        public EpisodeAddViewModel(EpisodeDTO existingEpisode = null)
        {
            MovieList = new ObservableCollection<MovieDTO>();
            LoadMovies();

            if (existingEpisode != null)
            {
                // Edit mode
                _episode = new EpisodeDTO
                {
                    EpisodeID = existingEpisode.EpisodeID,
                    MovieID = existingEpisode.MovieID,
                    EpisodeNumber = existingEpisode.EpisodeNumber,
                    Title = existingEpisode.Title,
                    Description = existingEpisode.Description,
                    Duration = existingEpisode.Duration,
                    PosterPath = existingEpisode.PosterPath,
                    VideoPath = existingEpisode.VideoPath,
                    ReleaseDate = existingEpisode.ReleaseDate,
                    ViewCount = existingEpisode.ViewCount
                };
                IsAddMode = false;

                // Set selected movie
                SelectedMovie = MovieList.FirstOrDefault(m => m.MovieID == _episode.MovieID);

                // Load poster if exists
                if (!string.IsNullOrEmpty(_episode.PosterPath) && File.Exists(_episode.PosterPath))
                {
                    LoadPosterFromPath(_episode.PosterPath);
                }
                else
                {
                    LoadDefaultPoster();
                }
            }
            else
            {
                // Add mode
                _episode = new EpisodeDTO
                {
                    ViewCount = 0,
                    EpisodeNumber = 1,
                    ReleaseDate = DateTime.Now
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

        private void LoadMovies()
        {
            var movies = _episodeBLL.GetSeriesMovies();
            MovieList.Clear();
            foreach (var m in movies)
            {
                MovieList.Add(m);
            }
        }

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
                    _tempPosterPath = openFileDialog.FileName;

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

        private void ExecuteUploadVideo()
        {
            try
            {
                var choice = MessageBox.Show(
                    "Chọn 'Yes' để tải video từ máy tính\nChọn 'No' để nhập đường dẫn video từ cloud",
                    "Chọn nguồn video",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (choice == MessageBoxResult.Yes)
                {
                    var openFileDialog = new OpenFileDialog
                    {
                        Filter = "Video files (*.mp4, *.avi, *.mkv, *.mov, *.wmv)|*.mp4;*.avi;*.mkv;*.mov;*.wmv|All files (*.*)|*.*",
                        Title = "Chọn file video"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        _tempVideoPath = openFileDialog.FileName;
                        VideoPath = _tempVideoPath;
                    }
                }
                else if (choice == MessageBoxResult.No)
                {
                    var inputWindow = new VideoUrlInputWindow();
                    if (inputWindow.ShowDialog() == true)
                    {
                        VideoPath = inputWindow.VideoUrl;
                        _tempVideoPath = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn video: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteClearVideo()
        {
            VideoPath = null;
            _tempVideoPath = null;
        }

        public string SavePoster()
        {
            try
            {
                if (string.IsNullOrEmpty(_tempPosterPath))
                {
                    return _episode?.PosterPath;
                }

                var posterDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Images", "Posters");

                if (!Directory.Exists(posterDirectory))
                {
                    Directory.CreateDirectory(posterDirectory);
                }

                var fileExtension = Path.GetExtension(_tempPosterPath);
                var safeTitle = ConvertToSafeFileName(_episode?.Title ?? "episode");
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileName = $"poster_ep{_episode?.EpisodeNumber}_{safeTitle}_{timestamp}{fileExtension}";
                var destinationPath = Path.Combine(posterDirectory, fileName);

                File.Copy(_tempPosterPath, destinationPath, true);

                if (!IsAddMode && !string.IsNullOrEmpty(_episode?.PosterPath) &&
                    File.Exists(_episode.PosterPath) &&
                    !_episode.PosterPath.Contains("default-poster"))
                {
                    try
                    {
                        File.Delete(_episode.PosterPath);
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
                MessageBox.Show($"Lỗi khi lưu ảnh: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return _episode?.PosterPath;
            }
        }

        public string SaveVideo()
        {
            try
            {
                if (string.IsNullOrEmpty(VideoPath))
                {
                    return null;
                }

                if (VideoPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    VideoPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return VideoPath;
                }

                if (string.IsNullOrEmpty(_tempVideoPath))
                {
                    return _episode?.VideoPath;
                }

                var videoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "Videos");

                if (!Directory.Exists(videoDirectory))
                {
                    Directory.CreateDirectory(videoDirectory);
                }

                var fileExtension = Path.GetExtension(_tempVideoPath);
                var safeTitle = ConvertToSafeFileName(_episode?.Title ?? "episode");
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileName = $"video_ep{_episode?.EpisodeNumber}_{safeTitle}_{timestamp}{fileExtension}";
                var destinationPath = Path.Combine(videoDirectory, fileName);

                File.Copy(_tempVideoPath, destinationPath, true);

                if (!IsAddMode && !string.IsNullOrEmpty(_episode?.VideoPath) &&
                    File.Exists(_episode.VideoPath))
                {
                    try
                    {
                        File.Delete(_episode.VideoPath);
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
                return _episode?.VideoPath;
            }
        }

        public bool Validate()
        {
            if (SelectedMovie == null)
            {
                MessageBox.Show("Vui lòng chọn phim!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (EpisodeNumber <= 0)
            {
                MessageBox.Show("Số tập phải lớn hơn 0!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề tập phim!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Duration.HasValue && Duration < 0)
            {
                MessageBox.Show("Thời lượng không hợp lệ!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}