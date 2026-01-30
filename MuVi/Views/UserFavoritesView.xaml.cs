using System.Windows.Controls;
using MuVi.ViewModels;

namespace MuVi.Views
{
    /// <summary>
    /// Code-behind cho UserFavoritesView
    /// </summary>
    public partial class UserFavoritesView : UserControl
    {
        public UserFavoritesView()
        {
            InitializeComponent();
            DataContext = new UserFavoritesViewModel();
        }
    }
}