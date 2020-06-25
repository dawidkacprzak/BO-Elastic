using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace BO.Elastic.Panel.ViewModels
{
    public class ClusterStatsWindowViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<KeyValuePair<string, string>> ClusterAttributes
        {
            get
            {

                clusterAttributes = new ObservableCollection<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("Status",ClusterStatus),
                    new KeyValuePair<string, string>("UUID",(string)GetNextWrapClusterData(EClusterAttributes.UUID)),
                    new KeyValuePair<string, string>("HTTP URI",(string)GetNextWrapClusterData(EClusterAttributes.HTTPUri)),
                    new KeyValuePair<string, string>("CPU",(string)GetNextWrapClusterData(EClusterAttributes.CPU)),
                    new KeyValuePair<string, string>("RAM",(string)GetNextWrapClusterData(EClusterAttributes.RAM)),
                    new KeyValuePair<string, string>("OS",(string)GetNextWrapClusterData(EClusterAttributes.OS)),
                    new KeyValuePair<string, string>("ROLES",(string)GetNextWrapClusterData(EClusterAttributes.Roles))
                    };

                return clusterAttributes;
            }
            set
            {
                clusterAttributes = value;
                NotifyPropertyChanged();
            }
        }

        private object GetValueIfNextWrapIsInitialized(EClusterAttributes value)
        {
            if (nextWrap == null)
            {
                return null;
            }
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
            if (nextWrap == null)
            {
                return null;
            }
            switch (value)
            {
                case EClusterAttributes.UUID:
                    if (ClusterStateResponse != null)
                    {
                        return ClusterStateResponse.ClusterUUID;
                    }
                    else return "...";

                case EClusterAttributes.ClusterName:
                    if (ClusterStateResponse != null)
                    {
                        return ClusterStateResponse.ClusterName;
                    }
                    else return string.Empty;

                case EClusterAttributes.HTTPUri:
                    if (ClusterStateResponse != null)
                    {
                        return ClusterStateResponse.ApiCall.Uri.AbsoluteUri;
                    }
                    else return string.Empty;

                case EClusterAttributes.CPU:
                    if (ClusterStatsResponse != null)
                    {
                        return ClusterStatsResponse.Nodes.Process.Cpu.Percent + "%";
                    }
                    else return string.Empty;

                case EClusterAttributes.RAM:
                    if (ClusterStatsResponse != null)
                    {
                        return ClusterStatsResponse.Nodes.Jvm.Memory.HeapUsedInBytes / 1024 + "/" + ClusterStatsResponse.Nodes.Jvm.Memory.HeapMaxInBytes / 1024;
                    }
                    else return string.Empty;

                case EClusterAttributes.OS:
                    if (NodeInfo != null)
                    {
                        return NodeInfo.OperatingSystem.PrettyName;
                    }
                    else return string.Empty;

                case EClusterAttributes.Roles:
                    if (NodeInfo != null)
                    {
                        return "[" + string.Join("] [", NodeInfo.Roles) + "]";
                    }
                    else return string.Empty;
            }
            throw new NotImplementedException("Brak definicji dla atrybutu klastra");
        }

        public ClusterStatsResponse ClusterStatsResponse
        {
            get
            {
                if (clusterStatsResponse == null)
                {
                    clusterStatsResponse = (ClusterStatsResponse)GetValueIfNextWrapIsInitialized(EClusterAttributes.Stats);
                }
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
                {
                    clusterStateResponse = (ClusterStateResponse)GetValueIfNextWrapIsInitialized(EClusterAttributes.State);
                }
                return clusterStateResponse;
            }
            set
            {
                clusterStateResponse = value;
                NotifyPropertyChanged();
            }
        }

        private NodeInfo nodeInfo;

        public NodeInfo NodeInfo
        {
            get
            {
                if (nodeInfo == null)
                {
                    nodeInfo = (NodeInfo)GetValueIfNextWrapIsInitialized(EClusterAttributes.NodeInfo);
                }
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

        private string clusterStatus;
        public string ClusterStatus
        {
            get
            {
                if (ClusterHealthResponse == null)
                {
                    return "...";
                }
                else
                {
                    if (clusterStatus == null)
                    {
                        clusterStatus = ClusterHealthResponse.Status.ToString();
                    }
                    return clusterStatus;
                }
            }
            set
            {
                clusterStatus = value;
            }
        }
        public ClusterHealthResponse ClusterHealthResponse
        {
            get
            {
                if (clusterHealthResponse == null)
                {
                    clusterHealthResponse = (ClusterHealthResponse)GetValueIfNextWrapIsInitialized(EClusterAttributes.Health);
                }
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
                {
                    return nodeInfo.Http.PublishAddress.Split(':')[1];
                }
                else
                {
                    return "...";
                }
            }
        }

        public string TransportPort
        {
            get
            {
                if (nodeInfo != null)
                {
                    return nodeInfo.TransportAddress.Split(':')[1];
                }
                else
                {
                    return "...";
                }
            }
        }
        private string clusterName;
        public string ClusterName
        {
            get
            {
                return clusterName;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    clusterName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private NextWrap nextWrap;
        private NetworkAddress clusterNetworkAddress;
        private ClusterStatsResponse clusterStatsResponse;
        private ClusterStateResponse clusterStateResponse;
        private ClusterHealthResponse clusterHealthResponse;
        private GetMappingResponse getMappingResponse;
        private CatResponse<CatIndicesRecord> catResponse;
        private ObservableCollection<KeyValuePair<string, string>> clusterAttributes;
        private System.Timers.Timer myTimer = new System.Timers.Timer();


        public ClusterStatsWindowViewModel(NetworkAddress networkAddress)
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

            myTimer.Elapsed += SetClusterStats; ;
            myTimer.Interval = 1000;
            myTimer.Start();
        }

        private void SetClusterStats(object sender, System.Timers.ElapsedEventArgs e)
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
                if (nextWrap == null)
                {
                    nextWrap = new NextWrap(clusterNetworkAddress);
                }
                //GetCatIndices zwraca podsumowane info o wszystkich indexach:nazwy, health, status, primaries/replicas, docs count 
                catResponse = nextWrap.GetCatIndices();
                //getMappingResponse.Indices zwraca KeyValuePair ze wszystkimi indexami. Key - Nazwa indexu, Value ->Mappings->Properties-> mamy KeyValuePair, gdzie w Value mamy no Type: date, Name: DataDnia
                getMappingResponse = nextWrap.GetAllIndicesMapping();
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
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
