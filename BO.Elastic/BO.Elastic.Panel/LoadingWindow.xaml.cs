using BO.Elastic.Panel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BO.Elastic.Panel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        private Action openApplication;
        public LoadingWindow()
        {
            try
            {
                openApplication = new Action(() =>
                {
                    MainPageWindow mainWindow = new MainPageWindow();
                    mainWindow.Show();
                    this.Close();
                });
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                this.DataContext = new LoadingWindowViewModel(openApplication);
                InitializeComponent();
            }
            catch(Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
