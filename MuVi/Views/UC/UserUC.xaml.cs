using MuVi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MuVi.Views.UC
{
    /// <summary>
    /// Interaction logic for UserUC.xaml
    /// </summary>
    public partial class UserUC : UserControl
    {
        private UserViewModel _userModel;

        public UserUC()
        {
            InitializeComponent();
            _userModel = new UserViewModel();
            DataContext = _userModel;
        }

        public void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UserViewModel vm)
            {
                vm.PreviousPage(); 
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UserViewModel vm)
            {
                vm.NextPage();
            }
        }
    }
}
