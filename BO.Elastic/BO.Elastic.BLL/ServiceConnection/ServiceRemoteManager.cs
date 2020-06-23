using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.ServiceConnection
{
    public class ServiceRemoteManager
    {
        private ConnectionInfo SSHNETConnectionInfo;
        private SSHConnectionInfo connectionInfo;

        public ServiceRemoteManager(SSHConnectionInfo connectionInfo)
        {
            this.connectionInfo = connectionInfo;
            SSHNETConnectionInfo = new ConnectionInfo(connectionInfo.IP,
                connectionInfo.SSHLoginData.Login,
                new PasswordAuthenticationMethod(connectionInfo.SSHLoginData.Login,
                connectionInfo.SSHLoginData.Password));
        }

        public void StopElasticService(NetworkAddress serviceAddress)
        {
            try
            {
                RunSudoCommand("systemctl stop elastic.service");
            }
            catch (Exception ex)
            {
                throw new SSHCommandExecuteException(ex.Message);
            }
        }

        public void StartElasticService(NetworkAddress serviceAddress)
        {
            try
            {
                RunSudoCommand("systemctl start elastic.service");
            }
            catch (Exception ex)
            {
                throw new SSHCommandExecuteException(ex.Message);
            }
        }

        public void RestartElasticService(NetworkAddress serviceAddress)
        {
            try
            {
                RunSudoCommand("systemctl restart elastic.service");
            }
            catch (Exception ex)
            {
                throw new SSHCommandExecuteException(ex.Message);
            }
        }

        private string RunSudoCommand(string command, SshClient client)
        {
            try
            {
                if (!client.IsConnected)
                    client.Connect();

                var sh = client.RunCommand($"echo {connectionInfo.SSHLoginData.Password} | sudo -S -k {command}");
                if (sh.ExitStatus != 0)
                {
                    throw new SSHCommandExecuteException(sh.Error.Replace(connectionInfo.SSHLoginData.Password, "<password>"));
                }
                else
                {
                    return sh.Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string RunSudoCommand(string command)
        {
            using (var SSHConnection = new SshClient(SSHNETConnectionInfo))
            {
                SSHConnection.ConnectionInfo.Timeout = TimeSpan.FromSeconds(2);
                return RunSudoCommand(command, SSHConnection);
            }
        }
    }
}
