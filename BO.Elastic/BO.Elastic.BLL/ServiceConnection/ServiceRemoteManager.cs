using System;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using Renci.SshNet;

namespace BO.Elastic.BLL.ServiceConnection
{
    public class ServiceRemoteManager
    {
        private readonly SshConnectionInfo connectionInfo;
        private readonly ConnectionInfo sshnetConnectionInfo;

        public ServiceRemoteManager(SshConnectionInfo connectionInfo)
        {
            this.connectionInfo = connectionInfo;
            sshnetConnectionInfo = new ConnectionInfo(connectionInfo.Ip,
                connectionInfo.SshLoginData.Login,
                new PasswordAuthenticationMethod(connectionInfo.SshLoginData.Login,
                    connectionInfo.SshLoginData.Password));
        }

        public void StopElasticService(NetworkAddress serviceAddress)
        {
            try
            {
                RunSudoCommand("systemctl stop elastic.service");
            }
            catch (Exception ex)
            {
                throw new SshCommandExecuteException(ex.Message);
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
                throw new SshCommandExecuteException(ex.Message);
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
                throw new SshCommandExecuteException(ex.Message);
            }
        }

        private string RunSudoCommand(string command, SshClient client)
        {
            try
            {
                if (!client.IsConnected)
                    client.Connect();

                SshCommand sh =
                    client.RunCommand($"echo {connectionInfo.SshLoginData.Password} | sudo -S -k {command}");
                if (sh.ExitStatus != 0)
                    throw new SshCommandExecuteException(sh.Error.Replace(connectionInfo.SshLoginData.Password,
                        "<password>"));
                return sh.Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string RunSudoCommand(string command)
        {
            using (SshClient sshConnection = new SshClient(sshnetConnectionInfo))
            {
                sshConnection.ConnectionInfo.Timeout = TimeSpan.FromSeconds(2);
                return RunSudoCommand(command, sshConnection);
            }
        }
    }
}