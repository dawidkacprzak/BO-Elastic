
using System.Windows.Input;
using System.Windows;
using BO.Elastic.Panel.Command;
using System;
using System.Collections.ObjectModel;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BO.Elastic.Panel.ViewModels
{
    public class MainPageWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Service> clusters;

        public ObservableCollection<Service> Clusters
        {
            get => clusters; set
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

            ConfigurationController configurationController = new ConfigurationController();
            Clusters = new ObservableCollection<Service>(configurationController.DownloadConfiguration());
        }

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
