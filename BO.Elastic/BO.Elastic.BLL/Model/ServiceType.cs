// ReSharper disable All
using System.Collections.Generic;

namespace BO.Elastic.BLL.Model
{
    public partial class ServiceType
    {
        public ServiceType()
        {
            Service = new HashSet<Service>();
        }

        public int Id { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Service> Service { get; set; }
    }
}