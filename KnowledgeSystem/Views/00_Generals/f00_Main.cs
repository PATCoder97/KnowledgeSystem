using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraEditors;
using KnowledgeSystem.Configs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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

        private void IniUserInfo()
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
        }

        private void GetUserLogin()
        {
            Size = new Size(100, 100);
            WindowState = FormWindowState.Normal;
            f00_Login frm = new f00_Login();
            frm.ShowDialog();

            if (!TempDatas.LoginSuccessful)
            {
                Close();
                return;
            }

            WindowState = FormWindowState.Maximized;
            Size = new Size(1000, 600);
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                           (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var userLogin = db.Users.First(r => r.Id == TempDatas.LoginId);
                string userName = userLogin.DisplayName;
                string idDept = userLogin.IdDepartment;
                var gradeName = db.dm_Departments.First(r => r.Id == idDept.Substring(0, 2)).DisplayName;
                var gradeClass = db.dm_Departments.First(r => r.Id == idDept).DisplayName;

                elementName.Text = userName;
                elementIdDept.Text = idDept;
                elementGrade.Text = gradeName;
                elementClass.Text = gradeClass;
            }
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
                XtraMessageBox.Show("舊密碼不正確或兩個新密碼不匹配！", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var userUpdate = db.Users.First(r => r.Id == TempDatas.LoginId);
                userUpdate.SecondaryPassword = newPassword;

                db.SaveChanges();
            }

            XtraMessageBox.Show("您的密碼已更新！", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GetSysStaticValue()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsStaticValue = db.sys_StaticValue.ToList();

                TempDatas.SoftNameEN = lsStaticValue.FirstOrDefault(r => r.KeyT == "SoftNameEN").ValueT;
                TempDatas.SoftNameTW = lsStaticValue.FirstOrDefault(r => r.KeyT == "SoftNameTW").ValueT;
                TempDatas.UrlUpdate = lsStaticValue.FirstOrDefault(r => r.KeyT == "UrlUpdate").ValueT;
                TempDatas.PathKnowledgeFile = lsStaticValue.FirstOrDefault(r => r.KeyT == "PathKnowledgeFile").ValueT;
            }
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            GetSysStaticValue();

#if DEBUG
            // Không cần check update khi debug
#else
            using (WebClient client = new WebClient())
            {
                string result = client.DownloadString(TempDatas.UrlUpdate);

                var lsUpdateInfos = JsonConvert.DeserializeObject<List<UpdateInfo>>(result)
                    .Where(r => r.app == TempDatas.SoftNameEN).ToList();
                if (lsUpdateInfos != null && lsUpdateInfos.Count > 0)
                {
                    UpdateInfo newUpdate = lsUpdateInfos.First();
                    if (newUpdate.version != AppCopyRight.version)
                    {
                        string msg = "Phần mềm có bản cập nhật mới.\r\nBấm OK để hệ thống cập nhật,\r\nVui lòng mở lại phần mềm sau khi cập nhật thành công!\r\n";
                        msg += "該系統有新的更新。\r\n按確定更新系統，\r\n更新成功後請重新打開系統！";
                        var dialogResult = XtraMessageBox.Show(msg, TempDatas.SoftNameTW, MessageBoxButtons.OKCancel);
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

            Text = TempDatas.SoftNameTW + AppCopyRight.CopyRightString();
            lbSoftName.Text = TempDatas.SoftNameTW;

            Size = new Size(100, 100);
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                          (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            StartPosition = FormStartPosition.CenterScreen;

            IniUserInfo();
        }

        private void fMain_Shown(object sender, EventArgs e)
        {
            GetUserLogin();
        }

        private void btnKnowHow_ItemClick(object sender, TileItemEventArgs e)
        {
            f00_FluentFrame formShow = new f00_FluentFrame(1);
            formShow.Text = e.Item.Text;
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void btnISODocuments_ItemClick(object sender, TileItemEventArgs e)
        {
            f00_FluentFrame formShow = new f00_FluentFrame(201);
            formShow.Text = e.Item.Text;
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void btnUserManage_ItemClick(object sender, TileItemEventArgs e)
        {
            f00_FluentFrame formShow = new f00_FluentFrame(7);
            formShow.Text = e.Item.Text;
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void btnRoleManage_ItemClick(object sender, TileItemEventArgs e)
        {
            f00_FluentFrame formShow = new f00_FluentFrame(17);
            formShow.Text = e.Item.Text;
            Hide();
            formShow.ShowDialog();
            Show();
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
    }
}