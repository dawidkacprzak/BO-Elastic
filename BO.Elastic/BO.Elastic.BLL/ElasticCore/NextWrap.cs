using BO.Elastic.BLL.Exceptions;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
                elasticClient = new ElasticClient(settings);
                string ip = clusterAddress.Split(':')[1].Replace("/", string.Empty);
                int port = int.Parse(clusterAddress.Split(':')[2]);
                if (PingHost(ip, port)){
                    PingResponse pr = elasticClient.Ping();
                    if (pr.ApiCall.HttpStatusCode != 200)
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
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

        public static bool PingHost(string hostUri, int portNumber)
        {
            var client = new TcpClient();
            var result = client.BeginConnect(hostUri, portNumber, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            if (!success)
            {
                return false;
            }
            return true;

        }
    }
}
