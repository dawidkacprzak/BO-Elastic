using System.ComponentModel;

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

    public enum EDBDataType
    {
        Undefined = -1,
        Bigint = 0,
        Numeric = 1,
        Bit = 2,
        Decimal = 3,
        Int = 4,
        Tinyint = 5,
        Smallint = 6,
        Money = 7,
        Smallmoney = 8,
        Float = 9,
        Real = 10,
        Date = 11,
        Datetime = 12,
        Nchar = 13,
        NVarchar = 14,
        NText = 15,
        Char = 16,
        Varchar = 17,
        Text = 18
    }

    public enum EElasticDataTypes
    {
        [Description("long")] Long = 0,
        [Description("boolean")] Boolean = 1,
        [Description("double")] Double = 2,
        [Description("integer")] Integer = 3,
        [Description("float")] Float = 4,
        [Description("text")] Text = 5,
        [Description("keyword")] Keyword = 6,
        [Description("ip")] IP = 7,
        [Description("completion")] Completion = 8,
        [Description("date")] Date = 9
    }

    public enum DBMSSystem
    {
        MSSQL = 1,
        MySQL = 2,
        PostgreSQL = 3,
        MongoDB = 4
    }
}