using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Model
{
    public class SSHConnectionInfo : NetworkAddress
    {
        public LoginData SSHLoginData { get; private set; }

        public SSHConnectionInfo(string ip, string port, LoginData loginData) : base(ip, port)
        {
            this.SSHLoginData = loginData;
        }

        public SSHConnectionInfo(string ip, int port, LoginData loginData) : base(ip, port)
        {
            this.SSHLoginData = loginData;
        }

        public SSHConnectionInfo(NetworkAddress address, LoginData loginData) : base(address.IP, address.Port)
        {
            this.SSHLoginData = loginData;
        }
    }
}
