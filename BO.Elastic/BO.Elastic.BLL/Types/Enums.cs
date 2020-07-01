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
        ConnectBySsh,
        Start,
        Stop,
        Restart,
        Information
    }

    public enum EClusterAttributes
    {
        Uuid = 0,
        Stats = 1,
        Health = 2,
        State = 3,
        NodeInfo = 4,
        ClusterName = 5,
        HttpUri = 6,
        Cpu = 7,
        Ram = 8,
        Os = 9,
        Roles = 10
    }

    public enum ESqlDatatypes
    {
        //todo
    }

    public enum DBMSSystem
    {
        MSSQL = 1
    }
}