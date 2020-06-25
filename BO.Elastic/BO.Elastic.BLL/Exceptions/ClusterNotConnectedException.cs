using System;

namespace BO.Elastic.BLL.Exceptions
{
    public class ClusterNotConnectedException : Exception
    {
        public ClusterNotConnectedException(string address) : base(
            "Błąd podczas połączenia z klastrem pod adresem: " + address)
        {
        }
    }
}