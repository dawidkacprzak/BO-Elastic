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
        Initializing
    }

    public enum Option
    {
        ConnectBySSH,
        Start,
        Stop,
        Information
    }
}
