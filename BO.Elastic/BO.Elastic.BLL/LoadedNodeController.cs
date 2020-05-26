using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BO.Elastic.BLL
{
    public class LoadedNodeController
    {
        private ObservableCollection<ServiceAddionalParameters> loadedServices;
        public List<int> SelectedNodeIds = new List<int>();

        public LoadedNodeController(ObservableCollection<ServiceAddionalParameters> Clusters)
        {
            RefreshServices(Clusters);
        }


        public void RefreshServices(ObservableCollection<ServiceAddionalParameters> Clusters)
        {
            loadedServices = new ObservableCollection<ServiceAddionalParameters>();
            foreach (var cluster in Clusters)
            {
                if (cluster.Service != null && cluster.Service.ClusterNodeCluster != null)
                {
                    foreach (var item in cluster.Service.ClusterNodeCluster)
                    {
                        loadedServices.Add(new ServiceAddionalParameters()
                        {
                            Service = item.Node,
                            IP = item.Node.Ip,
                            Port = item.Node.Port,
                            ServiceStatus = EServiceStatus.Initializing,
                            ServiceType = (EServiceType)item.Node.ServiceType
                        });
                    }
                }
            }
        }
    }
}
