using System;
using System.Windows;
using BO.Elastic.BLL.Extension;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceConnection;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;
using BO.Elastic.Panel.Helpers;

namespace BO.Elastic.Panel.Extension
{
    public static class ServiceExtension
    {
        public static Action GetActionParameters(this ServiceAddionalParameters parameters,
            EServiceAction eServiceAction)
        {
            ServiceRemoteManager manager;
            switch (eServiceAction)
            {
                case EServiceAction.ConnectBySsh:
                    break;

                case EServiceAction.Start:
                    return () =>
                    {
                        try
                        {
                            manager = GetServiceManager(parameters.Ip, parameters.Port);
                            manager.StartElasticService(parameters.GetSshNetworkAddress());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    };

                case EServiceAction.Stop:
                    return () =>
                    {
                        try
                        {
                            manager = GetServiceManager(parameters.Ip, parameters.Port);
                            manager.StopElasticService(parameters.GetSshNetworkAddress());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    };

                case EServiceAction.Information:
                    return () =>
                    {
                        ClusterStatsWindow csw = new ClusterStatsWindow(parameters.GetSshNetworkAddress());
                        csw.Show();
                    };

                case EServiceAction.Restart:
                    return () =>
                    {
                        try
                        {
                            manager = GetServiceManager(parameters.Ip, parameters.Port);
                            manager.RestartElasticService(parameters.GetSshNetworkAddress());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    };
                default: throw new NotImplementedException();
            }

            return () => { MessageBox.Show("Akcja usługi nie została zaimplementowana"); };
        }

        private static ServiceRemoteManager GetServiceManager(string ip, string port)
        {
            try
            {
                return new ServiceRemoteManager(
                    new SshConnectionInfo(
                        new NetworkAddress(ip, port),
                        SshLoginDataContainer.LoginData)
                );
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(
                    $@"Błąd podczas łączenia się przez SSH z usługą.
Upewnij się że dane logowania do ssh są uzupełnione w zakładce konfiguracyjnej.
                            
{ex.StackTrace}");
            }
        }
    }
}