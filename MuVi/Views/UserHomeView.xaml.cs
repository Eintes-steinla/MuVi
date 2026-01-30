using System.Windows.Controls;
using MuVi.ViewModels;

namespace MuVi.Views
{
    /// <summary>
    /// Code-behind cho UserHomeView
    /// </summary>
    public partial class UserHomeView : UserControl
    {
        public UserHomeView()
        {
            InitializeComponent();
            DataContext = new UserHomeViewModel();
        }
    }
}