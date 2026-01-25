using MuVi.Interfaces;
using MuVi.ViewModels;
using MuVi.Views.UC;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MuVi.Views
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : Window
    {
        /// <summary>
        /// Đóng window
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}