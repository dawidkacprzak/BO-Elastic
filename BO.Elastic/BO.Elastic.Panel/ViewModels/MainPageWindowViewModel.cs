
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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using BO.Elastic.BLL.Types;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BO.Elastic.Panel.ViewModels
{
    public class MainPageWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ServiceAddionalParameters> clusters;
        private ConfigurationController configurationController;
        private List<Service> downloadedConfiguration;

        public List<Service> DownloadedConfiguration
        {
            get
            {
                if (downloadedConfiguration == null)
                {
                    return new List<Service>();
                }
                else
                {
                    return downloadedConfiguration;
                }
            }
            private set
            {
                downloadedConfiguration = value;
                LoadedNodeController = new LoadedNodeController(DownloadedConfiguration, new Action(()=>
                {
                    NotifyPropertyChanged("LoadedNodeController");
                }));
            }
        }
        public ObservableCollection<ServiceAddionalParameters> Clusters
        {
            get => clusters;
            set
            {
                if ((this.clusters != null && this.clusters.SequenceEqual(value) && value != null) || (clusters == null || value != null))
                {
                    this.clusters = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private LoadedNodeController loadedNodeController;
        public LoadedNodeController LoadedNodeController
        {
            get
            {
                return loadedNodeController;
            }
            set
            {
                loadedNodeController = value;
                NotifyPropertyChanged();
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

        public static readonly object saveConfigurationLock = new object();
        public ICommand CloseAppEvent => new BasicCommand(new Action(() =>
        {
            CloseApplication();
        }));

        public ICommand RefreshConfiguration => new BasicCommand(new Action(() =>
        {
            RefreshAndSaveConfiguration();
        }));

        private static bool refreshState = false;

        Task updateTask1;
        Task updateTask2;
        Task updateTask3;

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
                foreach (var item in DownloadedConfiguration)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Clusters.Add(new ServiceAddionalParameters()
                        {
                            IP = item.Ip,
                            Port = item.Port,
                            ServiceStatus = EServiceStatus.Initializing,
                            ServiceType = (EServiceType)item.ServiceType
                        });
                    });
                }
                SetProgressBarPercent(70);
                SetProgressBarPercent(0);
                PrepareUpdateThreads();
                RefreshTimerTick(null, null);
            }).Start();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += RefreshTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

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
                    {
                        if (i < DownloadedConfiguration.Count)
                        {
                            ServiceAddionalParameters tempParam = GetAddionalClusterParameters(new List<Service>() { DownloadedConfiguration.ElementAt(i) }).First();
                            if (!refreshState)

                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    if (!refreshState && i < Clusters.Count)
                                        Clusters[i] = tempParam;
                                });
                        }

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
                foreach (var item in DownloadedConfiguration)
                {
                    Clusters.Add(new ServiceAddionalParameters()
                    {
                        IP = item.Ip,
                        Port = item.Port,
                        ServiceStatus = EServiceStatus.Initializing,
                        ServiceType = (EServiceType)item.ServiceType
                    });
                }
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
                using (FileStream fs = new FileStream(Path.Combine(Path.GetTempPath(), "boElasticConfiguration.dat"), FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    DownloadedConfiguration = (List<Service>)formatter.Deserialize(fs);
                }
            }
            catch (SerializationException e)
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
            {
                try
                {
                    SetProgressBarPercent(80);
                    lock (saveConfigurationLock)
                    {
                        using (FileStream fs = new FileStream(Path.Combine(Path.GetTempPath(), "boElasticConfiguration.dat"), FileMode.Create))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(fs, DownloadedConfiguration);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw;
                }
                finally
                {
                    SetProgressBarPercent(0);
                }
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
                else if (DownloadedConfiguration.Count == 2 && CheckTaskIsNotRunning(updateTask1) && CheckTaskIsNotRunning(updateTask2))
                {
                    PrepareUpdateThreads();
                    updateTask1.Start();
                    updateTask2.Start();
                }
                else if (DownloadedConfiguration.Count > 2 && CheckTaskIsNotRunning(updateTask1) && CheckTaskIsNotRunning(updateTask2) && CheckTaskIsNotRunning(updateTask3))
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
            foreach (var item in services)
            {
                yield return item.GetServiceAddionalParameters();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void CloseApplication()
        {
            ThreadStart ts = delegate ()
            {
                Application.Current.Dispatcher.Invoke((Action)delegate ()
                {
                    Application.Current.Shutdown();
                });
            };
            Thread t = new Thread(ts);
            t.Start();
        }
    }
}
