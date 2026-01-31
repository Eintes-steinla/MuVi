using MuVi.DTO.DTOs;
using MuVi.Resources.Themes;
using MuVi.ViewModels;
using System.Windows;

namespace MuVi.Views
{
    /// <summary>
    /// Code-behind cho UserMovieDetailView
    /// </summary>
    public partial class UserMovieDetailView : ModernWindowBase
    {
        public UserMovieDetailView(MovieDTO movie)
        {
            InitializeComponent();
            DataContext = new UserMovieDetailViewModel(movie);
        }

    }
}