using System;
using System.Windows;
using BO.Elastic.Panel.ViewModels;

namespace BO.Elastic.Panel
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        private readonly Action openApplication;

        public LoadingWindow()
        {
            try
            {
                openApplication = () =>
                {
                    MainPageWindow mainWindow = new MainPageWindow();
                    mainWindow.Show();
                    Close();
                };
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                DataContext = new LoadingWindowViewModel(openApplication);
                InitializeComponent();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}