using BO.Elastic.BLL.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Test
{
    public class IPAddressModel
    {
        [Test]
        public void CreateNetworkAddress()
        {
            NetworkAddress address = new NetworkAddress("192.168.1.1", "25565");
            Assert.AreEqual(address.IP, "192.168.1.1");
            Assert.AreEqual(address.Port, "25565");
            Assert.AreEqual(address.NumerPort, 25565);

            NetworkAddress addressWithoutPort = new NetworkAddress("192.168.1.1", "");
            Assert.Throws<ArgumentException>(() =>
            {
                int port = addressWithoutPort.NumerPort;
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("192.168.1.1", "abc");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("", "9");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("", "");
            });

            Assert.DoesNotThrow(() =>
            {
                NetworkAddress addressWithValidData = new NetworkAddress("192.168.1.1", "");
            });

            Assert.DoesNotThrow(() =>
            {
                NetworkAddress addressWithValidData = new NetworkAddress("192.168.1.1", "80");
            });

            Assert.DoesNotThrow(() =>
            {
                NetworkAddress addressWithValidData = new NetworkAddress("192.168.1.1", 80);
            });

            Assert.DoesNotThrow(() =>
            {
                NetworkAddress addressWithValidData = new NetworkAddress("192.168.1.1", 80);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("", 0);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithValidData = new NetworkAddress("192.168.1.1", "");
                string ipPort = addressWithValidData.IPPortMerge;
            });

            Assert.DoesNotThrow(() =>
            {
                NetworkAddress addressWithValidData = new NetworkAddress("192.168.1.1", "3");
                string ipPort = addressWithValidData.IPPortMerge;
            });
        }

        [Test]
        public void CreateSSHConnectionInfo()
        {
            NetworkAddress addressWithoutPort = new NetworkAddress("192.168.1.1", "");
            LoginData testData = new LoginData()
            {
                Login = "test",
                Password = "test"
            };

            Assert.DoesNotThrow(() =>
            {
                SSHConnectionInfo info = new SSHConnectionInfo(addressWithoutPort, testData);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                SSHConnectionInfo addressWithinvalidData = new SSHConnectionInfo("192.168.1.1", "abc", testData);
            });

            Assert.DoesNotThrow(() =>
            {
                SSHConnectionInfo validData = new SSHConnectionInfo("192.1.1.1", "123", testData);
            });

            Assert.DoesNotThrow(() =>
            {
                SSHConnectionInfo validData = new SSHConnectionInfo("192.1.1.1", 4, testData);
            });

            Assert.DoesNotThrow(() =>
            {
                SSHConnectionInfo validData = new SSHConnectionInfo("192.1.1.1", "4", testData);
            });
        }

        [Test]
        [TestCase("192.168.1.1","2")]
        [TestCase("192.168.1.4", "24")]
        [TestCase("192.168.1.78", "2")]
        [TestCase("1.2.3.4", "242")]
        public void HttpHttpsConvertWithPort(string ip,string port)
        {
            NetworkAddress addressWithValidData = new NetworkAddress(ip,port);

            Assert.AreEqual(addressWithValidData.HTTPAddress, "http://" + ip + ":" + port);
        }
        [Test]
        [TestCase("192.168.1.1", 2)]
        [TestCase("192.168.1.4", 24)]
        [TestCase("192.168.1.78", 2)]
        [TestCase("1.2.3.4", 242)]
        public void HttpHttpsConvertWithPort(string ip, int port)
        {
            NetworkAddress addressWithValidData = new NetworkAddress(ip, port);

            Assert.AreEqual(addressWithValidData.HTTPAddress, "http://" + ip + ":" + port);
        }

        [Test]
        [TestCase("192.168.1.1", "")]
        [TestCase("192.168.1.4", "")]
        [TestCase("192.168.1.78", "")]
        [TestCase("192.168.1.12", "")]
        public void HttpHttpsConvertWithoutPort(string ip, string port)
        {
            NetworkAddress addressWithValidData = new NetworkAddress(ip, port);

            Assert.AreEqual(addressWithValidData.HTTPAddress, "http://" + ip);
        }
    }
}
