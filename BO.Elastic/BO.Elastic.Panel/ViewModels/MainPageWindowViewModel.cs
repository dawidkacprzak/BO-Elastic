using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BO.Elastic.BLL;
using BO.Elastic.BLL.Extension;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;
using BO.Elastic.Panel.Command;
using BO.Elastic.Panel.Helpers;

namespace BO.Elastic.Panel.ViewModels
{
    public class MainPageWindowViewModel : INotifyPropertyChanged
    {
        public static readonly object SaveConfigurationLock = new object();

        private static bool refreshState;
        private ObservableCollection<ServiceAddionalParameters> clusters;
        private ConfigurationController configurationController;
        private List<Service> downloadedConfiguration;

        private LoadedNodeController loadedNodeController;

        private int progressValue;

        private Task updateTask1;
        private Task updateTask2;
        private Task updateTask3;

        public MainPageWindowViewModel()
        {
            progressValue = 0;

            new Thread(() =>
            {
                SetProgressBarPercent(10);
                configurationController = new ConfigurationController();
                SetCachedConfiguration();
                SetProgressBarPercent(50);
                Clusters = new ObservableCollection<ServiceAddionalParameters>();
                foreach (Service item in DownloadedConfiguration)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Clusters.Add(new ServiceAddionalParameters
                        {
                            Ip = item.Ip,
                            Port = item.Port,
                            ServiceStatus = EServiceStatus.Initializing,
                            ServiceType = (EServiceType) item.ServiceType
                        });
                    });
                SshLoginDataContainer.LoginData = LoginDataHelper.GetCachedLoginData();
                SetProgressBarPercent(70);
                SetProgressBarPercent(0);
                PrepareUpdateThreads();
                RefreshTimerTick(null, null);
            }).Start();
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += RefreshTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        public List<Service> DownloadedConfiguration
        {
            get
            {
                if (downloadedConfiguration == null)
                    return new List<Service>();
                return downloadedConfiguration;
            }
            private set
            {
                downloadedConfiguration = value;
                LoadedNodeController = new LoadedNodeController(DownloadedConfiguration,
                    () => { NotifyPropertyChanged("LoadedNodeController"); });
            }
        }

        public ObservableCollection<ServiceAddionalParameters> Clusters
        {
            get => clusters;
            set
            {
                if (clusters != null && clusters.SequenceEqual(value) && value != null || clusters == null ||
                    value != null)
                {
                    clusters = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public LoadedNodeController LoadedNodeController
        {
            get => loadedNodeController;
            set
            {
                loadedNodeController = value;
                NotifyPropertyChanged();
            }
        }

        public int ProgressValue
        {
            get => progressValue;
            set
            {
                if (value != progressValue)
                {
                    progressValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand CloseAppEvent => new BasicCommand(() => { CloseApplication(); });

        public ICommand RefreshConfiguration => new BasicCommand(() => { RefreshAndSaveConfiguration(); });

        public event PropertyChangedEventHandler PropertyChanged;

        private void PrepareUpdateThreads()
        {
            if (DownloadedConfiguration.Count == 0) return;
            if (DownloadedConfiguration.Count == 1)
            {
                CreateUpdateThreadInstance(ref updateTask1, 0, 1);
            }
            else if (DownloadedConfiguration.Count == 2)
            {
                int clusterCount = DownloadedConfiguration.Count / 2;

                CreateUpdateThreadInstance(ref updateTask1, 0, clusterCount);
                CreateUpdateThreadInstance(ref updateTask2, clusterCount, DownloadedConfiguration.Count);
            }
            else
            {
                int clusterCount = DownloadedConfiguration.Count / 3;
                CreateUpdateThreadInstance(ref updateTask1, 0, clusterCount);
                CreateUpdateThreadInstance(ref updateTask2, clusterCount, clusterCount * 2);
                CreateUpdateThreadInstance(ref updateTask3, clusterCount * 2, DownloadedConfiguration.Count);
            }
        }

        private void CreateUpdateThreadInstance(ref Task t, int startIndex, int endIndex)
        {
            t = new Task(() =>
            {
                if (!refreshState)
                    for (int i = startIndex; i < endIndex; i++)
                        if (i < DownloadedConfiguration.Count)
                        {
                            ServiceAddionalParameters tempParam =
                                GetAddionalClusterParameters(new List<Service> {DownloadedConfiguration.ElementAt(i)})
                                    .First();
                            if (!refreshState)

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    if (!refreshState && i < Clusters.Count)
                                        Clusters[i] = tempParam;
                                });
                        }
            });
        }

        private void RefreshAndSaveConfiguration()
        {
            Task refreshTask = new Task(() =>
            {
                refreshState = true;
                SetProgressBarPercent(30);
                DownloadedConfiguration = configurationController.DownloadClustersConfiguration();
                Clusters = new ObservableCollection<ServiceAddionalParameters>();
                PrepareUpdateThreads();
                foreach (Service item in DownloadedConfiguration)
                    Clusters.Add(new ServiceAddionalParameters
                    {
                        Ip = item.Ip,
                        Port = item.Port,
                        ServiceStatus = EServiceStatus.Initializing,
                        ServiceType = (EServiceType) item.ServiceType
                    });
                SetProgressBarPercent(60);
                SaveConfiguration();
                refreshState = false;
            });
            refreshTask.Start();
        }

        private void SetCachedConfiguration()
        {
            refreshState = true;
            SetProgressBarPercent(30);
            try
            {
                using (FileStream fs = new FileStream(Path.Combine(Path.GetTempPath(), "boElasticConfiguration.dat"),
                    FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    DownloadedConfiguration = (List<Service>) formatter.Deserialize(fs);
                }
            }
            catch (SerializationException)
            {
                MessageBox.Show("Błąd podczas wczytywania konfiguracji. Pobieram ponownie.");
            }
            catch (FileNotFoundException)
            {
                DownloadedConfiguration = configurationController.DownloadClustersConfiguration();
                SetProgressBarPercent(60);
                SaveConfiguration();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SetProgressBarPercent(0);
                refreshState = false;
            }
        }

        private void SaveConfiguration()
        {
            if (DownloadedConfiguration != null)
                try
                {
                    SetProgressBarPercent(80);
                    lock (SaveConfigurationLock)
                    {
                        using (FileStream fs =
                            new FileStream(Path.Combine(Path.GetTempPath(), "boElasticConfiguration.dat"),
                                FileMode.Create))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(fs, DownloadedConfiguration);
                        }
                    }
                }
                finally
                {
                    SetProgressBarPercent(0);
                }
        }

        private void SetProgressBarPercent(int value)
        {
            ProgressValue = value;
        }

        private void RefreshTimerTick(object sender, EventArgs e)
        {
            if (DownloadedConfiguration != null)
            {
                if (DownloadedConfiguration.Count == 1 && CheckTaskIsNotRunning(updateTask1))
                {
                    PrepareUpdateThreads();
                    updateTask1.Start();
                }
                else if (DownloadedConfiguration.Count == 2 && CheckTaskIsNotRunning(updateTask1) &&
                         CheckTaskIsNotRunning(updateTask2))
                {
                    PrepareUpdateThreads();
                    updateTask1.Start();
                    updateTask2.Start();
                }
                else if (DownloadedConfiguration.Count > 2 && CheckTaskIsNotRunning(updateTask1) &&
                         CheckTaskIsNotRunning(updateTask2) && CheckTaskIsNotRunning(updateTask3))
                {
                    PrepareUpdateThreads();
                    updateTask1.Start();
                    updateTask2.Start();
                    updateTask3.Start();
                }
            }
        }

        private bool CheckTaskIsNotRunning(Task t)
        {
            if (t == null) return true;
            return t.Status != TaskStatus.Running;
        }

        private IEnumerable<ServiceAddionalParameters> GetAddionalClusterParameters(List<Service> services)
        {
            foreach (Service item in services) yield return item.GetServiceAddionalParameters();
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CloseApplication()
        {
            ThreadStart ts = delegate
            {
                Application.Current.Dispatcher.Invoke(delegate { Application.Current.Shutdown(); });
            };
            Thread t = new Thread(ts);
            t.Start();
        }
    }
}