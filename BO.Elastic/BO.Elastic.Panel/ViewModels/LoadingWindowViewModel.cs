using BO.Elastic.Panel.Exceptions;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BO.Elastic.Panel.ViewModels
{
    public class LoadingWindowViewModel : BindableBase
    {
        public string LoadingStatus
        {
            get => loadingStatus;
            set
            {
                SetProperty<string>(ref loadingStatus, value);
            }
        }

        public ICommand CloseAppEvent =>
            closeAppEvent ?? (closeAppEvent = new DelegateCommand(CloseApplication));

        private DelegateCommand closeAppEvent = null;
        private string loadingStatus;
        private Action<string> updateStatusThreadSafeUI;

        public LoadingWindowViewModel()
        {
            loadingStatus = "Sprawdzanie dostępnych aktualizacji";

            updateStatusThreadSafeUI += new Action<string>((parameter) =>
            {
                LoadingStatus = parameter;
            });

            new Thread(() => RunApplication(updateStatusThreadSafeUI)).Start();
        }

        public void RunApplication(Action<string> changeStatusEvent)
        {
            DeleteOldFiles();
            if (IsUpdateAvailable())
            {
                try
                {
                    changeStatusEvent("Aktualizacja dostępna - trwa pobieranie.");
                    UpdateApplication();
                    ForceRestartApplication();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas aktualizacji aplikacji " + ex.Message + ex.StackTrace);
                }

            }
            else
            {
                changeStatusEvent("Aplikacja jest aktualna. Trwa uruchamianie.");
            }
        }

        public void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        private void ForceRestartApplication()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "BO.Elastic.Panel.exe");
            if (!p.Start()) MessageBox.Show("Błąd podczas restartu aktualizacji");
        }

        private void DeleteOldFiles()
        {
            string[] filesInAppDirectory = Directory.GetFiles(Directory.GetCurrentDirectory());
            string[] directoriesInAppDirectory = Directory.GetDirectories(Directory.GetCurrentDirectory());

            foreach (string file in filesInAppDirectory.Where(x => x.EndsWith(".old")).ToArray<string>())
            {
                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas usuwania plików tymczasowych starej aktualizacji " + ex.Message);
                }
            }

            foreach (string file in directoriesInAppDirectory.Where(x => x.EndsWith(".old")).ToArray<string>())
            {
                try
                {
                    if (Directory.Exists(file))
                    {
                        Directory.Delete(file, true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas usuwania folderów tymczasowych starej aktualizacji " + ex.Message);
                }
            }
        }

        private void UpdateApplication()
        {
            string downloadedFileName = Guid.NewGuid().ToString() + ".zip";
            string downloadedFileFullPath = Path.Join(Path.GetTempPath(), downloadedFileName);
            string[] filesInAppDirectory = Directory.GetFiles(Directory.GetCurrentDirectory());
            string[] directoriesInAppDirectory = Directory.GetDirectories(Directory.GetCurrentDirectory());

            string[] filesInDownloadedBuild;
            string[] directoriesInDownloadedBuild;

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile("http://213.32.122.228/Bo.Elastic.Panel/build.zip", downloadedFileFullPath);
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
            directoriesInDownloadedBuild = Directory.GetDirectories(tempUpdateDirectory);

            foreach (string file in filesInAppDirectory)
            {
                if (File.Exists(file))
                {
                    if (File.Exists(file + ".old")){
                        File.Delete(file + ".old");
                    }

                    File.Move(file, file + ".old");
                }
            }

            foreach (string directory in directoriesInAppDirectory)
            {
                if (Directory.Exists(directory))
                {
                    if (Directory.Exists(directory + ".old"))
                    {
                        Directory.Delete(directory + ".old", true);
                    }
                    Directory.Move(directory, directory + ".old");
                }
            }

            foreach (string file in filesInDownloadedBuild)
            {
                if (File.Exists(file))
                {
                    File.Move(file, Path.Join(Directory.GetCurrentDirectory(), Path.GetFileName(file)));
                }
            }

            foreach (string directory in directoriesInDownloadedBuild)
            {
                if (Directory.Exists(directory))
                {
                    string directoryName = new DirectoryInfo(directory).Name;
                    string newDirectory = Path.Join(Directory.GetCurrentDirectory(), directoryName);
                    Directory.Move(directory, newDirectory);
                }
            }

        }

        private bool IsUpdateAvailable()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://213.32.122.228/Bo.Elastic.Panel/version");

                int version;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    version = int.Parse(reader.ReadToEnd());
                }
                try
                {
                    int currentVersion = GetAppVersion();
                    if (version > currentVersion)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (VersionNotFoundException)
                {
                    MessageBox.Show("Nie znaleziono pliku z wersją aplikacji. Nastąpi wymuszona aktualizacja.");
                    try
                    {
                        using (StreamWriter writer = File.CreateText(Path.Combine(Directory.GetCurrentDirectory(), "version")))
                        {
                            writer.Write("0");
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (WebException)
            {
                throw new Exception("Błąd podczas komunikacji z serwerem.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int GetAppVersion()
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "version")))
            {
                throw new VersionNotFoundException();
            }
            else
            {
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
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
