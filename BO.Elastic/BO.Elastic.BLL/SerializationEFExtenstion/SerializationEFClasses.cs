using System;
using System.Collections.Generic;
using System.Text;
using BO.Elastic.BLL.Model;

namespace BO.Elastic.BLL.Model
{
    [Serializable]
    public partial class ClusterNode { }

    [Serializable]
    public partial class Service
    {
        private DateTime lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get
            {
                if (lastUpdateTime == null)
                {
                    return DateTime.Now;
                }
                return lastUpdateTime;
            }
            set
            {
                lastUpdateTime = value;
            }
        }
        public Service()
        {

            ClusterNodeCluster = new HashSet<ClusterNode>();
            LastUpdateTime = DateTime.Now;
        }
    }

    [Serializable]
    public partial class ServiceType { }
}
