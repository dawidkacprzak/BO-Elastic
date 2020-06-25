using System;
using System.Windows;

namespace BO.Elastic.Panel
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                (s, e) => App_DispatcherUnhandledException(s, (Exception) e.ExceptionObject);
        }

        private void App_DispatcherUnhandledException(object sender, Exception e)
        {
            // Process unhandled exception

            // Prevent default unhandled exception processing
            ;
        }
    }
}