using MuVi.Helpers;
using MuVi.Resources.Themes;
using System.Windows;

namespace MuVi.Views
{
    public partial class UserDashboardView : ModernWindowBase
    {
        public UserDashboardView()
        {
            InitializeComponent();

            // Hiển thị username
            if (AppSession.Instance.CurrentUser != null)
            {
                txtUsername.Text = AppSession.Instance.CurrentUser.Username;
            }

            // Load trang chủ mặc định
            MainContent.Content = new UserHomeView();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserHomeView();
        }

        private void btnMovieList_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserMovieListView();
        }

        private void btnFavorites_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserFavoritesView();
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserWatchHistoryView();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                AppSession.Instance.CurrentUser = null;
                var loginWindow = new LoginView();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}