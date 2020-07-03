using BO.Elastic.BLL.ElasticCore;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Test
{
    public class ElasticMappingIntegration
    {
        [Test]
        [TestCase("qazxswedcvc")]
        [TestCase("Adsd")]
        [TestCase("255324332")]
        public void FailIndexExists(string indexName)
        {
            NetworkAddress address = new NetworkAddress("10.10.1.214", 9201);
            NextWrap nw = new NextWrap(address);

            Assert.False(nw.IndexExists(indexName));
        }

        [Test]
        [TestCase("qazxswedcvc")]
        [TestCase("Adsd")]
        [TestCase("255324332")]
        public void FailClusterDoesNotExistIndexExists(string indexName)
        {
            Assert.Throws<ClusterNotConnectedException>(() =>
            {
                NetworkAddress address = new NetworkAddress("10.10.1.14", 9201);
                NextWrap nw = new NextWrap(address);

                Assert.False(nw.IndexExists(indexName));
            });
        }
    }
}
