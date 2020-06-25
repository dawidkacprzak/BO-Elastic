using System;
using BO.Elastic.BLL.Model;
using NUnit.Framework;

namespace BO.Elastic.BLL.Test
{
    public class IpAddressModel
    {
        [Test]
        public void CreateNetworkAddress()
        {
            NetworkAddress address = new NetworkAddress("192.168.1.1", "25565");
            Assert.AreEqual(address.Ip, "192.168.1.1");
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
                string ipPort = addressWithValidData.IpPortMerge;
            });

            Assert.DoesNotThrow(() =>
            {
                NetworkAddress addressWithValidData = new NetworkAddress("192.168.1.1", "3");
                string ipPort = addressWithValidData.IpPortMerge;
            });
        }

        [Test]
        public void CreateSshConnectionInfo()
        {
            NetworkAddress addressWithoutPort = new NetworkAddress("192.168.1.1", "");
            LoginData testData = new LoginData
            {
                Login = "test",
                Password = "test"
            };

            Assert.DoesNotThrow(() =>
            {
                SshConnectionInfo info = new SshConnectionInfo(addressWithoutPort, testData);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                SshConnectionInfo addressWithinvalidData = new SshConnectionInfo("192.168.1.1", "abc", testData);
            });

            Assert.DoesNotThrow(() =>
            {
                SshConnectionInfo validData = new SshConnectionInfo("192.1.1.1", "123", testData);
            });

            Assert.DoesNotThrow(() =>
            {
                SshConnectionInfo validData = new SshConnectionInfo("192.1.1.1", 4, testData);
            });

            Assert.DoesNotThrow(() =>
            {
                SshConnectionInfo validData = new SshConnectionInfo("192.1.1.1", "4", testData);
            });
        }

        [Test]
        [TestCase("192.168.1.1", "2")]
        [TestCase("192.168.1.4", "24")]
        [TestCase("192.168.1.78", "2")]
        [TestCase("1.2.3.4", "242")]
        public void HttpHttpsConvertWithPort(string ip, string port)
        {
            NetworkAddress addressWithValidData = new NetworkAddress(ip, port);

            Assert.AreEqual(addressWithValidData.HttpAddress, "http://" + ip + ":" + port);
        }

        [Test]
        [TestCase("192.168.1.1", 2)]
        [TestCase("192.168.1.4", 24)]
        [TestCase("192.168.1.78", 2)]
        [TestCase("1.2.3.4", 242)]
        public void HttpHttpsConvertWithPort(string ip, int port)
        {
            NetworkAddress addressWithValidData = new NetworkAddress(ip, port);

            Assert.AreEqual(addressWithValidData.HttpAddress, "http://" + ip + ":" + port);
        }

        [Test]
        [TestCase("192.168.1.1", "")]
        [TestCase("192.168.1.4", "")]
        [TestCase("192.168.1.78", "")]
        [TestCase("192.168.1.12", "")]
        public void HttpHttpsConvertWithoutPort(string ip, string port)
        {
            NetworkAddress addressWithValidData = new NetworkAddress(ip, port);

            Assert.AreEqual(addressWithValidData.HttpAddress, "http://" + ip);
        }
    }
}