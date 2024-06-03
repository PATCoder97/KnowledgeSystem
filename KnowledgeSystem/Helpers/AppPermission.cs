using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList.Painter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    class AppPermission
    {
        dm_FunctionRoleBUS _dm_FunctionRoleBUS = new dm_FunctionRoleBUS();

        public static List<int> lsPermissions = new List<int>();

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
            var lsUserRoles = dm_UserRoleBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var lsFuncRoles = _dm_FunctionRoleBUS.GetList();

            lsPermissions = (from data in lsUserRoles
                             join func in lsFuncRoles on data.IdRole equals func.IdRole
                             select func.IdFunction).Distinct().ToList();
        }

        /// <summary>
        /// Checks permissions for functions or interfaces.
        /// </summary>
        /// <returns>True if the permission exists, False otherwise.</returns>
        public bool CheckAppPermission(int idFunc)
        {
            return lsPermissions.Contains(idFunc);
        }

        public static int SysAdmin { get; set; }
        public static int Mod { get; set; }
        public static int KnowledgeMain { get; set; }
        public static int SafetyCertMain { get; set; }
        public static int WorkManagementMain { get; set; }
        public static int JFEnCSCMain { get; set; }
        public static int ISOAuditDocsMain { get; set; }
        public static int TechnicalPrjMain { get; set; }
        public static int ElectronicSignature { get; set; }

        public static List<int> GetListAutoOpenForm()
        {
            return new List<int>() { SysAdmin, Mod, KnowledgeMain, SafetyCertMain, JFEnCSCMain, TechnicalPrjMain };
        }

        // 207
        public const int DataStatistics = 6;
        public const int CustomerInfos = 19;

        // 401
        public const int EditUserJobAndDept = 33;

        // 202
        public const int EditDoc202 = 40;
    }
}
