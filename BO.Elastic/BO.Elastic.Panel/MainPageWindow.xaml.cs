using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BO.Elastic.Panel
{
    /// <summary>
    /// Interaction logic for MainPageWindow.xaml
    /// </summary>
    public partial class MainPageWindow : Window
    {
        public MainPageWindow()
        {
            try
            {
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private void GridOfToolbar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid move = sender as System.Windows.Controls.Grid;
            Window window = Window.GetWindow(move);
            window.DragMove();
        }

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnMenuToolbar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLogs_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
