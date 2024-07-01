using BusinessLayer;
using DataAccessLayer;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
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
using System.Reflection;
using System.Security.Principal;
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
            new AppCopyRight() { Version = "24.06.28" };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TPConfigs.SetSystemStaticValue();

            // Kiểm tra tham số dòng lệnh
            if (args.Length > 2)
            {
                string idUsr = "";
                string permCtrl = args[0];
                string formTaget = args[1];
                string paramter1 = args[2];
                string msg = "";

#if DEBUG
                idUsr = "VNW0014732";
#else
                TPConfigs.DomainComputer = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                if (TPConfigs.DomainComputer == DomainVNFPG.domainVNFPG)
                {
                    WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
                    idUsr = currentUser.Name.Split('\\')[1];
                }
                else
                {
                    msg = "請使用公司電腦！";
                    MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                    return;
                }
#endif

                TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById(idUsr);

                var funcs = dm_FunctionBUS.Instance.GetItemByControl(permCtrl);
                bool isDeny = !AppPermission.Instance.CheckAppPermission(funcs.Id);

                if (isDeny)
                {
                    msg = "<color=red>您沒有權限！</color>";
                    MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                    return;
                }

                switch (formTaget)
                {
                    case "f306_SignDocInfo":
                        int parameter = -1; int.TryParse(paramter1, out parameter);

                        var signDoc = dt306_BaseBUS.Instance.GetItemById(parameter);
                        if (signDoc.IsProcess == false || signDoc.NextStepProg != TPConfigs.LoginUser.Id)
                        {
                            msg = "文件已處理完！";
                            MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                            return;
                        }

                        f306_SignDocInfo fInfo = new f306_SignDocInfo();
                        fInfo.StartPosition = FormStartPosition.CenterScreen;
                        fInfo.idBase = signDoc.Id;
                        Application.Run(fInfo);
                        break;

                    default:
                        msg = "<color=red>系统错误！</color>";
                        MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                        break;
                }
            }
            else
            {
#if DEBUG
                // Application.Run(new f00_Main());

                TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById("VNW0014732");
                AppPermission.Instance.CheckAppPermission(7);
                TPConfigs.IdParentControl = AppPermission.SafetyCertMain;

                Application.Run(new f00_FluentFrame(55));
#else
                Application.Run(new f00_Main());
#endif
            }
        }
    }
}
