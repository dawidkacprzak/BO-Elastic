using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using BO.Elastic.BLL.Extension;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;

namespace BO.Elastic.BLL
{
    public class LoadedNodeController
    {
        private const int updateTaskCount = 5;
        private static readonly object updateLock = new object();
        private List<int> blockedNodesUpdate = new List<int>();
        private readonly Timer myTimer = new Timer();
        private int setSelectedClusterId;
        private readonly Action updateCallback;
        private readonly List<Task> updateTasks = new List<Task>();

        public LoadedNodeController()
        {
            ClusterNodes = new Dictionary<int, List<ServiceAddionalParameters>>();
        }

        public LoadedNodeController(List<Service> clusters, Action notifyCallback)
        {
            updateCallback = notifyCallback;
            ClusterNodes = new Dictionary<int, List<ServiceAddionalParameters>>();
            foreach (Service item in clusters)
            foreach (ClusterNode clusterFk in item.ClusterNodeCluster)
                if (ClusterNodes.ContainsKey(item.Id))
                {
                    ClusterNodes[item.Id].Add(new ServiceAddionalParameters
                    {
                        Ip = clusterFk.Node.Ip,
                        Port = clusterFk.Node.Port,
                        ServiceStatus = EServiceStatus.Initializing,
                        ServiceType = (EServiceType) clusterFk.Node.ServiceType,
                        Service = clusterFk.Node
                    });
                }
                else
                {
                    ClusterNodes[item.Id] = new List<ServiceAddionalParameters>();
                    ClusterNodes[item.Id].Add(new ServiceAddionalParameters
                    {
                        Ip = clusterFk.Node.Ip,
                        Port = clusterFk.Node.Port,
                        ServiceStatus = EServiceStatus.Initializing,
                        ServiceType = (EServiceType) clusterFk.Node.ServiceType,
                        Service = clusterFk.Node
                    });
                }

            if (ClusterNodes.Count > 0)
                setSelectedClusterId = ClusterNodes.First().Key;

            for (int i = 0; i < updateTaskCount; i++) AddUpdateTask();
            myTimer.Elapsed += StartUpdate;
            myTimer.Interval = 1000;
            myTimer.Start();
        }

        public Dictionary<int, List<ServiceAddionalParameters>> ClusterNodes { get; }

        public ObservableCollection<ServiceAddionalParameters> SelectedNodes
        {
            get
            {
                if (ClusterNodes.ContainsKey(setSelectedClusterId))
                    return new ObservableCollection<ServiceAddionalParameters>(ClusterNodes[setSelectedClusterId]);
                return new ObservableCollection<ServiceAddionalParameters>();
            }
        }

        public void SetSelectedClusterId(int id)
        {
            setSelectedClusterId = id;
        }

        private void AddUpdateTask()
        {
            updateTasks.Add(GetNewUpdateJob());
        }

        private Task GetNewUpdateJob()
        {
            return new Task(() =>
            {
                Random r = new Random();
                if (ClusterNodes.Count == 0) return;
                int rand = r.Next(0, ClusterNodes.Count - 1);
                lock (updateLock)
                {
                    if (blockedNodesUpdate.Contains(rand)) return;
                }

                var item = ClusterNodes.ElementAt(rand);
                int nodeIndex;
                ServiceAddionalParameters foundNode;
                lock (updateLock)
                {
                    foundNode = item.Value
                        .OrderBy(x => x.Service.LastUpdateTime).FirstOrDefault(x => !blockedNodesUpdate.Contains(x.Service.Id));
                    if (foundNode == null) return;

                    nodeIndex = ClusterNodes.ElementAt(rand).Value.IndexOf(foundNode);
                    blockedNodesUpdate.Add(foundNode.Service.Id);
                }

                foundNode = foundNode.Service.GetServiceAddionalParameters();
                ClusterNodes.ElementAt(rand).Value[nodeIndex] = foundNode;
                lock (updateLock)
                {
                    blockedNodesUpdate = blockedNodesUpdate.Where(x => x != foundNode.Service.Id).ToList();
                }
                updateCallback.Invoke();
            });
        }

        private void StartUpdate(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < updateTaskCount; i++)
                if (updateTasks.ElementAt(i) != null &&
                    updateTasks.ElementAt(i).Status != TaskStatus.Running)
                {
                    updateTasks[i] = GetNewUpdateJob();
                    updateTasks[i].Start();
                }
        }
    }
}