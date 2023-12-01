using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
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
#if DEBUG
            SqlConnectionStringBuilder sqlString = new SqlConnectionStringBuilder()
            {
                //DataSource = ".\\ANHTUAN",
                //InitialCatalog = "DBDocumentManagementSystem",
                //UserID = "PAT",
                //Password = "Anhtuan312",
                //MultipleActiveResultSets = true,
                //ApplicationName = "EntityFramework",

                //DataSource = "10.198.138.103",
                //InitialCatalog = "DBDocumentManagementSystem",
                //UserID = "PAT",
                //Password = "Anhtuan312",
                //MultipleActiveResultSets = true,
                //ApplicationName = "EntityFramework",

                DataSource = "TUANPHUONGPC",
                //DataSource = "10.198.138.153",
                InitialCatalog = "DBDocumentManagementSystem",
                UserID = "PAT",
                Password = "Anhtuan312",
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework",
            };
#else
            SqlConnectionStringBuilder sqlString = new SqlConnectionStringBuilder()
            {
                DataSource = "10.198.138.153",
                InitialCatalog = "DBDocumentManagementSystem",
                UserID = "PAT",
                Password = "Anhtuan312",
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework",
            };
#endif

            //Build an Entity Framework connection string
            EntityConnectionStringBuilder entityString = new EntityConnectionStringBuilder()
            {
                Provider = "System.Data.SqlClient",
                Metadata = "res://*",
                ProviderConnectionString = sqlString.ToString()
            };
            return entityString.ConnectionString;
        }

        // Thêm đoạn này vào DataLayer EF để cấu hình chuỗi kết nối
        //public DBDocumentManagementSystemEntities()
        //        : base(SingleConnection.ConString)
        //{
        //}
    }
}
