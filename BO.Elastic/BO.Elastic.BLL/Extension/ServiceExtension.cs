using System;
using System.Collections.Generic;
using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;
using Elasticsearch.Net;
using Nest;

namespace BO.Elastic.BLL.Extension
{
    public static class ServiceExtension
    {
        public static ServiceAddionalParameters GetServiceAddionalParameters(this Service service)
        {
            ServiceAddionalParameters addionalParameters = new ServiceAddionalParameters();
            addionalParameters.Ip = service.Ip;
            addionalParameters.Port = service.Port;
            addionalParameters.Service = service;
            addionalParameters.ActionList = new List<EServiceAction>();
            addionalParameters.ServiceType = (EServiceType) service.ServiceType;
            switch ((EServiceType) service.ServiceType)
            {
                case EServiceType.Node:

                    try
                    {
                        addionalParameters.NextWrap = new NextWrap(addionalParameters.GetSshNetworkAddress());
                        addionalParameters.ServiceStatus = EServiceStatus.Online;
                    }
                    catch (NodeNotConnectedException)
                    {
                        addionalParameters.ServiceStatus = addionalParameters.GetProperFailServiceStatus();
                    }
                    catch (ClusterNotConnectedException)
                    {
                        addionalParameters.ServiceStatus = addionalParameters.GetProperFailServiceStatus();
                    }

                    break;
                case EServiceType.Cluster:
                    try
                    {
                        addionalParameters.NextWrap = new NextWrap(addionalParameters.GetSshNetworkAddress());
                        ClusterHealthResponse clusterHealth = addionalParameters.NextWrap.GetClusterHealth();

                        if (clusterHealth.IsValid)
                        {
                            if (clusterHealth.Status == Health.Green)
                            {
                                addionalParameters.ServiceStatus = EServiceStatus.Online;
                                addionalParameters.ActionList.Add(EServiceAction.ConnectBySsh);
                                addionalParameters.ActionList.Add(EServiceAction.Stop);
                                addionalParameters.ActionList.Add(EServiceAction.Restart);
                                addionalParameters.ActionList.Add(EServiceAction.Information);
                            }
                            else if (clusterHealth.Status == Health.Red)
                            {
                                addionalParameters.ServiceStatus = EServiceStatus.Danger;
                                addionalParameters.ActionList.Add(EServiceAction.ConnectBySsh);
                                addionalParameters.ActionList.Add(EServiceAction.Restart);
                                addionalParameters.ActionList.Add(EServiceAction.Information);
                                addionalParameters.ActionList.Add(EServiceAction.Stop);
                            }
                            else if (clusterHealth.Status == Health.Yellow)
                            {
                                addionalParameters.ServiceStatus = EServiceStatus.Moderate;
                                addionalParameters.ActionList.Add(EServiceAction.ConnectBySsh);
                                addionalParameters.ActionList.Add(EServiceAction.Restart);
                                addionalParameters.ActionList.Add(EServiceAction.Information);
                                addionalParameters.ActionList.Add(EServiceAction.Stop);
                            }
                            else
                            {
                                throw new Exception("Błąd podczas pobrania statusu zdrowia klastra");
                            }
                        }
                        else
                        {
                            addionalParameters.ServiceStatus = addionalParameters.GetProperFailServiceStatus();
                            addionalParameters.ActionList.Add(EServiceAction.ConnectBySsh);
                            addionalParameters.ActionList.Add(EServiceAction.Information);
                            addionalParameters.ActionList.Add(EServiceAction.Start);
                        }
                    }
                    catch (ClusterNotConnectedException)
                    {
                        addionalParameters.ServiceStatus = addionalParameters.GetProperFailServiceStatus();
                        addionalParameters.ActionList.Add(EServiceAction.ConnectBySsh);
                        addionalParameters.ActionList.Add(EServiceAction.Information);
                        addionalParameters.ActionList.Add(EServiceAction.Start);
                    }

                    break;
                case EServiceType.Kibana:
                    break;
                default: throw new NotImplementedException();
            }

            return addionalParameters;
        }

        public static NetworkAddress GetSshNetworkAddress(this ServiceAddionalParameters parameters)
        {
            return new NetworkAddress(parameters.Ip, parameters.Port);
        }

        public static NetworkAddress GetSshNetworkAddress(this Service service)
        {
            return new NetworkAddress(service.Ip, service.Port);
        }

        public static void RefreshAddionalParameters(ref ServiceAddionalParameters serviceAddionalParameters)
        {
            serviceAddionalParameters = serviceAddionalParameters.Service.GetServiceAddionalParameters();
        }

        private static EServiceStatus GetProperFailServiceStatus(this ServiceAddionalParameters parameters)
        {
            EServiceStatus failStatus = EServiceStatus.Offline;
            if (NextWrap.PingHost(parameters.Ip)) failStatus = EServiceStatus.ServiceDown;
            return failStatus;
        }
    }
}