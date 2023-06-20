using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_Login : DevExpress.XtraEditors.XtraForm
    {
        public f00_Login()
        {
            InitializeComponent();
        }

        private void fLogin_Load(object sender, EventArgs e)
        {
            lbNameSoft.Text = TempDatas.SoftNameTW;
            lbVersion.Text = $"Version: {AppCopyRight.version}";

            var loginId = RegistryHelper.GetSetting(RegistryHelper.LoginId, "");
            txbUserID.Text = loginId != null ? loginId.ToString() : "";

            TempDatas.DomainComputer = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            TempDatas.LoginSuccessful = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowDefaultWaitForm();

            string userID = txbUserID.Text.Trim().ToUpper();
            string password = txbPassword.Text.Trim();

            int countUsers = 0;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                countUsers = db.Users.Count(u => u.Id == userID && u.SecondaryPassword == password);
            }

            if (countUsers > 0)
            {
                TempDatas.LoginId = userID;
                RegistryHelper.SaveSetting(RegistryHelper.LoginId, userID);
                SplashScreenManager.CloseDefaultWaitForm();
                TempDatas.LoginSuccessful = true;
                Close();
            }
            else
            {
                XtraMessageBox.Show("用戶名或密碼錯誤，請重試!", "通知!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txbPassword.Text = "";
                txbPassword.Focus();
                SplashScreenManager.CloseDefaultWaitForm();
                return;
            }
        }

        private void fLogin_Shown(object sender, EventArgs e)
        {
            txbPassword.Focus();
            txbPassword.Text = "1";

            btnLogin_Click(sender, e);
        }
    }
}