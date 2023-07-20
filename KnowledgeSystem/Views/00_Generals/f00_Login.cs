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
            txbUserID.Text = RegistryHelper.GetSetting(RegistryHelper.LoginId, "").ToString() ?? "";

            TempDatas.DomainComputer = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            TempDatas.LoginSuccessful = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowDefaultWaitForm();

            // Lấy userID và password từ TextBox
            string userID = txbUserID.Text.Trim().ToUpper();
            string password = txbPassword.Text.Trim();

            // Kiểm tra userID và password trong cơ sở dữ liệu
            using (var db = new DBDocumentManagementSystemEntities())
            {
                // Đếm số lượng người dùng có ID và mật khẩu tương ứng
                int countUsers = db.Users.Count(u => u.Id == userID && u.SecondaryPassword == password);

                if (countUsers > 0)
                {
                    // Lưu thông tin đăng nhập và đóng form
                    TempDatas.LoginId = userID;
                    RegistryHelper.SaveSetting(RegistryHelper.LoginId, userID);
                    TempDatas.LoginSuccessful = true;
                    Close();
                }
                else
                {
                    // Hiển thị thông báo lỗi nếu userID hoặc password không đúng
                    XtraMessageBox.Show("用戶名或密碼錯誤，請重試!", "通知!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txbPassword.Text = "";
                    txbPassword.Focus();
                }
            }

            SplashScreenManager.CloseDefaultWaitForm();
        }

        private void fLogin_Shown(object sender, EventArgs e)
        {
            txbPassword.Focus();
            txbPassword.Text = "1";

            // btnLogin_Click(sender, e);
        }
    }
}