using System.Windows;

namespace MuVi.Views.AddView
{
    public partial class VideoUrlInputWindow : Window
    {
        public string VideoUrl { get; set; }

        public VideoUrlInputWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVideoUrl.Text))
            {
                MessageBox.Show("Vui lòng nhập URL video!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            VideoUrl = txtVideoUrl.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}