using BO.Elastic.BLL.Exceptions;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BO.Elastic.BLL.ElasticCore
{
    public class NextWrap
    {
        private ElasticClient elasticClient;

        public NextWrap(string clusterAddress)
        {
            try
            {
                ConnectionSettings settings = new ConnectionSettings(new Uri(clusterAddress));
                settings.PingTimeout(TimeSpan.FromMilliseconds(1000));
                settings.DeadTimeout(new TimeSpan(0, 0, 1));
                settings.MaxDeadTimeout(TimeSpan.FromMilliseconds(1000));
                settings.MaxRetryTimeout(new TimeSpan(0, 0, 1));
                settings.MaximumRetries(1);
                System.Diagnostics.Debug.WriteLine("Wrap - before");
                elasticClient = new ElasticClient(settings);
                PingResponse pr = elasticClient.Ping();
                if (pr.ApiCall.HttpStatusCode != 200)
                {

                    throw new Exception();
                }
                System.Diagnostics.Debug.WriteLine("Wrap - after");
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Wrap - after");

                throw new ClusterNotConnectedException(clusterAddress);
            }
        }


        public NodeInfo GetNodeInfo(string ip, string port)
        {
            NodeExists(ip, port);
            return GetNodesFromIpAndPort(ip, port).First().Value;
        }

        public ClusterHealthResponse GetClusterHealth()
        {
            return elasticClient.Cluster.Health();
        }

        private bool NodeExists(string ip, string port)
        {
            IEnumerable<KeyValuePair<string, NodeInfo>> nodes = GetNodesFromIpAndPort(ip, port);
            if (nodes.Count() == 0)
            {
                throw new NodeNotConnectedException(ip + ":" + port);
            }
            else if (nodes.Count() > 1)
            {
                throw new Exception("Znaleziono duplikat nodów, taka sytuacja nie powinna zaistnieć");
            }
            else
            {
                return true;
            }
        }

        private IEnumerable<KeyValuePair<string, NodeInfo>> GetNodesFromIpAndPort(string ip, string port)
        {
            return elasticClient.Nodes.Info().Nodes.Where(x => x.Value.Http.PublishAddress.Equals(ip + ":" + port));

        }
    }
}
