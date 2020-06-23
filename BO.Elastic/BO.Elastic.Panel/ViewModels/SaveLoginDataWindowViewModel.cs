using BO.Elastic.BLL.Model;
using BO.Elastic.Panel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BO.Elastic.Panel.ViewModels
{
    public class SaveLoginDataWindowViewModel : INotifyPropertyChanged
    {
        public LoginData LoginData
        {
            get
            {

                if(SSHLoginDataContainer.LoginData == null)
                {
                    SSHLoginDataContainer.LoginData = new LoginData();
                }
                return SSHLoginDataContainer.LoginData;
            }
            set
            {
                SSHLoginDataContainer.LoginData = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
