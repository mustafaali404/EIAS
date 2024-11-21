using System.Configuration;
using Microsoft.Data.SqlClient;
using static EIAS.DBManager;
using System;
using System.Collections.Specialized;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace EIAS
{    /// <summary>
     /// Author: Timur Maistrenko
     /// <br></br>
     /// Database manager that handles secure communication with the database.
     /// </summary>
     /// TODO: Separate databases for production and testing
    public class DBManager: IDisposable
    {
       // private static DBManager? instance;
        private SqlConnection connection;
        private string connectionString;

        public enum Table
        {
            ForumUsers,
            Login,
            Posts,
            Threads
        }

        /*public static DBManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBManager();
                }
                return instance;
            }
        }*/

        public DBManager ()
        {
            // Temporary solution. The Program.cs class already fetches the string from config file,
            // but since both the Program.cs and config are incomplete, this is the stop-gap solution.
            connectionString = "Server=tcp:eias.database.windows.net,1433;Initial Catalog=EIAS;Persist Security Info=False;User ID=eiasadmin;Password=Xi)O2&E4/V20;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;";
            connection = new SqlConnection (connectionString);
        }

        public DBManager(string connectionString)
        {
            this.connectionString = connectionString;
            connection = new SqlConnection(connectionString);
        }

        private void OpenConnection()
        {
            if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
            {
                connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public ConnectionState GetConnectionState()
        {
            return connection.State;
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }


        public DataTable Query(string query, SqlParameter[] parameters = null)
        {
            try
            {
                OpenConnection();
                using (SqlCommand command = new(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception exception)
            {                
                // Must modify later
                throw new Exception("SQL ERROR", exception);
            }
            finally
            {
                CloseConnection();
            }
        }

        public int NonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                OpenConnection();
                using (SqlCommand command = new(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                // Must modify later
                throw new Exception("SQL ERROR", exception);
            }
            finally
            {
                CloseConnection();
            }
        }

       

    }
}
