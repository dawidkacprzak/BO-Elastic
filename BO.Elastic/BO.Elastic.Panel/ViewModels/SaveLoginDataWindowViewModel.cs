using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BO.Elastic.BLL.Model;
using BO.Elastic.Panel.Command;
using BO.Elastic.Panel.Helpers;

namespace BO.Elastic.Panel.ViewModels
{
    public class SaveLoginDataWindowViewModel : INotifyPropertyChanged
    {
        public LoginData LoginData
        {
            get
            {
                if (SshLoginDataContainer.LoginData == null) SshLoginDataContainer.LoginData = new LoginData();
                return SshLoginDataContainer.LoginData;
            }
            set
            {
                SshLoginDataContainer.LoginData = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ClearCachedLoginDataCommand => new BasicCommand(() =>
        {
            LoginDataHelper.ClearCachedLoginData();
        });

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}