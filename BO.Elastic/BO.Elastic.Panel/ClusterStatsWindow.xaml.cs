using BO.Elastic.BLL.Model;
using BO.Elastic.Panel.ViewModels;
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
    /// Interaction logic for ClusterStatsWindow.xaml
    /// </summary>
    public partial class ClusterStatsWindow : Window
    {
        ClusterStatsWindowViewModel viewModel;
        public ClusterStatsWindow(NetworkAddress clusterAddress)
        {
            InitializeComponent();
            viewModel = new ClusterStatsWindowViewModel(clusterAddress);
            this.DataContext = viewModel;
        }

        private void GridOfToolbar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid move = sender as System.Windows.Controls.Grid;
            Window window = Window.GetWindow(move);
            window.DragMove();
        }

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
