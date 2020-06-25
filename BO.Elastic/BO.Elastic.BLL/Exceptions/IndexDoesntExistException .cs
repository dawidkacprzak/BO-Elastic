using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Exceptions
{
    public class IndexDoesntExistException : Exception
    {
        public IndexDoesntExistException(string indexName) : base("Nie istnieje index o nazwie: " + indexName)
        {

        }
    }
}
