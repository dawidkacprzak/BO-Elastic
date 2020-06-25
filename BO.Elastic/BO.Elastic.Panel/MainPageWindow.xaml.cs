using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceConnection;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.Panel.ClassExtensions;
using BO.Elastic.Panel.Helpers;
using BO.Elastic.Panel.ViewModels;
using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore.Storage;
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

        }

        private void BtnDeleteCluster_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddCluster_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eloooo Ławki bloki klatki");
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
                        if (senderGrid.SelectedItem.GetType() == typeof(ServiceAddionalParameters))
                        {
                            ServiceAddionalParameters cluster = (ServiceAddionalParameters)senderGrid.SelectedItem;
                            if (cluster.Service != null)
                            {
                                ((MainPageWindowViewModel)DataContext).LoadedNodeController.SetSelectedClusterId(cluster.Service.Id);
                                ((MainPageWindowViewModel)DataContext).NotifyPropertyChanged("LoadedNodeController");
                            }
                        }
                    }
                }

            }
        }

        private void OnClusterRightclick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
                DependencyObject cell = VisualTreeHelper.GetParent(hit.VisualHit);

                while (cell != null && !(cell is System.Windows.Controls.DataGridCell)) cell = VisualTreeHelper.GetParent(cell);

                System.Windows.Controls.DataGridCell targetCell = cell as System.Windows.Controls.DataGridCell;
                if (targetCell != null && targetCell.DataContext != null && targetCell.DataContext.GetType() == typeof(ServiceAddionalParameters))
                {
                    ServiceAddionalParameters clickedCluster = (ServiceAddionalParameters)targetCell.DataContext;
                    ContextMenu context = new ContextMenu();
                    context.Items.Clear();

                    MenuItem header = new MenuItem();
                    header.Header = "Cluster " + clickedCluster.IP;
                    header.IsEnabled = false;
                    context.Items.Add(header);

                    foreach (var item in clickedCluster.ActionList)
                    {
                        MenuItem tempClick = new MenuItem();
                        tempClick.Click += delegate
                        {
                            clickedCluster.GetActionParameters(item).Invoke();
                        };
                        tempClick.Header = item.ToString();

                        context.Items.Add(tempClick);
                    }
                    if (context.Items.Count > 1)
                    {
                        Clusters.ContextMenu = context;
                        Clusters.ContextMenu.IsOpen = true;
                    }
                }
                else
                {
                    Clusters.ContextMenu = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            SavePasswordWindow savePasswordWindow = new SavePasswordWindow();
            savePasswordWindow.ShowDialog();
        }
    }
}
