
using System.Windows.Input;
using System.Windows;
using BO.Elastic.Panel.Command;
using System;
using System.Collections.ObjectModel;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Extension;
using BO.Elastic.BLL;
using System.Threading;

namespace BO.Elastic.Panel.ViewModels
{
    public class MainPageWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ServiceAddionalParameters> clusters;
        ConfigurationController configurationController;
        public ObservableCollection<ServiceAddionalParameters> Clusters
        {
            get => clusters; 
            set
            {
                if (value != null)
                {
                    this.clusters = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public MainPageWindowViewModel()
        {
            configurationController = new ConfigurationController();
            Clusters = new ObservableCollection<ServiceAddionalParameters>(GetAddionalClusterParameters(configurationController.DownloadConfiguration()));
        }

        private void Refresh()
        {
            Clusters = new ObservableCollection<ServiceAddionalParameters>(GetAddionalClusterParameters(configurationController.DownloadConfiguration()));
        }

        private IEnumerable<ServiceAddionalParameters> GetAddionalClusterParameters(List<Service> services)
        {
            foreach(var item in services)
            {
                yield return item.GetServiceAddionalParameters();
            }
        }

        public ICommand RefreshEvent => new BasicCommand(new Action(() =>
        {
            Refresh();
        }));

        public ICommand CloseAppEvent => new BasicCommand(new Action(() =>
        {
            CloseApplication();
        }));

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
