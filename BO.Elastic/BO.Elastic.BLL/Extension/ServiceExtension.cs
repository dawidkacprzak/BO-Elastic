using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Extension
{
    public static class ServiceExtension
    {
        public static ServiceAddionalParameters GetServiceAddionalParameters(this Service service)
        {
            ServiceAddionalParameters addionalParameters = new ServiceAddionalParameters();
            addionalParameters.IP = service.Ip;
            addionalParameters.Port = service.Port;
            addionalParameters.Service = service;
            addionalParameters.ActionList = new List<KeyValuePair<string, Action>>();
            addionalParameters.ServiceType = (EServiceType)service.ServiceType;
            switch ((EServiceType)service.ServiceType)
            {
                case EServiceType.Node:

                    try
                    {
                        addionalParameters.NextWrap = new NextWrap("http://" + service.ClusterNodeNode.Cluster.Ip + ":" + service.ClusterNodeNode.Cluster.Port);
                        NodeInfo nodeInfo = addionalParameters.NextWrap.GetNodeInfo(service.Ip, service.Port);
                        addionalParameters.ServiceStatus = EServiceStatus.Online;
                    }
                    catch (NodeNotConnectedException)
                    {
                        addionalParameters.ServiceStatus = EServiceStatus.Offline;
                    }
                    catch (ClusterNotConnectedException)
                    {
                        addionalParameters.ServiceStatus = EServiceStatus.Offline;
                    }

                    break;
                case EServiceType.Cluster:
                    try
                    {
                        addionalParameters.NextWrap = new NextWrap("http://" + service.Ip + ":" + service.Port);
                        ClusterHealthResponse clusterHealth = addionalParameters.NextWrap.GetClusterHealth();
                        if (clusterHealth.IsValid)
                        {
                            if (clusterHealth.Status == Elasticsearch.Net.Health.Green)
                            {
                                addionalParameters.ServiceStatus = EServiceStatus.Online;
                            }
                            else if (clusterHealth.Status == Elasticsearch.Net.Health.Red)
                            {
                                addionalParameters.ServiceStatus = EServiceStatus.Danger;
                            }
                            else if (clusterHealth.Status == Elasticsearch.Net.Health.Yellow)
                            {
                                addionalParameters.ServiceStatus = EServiceStatus.Moderate;
                            }
                            else throw new Exception("Błąd podczas pobrania statusu zdrowia klastra");
                        }
                        else
                        {
                            addionalParameters.ServiceStatus = EServiceStatus.Offline;
                        }
                    }
                    catch (ClusterNotConnectedException)
                    {
                        addionalParameters.ServiceStatus = EServiceStatus.Offline;
                    }

                    break;
                case EServiceType.Kibana:
                    break;
                default: throw new NotImplementedException();
            }

            return addionalParameters;
        }
    }
}
