using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using Nest;
using Timer = System.Timers.Timer;

namespace BO.Elastic.Panel.ViewModels
{
    public class ClusterStatsWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<KeyValuePair<string, string>> clusterAttributes;
        private ClusterHealthResponse clusterHealthResponse;
        private string clusterName;
        private readonly NetworkAddress clusterNetworkAddress;
        private ClusterStateResponse clusterStateResponse;
        private ClusterStatsResponse clusterStatsResponse;

        private string clusterStatus;
        private readonly Timer myTimer = new Timer();

        private NextWrap nextWrap;

        private NodeInfo nodeInfo;

        public ClusterStatsWindowViewModel(NetworkAddress networkAddress, bool startUpdateThread = true)
        {
            try
            {
                clusterNetworkAddress = networkAddress;
                nextWrap = new NextWrap(networkAddress);
                RefreshData();
            }
            catch (ClusterNotConnectedException)
            {
                ClusterStatus = "Offline";
            }

            if (startUpdateThread)
            {
                myTimer.Elapsed += SetClusterStats;
                myTimer.Interval = 1000;
                myTimer.Start();
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> ClusterAttributes
        {
            get
            {
                clusterAttributes = new ObservableCollection<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Status", ClusterStatus),
                    new KeyValuePair<string, string>("UUID", GetNextWrapClusterData(EClusterAttributes.Uuid)),
                    new KeyValuePair<string, string>("HTTP URI", GetNextWrapClusterData(EClusterAttributes.HttpUri)),
                    new KeyValuePair<string, string>("CPU", GetNextWrapClusterData(EClusterAttributes.Cpu)),
                    new KeyValuePair<string, string>("RAM", GetNextWrapClusterData(EClusterAttributes.Ram)),
                    new KeyValuePair<string, string>("OS", GetNextWrapClusterData(EClusterAttributes.Os)),
                    new KeyValuePair<string, string>("ROLES", GetNextWrapClusterData(EClusterAttributes.Roles))
                };

                return clusterAttributes;
            }
            set
            {
                clusterAttributes = value;
                NotifyPropertyChanged();
            }
        }

        public ClusterStatsResponse ClusterStatsResponse
        {
            get
            {
                if (clusterStatsResponse == null)
                    clusterStatsResponse =
                        (ClusterStatsResponse) GetValueIfNextWrapIsInitialized(EClusterAttributes.Stats);
                return clusterStatsResponse;
            }
            set
            {
                clusterStatsResponse = value;
                NotifyPropertyChanged();
            }
        }

        public ClusterStateResponse ClusterStateResponse
        {
            get
            {
                if (clusterStateResponse == null)
                    clusterStateResponse =
                        (ClusterStateResponse) GetValueIfNextWrapIsInitialized(EClusterAttributes.State);
                return clusterStateResponse;
            }
            set
            {
                clusterStateResponse = value;
                NotifyPropertyChanged();
            }
        }

        public NodeInfo NodeInfo
        {
            get
            {
                if (nodeInfo == null)
                    nodeInfo = (NodeInfo) GetValueIfNextWrapIsInitialized(EClusterAttributes.NodeInfo);
                return nodeInfo;
            }
            set
            {
                if (value != nodeInfo)
                {
                    nodeInfo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ClusterStatus
        {
            get
            {
                if (ClusterHealthResponse == null) return "...";

                if (clusterStatus == null) clusterStatus = ClusterHealthResponse.Status.ToString();
                return clusterStatus;
            }
            set => clusterStatus = value;
        }

        public ClusterHealthResponse ClusterHealthResponse
        {
            get
            {
                if (clusterHealthResponse == null)
                    clusterHealthResponse =
                        (ClusterHealthResponse) GetValueIfNextWrapIsInitialized(EClusterAttributes.Health);
                return clusterHealthResponse;
            }
            set
            {
                if (value != clusterHealthResponse)
                {
                    clusterHealthResponse = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string PublishPort
        {
            get
            {
                if (nodeInfo != null)
                    return nodeInfo.Http.PublishAddress.Split(':')[1];
                return "...";
            }
        }

        public string TransportPort
        {
            get
            {
                if (nodeInfo != null)
                    return nodeInfo.TransportAddress.Split(':')[1];
                return "...";
            }
        }

        public string ClusterName
        {
            get => clusterName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    clusterName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private object GetValueIfNextWrapIsInitialized(EClusterAttributes value)
        {
            if (nextWrap == null) return null;
            switch (value)
            {
                case EClusterAttributes.Stats:
                    return nextWrap.GetClusterStats();
                case EClusterAttributes.State:
                    return nextWrap.GetClusterState();
                case EClusterAttributes.Health:
                    return nextWrap.GetClusterHealth();
                case EClusterAttributes.NodeInfo:
                    return nextWrap.GetNodeInfo(clusterNetworkAddress);
            }

            return null;
        }

        private string GetNextWrapClusterData(EClusterAttributes value)
        {
            if (nextWrap == null) return null;
            switch (value)
            {
                case EClusterAttributes.Uuid:
                    if (ClusterStateResponse != null)
                        return ClusterStateResponse.ClusterUUID;
                    else return "...";

                case EClusterAttributes.ClusterName:
                    if (ClusterStateResponse != null)
                        return ClusterStateResponse.ClusterName;
                    else return string.Empty;

                case EClusterAttributes.HttpUri:
                    if (ClusterStateResponse != null)
                        return ClusterStateResponse.ApiCall.Uri.AbsoluteUri;
                    else return string.Empty;

                case EClusterAttributes.Cpu:
                    if (ClusterStatsResponse != null)
                        return ClusterStatsResponse.Nodes.Process.Cpu.Percent + "%";
                    else return string.Empty;

                case EClusterAttributes.Ram:
                    if (ClusterStatsResponse != null)
                        return ClusterStatsResponse.Nodes.Jvm.Memory.HeapUsedInBytes / 1024 + "/" +
                               ClusterStatsResponse.Nodes.Jvm.Memory.HeapMaxInBytes / 1024;
                    else return string.Empty;

                case EClusterAttributes.Os:
                    if (NodeInfo != null)
                        return NodeInfo.OperatingSystem.PrettyName;
                    else return string.Empty;

                case EClusterAttributes.Roles:
                    if (NodeInfo != null)
                        return "[" + string.Join("] [", NodeInfo.Roles) + "]";
                    else return string.Empty;
            }

            throw new NotImplementedException("Brak definicji dla atrybutu klastra");
        }

        private void SetClusterStats(object sender, ElapsedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    RefreshData();
                }
                catch (ClusterNotConnectedException)
                {
                    ClusterStatus = "Offline";
                }
                finally
                {
                    NotifyPropertyChanged("ClusterAttributes");
                    NotifyPropertyChanged("PublishPort");
                    NotifyPropertyChanged("TransportPort");
                }
            }).Start();
        }

        private void RefreshData()
        {
            try
            {
                nextWrap ??= new NextWrap(clusterNetworkAddress);
                ClusterStatsResponse = nextWrap.GetClusterStats();
                ClusterStateResponse = nextWrap.GetClusterState();
                NodeInfo = nextWrap.GetNodeInfo(clusterNetworkAddress);
                ClusterHealthResponse = nextWrap.GetClusterHealth();
                ClusterStatus = ClusterHealthResponse.Status.ToString();
                ClusterName = GetNextWrapClusterData(EClusterAttributes.ClusterName);
            }
            catch (NodeNotConnectedException)
            {
                ClusterStatus = "Offline";
                nodeInfo = null;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}