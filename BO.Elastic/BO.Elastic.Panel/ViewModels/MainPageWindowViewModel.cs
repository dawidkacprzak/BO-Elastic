
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

        public ICommand RefreshConfiguration => new BasicCommand(new Action(() =>
        {
            RefreshAndSaveConfiguration();
        }));

        private static bool updateFinished = false;

        public MainPageWindowViewModel()
        {
            progressValue = 0;

            new Thread(() =>
            {
                SetProgressBarPercent(10);
                configurationController = new ConfigurationController();
                SetCachedConfiguration();
                SetProgressBarPercent(50);
                Clusters = new ObservableCollection<ServiceAddionalParameters>(GetAddionalClusterParameters(downloadedConfiguration));
                updateFinished = true;
                SetProgressBarPercent(70);
                SetProgressBarPercent(0);
            }).Start();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += RefreshTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }


        private void RefreshAndSaveConfiguration()
        {
            SetProgressBarPercent(30);
            downloadedConfiguration = configurationController.DownloadConfiguration();
            SetProgressBarPercent(60);

            SaveConfiguration();
        }

        private void SetCachedConfiguration()
        {
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
