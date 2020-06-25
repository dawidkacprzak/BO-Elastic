using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
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
                    new KeyValuePair<string, string>("UUID",ClusterStatsResponse.ClusterUUID),
                    new KeyValuePair<string, string>("HTTP URI",ClusterStateResponse.ApiCall.Uri.AbsoluteUri),
                    new KeyValuePair<string, string>("CPU",ClusterStatsResponse.Nodes.Process.Cpu.Percent + "%"),
                    new KeyValuePair<string, string>("MEM",ClusterStatsResponse.Nodes.Jvm.Memory.HeapUsedInBytes/1024 + "/" + ClusterStatsResponse.Nodes.Jvm.Memory.HeapMaxInBytes/1024),
                    new KeyValuePair<string, string>("OS",NodeInfo.OperatingSystem.PrettyName),
                    new KeyValuePair<string, string>("Roles","["+string.Join("] [",NodeInfo.Roles)+"]"),
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
                {
                    clusterStatsResponse = nextWrap.GetClusterStats();
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
                    clusterStateResponse = nextWrap.GetClusterState();
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
                    nodeInfo = nextWrap.GetNodeInfo(clusterNetworkAddress);
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
                    return ClusterHealthResponse.Status.ToString();
                }
            }
        }
        public ClusterHealthResponse ClusterHealthResponse
        {
            get
            {
                if (clusterHealthResponse == null)
                {
                    clusterHealthResponse = nextWrap.GetClusterHealth();
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
            clusterNetworkAddress = networkAddress;
            nextWrap = new NextWrap(networkAddress);
            RefreshData();

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
                catch (ClusterNotConnectedException ex)
                {

                }
                finally
                {
                    NotifyPropertyChanged("ClusterAttributes");
                }
            }).Start();
        }
        private void RefreshData()
        {
            try
            {
                //GetCatIndices zwraca podsumowane info o wszystkich indexach:nazwy, health, status, primaries/replicas, docs count 
                catResponse = nextWrap.GetCatIndices();
                //getMappingResponse.Indices zwraca KeyValuePair ze wszystkimi indexami. Key - Nazwa indexu, Value ->Mappings->Properties-> mamy KeyValuePair, gdzie w Value mamy no Type: date, Name: DataDnia
                getMappingResponse = nextWrap.GetAllIndicesMapping();
                ClusterStatsResponse = nextWrap.GetClusterStats();
                ClusterStateResponse = nextWrap.GetClusterState();
                NodeInfo = nextWrap.GetNodeInfo(clusterNetworkAddress);
                ClusterHealthResponse = nextWrap.GetClusterHealth();

            }
            catch (Exception)
            {
                throw;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
