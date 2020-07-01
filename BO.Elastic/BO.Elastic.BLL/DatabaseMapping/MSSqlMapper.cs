using BO.Elastic.BLL.Abstract;
using BO.Elastic.BLL.Exceptions;
using BO.Elastic.BLL.Model;
using BO.Elastic.BLL.Types;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Elastic.BLL.DatabaseMapping
{
    public class MSSqlMapper : IDatabaseModelFetcher
    {
        ConnectionString connectionString;
        public MSSqlMapper(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public MSSqlMapper(string ip, string port, string database, string username, string password)
        {
            this.connectionString = new ConnectionString(DBMSSystem.MSSQL, ip, port, database, username, password);
        }

        public IEnumerable<KeyValuePair<string, EDBDataType>> GetTableColumns(SqlTableNamespace tableNamespace)
        {
            List<KeyValuePair<string, EDBDataType>> columns = new List<KeyValuePair<string, EDBDataType>>();
            using (SqlConnection connection = new SqlConnection(connectionString.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    SqlCommand getTableCommand = new SqlCommand(@$"
                        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE
                             TABLE_CATALOG = '{tableNamespace.Catalog}' AND
                             TABLE_SCHEMA = '{tableNamespace.Schema}'  AND
                             TABLE_NAME = '{tableNamespace.Name}'" 
                    , connection);
                    using (SqlDataReader reader = getTableCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(new KeyValuePair<string,EDBDataType>(reader.GetString(3),EDBDataType.Int));
                        }
                    }
                    return columns;
                }
                catch (Exception ex)
                {
                    throw new SqlMappingException("Błąd podczas pobierania tabel z bazy danych.\n" + ex.Message);
                }
            }
        }

        public IEnumerable<SqlTableNamespace> GetTables()
        {
            using (SqlConnection connection = new SqlConnection(connectionString.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    SqlCommand getTableCommand = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'", connection);
                    List<SqlTableNamespace> namespaces = new List<SqlTableNamespace>();
                    using (SqlDataReader reader = getTableCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            namespaces.Add(new SqlTableNamespace(reader.GetString(0), reader.GetString(1), reader.GetString(2)));
                        }
                    }
                    return namespaces;
                }catch(Exception ex)
                {
                    throw new SqlMappingException("Błąd podczas pobierania tabel z bazy danych.\n" + ex.Message);
                }
            }
        }

        public bool IsConnectionValid()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString.GetConnectionString()))
                {
                    connection.Open();
                }
            }catch(Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}
