using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Exceptions
{
    public class ClusterNotConnectedException : Exception
    {
        public ClusterNotConnectedException(string address) : base("Błąd podczas połączenia z klastrem pod adresem: " + address)
        {

        }
    }
}
