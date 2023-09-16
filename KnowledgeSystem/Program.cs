using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase;
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
                Version = "0.0.1",
                DateDeploy = "2023.09.15"
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new f00_Main());

            //TempDatas.LoginId = "VNW0014732";
            //TempDatas.RoleUserLogin = 0;
            //Application.Run(new f00_FluentFrame(1));
        }
    }
}
