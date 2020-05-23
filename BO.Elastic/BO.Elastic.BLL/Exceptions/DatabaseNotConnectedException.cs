using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Exceptions
{
    class DatabaseNotConnectedException : Exception
    {
        public DatabaseNotConnectedException(string message) : base("Błąd podczas połączenia z bazą danych\n" + message)
        {

        }
    }
}
