using System;

namespace BO.Elastic.BLL.Exceptions
{
    internal class DatabaseNotConnectedException : Exception
    {
        public DatabaseNotConnectedException(string message) : base("Błąd podczas połączenia z bazą danych\n" + message)
        {
        }
    }
}