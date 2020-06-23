using BO.Elastic.BLL.Extension;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;
using Nest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace BO.Elastic.BLL
{
    public class LoadedNodeController
    {
        public Dictionary<int, List<ServiceAddionalParameters>> ClusterNodes { get; private set; }
        private System.Timers.Timer myTimer = new System.Timers.Timer();
        private List<Task> updateTasks = new List<Task>();
        private const int updateTaskCount = 5;
        private List<int> blockedNodesUpdate = new List<int>();
        private static readonly object updateLock = new object();
        private int setSelectedClusterId = 0;
        private Action updateCallback;
        public ObservableCollection<ServiceAddionalParameters> SelectedNodes
        {
            get
            {
                if (ClusterNodes.ContainsKey(setSelectedClusterId)){
                    return new ObservableCollection<ServiceAddionalParameters>(ClusterNodes[setSelectedClusterId]);
                }
                else
                {
                    return new ObservableCollection<ServiceAddionalParameters>();
                }
            }
        }
        public LoadedNodeController()
        {
            ClusterNodes = new Dictionary<int, List<ServiceAddionalParameters>>();
        }

        public LoadedNodeController(List<Service> clusters, Action notifyCallback)
        {
            updateCallback = notifyCallback;
            ClusterNodes = new Dictionary<int, List<ServiceAddionalParameters>>();
            foreach (var item in clusters)
            {
                foreach (var clusterFK in item.ClusterNodeCluster)
                {
                    if (ClusterNodes.ContainsKey(item.Id))
                    {
                        ClusterNodes[item.Id].Add(new ServiceAddionalParameters()
                        {
                            IP = clusterFK.Node.Ip,
                            Port = clusterFK.Node.Port,
                            ServiceStatus = EServiceStatus.Initializing,
                            ServiceType = (EServiceType)clusterFK.Node.ServiceType,
                            Service = clusterFK.Node
                        });
                    }
                    else
                    {
                        ClusterNodes[item.Id] = new List<ServiceAddionalParameters>();
                        ClusterNodes[item.Id].Add(new ServiceAddionalParameters()
                        {
                            IP = clusterFK.Node.Ip,
                            Port = clusterFK.Node.Port,
                            ServiceStatus = EServiceStatus.Initializing,
                            ServiceType = (EServiceType)clusterFK.Node.ServiceType,
                            Service = clusterFK.Node
                        });
                    }
                }
            }
            if(ClusterNodes.Count > 0)
                setSelectedClusterId = ClusterNodes.First().Key;

            for (int i = 0; i < updateTaskCount; i++)
            {
                AddUpdateTask();
            }
            myTimer.Elapsed += StartUpdate;
            myTimer.Interval = 1000;
            myTimer.Start();
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
                System.Diagnostics.Debug.WriteLine("Start new up task");
                Random r = new Random();
                if (ClusterNodes.Count == 0) return;
                int rand = r.Next(0, ClusterNodes.Count - 1);
                if (blockedNodesUpdate.Contains(rand)) return;
                System.Diagnostics.Debug.WriteLine("Task get random cluster: " + rand);

                var item = ClusterNodes.ElementAt(rand);
                int nodeIndex = -1;
                ServiceAddionalParameters foundNode = null;
                lock (updateLock)
                {
                    foundNode = item.Value.OrderBy(x => x.Service.LastUpdateTime).Where(x => !blockedNodesUpdate.Contains(x.Service.Id)).FirstOrDefault();
                    if (foundNode == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Dispose, found is null");
                        return;
                    }
                    System.Diagnostics.Debug.WriteLine("Update task: update");

                    nodeIndex = ClusterNodes.ElementAt(rand).Value.IndexOf(foundNode);
                    blockedNodesUpdate.Add(foundNode.Service.Id);
                    System.Diagnostics.Debug.WriteLine("Update task: add block service:" + foundNode.Service.Id);

                }
                foundNode = foundNode.Service.GetServiceAddionalParameters();
                ClusterNodes.ElementAt(rand).Value[nodeIndex] = foundNode;
                blockedNodesUpdate = blockedNodesUpdate.Where(x => x != foundNode.Service.Id).ToList();
                updateCallback.Invoke();
                System.Diagnostics.Debug.WriteLine("Update task: remove block service:" + foundNode.Service.Id);
            });
        }

        private void StartUpdate(object sender, ElapsedEventArgs e)
        {
            for (int i = 0; i < updateTaskCount; i++)
            {
                if (updateTasks.ElementAt(i) != null &&
                    updateTasks.ElementAt(i).Status != System.Threading.Tasks.TaskStatus.Running)
                {
                    updateTasks[i] = GetNewUpdateJob();
                    updateTasks[i].Start();
                }
            }
        }
    }
}
