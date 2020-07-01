using BO.Elastic.BLL.Abstract;
using BO.Elastic.BLL.DatabaseMapping;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Extension
{
    public static class IDatabaseModelFetcherIntanceManager
    {
        public static IDatabaseModelFetcher GetInstance(DBMSSystem dBMS, ConnectionString connectionString)
        {
            switch (dBMS)
            {
                case Types.DBMSSystem.MSSQL:
                    return new MSSqlMapper(connectionString);
                default: throw new NotImplementedException("Ta baza danych nie ma jeszcze implementacji.");
            }
        }
     
        public static IDatabaseModelFetcher GetInstance(DBMSSystem dBMS, string ip, string port, string database, string username, string password)
        {
            switch (dBMS)
            {
                case Types.DBMSSystem.MSSQL:
                    return new MSSqlMapper(ip,port,database,username,password);
                default: throw new NotImplementedException("Ta baza danych nie ma jeszcze implementacji.");
            }
        }
    }
}
