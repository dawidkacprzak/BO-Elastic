using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Model
{
    public class SSHConnectionInfo : NetworkAddress
    {
        private LoginData sshLoginData;

        public SSHConnectionInfo(string ip, string port, LoginData loginData) : base(ip, port)
        {
            this.sshLoginData = loginData;
        }

        public SSHConnectionInfo(string ip, int port, LoginData loginData) : base(ip, port)
        {
            this.sshLoginData = loginData;
        }

        public SSHConnectionInfo(NetworkAddress address, LoginData loginData) : base(address.IP, address.Port)
        {
            this.sshLoginData = loginData;
        }
    }
}
