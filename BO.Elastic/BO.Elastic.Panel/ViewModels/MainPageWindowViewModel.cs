
using System.Windows.Input;
using System.Windows;
using BO.Elastic.Panel.Command;
using System;

namespace BO.Elastic.Panel.ViewModels
{
    public class MainPageWindowViewModel
    {
        public ICommand CloseAppEvent => new BasicCommand(new Action(() =>
        {
            CloseApplication();
        }));

        public void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
