using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Extension;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceConnection;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;
using BO.Elastic.Panel.Helpers;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace BO.Elastic.Panel.ClassExtensions
{
    public static class ServiceExtension
    {
        public static Action GetActionParameters(this ServiceAddionalParameters parameters, EServiceAction eServiceAction)
        {
            ServiceRemoteManager manager = new ServiceRemoteManager(
                new SSHConnectionInfo(
                    new NetworkAddress(parameters.IP, ""),
                    SSHLoginDataContainer.LoginData)
                );

            try
            {
                switch (eServiceAction)
                {
                    case EServiceAction.ConnectBySSH:
                        break;

                    case EServiceAction.Start:
                        return new Action(() =>
                            {
                                try
                                {
                                    manager.StartElasticService(parameters.GetSSHNetworkAddress());
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        );

                    case EServiceAction.Stop:
                        return new Action(() =>
                        {
                            try
                            {
                                manager.StopElasticService(parameters.GetSSHNetworkAddress());
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        );

                    case EServiceAction.Information:
                        return new Action(() =>
                        {
                            NextWrap nextWrap = new NextWrap(parameters.GetSSHNetworkAddress());
                            ClusterStateResponse test = nextWrap.GetClusterState();
                            ClusterStatsResponse test2 = nextWrap.GetClusterStats();
                            int percentageMemoryUsage = test2.Nodes.OperatingSystem.Memory.UsedPercent;
                            int percentageCpuUsage = test2.Nodes.Process.Cpu.Percent;
                        });

                    case EServiceAction.Restart:
                        return new Action(() =>
                        {
                            try
                            {
                                manager.RestartElasticService(parameters.GetSSHNetworkAddress());
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    );
                    default: throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return new Action(() => { MessageBox.Show("Oj oj, coś poszło nie tak :/"); });
        }
    }
}
