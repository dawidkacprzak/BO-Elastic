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

namespace BO.Elastic.Panel.Views
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

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
