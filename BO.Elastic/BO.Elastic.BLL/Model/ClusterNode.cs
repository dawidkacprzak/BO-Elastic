// ReSharper disable All
namespace BO.Elastic.BLL.Model
{
    public partial class ClusterNode
    {
        public int Id { get; set; }
        public int ClusterId { get; set; }
        public int NodeId { get; set; }

        public virtual Service Cluster { get; set; }
        public virtual Service Node { get; set; }
    }
}