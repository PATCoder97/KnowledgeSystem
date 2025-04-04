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
using KnowledgeSystem.Views._03_DepartmentManage._02_NewPersonnel;
using KnowledgeSystem.Views._03_DepartmentManage._03_ShiftSchedule;
using KnowledgeSystem.Views._03_DepartmentManage._06_Signature;
using KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck;
using KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
            new AppCopyRight() { Version = "25.03.17" };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TPConfigs.SetSystemStaticValue();
            WindowsFormsSettings.ScrollUIMode = ScrollUIMode.Fluent;

            // Kiểm tra tham số dòng lệnh
            if (args.Length > 0)
            {
                int permCtrl = 0;
                int.TryParse(args[0], out permCtrl);

                string idUsr = "";
                string msg = "";

#if DEBUG
                idUsr = "VNW0014732";
#else
                // Kiểm tra xem ứng dụng đã chạy hay chưa
                string appName = Process.GetCurrentProcess().ProcessName;
                var runningProcesses = Process.GetProcessesByName(appName);

                if (runningProcesses.Count() > 1)
                {
                    msg = "該應用程式已經在運行。結束程式！";
                    MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                    return;
                }

                // Kiểm tra có dùng máy công ty không
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

                // Kiểm tra có phải bản mới không
                using (WebClient client = new WebClient())
                {
                    string result = client.DownloadString(TPConfigs.UrlUpdate);

                    var lsUpdateInfos = JsonConvert.DeserializeObject<List<UpdateInfo>>(result)
                        .Where(r => r.app == TPConfigs.SoftNameEN).ToList();
                    if (lsUpdateInfos != null && lsUpdateInfos.Count > 0)
                    {
                        UpdateInfo newUpdate = lsUpdateInfos.First();
                        if (newUpdate.version != AppCopyRight.version)
                        {
                            msg = "請使用最新版本！";
                            MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                            return;
                        }
                    }
                }
#endif

                TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById(idUsr);

                bool isDeny = !AppPermission.Instance.CheckAppPermission(permCtrl);

                if (isDeny)
                {
                    msg = "<color=red>您沒有權限！</color>";
                    MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                    return;
                }

                f00_FluentFrame formShow = new f00_FluentFrame(permCtrl);
                formShow.Text = TPConfigs.SoftNameTW;
                formShow.ShowDialog();
            }
            else
            {
#if DEBUG
                TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById("VNW0014732");
                TPConfigs.idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
                AppPermission.Instance.CheckAppPermission(7);
                TPConfigs.IdParentControl = AppPermission.SafetyCertMain;

                //Application.Run(new f00_FluentFrame(87));
                Application.Run(new f00_Main());
#else
                Application.Run(new f00_Main());
#endif
            }
        }
    }
}
