using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
        #endregion

        #region Properties

        public MovieDTO Movie
        {
            get => _movie;
            set => SetProperty(ref _movie, value);
        }

        public ObservableCollection<CountryDTO> CountryList { get; set; }

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
                }
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

        #endregion

        #region Constructor

        public MovieAddViewModel(MovieDTO existingMovie = null)
        {
            CountryList = new ObservableCollection<CountryDTO>();
            LoadCountries();

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
        }

        #endregion

        #region Methods

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
                    "Assets", "Posters", "default-poster.png");

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
        /// Lưu ảnh vào thư mục Assets/Posters và trả về đường dẫn
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
                    "Assets", "Posters");

                if (!Directory.Exists(posterDirectory))
                {
                    Directory.CreateDirectory(posterDirectory);
                }

                // Tạo tên file unique (title_timestamp.extension)
                var fileExtension = Path.GetExtension(_tempPosterPath);
                var safeTitle = _movie?.Title?.ToLower().Replace(" ", "_") ?? "movie";
                // Giới hạn độ dài tên file
                if (safeTitle.Length > 50)
                {
                    safeTitle = safeTitle.Substring(0, 50);
                }
                var fileName = $"{safeTitle}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
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