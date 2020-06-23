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
                NetworkAddress addressWithinvalidData = new NetworkAddress("abc", "123");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("192..1.1", "123");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("192..1.1", "123");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("192.1.1", "123");
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
                NetworkAddress addressWithinvalidData = new NetworkAddress("192..1.1", 123);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("192..1.1", 123);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("192.1.1", 123);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                NetworkAddress addressWithinvalidData = new NetworkAddress("", 0);
            });
        }
    }
}
