using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using Elasticsearch.Net;
using Nest;

namespace BO.Elastic.BLL.ElasticCore
{
    public class NextWrap
    {
        private readonly ElasticClient elasticClient;

        public NextWrap(NetworkAddress address)
        {
            try
            {
                ConnectionSettings settings = new ConnectionSettings(new Uri(address.HttpAddress));
                settings.PingTimeout(TimeSpan.FromMilliseconds(1000));
                settings.DeadTimeout(new TimeSpan(0, 0, 1));
                settings.MaxDeadTimeout(TimeSpan.FromMilliseconds(1000));
                settings.MaxRetryTimeout(new TimeSpan(0, 0, 1));
                settings.MaximumRetries(1);
                elasticClient = new ElasticClient(settings);
                if (PingHost(address.Ip))
                {
                    PingResponse pr = elasticClient.Ping();
                    if (pr.ApiCall.HttpStatusCode != 200) throw new Exception();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new ClusterNotConnectedException(address.IpPortMerge);
            }
        }


        public NodeInfo GetNodeInfo(NetworkAddress address)
        {
            NodeExists(address);
            var nodeinfos = GetNodesFromIpAndPort(address);
            if (nodeinfos == null || nodeinfos.Count() == 0) throw new NodeNotConnectedException(address.IpPortMerge);
            return GetNodesFromIpAndPort(address).First().Value;
        }

        public ClusterHealthResponse GetClusterHealth()
        {
            try
            {
                return elasticClient.Cluster.Health();
            }
            catch (UnexpectedElasticsearchClientException)
            {
                return new ClusterHealthResponse();
            }
        }

        public ClusterStateResponse GetClusterState()
        {
            try
            {
                return elasticClient.Cluster.State();
            }
            catch (UnexpectedElasticsearchClientException)
            {
                return new ClusterStateResponse();
            }
        }

        public ClusterStatsResponse GetClusterStats()
        {
            try
            {
                return elasticClient.Cluster.Stats();
            }
            catch (UnexpectedElasticsearchClientException)
            {
                return new ClusterStatsResponse();
            }
        }

        public CatResponse<CatIndicesRecord> GetCatIndices()
        {
            try
            {
                return elasticClient.Cat.Indices();
            }
            catch (UnexpectedElasticsearchClientException)
            {
                return new CatResponse<CatIndicesRecord>();
            }
        }

        public GetMappingResponse GetAllIndicesMapping()
        {
            try
            {
                return elasticClient.Indices.GetMapping(new GetMappingRequest(Indices.AllIndices));
            }
            catch (UnexpectedElasticsearchClientException)
            {
                return new GetMappingResponse();
            }
        }

        public CreateIndexResponse CreateIndex<T>(string indexName, IndexState indexState, T typeObject) where T: class
        {
            try
            {
                if (!elasticClient.Indices.Exists(indexName).Exists)
                {
                    CreateIndexResponse test = elasticClient.Indices.Create(indexName, x => x.InitializeUsing(indexState).Map<T>(mp => mp.AutoMap(1)));
                        return test;
                }
                else
                {
                    throw new IndexAlreadyExistsException(indexName);
                }                
            }
            catch (UnexpectedElasticsearchClientException)
            {
                throw;
            }
        }

        public DeleteIndexResponse DeleteIndex(string indexName)
        {
            try
            {
                if (elasticClient.Indices.Exists(indexName).Exists)
                {
                    return elasticClient.Indices.Delete(indexName);
                }
                else
                {
                    throw new IndexDoesntExistException(indexName);
                }
            }
            catch (UnexpectedElasticsearchClientException)
            {
                throw;
            }
        }


        private bool NodeExists(NetworkAddress address)
        {
            var nodes = GetNodesFromIpAndPort(address);
            var keyValuePairs = nodes.ToList();
            if (!keyValuePairs.Any())
                throw new NodeNotConnectedException(address.IpPortMerge);
            if (keyValuePairs.Count() > 1)
                throw new Exception("Znaleziono duplikat nodów, taka sytuacja nie powinna zaistnieć");
            return true;
        }

        private IEnumerable<KeyValuePair<string, NodeInfo>> GetNodesFromIpAndPort(NetworkAddress address)
        {
            return elasticClient.Nodes.Info().Nodes.Where(x => x.Value.Http.PublishAddress.Equals(address.IpPortMerge));
        }

        public static bool PingHost(string hostUri)
        {
            try
            {
                PingReply pingReply = new Ping().Send(hostUri, 1000);

                return pingReply == null || pingReply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}