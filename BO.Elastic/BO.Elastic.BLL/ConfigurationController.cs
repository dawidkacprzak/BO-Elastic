using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BO.Elastic.BLL
{
    public class ConfigurationController
    {
        /// <summary>
        /// Fetch configuration contains clusters with iherited nodes.
        /// </summary>
        /// <returns>List of clusters with inherited nodes</returns>
        public List<Service> DownloadConfiguration()
        {
            using(ElasticContext ctx = new ElasticContext())
            {
                return ctx.Service.Where(x => x.ServiceType == (int)EServiceType.Cluster)
                    .Include(x => x.ClusterNodeCluster)
                    .ThenInclude(x => x.Node).ToList<Service>();
            }
        }
    }
}
