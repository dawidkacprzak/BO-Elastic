using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using BO.Elastic.BLL.Model;

namespace BO.Elastic.BLL.Model
{
    [Serializable]
    public partial class ClusterNode { }

    [Serializable]
    public partial class Service
    {
        [NotMapped]
        private DateTime lastUpdateTime;
        [NotMapped]
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

    [Serializable]
    public partial class LoginData { }
}
