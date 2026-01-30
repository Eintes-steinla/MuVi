using System.Windows.Controls;
using MuVi.ViewModels;

namespace MuVi.Views
{
    /// <summary>
    /// Code-behind cho UserMovieListView
    /// </summary>
    public partial class UserMovieListView : UserControl
    {
        public UserMovieListView()
        {
            InitializeComponent();
            DataContext = new UserMovieListViewModel();
        }
    }
}