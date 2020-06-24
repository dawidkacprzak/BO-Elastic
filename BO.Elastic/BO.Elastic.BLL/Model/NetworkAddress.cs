using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BO.Elastic.BLL.Model
{
    public class NetworkAddress
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public string IPPortMerge
        {
            get
            {
                if (string.IsNullOrEmpty(Port))
                {
                    throw new ArgumentException("Port nie został zdefiniownay");
                }
                return IP + ":" + Port;
            }
        }
        public int NumerPort
        {
            get
            {
                if (string.IsNullOrEmpty(Port))
                {
                    throw new ArgumentException("Port nie został zdefiniowany");
                }
                return int.Parse(Port);
            }
        }

        public string HTTPAddress
        {
            get
            {
                if (string.IsNullOrEmpty(Port))
                {
                    return "http://" + IP;
                }
                else
                {
                    return "http://" + IP + ":" + Port;
                }
            }
        }

        public string HTTPSAddress
        {
            get
            {
                return HTTPAddress.Replace("http", "https");
            }
        }
        public NetworkAddress(string ip, string port)
        {
            ValidateIP(ip);
            ValidatePort(port);
            this.IP = ip;
            this.Port = port;
        }

        public NetworkAddress(string ip, int port)
        {
            ValidateIP(ip);
            this.IP = ip;
            this.Port = port.ToString();
        }

        private void ValidateIP(string ip)
        {
            if(ip.Length == 0)
            {
                throw new ArgumentException("Adres IP jest nieprawidłowy");
            }
        }

        private void ValidatePort(string port)
        {
            if (!string.IsNullOrEmpty(port))
            {
                if (!int.TryParse(port, out _))
                {
                    throw new ArgumentException("Port jest nieprawidłowy");
                }
            }
        }
    }
}
