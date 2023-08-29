using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_Login : XtraForm
    {
        public f00_Login()
        {
            InitializeComponent();
        }

        private void fLogin_Load(object sender, EventArgs e)
        {
            lbNameSoft.Text = TempDatas.SoftNameTW;
            lbVersion.Text = $"Version: {AppCopyRight.version}";
            txbUserID.Text = RegistryHelper.GetSetting(RegistryHelper.LoginId, RegistryHelper.DefaulLoginId).ToString() ?? "";

            TempDatas.DomainComputer = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            TempDatas.LoginSuccessful = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                // Lấy userID và password từ TextBox
                string userID = txbUserID.Text.Trim().ToUpper();
                string password = txbPassword.Text.Trim();

                User userLogin = null;

                // Kiểm tra userID và password trong cơ sở dữ liệu
                using (var db = new DBDocumentManagementSystemEntities())
                {
                    // Kiểm tra xem có nằm trong DoMain VNFPG không, nếu có thì check tk OA
                    if (TempDatas.DomainComputer == DomainVNFPG.domainName)
                    {
                        string userNameByDomain = DomainVNFPG.Instance.GetAccountName(userID);
                        if (!string.IsNullOrEmpty(userNameByDomain))
                        {
                            bool isLoginSuccessful = DomainVNFPG.Instance.CheckLoginDomain(userID, password);
                            if (isLoginSuccessful)
                            {
                                userLogin = db.Users.FirstOrDefault(r => r.Id == userID);
                                if (userLogin == null)
                                {
                                    string[] displayNameFHS = userNameByDomain.Split('/');
                                    string idDeptFHS = displayNameFHS[0].Replace("LG", string.Empty);
                                    string userNameFHS = displayNameFHS[1];

                                    userLogin = new User()
                                    {
                                        Id = userID,
                                        IdDepartment = idDeptFHS,
                                        DisplayName = userNameFHS,
                                        DateCreate = DateTime.Now,
                                    };

                                    db.Users.AddOrUpdate(userLogin);
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            userLogin = db.Users.FirstOrDefault(r => r.Id == userID && r.SecondaryPassword == password);
                        }
                    }
                    else
                    {
                        userLogin = db.Users.FirstOrDefault(r => r.Id == userID && r.SecondaryPassword == password);
                    }

                    if (userLogin != null)
                    {
                        // Lưu thông tin đăng nhập và đóng form
                        TempDatas.LoginId = userID;
                        TempDatas.RoleUserLogin = userLogin.IdRole ?? 0;
                        RegistryHelper.SaveSetting(RegistryHelper.LoginId, userID);
                        TempDatas.LoginSuccessful = true;

                        Close();
                    }
                    else
                    {
                        // Hiển thị thông báo lỗi nếu userID hoặc password không đúng
                        XtraMessageBox.Show("用戶名或密碼錯誤，請重試!", "通知!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txbPassword.Text = "";
                    }
                }
            }
            txbPassword.Focus();
        }

        private void fLogin_Shown(object sender, EventArgs e)
        {
            txbPassword.Focus();
        }
    }
}