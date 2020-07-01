using BO.Elastic.BLL.Abstract;
using BO.Elastic.BLL.DatabaseMapping;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.ServiceConnection;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BO.Elastic.BLL.Test
{
    public class SshConnectionIntegration
    {
        [Test]
        public void InvokeSshActionWithInvalidLoginData()
        {
            SshConnectionInfo info = new SshConnectionInfo("10.10.1.216", "", new LoginData
            {
                Login = "test",
                Password = "test1923"
            });
            ServiceRemoteManager manager = new ServiceRemoteManager(info);

            Assert.Throws<SshCommandExecuteException>(() => { manager.StartElasticService(info); });
            Assert.Throws<SshCommandExecuteException>(() => { manager.StopElasticService(info); });
            Assert.Throws<SshCommandExecuteException>(() => { manager.RestartElasticService(info); });
        }

        [Test]
        public void InvokeSshStartWithInvalidAddress()
        {
            SshConnectionInfo info = new SshConnectionInfo("10.10.1.116", "", new LoginData
            {
                Login = "test",
                Password = "test123"
            });
            ServiceRemoteManager manager = new ServiceRemoteManager(info);

            Assert.Throws<SshCommandExecuteException>(() => { manager.StartElasticService(info); });
        }

        [Test]
        public void InvokeSshStopWithInvalidAddress()
        {
            SshConnectionInfo info = new SshConnectionInfo("10.10.1.116", "", new LoginData
            {
                Login = "test",
                Password = "test123"
            });
            ServiceRemoteManager manager = new ServiceRemoteManager(info);

            Assert.Throws<SshCommandExecuteException>(() => { manager.StopElasticService(info); });
        }

        [Test]
        public void InvokeSshRestartWithInvalidAddress()
        {
            SshConnectionInfo info = new SshConnectionInfo("10.10.1.116", "", new LoginData
            {
                Login = "test",
                Password = "test123"
            });
            ServiceRemoteManager manager = new ServiceRemoteManager(info);

            Assert.Throws<SshCommandExecuteException>(() => { manager.RestartElasticService(info); });
        }
    }
}