using BO.Elastic.Panel.Exceptions;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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
            if (IsUpdateAvailable())
            {
                changeStatusEvent("Aktualizacja dostępna - trwa pobieranie.");
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

        private bool IsUpdateAvailable()
        {
            try
            {
                HttpClient client = new HttpClient();
     
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://213.32.122.228/Bo.Elastic.Panel/version");

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
            catch(WebException)
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
