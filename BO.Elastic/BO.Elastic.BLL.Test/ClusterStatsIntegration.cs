using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using BO.Elastic.Panel.ViewModels;
using Nest;
using NUnit.Framework;

namespace BO.Elastic.BLL.Test
{
    public class ClusterStatsIntegration
    {
        private const string invalidClusterIp = "10.1.1.214";
        private const string invalidClusterPort = "9001";
        private const string validClusterIp = "10.10.1.214";
        private const string validClusterPort = "9201";

        [Test]
        [TestCase("192.168.1.1", "43")]
        [TestCase("21.43.4.1", "4")]
        [TestCase("22.5.1.13", "25565")]
        [TestCase("21.168.1.1", "11111")]
        public void ConstructorWithValidParameters(string ip, string port)
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterStatsWindowViewModel clusterStatsWindowViewModel = new ClusterStatsWindowViewModel(new NetworkAddress(ip, port),false);
            });
        }

        [Test]
        [TestCase("192.1a68.1.1", null)]
        [TestCase("1.43.4.1", "")]
        [TestCase("22.5.1.1gg3", "")]
        [TestCase("", "11111")]
        public void ConstructorWithBadParameters(string ip, string port)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ClusterStatsWindowViewModel clusterStatsWindowViewModel = new ClusterStatsWindowViewModel(new NetworkAddress(ip, port),false);
            });
        }

        [Test]
        public void GetValidClusterHealth()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterHealthResponse response =
                    new ClusterStatsWindowViewModel(new NetworkAddress(validClusterIp, validClusterPort),false)
                        .ClusterHealthResponse;
                if (!response.IsValid) Assert.Fail();
            });
        }


        [Test]
        public void GetInvalidClusterHealth()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterHealthResponse response =
                    new ClusterStatsWindowViewModel(new NetworkAddress(invalidClusterIp, invalidClusterPort),false)
                        .ClusterHealthResponse;
                if (response != null) Assert.Fail();
            });
        }


        [Test]
        public void GetValidClusterStats()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterStatsResponse response =
                    new ClusterStatsWindowViewModel(new NetworkAddress(validClusterIp, validClusterPort),false)
                        .ClusterStatsResponse;
                if (!response.IsValid) Assert.Fail();
            });
        }


        [Test]
        public void GetInvalidClusterStats()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterStatsResponse response =
                    new ClusterStatsWindowViewModel(new NetworkAddress(invalidClusterIp, invalidClusterPort),false)
                        .ClusterStatsResponse;
                if (response != null) Assert.Fail();
            });
        }


        [Test]
        public void GetValidClusterState()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterStateResponse response =
                    new ClusterStatsWindowViewModel(new NetworkAddress(validClusterIp, validClusterPort),false)
                        .ClusterStateResponse;
                if (!response.IsValid) Assert.Fail();
            });
        }

        [Test]
        public void GetInvalidClusterState()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterStateResponse response =
                    new ClusterStatsWindowViewModel(new NetworkAddress(invalidClusterIp, invalidClusterPort), false)
                        .ClusterStateResponse;
                if (response != null) Assert.Fail();
            });
        }

        [Test]
        public void GetInvalidNodeInfo()
        {
            Assert.DoesNotThrow(() =>
            {
                NodeInfo response =
                    new ClusterStatsWindowViewModel(new NetworkAddress(invalidClusterIp, invalidClusterPort), false)
                        .NodeInfo;
                if(response != null)
                {
                    Assert.Fail();
                }
            });
        }

        [Test]
        public void GetValidNodeInfo()
        {
            Assert.DoesNotThrow(() =>
            {
                NodeInfo response =
                    new ClusterStatsWindowViewModel(new NetworkAddress(validClusterIp, validClusterPort), false)
                        .NodeInfo;
                if (response == null)
                {
                    Assert.Fail();
                }
            });
        }

        [Test]
        [TestCase(validClusterIp,validClusterPort)]
        [TestCase(invalidClusterIp, invalidClusterPort)]
        public void GetClusterAttributes(string ip,string port)
        {
            Assert.DoesNotThrow(() =>
            {
                ObservableCollection<KeyValuePair<string, string>> response =
                    new ClusterStatsWindowViewModel(new NetworkAddress(validClusterIp, validClusterPort), false)
                        .ClusterAttributes;
                if (response != null)
                {
                    Assert.IsInstanceOf<ObservableCollection<KeyValuePair<string, string>>>(response);
                }
                else
                {
                    Assert.Fail();
                }
            });
        }
    }
}