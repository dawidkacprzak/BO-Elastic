using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BO.Elastic.Panel.Command;
using BO.Elastic.Panel.Exceptions;

namespace BO.Elastic.Panel.ViewModels
{
    public class LoadingWindowViewModel : INotifyPropertyChanged
    {
        private string loadingStatus;
        private readonly Action<string> updateStatusThreadSafeUi;

        public LoadingWindowViewModel(Action loadingCallback)
        {
            loadingStatus = "Sprawdzanie dostępnych aktualizacji";

            updateStatusThreadSafeUi += parameter => { LoadingStatus = parameter; };

            new Thread(() => RunApplication(updateStatusThreadSafeUi, loadingCallback)).Start();
        }

        public string LoadingStatus
        {
            get => loadingStatus;
            set
            {
                if (value != loadingStatus)
                {
                    loadingStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand CloseAppEvent => new BasicCommand(() => { CloseApplication(); });

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RunApplication(Action<string> changeStatusEvent, Action loadingCallback)
        {
#if DEBUG
            App.Current.Dispatcher.Invoke(loadingCallback);
#endif
#if RELEASE
            Thread.Sleep(1000);
            if (IsUpdateAvailable())
            {
                DeleteOldFiles();
                try
                {
                    changeStatusEvent("Aktualizacja dostępna - trwa pobieranie.");
                    UpdateApplication(changeStatusEvent);
                    changeStatusEvent("Restart aplikacji...");
                    ForceRestartApplication();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczasz aktualizacji aplikacji " + ex.Message);
                }

            }
            else
            {
                changeStatusEvent("Aplikacja jest aktualna. Trwa uruchamianie.");
                App.Current.Dispatcher.Invoke(loadingCallback);
            }
#endif

        }

        public void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        private void ForceRestartApplication()
        {
            Process p = new Process();
            p.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "BO.Elastic.Panel.exe");
            if (!p.Start()) MessageBox.Show("Błąd podczas restartu aktualizacji");
            Application.Current.Dispatcher.Invoke(() => { Application.Current.Shutdown(); });
        }

        private void DeleteOldFiles()
        {
            var filesInAppDirectory = Directory.GetFiles(Directory.GetCurrentDirectory());
            var directoriesInAppDirectory = Directory.GetDirectories(Directory.GetCurrentDirectory());

            foreach (string file in filesInAppDirectory.Where(x => x.EndsWith(".old")).ToArray())
                try
                {
                    if (File.Exists(file)) File.Delete(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas usuwania plików tymczasowych starej aktualizacji " + ex.Message);
                }

            foreach (string file in directoriesInAppDirectory.Where(x => x.EndsWith(".old")).ToArray())
                try
                {
                    if (Directory.Exists(file)) Directory.Delete(file, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas usuwania folderów tymczasowych starej aktualizacji " + ex.Message);
                }
        }

        private void UpdateApplication(Action<string> changeStatusEvent)
        {
            string downloadedFileName = Guid.NewGuid() + ".zip";
            string downloadedFileFullPath = Path.Join(Path.GetTempPath(), downloadedFileName);
            var filesInAppDirectory = Directory.GetFiles(Directory.GetCurrentDirectory());
            var directoriesInAppDirectory = Directory.GetDirectories(Directory.GetCurrentDirectory());

            string[] filesInDownloadedBuild;

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile("http://213.32.122.228:81/Bo.Elastic.Panel/build.zip", downloadedFileFullPath);
            }

            string tempUpdateDirectory = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());

            if (!Directory.Exists(tempUpdateDirectory))
            {
                Directory.CreateDirectory(tempUpdateDirectory);
            }
            else
            {
                Directory.Delete(tempUpdateDirectory);
                Directory.CreateDirectory(tempUpdateDirectory);
            }

            ZipFile.ExtractToDirectory(downloadedFileFullPath, tempUpdateDirectory);

            filesInDownloadedBuild = Directory.GetFiles(tempUpdateDirectory);
            Directory.GetDirectories(tempUpdateDirectory);

            foreach (string file in filesInAppDirectory)
                if (File.Exists(file))
                {
                    if (File.Exists(file + ".old")) File.Delete(file + ".old");

                    File.Move(file, file + ".old");
                }

            foreach (string file in filesInDownloadedBuild)
            {
                changeStatusEvent("Aktualizacja pliku " + file);
                if (File.Exists(file))
                    File.Move(file, Path.Join(Directory.GetCurrentDirectory(), Path.GetFileName(file)));
            }
        }

        private bool IsUpdateAvailable()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpWebRequest request =
                    (HttpWebRequest) WebRequest.Create("http://213.32.122.228:81/Bo.Elastic.Panel/version");

                int version;
                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    version = int.Parse(reader.ReadToEnd());
                }

                try
                {
                    int currentVersion = GetAppVersion();
                    return version > currentVersion;
                }
                catch (VersionNotFoundException)
                {
                    MessageBox.Show("Nie znaleziono pliku z wersją aplikacji. Nastąpi wymuszona aktualizacja.");
                    using StreamWriter writer =
                        File.CreateText(Path.Combine(Directory.GetCurrentDirectory(), "version"));
                    writer.Write("0");

                    return true;
                }
            }
            catch (WebException)
            {
                throw new Exception("Błąd podczas komunikacji z serwerem.");
            }
        }

        private int GetAppVersion()
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "version")))
                throw new VersionNotFoundException();

            string content = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "version"));
            try
            {
                int version = int.Parse(content);
                return version;
            }
            catch (FormatException)
            {
                throw new FormatException("Wystąpił błąd podczas odczytu wersji aplikacji z serwera.");
            }
        }
    }
}