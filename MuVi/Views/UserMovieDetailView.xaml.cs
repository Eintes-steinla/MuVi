using System.Windows;
using MuVi.DTO.DTOs;
using MuVi.ViewModels;

namespace MuVi.Views
{
    /// <summary>
    /// Code-behind cho UserMovieDetailView
    /// </summary>
    public partial class UserMovieDetailView : Window
    {
        public UserMovieDetailView(MovieDTO movie)
        {
            InitializeComponent();
            DataContext = new UserMovieDetailViewModel(movie);
        }
    }
}