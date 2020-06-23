using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Exceptions
{
    public class SSHCommandExecuteException : Exception
    {
        public SSHCommandExecuteException(string response) : base ($"Błąd podczas wykonywania polecenia. Response: {response}")
        {

        }
    }
}
