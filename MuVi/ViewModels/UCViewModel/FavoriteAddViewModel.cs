using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MuVi.ViewModels.UCViewModel
{
    public class FavoriteAddViewModel : BaseViewModel
    {
        #region Fields
        private FavoriteBLL _favoriteBLL = new FavoriteBLL();
        private int _selectedUserId;
        private int _selectedMovieId;
        #endregion

        #region Properties

        public ObservableCollection<UserDTO> UserList { get; set; }
        public ObservableCollection<MovieDTO> MovieList { get; set; }

        private UserDTO? _selectedUser;
        public UserDTO? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                if (value != null)
                {
                    _selectedUserId = value.UserID;
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
                if (value != null)
                {
                    _selectedMovieId = value.MovieID;
                }
            }
        }

        #endregion

        #region Constructor

        public FavoriteAddViewModel()
        {
            UserList = new ObservableCollection<UserDTO>();
            MovieList = new ObservableCollection<MovieDTO>();

            LoadUsers();
            LoadMovies();
        }

        #endregion

        #region Methods

        private void LoadUsers()
        {
            var users = _favoriteBLL.GetAllUsers();
            UserList.Clear();
            foreach (var u in users)
            {
                UserList.Add(u);
            }
        }

        private void LoadMovies()
        {
            var movies = _favoriteBLL.GetAllMovies();
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
            if (SelectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn người dùng!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedMovie == null)
            {
                MessageBox.Show("Vui lòng chọn phim!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public int GetSelectedUserId() => _selectedUserId;
        public int GetSelectedMovieId() => _selectedMovieId;

        #endregion
    }
}