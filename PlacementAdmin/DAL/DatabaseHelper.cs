using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;

namespace PlacementAdmin.DAL
{
    public class DatabaseHelper : IDisposable
    {
        private readonly SqlConnection sqlConnection;

        public DatabaseHelper(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString)) 
            { 
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found in the configuration."); 
            }
            sqlConnection = new SqlConnection(connectionString);
        }

        public SqlConnection GetConnection()
        {
            //if (string.IsNullOrEmpty(sqlConnection.ConnectionString)) 
            //{ 
            //    throw new InvalidOperationException("Connection string has not been initialized."); 
            //}
            //if (sqlConnection.State == ConnectionState.Closed)
            //{
            //    return sqlConnection;
            //}
            return sqlConnection;
        }

        public void Dispose()
        {
            if (sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
            sqlConnection.Dispose();
        }
    }
}
