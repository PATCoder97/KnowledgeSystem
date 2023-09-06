using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList.Painter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Configs
{
    class AppPermission
    {
        static List<int> lsPermissions = new List<int>();

        private static AppPermission instance;

        public static AppPermission Instance
        {
            get { if (instance == null) instance = new AppPermission(); return instance; }
            private set { instance = value; }
        }

        public void Dispose()
        {
            instance = null;
        }

        private AppPermission()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsPermissions = db.FunctionRoles.Where(r => r.IdRole == TempDatas.RoleUserLogin).Select(r => r.IdFunction ?? 0).ToList();
            }
        }

        /// <summary>
        /// Checks permissions for functions or interfaces.
        /// </summary>
        /// <returns>True if the permission exists, False otherwise.</returns>
        public bool CheckAppPermission(int idFunc)
        {
            return lsPermissions.Contains(idFunc);
        }

        public const int SysAdmin = 17;
        public const int Mod = 7;

        // 207
        public const int KnowledgeMain = 1;
        public const int DataStatistics = 6;
        public const int CustomerInfos = 19;
    }
}
