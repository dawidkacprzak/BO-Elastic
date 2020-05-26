using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.Panel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
                MainPageWindowViewModel viewModel = new MainPageWindowViewModel();
                this.DataContext = viewModel;
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

        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnLogs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Clusters_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {

                if (sender.GetType() == typeof(DataGrid))
                {
                    DataGrid senderGrid = (DataGrid)sender;
                    if (senderGrid.SelectedItem != null)
                    {
                        if(senderGrid.SelectedItem.GetType() == typeof(ServiceAddionalParameters))
                        {
                            ServiceAddionalParameters cluster = (ServiceAddionalParameters)senderGrid.SelectedItem;
                            List<int> selectedIds = cluster.Service.ClusterNodeCluster.Select(x => x.NodeId).ToList();
                        }
                    }
                }

            }
        }
    }
}
