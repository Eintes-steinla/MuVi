using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using MuVi.Helpers;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MuVi.ViewModels
{
    /// <summary>
    /// ViewModel cho video player
    /// Chức năng: Phát video, tua, tạm dừng, toàn màn hình, lưu tiến độ
    /// </summary>
    public class VideoPlayerViewModel : BaseViewModel
    {
        private readonly ViewHistoryBLL _viewHistoryBLL;
        private readonly MovieBLL _movieBLL;
        private DispatcherTimer _progressTimer;

        #region Properties

        // Phim đang phát
        private MovieDTO _currentMovie;
        public MovieDTO CurrentMovie
        {
            get => _currentMovie;
            set
            {
                _currentMovie = value;
                OnPropertyChanged(nameof(CurrentMovie));
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        // Tập phim đang phát (nếu là phim bộ)
        private EpisodeDTO _currentEpisode;
        public EpisodeDTO CurrentEpisode
        {
            get => _currentEpisode;
            set
            {
                _currentEpisode = value;
                OnPropertyChanged(nameof(CurrentEpisode));
                OnPropertyChanged(nameof(WindowTitle));
                OnPropertyChanged(nameof(VideoSource));
            }
        }

        // Đường dẫn video
        public string VideoSource
        {
            get
            {
                if (CurrentEpisode != null)
                    return CurrentEpisode.VideoPath;
                return CurrentMovie?.VideoPath;
            }
        }

        // Tiêu đề window
        public string WindowTitle
        {
            get
            {
                if (CurrentEpisode != null)
                    return $"{CurrentMovie?.Title} - Tập {CurrentEpisode.EpisodeNumber}";
                return CurrentMovie?.Title;
            }
        }

        // Trạng thái phát
        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
                OnPropertyChanged(nameof(PlayPauseIcon));
            }
        }

        public string PlayPauseIcon => IsPlaying ? "/Assets/Icons/icons8-pause-60.png" : "/Assets/Icons/icons8-play-48.png";

        // Volume
        private double _volume = 0.5;
        public double Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        // Position (giây)
        private double _position;
        public double Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PositionText));
            }
        }

        // Duration (giây)
        private double _duration;
        public double Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged(nameof(Duration));
                OnPropertyChanged(nameof(DurationText));
            }
        }

        // Hiển thị position dạng text
        public string PositionText => TimeSpan.FromSeconds(Position).ToString(@"hh\:mm\:ss");
        public string DurationText => TimeSpan.FromSeconds(Duration).ToString(@"hh\:mm\:ss");

        // Toàn màn hình
        private bool _isFullScreen;
        public bool IsFullScreen
        {
            get => _isFullScreen;
            set
            {
                _isFullScreen = value;
                OnPropertyChanged(nameof(IsFullScreen));
            }
        }

        // Hiển thị controls
        private bool _showControls = true;
        public bool ShowControls
        {
            get => _showControls;
            set
            {
                _showControls = value;
                OnPropertyChanged(nameof(ShowControls));
            }
        }

        #endregion

        #region Commands

        public ICommand PlayPauseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SeekCommand { get; }
        public ICommand ToggleFullScreenCommand { get; }
        public ICommand SkipForwardCommand { get; }
        public ICommand SkipBackwardCommand { get; }
        public ICommand IncreaseVolumeCommand { get; }
        public ICommand DecreaseVolumeCommand { get; }
        public ICommand SaveProgressCommand { get; }

        #endregion

        #region Constructor

        public VideoPlayerViewModel(MovieDTO movie, EpisodeDTO episode = null)
        {
            // Khởi tạo BLL
            _viewHistoryBLL = new ViewHistoryBLL();
            _movieBLL = new MovieBLL();

            // Set movie và episode
            CurrentMovie = movie;
            CurrentEpisode = episode;

            // Khởi tạo commands
            PlayPauseCommand = new RelayCommand(PlayPause);
            StopCommand = new RelayCommand(Stop);
            SeekCommand = new RelayCommand(Seek);
            ToggleFullScreenCommand = new RelayCommand(ToggleFullScreen);
            SkipForwardCommand = new RelayCommand(SkipForward);
            SkipBackwardCommand = new RelayCommand(SkipBackward);
            IncreaseVolumeCommand = new RelayCommand(IncreaseVolume);
            DecreaseVolumeCommand = new RelayCommand(DecreaseVolume);
            SaveProgressCommand = new RelayCommand(SaveProgress);

            // Khởi tạo timer để cập nhật progress
            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromSeconds(10); // Lưu progress mỗi 10 giây
            _progressTimer.Tick += ProgressTimer_Tick;

            // Load tiến độ đã xem trước đó
            LoadLastPosition();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load vị trí xem lần trước
        /// </summary>
        private void LoadLastPosition()
        {
            if (AppSession.Instance.CurrentUser == null) return;

            try
            {
                var history = _viewHistoryBLL.GetViewHistoryByUser(AppSession.Instance.CurrentUser.UserID, out string message);
                if (history != null)
                {
                    ViewHistoryDTO lastView;

                    if (CurrentEpisode != null)
                    {
                        // Tìm lịch sử của episode này
                        lastView = history
                            .Where(h => h.MovieID == CurrentMovie.MovieID && h.EpisodeID == CurrentEpisode.EpisodeID)
                            .OrderByDescending(h => h.WatchedAt)
                            .FirstOrDefault();
                    }
                    else
                    {
                        // Tìm lịch sử của phim lẻ
                        lastView = history
                            .Where(h => h.MovieID == CurrentMovie.MovieID && h.EpisodeID == null)
                            .OrderByDescending(h => h.WatchedAt)
                            .FirstOrDefault();
                    }

                    if (lastView != null && lastView.WatchDuration.HasValue)
                    {
                        Position = lastView.WatchDuration.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading last position: {ex.Message}");
            }
        }

        /// <summary>
        /// Phát/tạm dừng
        /// </summary>
        private void PlayPause(object parameter)
        {
            IsPlaying = !IsPlaying;

            if (IsPlaying)
            {
                _progressTimer.Start();
            }
            else
            {
                _progressTimer.Stop();
                SaveProgress(null);
            }
        }

        /// <summary>
        /// Dừng phát
        /// </summary>
        private void Stop(object parameter)
        {
            IsPlaying = false;
            Position = 0;
            _progressTimer.Stop();
            SaveProgress(null);
        }

        /// <summary>
        /// Tua đến vị trí
        /// </summary>
        private void Seek(object parameter)
        {
            if (parameter is double position)
            {
                Position = position;
            }
        }

        /// <summary>
        /// Tua tới 10 giây
        /// </summary>
        private void SkipForward(object parameter)
        {
            Position = Math.Min(Position + 10, Duration);
        }

        /// <summary>
        /// Tua lùi 10 giây
        /// </summary>
        private void SkipBackward(object parameter)
        {
            Position = Math.Max(Position - 10, 0);
        }

        /// <summary>
        /// Tăng âm lượng
        /// </summary>
        private void IncreaseVolume(object parameter)
        {
            Volume = Math.Min(Volume + 0.1, 1.0);
        }

        /// <summary>
        /// Giảm âm lượng
        /// </summary>
        private void DecreaseVolume(object parameter)
        {
            Volume = Math.Max(Volume - 0.1, 0.0);
        }

        /// <summary>
        /// Toàn màn hình
        /// </summary>
        private void ToggleFullScreen(object parameter)
        {
            IsFullScreen = !IsFullScreen;
        }

        /// <summary>
        /// Lưu tiến độ xem
        /// </summary>
        private void SaveProgress(object parameter)
        {
            if (AppSession.Instance.CurrentUser == null) return;

            try
            {
                // Tìm lịch sử xem hiện tại
                var history = _viewHistoryBLL.GetViewHistoryByUser(AppSession.Instance.CurrentUser.UserID, out string message);
                ViewHistoryDTO currentView = null;

                if (history != null)
                {
                    if (CurrentEpisode != null)
                    {
                        currentView = history
                            .Where(h => h.MovieID == CurrentMovie.MovieID && h.EpisodeID == CurrentEpisode.EpisodeID)
                            .OrderByDescending(h => h.WatchedAt)
                            .FirstOrDefault();
                    }
                    else
                    {
                        currentView = history
                            .Where(h => h.MovieID == CurrentMovie.MovieID && h.EpisodeID == null)
                            .OrderByDescending(h => h.WatchedAt)
                            .FirstOrDefault();
                    }
                }

                if (currentView != null)
                {
                    // Update progress
                    currentView.WatchDuration = (int)Position;
                    currentView.WatchedAt = DateTime.Now;
                    _viewHistoryBLL.AddOrUpdateHistory(currentView, out string msg);
                }
                else
                {
                    // Create new history
                    var newHistory = new ViewHistoryDTO
                    {
                        UserID = AppSession.Instance.CurrentUser.UserID,
                        MovieID = CurrentMovie.MovieID,
                        EpisodeID = CurrentEpisode?.EpisodeID,
                        WatchDuration = (int)Position,
                        WatchedAt = DateTime.Now
                    };
                    _viewHistoryBLL.AddViewHistory(newHistory, out string msg);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving progress: {ex.Message}");
            }
        }

        /// <summary>
        /// Timer tick - tự động lưu progress
        /// </summary>
        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            SaveProgress(null);
        }

        /// <summary>
        /// Cleanup khi đóng
        /// </summary>
        public void Cleanup()
        {
            _progressTimer?.Stop();
            SaveProgress(null);
        }

        #endregion
    }
}