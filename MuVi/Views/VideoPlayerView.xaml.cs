using MuVi.DTO.DTOs;
using MuVi.Resources.Themes;
using MuVi.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MuVi.Views
{
    /// <summary>
    /// Code-behind cho VideoPlayerView
    /// </summary>
    public partial class VideoPlayerView : ModernWindowBase
    {
        private VideoPlayerViewModel _viewModel;
        private DispatcherTimer _progressTimer;
        private DispatcherTimer _controlsTimer;
        private bool _isDraggingSlider = false;

        public VideoPlayerView(MovieDTO movie, EpisodeDTO episode = null)
        {
            InitializeComponent();

            _viewModel = new VideoPlayerViewModel(movie, episode);
            DataContext = _viewModel;

            // Timer để update progress bar
            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromMilliseconds(100);
            _progressTimer.Tick += ProgressTimer_Tick;

            // Timer để ẩn controls
            _controlsTimer = new DispatcherTimer();
            _controlsTimer.Interval = TimeSpan.FromSeconds(3);
            _controlsTimer.Tick += ControlsTimer_Tick;
        }

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Tự động phát video khi load
            VideoPlayer.Play();
            _viewModel.IsPlaying = true;
            _progressTimer.Start();
            _controlsTimer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Cleanup
            _progressTimer?.Stop();
            _controlsTimer?.Stop();
            VideoPlayer?.Stop();
            _viewModel?.Cleanup();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    TogglePlayPause();
                    break;
                case Key.F:
                case Key.F11:
                    ToggleFullScreen();
                    break;
                case Key.Escape:
                    if (_viewModel.IsFullScreen)
                        ToggleFullScreen();
                    else
                        Close();
                    break;
                case Key.Left:
                    _viewModel.SkipBackwardCommand.Execute(null);
                    break;
                case Key.Right:
                    _viewModel.SkipForwardCommand.Execute(null);
                    break;
                case Key.Up:
                    _viewModel.IncreaseVolumeCommand.Execute(null);
                    break;
                case Key.Down:
                    _viewModel.DecreaseVolumeCommand.Execute(null);
                    break;
            }
        }

        #endregion

        #region Video Player Events

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                _viewModel.Duration = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;

                // Seek to last position if exists
                if (_viewModel.Position > 0)
                {
                    VideoPlayer.Position = TimeSpan.FromSeconds(_viewModel.Position);
                }
            }
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            _viewModel.IsPlaying = false;
            _progressTimer.Stop();
            _viewModel.Position = 0;
        }

        private void VideoPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TogglePlayPause();
            ShowPlayPauseAnimation();
        }

        #endregion

        #region Timer Events

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (!_isDraggingSlider && VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                _viewModel.Position = VideoPlayer.Position.TotalSeconds;
            }
        }

        private void ControlsTimer_Tick(object sender, EventArgs e)
        {
            // Ẩn controls sau 3 giây không di chuột
            ControlsPanel.Visibility = Visibility.Collapsed;
            Cursor = Cursors.None;
            _controlsTimer.Stop();
        }

        #endregion

        #region Progress Slider Events

        private void ProgressSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDraggingSlider = true;
        }

        private void ProgressSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDraggingSlider = false;
            VideoPlayer.Position = TimeSpan.FromSeconds(_viewModel.Position);
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDraggingSlider)
            {
                // Update position text while dragging
                _viewModel.Position = e.NewValue;
            }
        }

        #endregion

        #region Control Buttons

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePlayPause();
            ShowPlayPauseAnimation();
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullScreen();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Mouse Movement

        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            // Hiện controls khi di chuột
            ControlsPanel.Visibility = Visibility.Visible;
            Cursor = Cursors.Arrow;

            // Reset timer
            _controlsTimer.Stop();
            _controlsTimer.Start();
        }

        #endregion

        #region Helper Methods

        private void TogglePlayPause()
        {
            if (_viewModel.IsPlaying)
            {
                VideoPlayer.Pause();
                _viewModel.IsPlaying = false;
            }
            else
            {
                VideoPlayer.Play();
                _viewModel.IsPlaying = true;
            }
        }

        private void ToggleFullScreen()
        {
            if (_viewModel.IsFullScreen)
            {
                // Exit fullscreen
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Normal;
                ResizeMode = ResizeMode.CanResize;
                _viewModel.IsFullScreen = false;
                //FullscreenIcon.Text = "⛶";
            }
            else
            {
                // Enter fullscreen
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
                _viewModel.IsFullScreen = true;
                //FullscreenIcon.Text = "⛶";
            }
        }

        private void ShowPlayPauseAnimation()
        {
            // Hiển thị icon play/pause overlay
            var storyboard = new System.Windows.Media.Animation.Storyboard();

            var fadeIn = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            var fadeOut = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                BeginTime = TimeSpan.FromMilliseconds(500)
            };

            System.Windows.Media.Animation.Storyboard.SetTarget(fadeIn, PlayPauseOverlay);
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));

            System.Windows.Media.Animation.Storyboard.SetTarget(fadeOut, PlayPauseOverlay);
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));

            storyboard.Children.Add(fadeIn);
            storyboard.Children.Add(fadeOut);
            storyboard.Begin();
        }

        #endregion
    }
}