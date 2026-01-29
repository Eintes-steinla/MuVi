using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels;
using MuVi.ViewModels.UCViewModel;
using System;
using System.Windows;

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

                // Lưu ảnh và lấy đường dẫn
                var posterPath = _viewModel.SavePoster();
                if (!string.IsNullOrEmpty(posterPath))
                {
                    _viewModel.PosterPath = posterPath;
                }

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

                    success = movieBLL.AddMovie(newMovie, out message);
                }
                else
                {
                    // Cập nhật phim
                    _viewModel.Movie.PosterPath = posterPath;
                    success = movieBLL.UpdateMovie(_viewModel.Movie, out message);
                }

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