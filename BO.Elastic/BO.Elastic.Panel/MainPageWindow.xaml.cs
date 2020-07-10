using BO.Elastic.Panel.Extension;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.Panel.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BO.Elastic.BLL.Types;

namespace BO.Elastic.Panel
{
    /// <summary>
    ///     Interaction logic for MainPageWindow.xaml
    /// </summary>
    public partial class MainPageWindow : Window
    {
        private bool mRestoreForDragMove;

        public MainPageWindow()
        {
            try
            {
                MainPageWindowViewModel viewModel = new MainPageWindowViewModel();
                DataContext = viewModel;
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                InitializeComponent();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private void GridOfToolbar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (ResizeMode != ResizeMode.CanResize &&
                    ResizeMode != ResizeMode.CanResizeWithGrip)
                {
                    return;
                }
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
            else
            {
                mRestoreForDragMove = WindowState == WindowState.Maximized;
                DragMove();
            }
        }

        private void GridOfToolbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (mRestoreForDragMove)
            {
                mRestoreForDragMove = false;
                var point = PointToScreen(e.MouseDevice.GetPosition(this));
                Left = point.X - (RestoreBounds.Width * 0.5);
                Top = point.Y;
                WindowState = WindowState.Normal;
                DragMove();
            }
        }
        private void GridOfToolbar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mRestoreForDragMove = false;
        }

        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnDeleteCluster_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddCluster_Click(object sender, RoutedEventArgs e)
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
            HitTestResult hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
            DependencyObject cell = VisualTreeHelper.GetParent(hit.VisualHit);

            while (cell != null && !(cell is DataGridCell)) cell = VisualTreeHelper.GetParent(cell);

            DataGridCell targetCell = cell as DataGridCell;
            if (targetCell != null && targetCell.DataContext != null &&
                targetCell.DataContext.GetType() == typeof(ServiceAddionalParameters))
            {
                ServiceAddionalParameters clickedCluster = (ServiceAddionalParameters)targetCell.DataContext;
                ContextMenu context = new ContextMenu();
                context.Items.Clear();

                MenuItem header = new MenuItem();
                header.Header = "Cluster " + clickedCluster.Ip;
                header.IsEnabled = false;
                context.Items.Add(header);

                foreach (EServiceAction item in clickedCluster.ActionList)
                {
                    MenuItem tempClick = new MenuItem();
                    tempClick.Click += delegate { clickedCluster.GetActionParameters(item).Invoke(); };
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

        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            SavePasswordWindow savePasswordWindow = new SavePasswordWindow();
            savePasswordWindow.ShowDialog();
        }
    }
}