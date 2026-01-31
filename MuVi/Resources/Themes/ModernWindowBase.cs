using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MuVi.Resources.Themes
{
    public class ModernWindowBase : Window
    {
        public ModernWindowBase()
        {
            this.Style = (Style)Application.Current.FindResource("ModernWindowStyle");
            this.Loaded += ModernWindowBase_Loaded;
            this.StateChanged += ModernWindowBase_StateChanged;
        }

        private void ModernWindowBase_StateChanged(object sender, EventArgs e)
        {
            var mainBorder = this.Template.FindName("MainBorder", this) as Border;

            if (this.WindowState == WindowState.Maximized)
            {
                // Thêm margin để bù trừ phần window bị che
                // Giá trị 7 phù hợp với Windows 11, có thể điều chỉnh thành 8 cho Windows 10
                if (mainBorder != null)
                {
                    mainBorder.Margin = new Thickness(7);
                }
            }
            else
            {
                // Khôi phục margin ban đầu
                if (mainBorder != null)
                {
                    mainBorder.Margin = new Thickness(0);
                }
            }
        }

        private void ModernWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            var titleBar = this.Template.FindName("PART_TitleBar", this) as Border;
            var minimizeButton = this.Template.FindName("PART_MinimizeButton", this) as Button;
            var maximizeButton = this.Template.FindName("PART_MaximizeButton", this) as Button;
            var closeButton = this.Template.FindName("PART_CloseButton", this) as Button;

            if (titleBar != null)
                titleBar.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;

            if (minimizeButton != null)
                minimizeButton.Click += MinimizeButton_Click;

            if (maximizeButton != null)
                maximizeButton.Click += MaximizeButton_Click;

            if (closeButton != null)
                closeButton.Click += CloseButton_Click;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaximizeButton_Click(sender, e);
            }
            else
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    var point = PointToScreen(e.GetPosition(this));
                    this.Left = point.X - (this.ActualWidth / 2);
                    this.Top = point.Y;
                }
                this.DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}