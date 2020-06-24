using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Model;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BO.Elastic.Panel.ViewModels
{
    public class ClusterStatsWindowViewModel : INotifyPropertyChanged
    {
        private NextWrap nextWrap;
        private ClusterStatsResponse clusterStatsResponse;
        private ClusterStateResponse clusterStateResponse;

        public ClusterStatsResponse ClusterStatsResponse
        {
            get
            {
                if (ClusterStatsResponse == null)
                {
                    ClusterStatsResponse = nextWrap.GetClusterStats();
                }
                return ClusterStatsResponse;
            }
            set
            {
                ClusterStatsResponse = value;
                NotifyPropertyChanged();
            }
        }

        public ClusterStateResponse ClusterStateResponse
        {
            get
            {
                if (ClusterStateResponse == null)
                {
                    ClusterStateResponse = nextWrap.GetClusterState();
                }
                return ClusterStateResponse;
            }
            set
            {
                ClusterStateResponse = value;
                NotifyPropertyChanged();
            }
        }

        public ClusterStatsWindowViewModel(NetworkAddress networkAddress)
        {
            nextWrap = new NextWrap(networkAddress);
            clusterStatsResponse = nextWrap.GetClusterStats();
            clusterStateResponse = nextWrap.GetClusterState();

        }

        private void SetClusterStats()
        {
/*            clusterStats.PercentageMemoryUsage = clusterStatsResponse.Nodes.OperatingSystem.Memory.UsedPercent;
            clusterStats.PercentageCpuUsage = clusterStatsResponse.Nodes.Process.Cpu.Percent;
            string ip = clusterStatsResponse.ApiCall.Uri.Host;
            string port = clusterStatsResponse.ApiCall.Uri.Port.ToString();
            clusterStats.NetworkAddress = new NetworkAddress(ip, port);*/
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
