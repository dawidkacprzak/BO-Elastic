using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using System.Windows;

namespace BO.Elastic.Panel.ViewModels
{
    public class MainPageWindowViewModel : BindableBase
    {
        public ICommand CloseAppEvent =>
            closeAppEvent ?? (closeAppEvent = new DelegateCommand(CloseApplication));

        private DelegateCommand closeAppEvent = null;

        public void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
