using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEF
{
    public class SingleConnection
    {
        private SingleConnection() { }
        private static SingleConnection _ConsString = null;
        private String _String = null;

        public static string ConString
        {
            get
            {
                if (_ConsString == null)
                {
                    _ConsString = new SingleConnection { _String = SingleConnection.Connect() };
                    return _ConsString._String;
                }
                else
                    return _ConsString._String;
            }
        }

        public static string Connect()
        {
            SqlConnectionStringBuilder sqlString = new SqlConnectionStringBuilder()
            {
                DataSource = ".\\ANHTUAN",
                InitialCatalog = "DBDocumentManagementSystem",
                UserID = "PAT",
                Password = "Anhtuan312",
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework",
            };

            //SqlConnectionStringBuilder sqlString = new SqlConnectionStringBuilder()
            //{
            //    DataSource = "10.198.138.153",
            //    InitialCatalog = "DBDocumentManagementSystem",
            //    UserID = "PAT",
            //    Password = "Anhtuan312",
            //    MultipleActiveResultSets = true,
            //    ApplicationName = "EntityFramework",
            //};

            //SqlConnectionStringBuilder sqlString = new SqlConnectionStringBuilder()
            //{
            //    DataSource = "10.199.232.146",
            //    InitialCatalog = "DBDocumentManagementSystem",
            //    UserID = "PAT",
            //    Password = "Anhtuan312",
            //    MultipleActiveResultSets = true,
            //    ApplicationName = "EntityFramework",
            //};

            //Build an Entity Framework connection string
            EntityConnectionStringBuilder entityString = new EntityConnectionStringBuilder()
            {
                Provider = "System.Data.SqlClient",
                Metadata = "res://*",
                ProviderConnectionString = sqlString.ToString()
            };
            return entityString.ConnectionString;
        }
    }
}
