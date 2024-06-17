using BusinessLayer;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase;
using KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate;
using KnowledgeSystem.Views._03_DepartmentManage._03_ShiftSchedule;
using KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new AppCopyRight()
            {
                Version = "24.01.26",
                DateDeploy = "2024.04.01"
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG

            Application.Run(new f00_Main());

            //TPConfigs.SoftNameTW = "冶金文管系統";
            //TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById("VNW0014732");
            //AppPermission.Instance.CheckAppPermission(7);

            //TPConfigs.IdParentControl = AppPermission.SafetyCertMain;
            //var lsStaticValue = sys_StaticValueBUS.Instance.GetList();
            //TPConfigs.SoftNameEN = lsStaticValue.FirstOrDefault(r => r.KeyT == "SoftNameEN").ValueT;
            //TPConfigs.SoftNameTW = lsStaticValue.FirstOrDefault(r => r.KeyT == "SoftNameTW").ValueT;
            //TPConfigs.UrlUpdate = lsStaticValue.FirstOrDefault(r => r.KeyT == "UrlUpdate").ValueT;
            //TPConfigs.FolderData = lsStaticValue.FirstOrDefault(r => r.KeyT == "FolderData").ValueT;
            //TPConfigs.SetFolderData();
            //Application.Run(new f00_FluentFrame(55));

            //Application.Run(new f00_AdminChangeUser());

#else
            Application.Run(new f00_Main());
#endif

        }
    }
}
