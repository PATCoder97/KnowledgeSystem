using BusinessLayer;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase;
using KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate;
using KnowledgeSystem.Views._03_DepartmentManage._03_ShiftSchedule;
using KnowledgeSystem.Views._03_DepartmentManage._06_Signature;
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
        static void Main(string[] args)
        {
            new AppCopyRight()
            {
                Version = "24.06.19",
                DateDeploy = "2024.06.19"
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG

            // Application.Run(new f00_Main());

            // TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById("VNW0014732");
            // AppPermission.Instance.CheckAppPermission(7);

            //TPConfigs.IdParentControl = AppPermission.SafetyCertMain;
            //var lsStaticValue = sys_StaticValueBUS.Instance.GetList();
            //TPConfigs.SoftNameEN = lsStaticValue.FirstOrDefault(r => r.KeyT == "SoftNameEN").ValueT;
            //TPConfigs.SoftNameTW = lsStaticValue.FirstOrDefault(r => r.KeyT == "SoftNameTW").ValueT;
            //TPConfigs.UrlUpdate = lsStaticValue.FirstOrDefault(r => r.KeyT == "UrlUpdate").ValueT;
            //TPConfigs.FolderData = lsStaticValue.FirstOrDefault(r => r.KeyT == "FolderData").ValueT;
            TPConfigs.SetSystemStaticValue();
            //Application.Run(new f00_FluentFrame(55));



            // Kiểm tra tham số dòng lệnh
            if (args.Length > 1)
            {
                string idUsr = args[0];
                int parameter = -1; int.TryParse(args[1], out parameter);

                var signDoc = dt306_BaseBUS.Instance.GetItemById(parameter);
                if (signDoc.IsProcess == false)
                {
                    string msg = "文件已處理完！";
                    MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                    return;
                }

                TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById(idUsr);
                f306_SignDocInfo fInfo = new f306_SignDocInfo();
                fInfo.StartPosition = FormStartPosition.CenterScreen;
                fInfo.idBase = 47;
                Application.Run(fInfo);
            }
            else
            {
                Application.Run(new f00_Main());
            }

#else
            Application.Run(new f00_Main());
#endif

        }
    }
}
