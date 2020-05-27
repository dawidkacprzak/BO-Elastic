using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nest;
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
        public List<Service> DownloadClustersConfiguration()
        {
            try
            {
                using (ElasticContext ctx = new ElasticContext())
                {
                    System.Diagnostics.Debug.WriteLine("Before get configuration");
                    return ctx.Service.Where(x => x.ServiceType == (int)EServiceType.Cluster)
                        .Include(x => x.ClusterNodeCluster)
                        .ThenInclude(x => x.Node).ToList<Service>();
                }
            }
            catch (InvalidOperationException)
            {
                System.Diagnostics.Debug.WriteLine("return empty conf");

                return new List<Service>();
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("return empty conf");

                return new List<Service>();
            }
        }
    }
}
