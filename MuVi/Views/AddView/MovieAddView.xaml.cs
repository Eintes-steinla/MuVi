using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels;
using MuVi.ViewModels.UCViewModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace MuVi.Views.AddView
{
    /// <summary>
    /// Interaction logic for MovieAddView.xaml
    /// </summary>
    public partial class MovieAddView : Window
    {
        private MovieAddViewModel _viewModel;

        /// <summary>
        /// Constructor cho chế độ Add (thêm mới)
        /// </summary>
        public MovieAddView()
        {
            InitializeComponent();

            _viewModel = new MovieAddViewModel();
            DataContext = _viewModel;

            Title = "Thêm phim";
        }

        /// <summary>
        /// Constructor cho chế độ Edit (chỉnh sửa)
        /// </summary>
        public MovieAddView(MovieDTO existingMovie)
        {
            InitializeComponent();

            _viewModel = new MovieAddViewModel(existingMovie);
            DataContext = _viewModel;

            Title = "Chỉnh sửa phim";
        }

        /// <summary>
        /// Double click để thêm thể loại
        /// </summary>
        private void lstAvailableGenres_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstAvailableGenres.SelectedItem is GenreDTO genre)
            {
                if (!_viewModel.SelectedGenres.Contains(genre))
                {
                    _viewModel.SelectedGenres.Add(genre);
                }
            }
        }

        /// <summary>
        /// Double click để xóa thể loại
        /// </summary>
        private void lstSelectedGenres_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstSelectedGenres.SelectedItem is GenreDTO genre)
            {
                _viewModel.SelectedGenres.Remove(genre);
            }
        }

        /// <summary>
        /// Lưu thông tin phim
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate
                if (!_viewModel.Validate())
                {
                    return;
                }

                // Kiểm tra thể loại
                if (_viewModel.SelectedGenres.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một thể loại cho phim!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Hiển thị loading nếu có upload video (file lớn)
                Mouse.OverrideCursor = Cursors.Wait;

                // Lưu ảnh poster và lấy đường dẫn
                var posterPath = _viewModel.SavePoster();
                if (!string.IsNullOrEmpty(posterPath))
                {
                    _viewModel.PosterPath = posterPath;
                }

                // Lưu video và lấy đường dẫn
                var videoPath = _viewModel.SaveVideo();
                if (!string.IsNullOrEmpty(videoPath))
                {
                    _viewModel.VideoPath = videoPath;
                }

                // Lấy danh sách GenreIDs
                var genreIds = _viewModel.GetSelectedGenreIds();

                var movieBLL = new MovieBLL();
                bool success;
                string message;

                if (_viewModel.IsAddMode)
                {
                    // Thêm phim mới
                    var newMovie = new MovieDTO
                    {
                        Title = _viewModel.Title?.Trim(),
                        Description = _viewModel.Description?.Trim(),
                        PosterPath = posterPath,
                        TrailerURL = _viewModel.TrailerURL?.Trim(),
                        VideoPath = videoPath,
                        ReleaseYear = _viewModel.ReleaseYear,
                        TotalEpisodes = _viewModel.TotalEpisodes,
                        Duration = _viewModel.Duration,
                        Director = _viewModel.Director?.Trim(),
                        Rating = _viewModel.Rating,
                        ViewCount = _viewModel.ViewCount ?? 0,
                        CountryID = _viewModel.SelectedCountry?.CountryID,
                        Status = _viewModel.Status,
                        MovieType = _viewModel.MovieType
                    };

                    success = movieBLL.AddMovie(newMovie, genreIds, out message);
                }
                else
                {
                    // Cập nhật phim
                    _viewModel.Movie.PosterPath = posterPath;
                    _viewModel.Movie.VideoPath = videoPath;
                    success = movieBLL.UpdateMovie(_viewModel.Movie, genreIds, out message);
                }

                Mouse.OverrideCursor = null;

                MessageBox.Show(message,
                    success ? "Thành công" : "Lỗi",
                    MessageBoxButton.OK,
                    success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show($"Lỗi: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Hủy bỏ
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}