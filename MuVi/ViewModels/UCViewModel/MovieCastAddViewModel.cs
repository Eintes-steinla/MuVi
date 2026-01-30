using MuVi.BLL;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MuVi.ViewModels.UCViewModel
{
    public class MovieCastAddViewModel : BaseViewModel
    {
        #region Fields
        private MovieCastDTO _cast;
        private MovieCastBLL _castBLL = new MovieCastBLL();
        private bool _isAddMode = true;
        #endregion

        #region Properties

        public MovieCastDTO Cast
        {
            get => _cast;
            set => SetProperty(ref _cast, value);
        }

        public ObservableCollection<ActorDTO> ActorList { get; set; }
        public ObservableCollection<MovieDTO> MovieList { get; set; }

        public string RoleName
        {
            get => _cast?.RoleName;
            set
            {
                if (_cast != null)
                {
                    _cast.RoleName = value;
                    OnPropertyChanged(nameof(RoleName));
                }
            }
        }

        public int? Order
        {
            get => _cast?.Order;
            set
            {
                if (_cast != null)
                {
                    _cast.Order = value;
                    OnPropertyChanged(nameof(Order));
                }
            }
        }

        private ActorDTO? _selectedActor;
        public ActorDTO? SelectedActor
        {
            get => _selectedActor;
            set
            {
                _selectedActor = value;
                OnPropertyChanged(nameof(SelectedActor));
                if (_cast != null && value != null)
                {
                    _cast.ActorID = value.ActorID;
                }
            }
        }

        private MovieDTO? _selectedMovie;
        public MovieDTO? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged(nameof(SelectedMovie));
                if (_cast != null && value != null)
                {
                    _cast.MovieID = value.MovieID;
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

        public MovieCastAddViewModel(MovieCastDTO existingCast = null)
        {
            ActorList = new ObservableCollection<ActorDTO>();
            MovieList = new ObservableCollection<MovieDTO>();

            LoadActors();
            LoadMovies();

            if (existingCast != null)
            {
                // Edit mode
                _cast = new MovieCastDTO
                {
                    MovieID = existingCast.MovieID,
                    ActorID = existingCast.ActorID,
                    RoleName = existingCast.RoleName,
                    Order = existingCast.Order
                };
                IsAddMode = false;

                // Set selected actor
                SelectedActor = ActorList.FirstOrDefault(a => a.ActorID == _cast.ActorID);

                // Set selected movie
                SelectedMovie = MovieList.FirstOrDefault(m => m.MovieID == _cast.MovieID);
            }
            else
            {
                // Add mode
                _cast = new MovieCastDTO
                {
                    Order = 1  // Mặc định vai chính
                };
                IsAddMode = true;
            }
        }

        #endregion

        #region Methods

        private void LoadActors()
        {
            var actors = _castBLL.GetAllActors();
            ActorList.Clear();
            foreach (var a in actors)
            {
                ActorList.Add(a);
            }
        }

        private void LoadMovies()
        {
            var movies = _castBLL.GetAllMovies();
            MovieList.Clear();
            foreach (var m in movies)
            {
                MovieList.Add(m);
            }
        }

        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        public bool Validate()
        {
            if (SelectedActor == null)
            {
                MessageBox.Show("Vui lòng chọn diễn viên!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedMovie == null)
            {
                MessageBox.Show("Vui lòng chọn phim!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(RoleName))
            {
                MessageBox.Show("Vui lòng nhập tên vai diễn!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!Order.HasValue || Order < 1)
            {
                MessageBox.Show("Thứ tự phải là số dương (1 = vai chính, 2+ = vai phụ)!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}