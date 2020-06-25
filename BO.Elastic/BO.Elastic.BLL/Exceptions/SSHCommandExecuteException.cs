using System;

namespace BO.Elastic.BLL.Exceptions
{
    public class SshCommandExecuteException : Exception
    {
        public SshCommandExecuteException(string response) : base(
            $"Błąd podczas wykonywania polecenia. Response: {response}")
        {
        }
    }
}