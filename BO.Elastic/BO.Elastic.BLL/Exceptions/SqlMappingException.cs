using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Exceptions
{
    public class SqlMappingException : Exception
    {
        public SqlMappingException(string message) : base(message)
        {

        }

    }
}
