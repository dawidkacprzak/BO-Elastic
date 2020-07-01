using BO.Elastic.BLL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.Model
{
    public class ConnectionString : NetworkAddress
    {
        public DBMSSystem DBMSSystem { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ConnectionString(DBMSSystem dbmsSystem, string ip, string port,string database, string username,string password) 
            : base(ip, 
                      (string.IsNullOrEmpty(port) ? (
                        dbmsSystem == DBMSSystem.MSSQL ? 
                            "1433" : throw new ArgumentException("Port w connection stringu jest nieprawidłowy")
                      ) : port)
                  )
        {
            this.DBMSSystem = dbmsSystem;
            this.Database = database;
            this.Username = username;
            this.Password = password;
            this.Ip = ip;
        }

        public string GetConnectionString()
        {
            switch (DBMSSystem)
            {
                case DBMSSystem.MSSQL:
                    return $"Server={Ip},{Port};Database={Database}; User Id={Username}; Password={Password};Connection Timeout=3";
                default: throw new NotImplementedException("Nie można pobrać connection stringu dla bazy danych: " + DBMSSystem.ToString() + ". Brak implementacji.");
            }
        }
    }
}
