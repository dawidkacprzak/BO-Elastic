using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Exceptions
{
    public class IndexAlreadyExistsException : Exception
    {
        public IndexAlreadyExistsException(string indexName) : base("Istnieje już index o nazwie: " + indexName)
        {

        }
    }
}
