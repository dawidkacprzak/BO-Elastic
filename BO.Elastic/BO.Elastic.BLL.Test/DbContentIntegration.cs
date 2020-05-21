using BO.Elastic.BLL.Extension;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace BO.Elastic.BLL.Test
{
    public class DbIntegration
    {
        [Test]
        public void TestDownloadedConfigurationCheckAllServicesFromFetchedClustersAreNodeType()
        {
            ConfigurationController controller = new ConfigurationController();
            List<Service> configuration = controller.DownloadConfiguration();
            Assert.IsNotNull(configuration);
            Assert.IsTrue(configuration.All(x => x.ServiceType == (int)EServiceType.Cluster));
            Assert.IsTrue(configuration.Where(x => x.ClusterNodeCluster.Count > 0).Count() > 0);
            Assert.IsTrue(configuration.All(x => x.ClusterNodeCluster.All(x => x.Node.ServiceType == (int)EServiceType.Node)));
        }

        [Test]
        public void GetAddionalParametersFromNodeService()
        {
            ConfigurationController controller = new ConfigurationController();
            List<Service> configuration = controller.DownloadConfiguration();
            Service firstNode = configuration.First().ClusterNodeCluster.First().Node;
            firstNode.GetServiceAddionalParameters();
            int x = 0x0;
        }
    }
}