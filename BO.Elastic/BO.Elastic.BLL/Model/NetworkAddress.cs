using System;

namespace BO.Elastic.BLL.Model
{
    public class NetworkAddress
    {
        public NetworkAddress(string ip, string port)
        {
            ValidateIp(ip);
            ValidatePort(port);
            Ip = ip;
            Port = port;
        }

        public NetworkAddress(string ip, int port)
        {
            ValidateIp(ip);
            Ip = ip;
            Port = port.ToString();
        }

        public string Ip { get; set; }
        public string Port { get; set; }

        public string IpPortMerge
        {
            get
            {
                if (string.IsNullOrEmpty(Port)) throw new ArgumentException("Port nie został zdefiniownay");
                return Ip + ":" + Port;
            }
        }

        public int NumerPort
        {
            get
            {
                if (string.IsNullOrEmpty(Port)) throw new ArgumentException("Port nie został zdefiniowany");
                return int.Parse(Port);
            }
        }

        public string HttpAddress
        {
            get
            {
                if (string.IsNullOrEmpty(Port))
                    return "http://" + Ip;
                return "http://" + Ip + ":" + Port;
            }
        }

        public string HttpsAddress => HttpAddress.Replace("http", "https");

        private void ValidateIp(string ip)
        {
            if (ip.Length == 0) throw new ArgumentException("Adres IP jest nieprawidłowy");
        }

        private void ValidatePort(string port)
        {
            if (!string.IsNullOrEmpty(port))
                if (!int.TryParse(port, out _))
                    throw new ArgumentException("Port jest nieprawidłowy");
        }
    }
}