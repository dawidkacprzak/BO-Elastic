using BO.Elastic.BLL.Model;
using BO.Elastic.Panel.ViewModels;
using Nest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Test
{
    public class ClusterStatsIntegration
    {
        string validClusterIP = "10.10.1.214";
        string validClusterPort = "9201";
        string invalidClusterIP = "10.1.1.214";
        string invalidClusterPort = "9001";
        [Test]
        [TestCase("192.168.1.1","43")]
        [TestCase("21.43.4.1", "4")]
        [TestCase("22.5.1.13", "25565")]
        [TestCase("21.168.1.1", "11111")]
        public void ConstructorWithValidParameters(string ip, string port)
        {
            Assert.DoesNotThrow(() =>
            {
                new ClusterStatsWindowViewModel(new NetworkAddress(ip, port), false);
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
                new ClusterStatsWindowViewModel(new NetworkAddress(ip, port), false);
            });
        }

        [Test]
        public void GetValidClusterHealth()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterHealthResponse response = new ClusterStatsWindowViewModel(new NetworkAddress(validClusterIP, validClusterPort)).ClusterHealthResponse;
                if (!response.IsValid)
                {
                    Assert.Fail();
                }
            });
        }


        [Test]
        public void GetInvalidClusterHealth()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterHealthResponse response = new ClusterStatsWindowViewModel(new NetworkAddress(invalidClusterIP, invalidClusterPort)).ClusterHealthResponse;
                if (response != null)
                {
                    Assert.Fail();
                }
            });
        }


        [Test]
        public void GetValidClusterStats()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterStatsResponse response = new ClusterStatsWindowViewModel(new NetworkAddress(validClusterIP, validClusterPort)).ClusterStatsResponse;
                if (!response.IsValid)
                {
                    Assert.Fail();
                }
            });
        }


        [Test]
        public void GetInvalidClusterStats()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterStatsResponse response = new ClusterStatsWindowViewModel(new NetworkAddress(invalidClusterIP, invalidClusterPort)).ClusterStatsResponse;
                if (response != null)
                {
                    Assert.Fail();
                }
            });
        }


        [Test]
        public void GetValidClusterState()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterStateResponse response = new ClusterStatsWindowViewModel(new NetworkAddress(validClusterIP, validClusterPort)).ClusterStateResponse;
                if (!response.IsValid)
                {
                    Assert.Fail();
                }
            });
        }

/*
        [Test]
        public void GetInvalidClusterState()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterHealthResponse response = new ClusterStatsWindowViewModel(new NetworkAddress(invalidClusterIP, invalidClusterPort)).ClusterStateResponse;
                if (response != null)
                {
                    Assert.Fail();
                }
            });
        }


        [Test]
        public void GetValidClusterHealth()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterHealthResponse response = new ClusterStatsWindowViewModel(new NetworkAddress(validClusterIP, validClusterPort)).ClusterHealthResponse;
                if (!response.IsValid)
                {
                    Assert.Fail();
                }
            });
        }


        [Test]
        public void GetInvalidClusterHealth()
        {
            Assert.DoesNotThrow(() =>
            {
                ClusterHealthResponse response = new ClusterStatsWindowViewModel(new NetworkAddress(invalidClusterIP, invalidClusterPort)).ClusterHealthResponse;
                if (response != null)
                {
                    Assert.Fail();
                }
            });
        }*/

    }
}
