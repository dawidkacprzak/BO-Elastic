using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Exceptions
{
    public class NodeNotConnectedException : Exception
    {
        public NodeNotConnectedException(string address) : base("Błąd podczas połączenia z nodem pod adresem: " + address)
        {

        }
    }
}
