
using System.Windows.Input;
using System.Windows;
using BO.Elastic.Panel.Command;
using System;
using System.Collections.ObjectModel;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Extension;
using System.Threading;
using System.Linq;

namespace BO.Elastic.Panel.ViewModels
{
    public class MainPageWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ServiceAddionalParameters> clusters;
        private ConfigurationController configurationController;
        private List<Service> downloadedConfiguration;
        public ObservableCollection<ServiceAddionalParameters> Clusters
        {
            get => clusters;
            set
            {
                if ((this.Clusters != null && value != null) && this.Clusters.SequenceEqual(value))
                {
                    System.Diagnostics.Debug.WriteLine("update clusters");
                    this.clusters = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int progressValue;
        public int ProgressValue
        {
            get => progressValue;
            set
            {
                if (value != progressValue)
                {
                    this.progressValue = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private static bool updateFinished = true;

        public MainPageWindowViewModel()
        {
            progressValue = 0;
            new Thread(() =>
            {
                SetProgressBarPercent(10);
                configurationController = new ConfigurationController();
                SetProgressBarPercent(30);
                downloadedConfiguration = configurationController.DownloadConfiguration();
                Clusters = new ObservableCollection<ServiceAddionalParameters>(GetAddionalClusterParameters(downloadedConfiguration));
                SetProgressBarPercent(70);
                Refresh();
                SetProgressBarPercent(0);
            }).Start();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += RefreshTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        private void SetProgressBarPercent(int value)
        {
            ProgressValue = value;
        }

        private void RefreshTimerTick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ref att!");

            if (updateFinished)
            {
                updateFinished = false;
                new Thread(() =>
                {
                    System.Diagnostics.Debug.WriteLine("go!");
                    RefreshWithProgressBar();
                    updateFinished = true;
                }).Start();
            }
        }

        private void RefreshWithProgressBar()
        {
            SetProgressBarPercent(50);
            Refresh();
            updateFinished = true;
            SetProgressBarPercent(0);

        }

        private void Refresh()
        {
            if (downloadedConfiguration != null)
            {
                Clusters = new ObservableCollection<ServiceAddionalParameters>(GetAddionalClusterParameters(downloadedConfiguration));
            }
        }

        private IEnumerable<ServiceAddionalParameters> GetAddionalClusterParameters(List<Service> services)
        {
            foreach (var item in services)
            {
                yield return item.GetServiceAddionalParameters();
            }
        }

        public ICommand RefreshEvent => new BasicCommand(new Action(() =>
        {
            new Thread(() =>
            {
                RefreshWithProgressBar();
            }).Start();
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
