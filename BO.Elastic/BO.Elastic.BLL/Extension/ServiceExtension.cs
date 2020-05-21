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
            addionalParameters.ActionList = new List<KeyValuePair<string, Action>>();
            switch ((EServiceType)service.ServiceType)
            {
                case EServiceType.Node:
                    addionalParameters.IP = service.Ip;
                    addionalParameters.Port = service.Port;
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

                    break;
                case EServiceType.Cluster:
                    break;
                case EServiceType.Kibana:
                    break;
                default: throw new NotImplementedException();
            }

            return addionalParameters;
        }
    }
}
