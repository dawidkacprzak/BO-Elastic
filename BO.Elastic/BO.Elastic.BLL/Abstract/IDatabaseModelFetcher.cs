using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Abstract
{
    public interface IDatabaseModelFetcher
    {
        public IEnumerable<SqlTableNamespace> GetTables();
        public IEnumerable<KeyValuePair<string, ESqlDatatypes>> GetTableColumns(SqlTableNamespace tableNamespace);
    }
}
