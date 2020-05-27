using System;
using System.Collections.Generic;

namespace BO.Elastic.BLL.Model
{
    public partial class Service
    {
        public int Id { get; set; }
        public int ServiceType { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }

        public virtual ServiceType ServiceTypeNavigation { get; set; }
        public virtual ClusterNode ClusterNodeNode { get; set; }
        public virtual ICollection<ClusterNode> ClusterNodeCluster { get; set; }
    }
}
