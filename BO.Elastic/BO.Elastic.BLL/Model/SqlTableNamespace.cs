using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Model
{
    public class SqlTableNamespace
    {
        public string Catalog { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }

        public SqlTableNamespace(string catalog,string schema,string name)
        {
            this.Catalog = catalog;
            this.Schema = schema;
            this.Name = name;
        }
    }
}
