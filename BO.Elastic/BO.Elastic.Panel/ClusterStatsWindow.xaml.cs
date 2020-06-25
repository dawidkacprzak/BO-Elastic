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
    }
}