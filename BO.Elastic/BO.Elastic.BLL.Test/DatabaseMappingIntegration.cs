using BO.Elastic.BLL.Abstract;
using BO.Elastic.BLL.DatabaseMapping;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BO.Elastic.BLL.Test
{
    public class DatabaseMappingIntegration
    {
        [Test]
        public void GetInvalidCredentialsTablesFromIDatabaseModelFetcher()
        {
            Assert.Throws<SqlMappingException>(() =>
            {
                IDatabaseModelFetcher mapper = new MSSqlMapper("10.10.1.214", "", "Elastic", "testnotvalid", "test");
                IEnumerable<SqlTableNamespace> tables = mapper.GetTables();
                Assert.IsTrue(tables.Count() > 0);
            });
        }

        [Test]
        public void GetValidTablesFromIDatabaseModelFetcher()
        {
            Assert.DoesNotThrow(() =>
            {
                IDatabaseModelFetcher mapper = new MSSqlMapper("10.10.1.214", "", "Elastic", "test", "test");
                IEnumerable<SqlTableNamespace> tables = mapper.GetTables();
                Assert.IsTrue(tables.Count() > 0);
            });
        }
    }
}
