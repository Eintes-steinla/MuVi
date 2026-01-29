using MuVi.ViewModels;
using MuVi.BLL;
using MuVi.DTO.DTOs;
using MuVi.ViewModels.UCViewModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace MuVi.Views.AddView
{
    public partial class ActorAddView : Window
    {
        private ActorAddViewModel _viewModel;

        /// <summary>
        /// Constructor cho chế độ Add (thêm mới)
        /// </summary>
        public ActorAddView()
        {
            InitializeComponent();

            _viewModel = new ActorAddViewModel();
            DataContext = _viewModel;

            Title = "Thêm diễn viên";
        }

        /// <summary>
        /// Constructor cho chế độ Edit (chỉnh sửa)
        /// </summary>
        public ActorAddView(ActorDTO existingActor)
        {
            InitializeComponent();

            _viewModel = new ActorAddViewModel(existingActor);
            DataContext = _viewModel;

            Title = "Chỉnh sửa diễn viên";
        }

        /// <summary>
        /// Lưu thông tin diễn viên
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

                // Hiển thị loading
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                // Lưu ảnh photo và lấy đường dẫn
                var photoPath = _viewModel.SavePhoto();
                if (!string.IsNullOrEmpty(photoPath))
                {
                    _viewModel.PhotoPath = photoPath;
                }

                var actorBLL = new ActorBLL();
                bool success;
                string message;

                if (_viewModel.IsAddMode)
                {
                    // Thêm diễn viên mới
                    var newActor = new ActorDTO
                    {
                        ActorName = _viewModel.ActorName?.Trim(),
                        Bio = _viewModel.Bio?.Trim(),
                        PhotoPath = photoPath,
                        DateOfBirth = _viewModel.DateOfBirth,
                        Nationality = _viewModel.Nationality?.Trim()
                    };

                    success = actorBLL.AddActor(newActor, out message);
                }
                else
                {
                    // Cập nhật diễn viên
                    _viewModel.Actor.PhotoPath = photoPath;
                    success = actorBLL.UpdateActor(_viewModel.Actor, out message);
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