using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO.Elastic.BLL.Model;
using BO.Elastic.Panel.ViewModels;

namespace BO.Elastic.Panel
{
    /// <summary>
    ///     Interaction logic for ClusterStatsWindow.xaml
    /// </summary>
    public partial class ClusterStatsWindow : Window
    {
        private readonly ClusterStatsWindowViewModel viewModel;

        public ClusterStatsWindow(NetworkAddress clusterAddress)
        {
            InitializeComponent();
            viewModel = new ClusterStatsWindowViewModel(clusterAddress);
            DataContext = viewModel;
        }

        private void GridOfToolbar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid move = sender as Grid;
            Window window = GetWindow(move);
            window.DragMove();
        }

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HideAllContentElements()
        {
            Clusters.Visibility = Visibility.Hidden;
            index_mapping.Visibility = Visibility.Hidden;
        }

        private void btn_show_status_Click(object sender, RoutedEventArgs e)
        {
            HideAllContentElements();
            Clusters.Visibility = Visibility.Visible;
        }

        private void btn_index_mapping_Click(object sender, RoutedEventArgs e)
        {
            HideAllContentElements();
            index_mapping.Visibility = Visibility.Visible;

        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(this.DataContext != null)
            {
                ((ClusterStatsWindowViewModel)this.DataContext).MappingPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}