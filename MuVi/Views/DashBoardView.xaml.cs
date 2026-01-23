using MuVi.Helpers;
using MuVi.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MuVi.Views
{
    public partial class DashboardView : Window
    {
        private DashboardViewModel _viewModel;
        private Button _activeButton;

        public DashboardView()
        {
            InitializeComponent();

            // Khởi tạo ViewModel
            _viewModel = new DashboardViewModel();
            this.DataContext = _viewModel;

            // Set button Home là active mặc định
            _activeButton = btnHome;
            SetActiveButton(btnHome);
        }

        #region Navigation Methods

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        /// <summary>
        /// Xử lý sự kiện click nút Home
        /// </summary>
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            _viewModel.LoadSection("Home");
        }

        /// <summary>
        /// Xử lý sự kiện click nút Movies (Phim lẻ)
        /// </summary>
        private void btnMovies_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            _viewModel.LoadSection("Movies");
        }

        /// <summary>
        /// Xử lý sự kiện click nút Series (Phim bộ)
        /// </summary>
        private void btnSeries_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            _viewModel.LoadSection("Series");
        }

        /// <summary>
        /// Xử lý sự kiện click nút Favorites
        /// </summary>
        private void btnFavorites_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            _viewModel.LoadSection("Favorites");
        }

        /// <summary>
        /// Xử lý sự kiện click nút History
        /// </summary>
        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            _viewModel.LoadSection("History");
        }

        /// <summary>
        /// Xử lý sự kiện click nút thể loại
        /// </summary>
        private void btnGenre_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            SetActiveButton(button);

            var genreTag = button.Tag?.ToString();
            // TODO: Implement genre filtering based on tag
            MessageBox.Show(
                $"Lọc theo thể loại: {genreTag}",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Đặt button được chọn làm active và bỏ active các button khác
        /// </summary>
        private void SetActiveButton(Button button)
        {
            if (_activeButton != null)
            {
                _activeButton.SetValue(TagProperty, null);
            }

            if (button != null)
            {
                button.SetValue(TagProperty, "Active");
                _activeButton = button;
            }
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// Xử lý sự kiện nhấn phím trong ô tìm kiếm
        /// </summary>
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.SearchCommand?.Execute(null);
            }
        }

        #endregion

        #region Movie Card Methods

        /// <summary>
        /// Xử lý sự kiện click vào card phim
        /// </summary>
        private void MovieCard_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.Tag != null && int.TryParse(border.Tag.ToString(), out int movieId))
            {
                NavigateToMovieDetail(movieId);
            }
        }

        /// <summary>
        /// Điều hướng đến trang chi tiết phim
        /// </summary>
        private void NavigateToMovieDetail(int movieId)
        {
            try
            {
                // TODO: Implement navigation to movie detail window
                MessageBox.Show(
                    $"Mở chi tiết phim ID: {movieId}",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                // Ví dụ:
                // var movieDetailView = new MovieDetailView(movieId);
                // movieDetailView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi mở chi tiết phim: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Xử lý sự kiện thêm phim vào yêu thích
        /// </summary>
        private void btnAddToFavorite_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null && int.TryParse(button.Tag.ToString(), out int movieId))
            {
                _viewModel.AddToFavoritesCommand?.Execute(movieId);

                // Đổi icon hoặc màu sắc để báo hiệu đã thêm
                // TODO: Update UI to show favorite status
            }

            // Ngăn sự kiện lan truyền đến MovieCard_Click
            e.Handled = true;
        }

        #endregion

        #region Featured Section Methods

        /// <summary>
        /// Xử lý sự kiện click nút "Xem ngay" trên featured section
        /// </summary>
        private void btnWatchNow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Mở trình phát video",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // TODO: Implement video player
            // var videoPlayerView = new VideoPlayerView(movieId);
            // videoPlayerView.ShowDialog();
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Chi tiết" trên featured section
        /// </summary>
        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Get featured movie ID and navigate
            MessageBox.Show(
                "Mở chi tiết phim nổi bật",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        #endregion

        #region popup genre
        
        private string selectedGenre = "All";

        // Hiển thị popup khi chuột vào button Genre
        private void btnGenres_MouseEnter(object sender, MouseEventArgs e)
        {
            popupGenres.IsOpen = true;
        }

        // Ẩn popup khi chuột rời khỏi button
        private void btnGenres_MouseLeave(object sender, MouseEventArgs e)
        {
            // Delay nhỏ để user có thể di chuyển chuột vào popup
            // Nếu không có delay, popup sẽ đóng ngay
        }

        // Ẩn popup khi chuột rời khỏi popup
        private void popupGenres_MouseLeave(object sender, MouseEventArgs e)
        {
            popupGenres.IsOpen = false;
        }

        // Xử lý khi click vào genre item
        private void GenreItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                selectedGenre = btn.Tag.ToString();
                string genreName = btn.Content.ToString();

                // Đóng popup
                popupGenres.IsOpen = false;

                // Cập nhật tiêu đề trang
                //txtPageTitle.Text = $"Phim {genreName}";

                // Gọi hàm lọc phim theo genre
                //FilterMoviesByGenre(selectedGenre);

                // Có thể thêm visual feedback cho button đã chọn
                UpdateGenreButtonText(genreName);
            }
        }

        // Cập nhật text của button Genre để hiển thị genre đang chọn
        private void UpdateGenreButtonText(string genreName)
        {
            // Tìm TextBlock trong button Genre và update text
            var stackPanel = btnGenres.Content as StackPanel;
            if (stackPanel != null && stackPanel.Children.Count > 1)
            {
                var textBlock = stackPanel.Children[1] as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Text = genreName;
                }
            }
        }
        #endregion

        #region Header Methods

        /// <summary>
        /// Xử lý sự kiện click nút Settings
        /// </summary>
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Mở cài đặt",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // TODO: Open settings window
            // var settingsView = new SettingsView();
            // settingsView.ShowDialog();
        }

        #endregion

        #region User Profile Methods

        /// <summary>
        /// Xử lý sự kiện click vào profile người dùng
        /// </summary>
        private void UserProfile_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(
                "Mở trang cá nhân",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // TODO: Open user profile window
            // var userProfileView = new UserProfileView();
            // userProfileView.ShowDialog();
        }

        /// <summary>
        /// Xử lý sự kiện logout
        /// </summary>
        private void Logout_Click(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                // Xóa session
                AppSession.Instance.CurrentUser = null;
                //AppSession.Instance.LoginTime = null;

                // Quay lại trang đăng nhập
                var loginView = new LoginView();
                loginView.Show();

                this.Close();
            }

            // Ngăn sự kiện lan truyền
            e.Handled = true;
        }

        #endregion

        #region Window Methods

        /// <summary>
        /// Cho phép di chuyển cửa sổ khi kéo
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Chỉ cho phép kéo khi click vào vùng header
            if (e.OriginalSource is Border border && border.Name == "HeaderBorder")
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Xử lý khi cửa sổ được load
        /// </summary>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // Load dữ liệu ban đầu
            _viewModel.LoadDataCommand?.Execute(null);
        }

        #endregion
    }
}