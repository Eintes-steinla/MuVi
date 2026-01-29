using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System;
using System.Windows;
using System.Windows.Input;

namespace MuVi.ViewModels.UCViewModel
{
    public class GenreAddViewModel : BaseViewModel
    {
        #region Fields
        private GenreDTO _genre;
        private GenreBLL _genreBLL = new GenreBLL();
        private bool _isAddMode = true;
        #endregion

        #region Properties

        public GenreDTO Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        public string GenreName
        {
            get => _genre?.GenreName;
            set
            {
                if (_genre != null)
                {
                    _genre.GenreName = value;
                    OnPropertyChanged(nameof(GenreName));
                }
            }
        }

        public string Description
        {
            get => _genre?.Description;
            set
            {
                if (_genre != null)
                {
                    _genre.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
            set => SetProperty(ref _isAddMode, value);
        }

        #endregion

        #region Constructor

        public GenreAddViewModel(GenreDTO existingGenre = null)
        {
            if (existingGenre != null)
            {
                // Edit mode
                _genre = new GenreDTO
                {
                    GenreID = existingGenre.GenreID,
                    GenreName = existingGenre.GenreName,
                    Description = existingGenre.Description
                };
                IsAddMode = false;
            }
            else
            {
                // Add mode
                _genre = new GenreDTO();
                IsAddMode = true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(GenreName))
            {
                MessageBox.Show("Vui lòng nhập tên thể loại!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}