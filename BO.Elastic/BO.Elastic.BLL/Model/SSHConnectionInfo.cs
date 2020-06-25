namespace BO.Elastic.BLL.Model
{
    public class SshConnectionInfo : NetworkAddress
    {
        public SshConnectionInfo(string ip, string port, LoginData loginData) : base(ip, port)
        {
            SshLoginData = loginData;
        }

        public SshConnectionInfo(string ip, int port, LoginData loginData) : base(ip, port)
        {
            SshLoginData = loginData;
        }

        public SshConnectionInfo(NetworkAddress address, LoginData loginData) : base(address.Ip, address.Port)
        {
            SshLoginData = loginData;
        }

        public LoginData SshLoginData { get; }
    }
}