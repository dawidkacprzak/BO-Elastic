using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceConnection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Test
{
    public class SSHConnectionIntegration
    {
        [Test]
        public void InvokeSSHActionWithInvalidLoginData()
        {
            SSHConnectionInfo info = new SSHConnectionInfo("10.10.1.216","",new LoginData()
            {
                Login = "test",
                Password = "test1923"
            });
            ServiceRemoteManager manager = new ServiceRemoteManager(info);

            Assert.Throws<SSHCommandExecuteException>(() =>
            {
                manager.StartElasticService(info);
            });
            Assert.Throws<SSHCommandExecuteException>(() =>
            {
                manager.StopElasticService(info);
            });
            Assert.Throws<SSHCommandExecuteException>(() =>
            {
                manager.RestartElasticService(info);
            });
        }

        [Test]
        public void InvokeSSHActionWithInvalidAddress()
        {
            SSHConnectionInfo info = new SSHConnectionInfo("10.10.1.116", "", new LoginData()
            {
                Login = "test",
                Password = "test123"
            });
            ServiceRemoteManager manager = new ServiceRemoteManager(info);

            Assert.Throws<SSHCommandExecuteException>(() =>
            {
                manager.StartElasticService(info);
            });
            Assert.Throws<SSHCommandExecuteException>(() =>
            {
                manager.StopElasticService(info);
            });
            Assert.Throws<SSHCommandExecuteException>(() =>
            {
                manager.RestartElasticService(info);
            });
        }
    }
}
