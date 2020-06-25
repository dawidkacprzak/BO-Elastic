using System.Linq;
using BO.Elastic.BLL.Extension;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceExtenstionModel;
using BO.Elastic.BLL.Types;
using NUnit.Framework;

namespace BO.Elastic.BLL.Test
{
    public class DbIntegration
    {
        [Test]
        public void TestDownloadedConfigurationCheckAllServicesFromFetchedClustersAreNodeType()
        {
            ConfigurationController controller = new ConfigurationController();
            var configuration = controller.DownloadClustersConfiguration();
            Assert.IsNotNull(configuration);
            Assert.IsTrue(configuration.All(x => x.ServiceType == (int) EServiceType.Cluster));
            Assert.IsTrue(configuration.Where(x => x.ClusterNodeCluster.Count > 0).Count() > 0);
            Assert.IsTrue(configuration.All(x =>
                x.ClusterNodeCluster.All(x => x.Node.ServiceType == (int) EServiceType.Node)));
        }

        [Test]
        public void GetAddionalParametersFromNodeService()
        {
            ConfigurationController controller = new ConfigurationController();
            var configuration = controller.DownloadClustersConfiguration();
            Service firstNode = configuration.First().ClusterNodeCluster.First().Node;
            ServiceAddionalParameters nodeParameters = firstNode.GetServiceAddionalParameters();
            Assert.IsTrue(nodeParameters.ServiceStatus == EServiceStatus.Online);
            Assert.IsTrue(!string.IsNullOrEmpty(nodeParameters.Ip));
            Assert.IsTrue(!string.IsNullOrEmpty(nodeParameters.Port));
            Assert.IsTrue(nodeParameters.ActionList != null);
        }

        [Test]
        public void GetAddionalParametersFromNodeServiceFail()
        {
            ConfigurationController controller = new ConfigurationController();
            var configuration = controller.DownloadClustersConfiguration();
            Service firstNode = new Service
            {
                Ip = "0.0.0.0",
                Port = "25565",
                ClusterNodeNode = new ClusterNode
                {
                    Cluster = new Service
                    {
                        Ip = "localhost",
                        Port = "25565"
                    }
                },
                ServiceType = (int) EServiceType.Node
            };

            ServiceAddionalParameters nodeParameters = firstNode.GetServiceAddionalParameters();
            Assert.IsTrue(nodeParameters.ServiceStatus == EServiceStatus.Offline);
            Assert.IsTrue(!string.IsNullOrEmpty(nodeParameters.Ip));
            Assert.IsTrue(!string.IsNullOrEmpty(nodeParameters.Port));
            Assert.IsTrue(nodeParameters.ActionList != null);
        }

        [Test]
        public void GetAddionalParametersFromClusterService()
        {
            ConfigurationController controller = new ConfigurationController();
            var configuration = controller.DownloadClustersConfiguration();
            Service firstCluster = configuration.First();
            ServiceAddionalParameters nodeParameters = firstCluster.GetServiceAddionalParameters();
            Assert.IsTrue(nodeParameters.ServiceStatus == EServiceStatus.Online ||
                          nodeParameters.ServiceStatus == EServiceStatus.Moderate);
            Assert.IsTrue(!string.IsNullOrEmpty(nodeParameters.Ip));
            Assert.IsTrue(!string.IsNullOrEmpty(nodeParameters.Port));
            Assert.IsTrue(nodeParameters.ActionList != null);
        }

        [Test]
        public void GetAddionalParametersFromClusterServiceFail()
        {
            ConfigurationController controller = new ConfigurationController();
            Service cluster = new Service
            {
                Ip = "98.14.75.177",
                Port = "80",
                ServiceType = (int) EServiceType.Cluster
            };

            ServiceAddionalParameters nodeParameters = cluster.GetServiceAddionalParameters();
            Assert.IsTrue(nodeParameters.ServiceStatus == EServiceStatus.Offline);
            Assert.IsTrue(!string.IsNullOrEmpty(nodeParameters.Ip));
            Assert.IsTrue(!string.IsNullOrEmpty(nodeParameters.Port));
            Assert.IsTrue(nodeParameters.ActionList != null);
        }
    }
}