using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_Main : DevExpress.XtraEditors.XtraForm
    {
        public f00_Main()
        {
            InitializeComponent();
        }

        TileItemElement elementName = new TileItemElement();
        TileItemElement elementIdDept = new TileItemElement();
        TileItemElement elementGrade = new TileItemElement();
        TileItemElement elementClass = new TileItemElement();

        private void InitializeControl()
        {
            elementName.Appearance.Normal.Font = new Font("DFKai-SB", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementName.Appearance.Normal.Options.UseFont = true;
            elementName.TextAlignment = TileItemContentAlignment.TopLeft;

            elementIdDept.Appearance.Normal.Font = new Font("DFKai-SB", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementIdDept.Appearance.Normal.Options.UseFont = true;
            elementIdDept.TextAlignment = TileItemContentAlignment.BottomLeft;
            elementIdDept.TextLocation = new Point(5, -40);

            elementGrade.Appearance.Normal.Font = new Font("DFKai-SB", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementGrade.Appearance.Normal.Options.UseFont = true;
            elementGrade.TextAlignment = TileItemContentAlignment.BottomLeft;
            elementGrade.TextLocation = new Point(5, -20);

            elementClass.Appearance.Normal.Font = new Font("DFKai-SB", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementClass.Appearance.Normal.Options.UseFont = true;
            elementClass.TextAlignment = TileItemContentAlignment.BottomLeft;
            elementClass.TextLocation = new Point(5, 0);

            tileInfoUser.Elements.Add(elementName);
            tileInfoUser.Elements.Add(elementIdDept);
            tileInfoUser.Elements.Add(elementGrade);
            tileInfoUser.Elements.Add(elementClass);

            var lsFuncs = dm_FunctionBUS.Instance.GetList();
            btnSysAdmin.Text = lsFuncs.First(r => r.Id == AppPermission.SysAdmin).DisplayName;
            btnMod.Text = lsFuncs.First(r => r.Id == AppPermission.Mod).DisplayName;
            btnKnowHow.Text = lsFuncs.First(r => r.Id == AppPermission.KnowledgeMain).DisplayName;
            btnSafetyCert.Text = lsFuncs.First(r => r.Id == AppPermission.SafetyCertMain).DisplayName;
        }

        private void GetUserLogin()
        {
            f00_Login frm = new f00_Login();
            frm.ShowDialog();

            if (!TPConfigs.LoginSuccessful)
            {
                Close();
                return;
            }

            WindowState = FormWindowState.Maximized;
            Size = new Size(1000, 600);
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                           (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);

            var _userLogin = dm_UserBUS.Instance.GetItemById(TPConfigs.LoginUser.Id);
            string userName = _userLogin.DisplayName;
            string idDept = _userLogin.IdDepartment;
            var gradeName = dm_DeptBUS.Instance.GetItemById(idDept.Substring(0, 2)).DisplayName;
            var gradeClass = dm_DeptBUS.Instance.GetItemById(idDept).DisplayName;
            elementName.Text = userName;
            elementIdDept.Text = idDept;
            elementGrade.Text = gradeName;
            elementClass.Text = gradeClass;
        }

        private void ChangePassword()
        {
            uc00_ChangePassword uc00_ChangePass = new uc00_ChangePassword();

            var dlg = VBFlyoutDialog.ShowFormPopup(this, null, uc00_ChangePass);
            if (dlg != DialogResult.OK)
            {
                return;
            }

            string newPassword = uc00_ChangePass.NewPassword;
            if (string.IsNullOrEmpty(newPassword))
            {
                XtraMessageBox.Show("舊密碼不正確或兩個新密碼不匹配！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dm_UserBUS.Instance.ChangePass(TPConfigs.LoginUser.Id, newPassword);
            XtraMessageBox.Show("您的密碼已更新！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GetSysStaticValue()
        {
            var lsStaticValue = sys_StaticValueBUS.Instance.GetList();

            TPConfigs.SoftNameEN = lsStaticValue.FirstOrDefault(r => r.KeyT == "SoftNameEN").ValueT;
            TPConfigs.SoftNameTW = lsStaticValue.FirstOrDefault(r => r.KeyT == "SoftNameTW").ValueT;
            TPConfigs.UrlUpdate = lsStaticValue.FirstOrDefault(r => r.KeyT == "UrlUpdate").ValueT;
            TPConfigs.FolderData = lsStaticValue.FirstOrDefault(r => r.KeyT == "FolderData").ValueT;
            TPConfigs.Folder207 = Path.Combine(TPConfigs.FolderData, "207"); 
            TPConfigs.Folder302 = Path.Combine(TPConfigs.FolderData, "302");
            TPConfigs.Folder202 = Path.Combine(TPConfigs.FolderData, "202");

            // Lấy các role
            AppPermission.SysAdmin = Convert.ToInt16(lsStaticValue.FirstOrDefault(r => r.KeyT == "RoleSysAdmin").ValueT);
            AppPermission.Mod = Convert.ToInt16(lsStaticValue.FirstOrDefault(r => r.KeyT == "RoleMod").ValueT);
            AppPermission.KnowledgeMain = Convert.ToInt16(lsStaticValue.FirstOrDefault(r => r.KeyT == "RoleKnowledgeMain").ValueT);
            AppPermission.SafetyCertMain = Convert.ToInt16(lsStaticValue.FirstOrDefault(r => r.KeyT == "RoleSafetyCertMain").ValueT);
            AppPermission.WorkManagementMain = Convert.ToInt16(lsStaticValue.FirstOrDefault(r => r.KeyT == "RoleWorkManagementMain").ValueT);
            AppPermission.JFEnCSCMain = Convert.ToInt16(lsStaticValue.FirstOrDefault(r => r.KeyT == "JFEnCSCMain").ValueT);
            AppPermission.ISOAuditDocsMain = Convert.ToInt16(lsStaticValue.FirstOrDefault(r => r.KeyT == "ISOAuditDocsMain").ValueT);
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            BackgroundImage = TPSvgimages.Background;

            GetSysStaticValue();
            GetUserLogin();

#if DEBUG
            // Không cần check update khi debug
#else
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
                        var dialogResult = MsgTP.MsgUpdateSoftware();
                        if (dialogResult == DialogResult.OK)
                        {
                            f00_UpdateSoftware f00_Update = new f00_UpdateSoftware(newUpdate.url);
                            f00_Update.ShowDialog();
                        }

                        Close();
                    }
                }
            }
#endif

            try
            {
                // Xoá thư mục tạm nơi lưu các file tải về để xem
                Directory.Delete(TPConfigs.TempFolderData, true);
            }
            catch { }

            Text = TPConfigs.SoftNameTW + AppCopyRight.CopyRightString();
            lbSoftName.Text = TPConfigs.SoftNameTW;

            InitializeControl();
        }

        private void fMain_Shown(object sender, EventArgs e)
        {

        }

        private void ShowFromByFrame(int IdForm_, TileItemEventArgs e)
        {
            bool IsGranted = AppPermission.Instance.CheckAppPermission(IdForm_);
            if (!IsGranted)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            TPConfigs.IdParentControl = IdForm_;
            f00_FluentFrame formShow = new f00_FluentFrame(IdForm_);
            formShow.Text = e.Item.Text;
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void btnKnowHow_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.KnowledgeMain, e);
        }

        private void btnUserManage_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.Mod, e);
        }

        private void btnRoleManage_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.SysAdmin, e);
        }

        private void btnSafetyCert_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.SafetyCertMain, e);
        }

        private void btnWorkManagement_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.WorkManagementMain, e);
        }

        private void tileInfoUser_ItemClick(object sender, TileItemEventArgs e)
        {
            uc00_UserControl uc00_User = new uc00_UserControl();

            var dlg = VBFlyoutDialog.ShowFormPopup(this, null, uc00_User);
            if (dlg != DialogResult.OK)
            {
                return;
            }

            UserComtrolE eventControl = uc00_User.EventControl;
            switch (eventControl)
            {
                case UserComtrolE.ChangePass:
                    ChangePassword();
                    break;
                case UserComtrolE.LogOut:
                    AppPermission.Instance.Dispose();
                    GetUserLogin();
                    break;
                default:
                    break;
            }
        }

        public class VBFlyoutDialog : FlyoutDialog
        {
            public VBFlyoutDialog(Form form, FlyoutAction action, Control userControl) : base(form, action)
            {
                Properties.HeaderOffset = 0;
                Properties.Alignment = ContentAlignment.MiddleCenter;
                Properties.Style = FlyoutStyle.Popup;
                FlyoutControl = userControl;
            }

            public static DialogResult ShowFormPopup(Form form, FlyoutAction action, Control userControl)
            {
                var vbFlyout = new VBFlyoutDialog(form, action, userControl);
                return vbFlyout.ShowDialog();
            }
        }

        private void btnJFEnCSC_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.JFEnCSCMain, e);
        }

        private void btnISOAuditDocs_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.ISOAuditDocsMain, e);
        }
    }
}