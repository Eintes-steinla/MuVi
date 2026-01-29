using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels;
using MuVi.ViewModels.UCViewModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace MuVi.Views.AddView
{
    public partial class EpisodeAddView : Window
    {
        private EpisodeAddViewModel _viewModel;

        public EpisodeAddView()
        {
            InitializeComponent();

            _viewModel = new EpisodeAddViewModel();
            DataContext = _viewModel;

            Title = "Thêm tập phim";
        }

        public EpisodeAddView(EpisodeDTO existingEpisode)
        {
            InitializeComponent();

            _viewModel = new EpisodeAddViewModel(existingEpisode);
            DataContext = _viewModel;

            Title = "Chỉnh sửa tập phim";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_viewModel.Validate())
                {
                    return;
                }

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                var posterPath = _viewModel.SavePoster();
                if (!string.IsNullOrEmpty(posterPath))
                {
                    _viewModel.PosterPath = posterPath;
                }

                var videoPath = _viewModel.SaveVideo();
                if (!string.IsNullOrEmpty(videoPath))
                {
                    _viewModel.VideoPath = videoPath;
                }

                var episodeBLL = new EpisodeBLL();
                bool success;
                string message;

                if (_viewModel.IsAddMode)
                {
                    var newEpisode = new EpisodeDTO
                    {
                        MovieID = _viewModel.SelectedMovie.MovieID,
                        EpisodeNumber = _viewModel.EpisodeNumber,
                        Title = _viewModel.Title?.Trim(),
                        Description = _viewModel.Description?.Trim(),
                        Duration = _viewModel.Duration,
                        PosterPath = posterPath,
                        VideoPath = videoPath,
                        ReleaseDate = _viewModel.ReleaseDate,
                        ViewCount = _viewModel.ViewCount ?? 0
                    };

                    success = episodeBLL.AddEpisode(newEpisode, out message);
                }
                else
                {
                    _viewModel.Episode.PosterPath = posterPath;
                    _viewModel.Episode.VideoPath = videoPath;
                    success = episodeBLL.UpdateEpisode(_viewModel.Episode, out message);
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}