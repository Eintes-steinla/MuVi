using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using MuVi.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace MuVi.ViewModels
{
    /// <summary>
    /// ViewModel cho lịch sử xem phim của người dùng
    /// </summary>
    public class UserWatchHistoryViewModel : BaseViewModel
    {
        private readonly ViewHistoryBLL _viewHistoryBLL;
        private readonly MovieBLL _movieBLL;
        private readonly EpisodeBLL _episodeBLL;

        #region Properties

        // Danh sách lịch sử xem
        private ObservableCollection<WatchHistoryItemViewModel> _historyItems;
        public ObservableCollection<WatchHistoryItemViewModel> HistoryItems
        {
            get => _historyItems;
            set { _historyItems = value; OnPropertyChanged(nameof(HistoryItems)); }
        }

        // Loading
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        #endregion

        #region Commands

        public ICommand LoadDataCommand { get; }
        public ICommand ContinueWatchingCommand { get; }
        public ICommand ViewMovieDetailCommand { get; }
        public ICommand ClearHistoryCommand { get; }

        #endregion

        #region Constructor

        public UserWatchHistoryViewModel()
        {
            // Khởi tạo BLL
            _viewHistoryBLL = new ViewHistoryBLL();
            _movieBLL = new MovieBLL();
            _episodeBLL = new EpisodeBLL();

            // Khởi tạo collections
            HistoryItems = new ObservableCollection<WatchHistoryItemViewModel>();

            // Khởi tạo commands
            LoadDataCommand = new RelayCommand(LoadData);
            ContinueWatchingCommand = new RelayCommand(ContinueWatching);
            ViewMovieDetailCommand = new RelayCommand(ViewMovieDetail);
            ClearHistoryCommand = new RelayCommand(ClearHistory);

            // Load data
            LoadData(null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load lịch sử xem
        /// </summary>
        private void LoadData(object parameter)
        {
            if (AppSession.Instance.CurrentUser == null)
            {
                System.Windows.MessageBox.Show("Vui lòng đăng nhập để xem lịch sử!",
                    "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            IsLoading = true;

            try
            {
                // Lấy lịch sử của user
                var history = _viewHistoryBLL.GetViewHistoryByUser(AppSession.Instance.CurrentUser.UserID, out string message);

                if (history != null && history.Count > 0)
                {
                    HistoryItems.Clear();

                    // Group by Movie và lấy record mới nhất
                    var groupedHistory = history
                        .GroupBy(h => h.MovieID)
                        .Select(g => g.OrderByDescending(h => h.WatchedAt).First())
                        .OrderByDescending(h => h.WatchedAt)
                        .ToList();

                    foreach (var viewHistory in groupedHistory)
                    {
                        var movie = _movieBLL.GetMovieById(viewHistory.MovieID);
                        if (movie != null)
                        {
                            var item = new WatchHistoryItemViewModel
                            {
                                ViewHistory = viewHistory,
                                Movie = movie
                            };

                            // Nếu là phim bộ, load episode info
                            if (viewHistory.EpisodeID.HasValue)
                            {
                                var episode = _episodeBLL.GetEpisodeById(viewHistory.EpisodeID.Value);
                                item.Episode = episode;
                            }

                            HistoryItems.Add(item);
                        }
                    }
                }
                else
                {
                    HistoryItems.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải lịch sử: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Xem tiếp
        /// </summary>
        private void ContinueWatching(object parameter)
        {
            if (parameter is WatchHistoryItemViewModel item)
            {
                var playerWindow = new Views.VideoPlayerView(item.Movie, item.Episode);
                playerWindow.Show();
            }
        }

        /// <summary>
        /// Xem chi tiết phim
        /// </summary>
        private void ViewMovieDetail(object parameter)
        {
            if (parameter is WatchHistoryItemViewModel item)
            {
                var detailWindow = new Views.UserMovieDetailView(item.Movie);
                detailWindow.Show();
            }
        }

        /// <summary>
        /// Xóa lịch sử
        /// </summary>
        private void ClearHistory(object parameter)
        {
            var result = System.Windows.MessageBox.Show(
                "Bạn có chắc chắn muốn xóa toàn bộ lịch sử xem?",
                "Xác nhận",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    var history = _viewHistoryBLL.GetViewHistoryByUser(AppSession.Instance.CurrentUser.UserID, out string msg);

                    if (history != null)
                    {
                        foreach (var item in history)
                        {
                            _viewHistoryBLL.DeleteHistory(item.HistoryID, out string message);
                        }

                        HistoryItems.Clear();

                        System.Windows.MessageBox.Show("Đã xóa toàn bộ lịch sử xem!",
                            "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Lỗi khi xóa lịch sử: {ex.Message}",
                        "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// ViewModel item cho mỗi dòng trong lịch sử xem
    /// </summary>
    public class WatchHistoryItemViewModel : BaseViewModel
    {
        public ViewHistoryDTO ViewHistory { get; set; }
        public MovieDTO Movie { get; set; }
        public EpisodeDTO Episode { get; set; }

        // Tiêu đề hiển thị
        public string DisplayTitle
        {
            get
            {
                if (Episode != null)
                    return $"{Movie?.Title} - {Episode.Title}";
                return Movie?.Title;
            }
        }

        // Thời gian xem
        public string WatchedTime => ViewHistory?.WatchedAt?.ToString("dd/MM/yyyy HH:mm") ?? "";

        // Tiến độ xem (%)
        public double Progress
        {
            get
            {
                if (ViewHistory?.WatchDuration == null) return 0;

                int totalDuration = Episode?.Duration ?? Movie?.Duration ?? 0;
                if (totalDuration == 0) return 0;

                return (double)ViewHistory.WatchDuration.Value / totalDuration * 100;
            }
        }

        public string ProgressText => $"{Progress:F0}%";
    }
}