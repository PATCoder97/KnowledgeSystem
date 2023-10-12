using BusinessLayer;
using DataAccessLayer;
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
        dm_FunctionRoleBUS _dm_FunctionRoleBUS = new dm_FunctionRoleBUS();

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
            lsPermissions = _dm_FunctionRoleBUS.GetListByRole(TPConfigs.LoginUser.IdRole).Select(r => r.IdFunction).ToList();
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
        public const int KnowledgeMain = 1;

        // 207
        public const int DataStatistics = 6;
        public const int CustomerInfos = 19;

        // 401
        //public const int P401AllDept = 1;
    }
}
