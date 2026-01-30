using System.Windows.Controls;
using MuVi.ViewModels;

namespace MuVi.Views
{
    /// <summary>
    /// Code-behind cho UserWatchHistoryView
    /// </summary>
    public partial class UserWatchHistoryView : UserControl
    {
        public UserWatchHistoryView()
        {
            InitializeComponent();
            DataContext = new UserWatchHistoryViewModel();
        }
    }
}