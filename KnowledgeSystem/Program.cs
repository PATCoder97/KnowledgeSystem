using BusinessLayer;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase;
using KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate;
using KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin;
using System;
using System.Collections.Generic;
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
                Version = "1.0.3",
                DateDeploy = "2023.10.14"
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new f00_Main());

            //TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById("VNW0014732");
            //AppPermission.Instance.CheckAppPermission(7);
            //Application.Run(new f00_FluentFrame(21));
        }
    }
}
