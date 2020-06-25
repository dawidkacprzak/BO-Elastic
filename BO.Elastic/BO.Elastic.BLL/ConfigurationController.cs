using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BO.Elastic.BLL
{
    public class ConfigurationController
    {
        /// <summary>
        ///     Fetch configuration contains clusters with iherited nodes.
        /// </summary>
        /// <returns>List of clusters with inherited nodes</returns>
        public List<Service> DownloadClustersConfiguration()
        {
            try
            {
                using (ElasticContext ctx = new ElasticContext())
                {
                    return ctx.Service.Where(x => x.ServiceType == (int) EServiceType.Cluster)
                        .Include(x => x.ClusterNodeCluster)
                        .ThenInclude(x => x.Node).ToList();
                }
            }
            catch (InvalidOperationException)
            {
                return new List<Service>();
            }
            catch (SqlException)
            {
                return new List<Service>();
            }
        }
    }
}