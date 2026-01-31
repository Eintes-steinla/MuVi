using MuVi.Resources.Themes;
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
    public partial class AdminView : ModernWindowBase
    {
        public AdminView()
        {
            InitializeComponent();
            ContentArea.Content = new UserUC();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra nếu nguồn phát ra sự kiện đúng là TabControl
            if (e.Source is TabControl tabControl)
            {
                TabItem selectedTab = tabControl.SelectedItem as TabItem;

                if (selectedTab != null && selectedTab.Tag != null)
                {
                    string destination = selectedTab.Tag.ToString();

                    // Sử dụng switch-case để điều hướng giao diện dựa trên Tag
                    switch (destination)
                    {
                        case "User":
                            ContentArea.Content = new UserUC();
                            break;

                        case "Movie":
                            ContentArea.Content = new MovieUC();
                            break;

                        case "Genre":
                            ContentArea.Content = new GenreUC();
                            break;

                        case "Episode":
                            ContentArea.Content = new EpisodeUC();
                            break;

                        case "Actor":
                            ContentArea.Content = new ActorUC();
                            break;

                        case "Country":
                            ContentArea.Content = new CountryUC();
                            break;

                        case "Favorite":
                            ContentArea.Content = new FavoriteUC();
                            break;

                        case "History":
                            ContentArea.Content = new HistoryUC();
                            break;

                        case "Review":
                            ContentArea.Content = new ReviewUC();
                            break;
                        
                        case "MovieCast":
                            ContentArea.Content = new MovieCastUC();
                            break;

                        default:
                            ContentArea.Content = new UserUC();
                            break;
                    }
                }
            }
        }
    }
}