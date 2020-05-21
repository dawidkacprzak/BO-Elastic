using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BO.Elastic.Panel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        Mutex oneInstanceMutes;
        public App()
        {
            bool aIsNewInstance = false;
            oneInstanceMutes = new Mutex(true,"Bo.Elastic.Panel", out aIsNewInstance);
            if (!aIsNewInstance)
            {
                App.Current.Shutdown();
            }

            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        protected override Window CreateShell()
        {
            try
            {
                Views.LoadingWindow w = Container.Resolve<Views.LoadingWindow>();
                return w;
            }catch(Exception ex)
            {
                throw ex.InnerException;
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Nieobsłużony wyjątek: "+e.Exception.Message);
            e.Handled = true;
        }
    }
}
