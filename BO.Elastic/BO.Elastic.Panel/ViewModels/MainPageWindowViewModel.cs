
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
                if ((this.clusters != null && this.clusters.SequenceEqual(value) && value != null) || (clusters == null || value != null))
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
                foreach (var item in downloadedConfiguration)
                {
                    Clusters.Add(new ServiceAddionalParameters()
                    {
                        IP = item.Ip,
                        Port = item.Port,
                        ServiceStatus = EServiceStatus.Initializing,
                        ServiceType = (EServiceType)item.ServiceType
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
            if (downloadedConfiguration.Count == 0) return;
            if (downloadedConfiguration.Count == 1)
            {
                updateTask1 = new Task(() =>
                {

                    ServiceAddionalParameters tempParam = GetAddionalClusterParameters(new List<Service>() { downloadedConfiguration.ElementAt(0) }).First();
                    if (!refreshState)
                        App.Current.Dispatcher.Invoke(() =>
                    {
                        if (!refreshState && 1 < Clusters.Count)
                            Clusters[0] = tempParam;

                    });
                });
            }
            else if (downloadedConfiguration.Count == 2)
            {
                int clusterCount = downloadedConfiguration.Count / 2;

                updateTask1 = new Task(() =>
                {
                    for (int i = 0; i < clusterCount; i++)
                    {
                        if (i < downloadedConfiguration.Count)
                        {
                            ServiceAddionalParameters tempParam = GetAddionalClusterParameters(new List<Service>() { downloadedConfiguration.ElementAt(i) }).First();
                            if (!refreshState)
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    if (!refreshState && i < Clusters.Count)
                                        Clusters[i] = tempParam; Clusters[i] = tempParam;

                                });
                        }
                    }
                });

                updateTask2 = new Task(() =>
                {
                    if (!refreshState)
                        for (int i = clusterCount; i < downloadedConfiguration.Count; i++)
                        {
                            if (i < downloadedConfiguration.Count)
                            {
                                ServiceAddionalParameters tempParam = GetAddionalClusterParameters(new List<Service>() { downloadedConfiguration.ElementAt(i) }).First();
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
            else
            {
                int clusterCount = downloadedConfiguration.Count / 3;

                updateTask1 = new Task(() =>
                {
                    for (int i = 0; i < clusterCount; i++)
                    {
                        if (i < downloadedConfiguration.Count)
                        {
                            ServiceAddionalParameters tempParam = GetAddionalClusterParameters(new List<Service>() { downloadedConfiguration.ElementAt(i) }).First();
                            if (!refreshState)

                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    if (!refreshState && i < Clusters.Count)
                                        Clusters[i] = tempParam;

                                });
                        }

                    }
                });

                updateTask2 = new Task(() =>
                {
                    if (!refreshState)
                        for (int i = clusterCount; i < clusterCount * 2; i++)
                        {
                            if (i < downloadedConfiguration.Count)
                            {
                                ServiceAddionalParameters tempParam = GetAddionalClusterParameters(new List<Service>() { downloadedConfiguration.ElementAt(i) }).First();
                                if (!refreshState)

                                    App.Current.Dispatcher.Invoke(() =>
                                    {
                                        if (!refreshState && i < Clusters.Count)
                                            Clusters[i] = tempParam;
                                    });
                            }

                        }
                });
                updateTask3 = new Task(() =>
                {
                    if (!refreshState)
                        for (int i = clusterCount * 2; i < downloadedConfiguration.Count; i++)
                        {
                            if (i < downloadedConfiguration.Count)
                            {
                                ServiceAddionalParameters tempParam = GetAddionalClusterParameters(new List<Service>() { downloadedConfiguration.ElementAt(i) }).First();
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
        }

        private void RefreshAndSaveConfiguration()
        {
            updateTask1 = null;
            updateTask2 = null;
            updateTask3 = null;
            Task refreshTask = new Task(() =>
            {
                refreshState = true;
                SetProgressBarPercent(30);
                downloadedConfiguration = configurationController.DownloadConfiguration();
                Clusters = new ObservableCollection<ServiceAddionalParameters>();
                PrepareUpdateThreads();
                foreach (var item in downloadedConfiguration)
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
                    downloadedConfiguration = (List<Service>)formatter.Deserialize(fs);
                }
            }
            catch (SerializationException e)
            {
                MessageBox.Show("Błąd podczas wczytywania konfiguracji. Pobieram ponownie.");
            }
            catch (FileNotFoundException)
            {
                downloadedConfiguration = configurationController.DownloadConfiguration();
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
            if (downloadedConfiguration != null)
            {
                try
                {
                    SetProgressBarPercent(80);

                    using (FileStream fs = new FileStream(Path.Combine(Path.GetTempPath(), "boElasticConfiguration.dat"), FileMode.Create))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(fs, downloadedConfiguration);
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
            if (updateTask1 != null && updateTask2 == null && updateTask1.Status != TaskStatus.Running && updateTask1.Status != TaskStatus.RanToCompletion && !refreshState)
            {
                PrepareUpdateThreads();
                System.Diagnostics.Debug.WriteLine("updarw");
                SetProgressBarPercent(50);
                updateTask1.Start();
            }
            else if ((updateTask1 != null && updateTask2 != null && updateTask3 == null) 
                && updateTask1.Status != TaskStatus.Running && updateTask1.Status != TaskStatus.RanToCompletion
                && updateTask2.Status != TaskStatus.Running && updateTask2.Status != TaskStatus.RanToCompletion
                && !refreshState)
            {
                PrepareUpdateThreads();
                System.Diagnostics.Debug.WriteLine("updarw");
                SetProgressBarPercent(50);
                updateTask1.Start();
                updateTask2.Start();
            }
            else if (((updateTask1 != null && updateTask2 != null && updateTask3 != null) 
                && updateTask1.Status != TaskStatus.Running && updateTask1.Status != TaskStatus.RanToCompletion 
                && updateTask2.Status != TaskStatus.Running && updateTask2.Status != TaskStatus.RanToCompletion
                && updateTask3.Status != TaskStatus.Running && updateTask3.Status != TaskStatus.RanToCompletion && !refreshState))
            {
                PrepareUpdateThreads();
                System.Diagnostics.Debug.WriteLine("updarw");
                SetProgressBarPercent(50);
                updateTask1.Start();
                updateTask2.Start();
                updateTask3.Start();
            }
        }


        private IEnumerable<ServiceAddionalParameters> GetAddionalClusterParameters(List<Service> services)
        {
            foreach (var item in services)
            {
                yield return item.GetServiceAddionalParameters();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
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
