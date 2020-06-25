using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Types
{
    public enum EServiceType
    {
        Cluster = 1,
        Node = 2,
        Kibana = 3
    }

    public enum EServiceStatus
    {
        Online,
        Moderate,
        Danger,
        Offline,
        ServiceDown,
        Initializing
    }

    public enum EServiceAction
    {
        ConnectBySSH,
        Start,
        Stop,
        Restart,
        Information
    }

    public enum EClusterAttributes
    {
        UUID = 0,
        Stats = 1,
        Health = 2,
        State = 3,
        NodeInfo = 4,
        ClusterName = 5
    }
}
