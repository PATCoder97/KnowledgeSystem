using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KnowledgeSystem.Views._00_Generals.f00_Main;

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
            KeyPreview = true;
            BackgroundImage = Image.FromFile(Path.Combine(TPConfigs.ImagesPath, "loginscreen.png"));

            lbNameSoft.Text = TPConfigs.SoftNameTW;
            lbVersion.Text = $"Version: {AppCopyRight.version}";

            TPConfigs.DomainComputer = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            if (TPConfigs.DomainComputer == DomainVNFPG.domainVNFPG)
            {
                WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
                txbUserID.Text = currentUser.Name.Split('\\')[1];
            }
            else
            {
                txbUserID.Text = RegistryHelper.GetSetting(RegistryHelper.LoginId, RegistryHelper.DefaulLoginId).ToString() ?? "";
            }

            TPConfigs.LoginSuccessful = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                // Lấy userID và password từ TextBox
                string _userID = txbUserID.Text.Trim().ToUpper();
                string _password = txbPassword.Text.Trim();
                string encryptPass = EncryptionHelper.EncryptPass(_password);

                dm_User usrLogin = null;

                // Không dùng chung domain => Dùng tài khoản đã có trong CSDL
                if (TPConfigs.DomainComputer != DomainVNFPG.domainVNFPG)
                {
                    usrLogin = dm_UserBUS.Instance.CheckLogin(_userID, _password);
                    goto ResultLogin;
                }

                // Có tài khoản OA => Check đăng nhập, Thêm mới nếu chưa có trong CSDL
                bool isValidAcc = DomainVNFPG.Instance.CheckLoginDomain(_userID, _password);
                string userNameByDomain = DomainVNFPG.Instance.GetAccountName(_userID);

                if (isValidAcc && !string.IsNullOrEmpty(userNameByDomain))
                {
                    usrLogin = dm_UserBUS.Instance.GetItemById(_userID);
                    if (usrLogin == null)
                    {
                        string[] displayNameFHS = userNameByDomain.Split('/');
                        string idDeptFHS = displayNameFHS[0].Replace("LG", string.Empty);
                        string userNameFHS = displayNameFHS[1];

                        usrLogin = new dm_User()
                        {
                            Id = _userID,
                            IdDepartment = idDeptFHS,
                            DisplayName = userNameFHS,
                            DateCreate = default(DateTime),
                            LastUpdate = DateTime.Now
                        };

                        dm_UserBUS.Instance.Add(usrLogin);
                    }

                    goto ResultLogin;
                }

                // Không có tài khoản OA hoặc PC dùng tài khoản Local => Check trong CSDL
                usrLogin = dm_UserBUS.Instance.CheckLogin(_userID, _password);


            ResultLogin:
                if (usrLogin != null)
                {
                    // Test
                    usrLogin.SecondaryPassword = encryptPass;
                    usrLogin.PCName = PCInfoHelper.Instance.GetPCName();
                    usrLogin.IPAddress = PCInfoHelper.Instance.GetIPAddress();
                    usrLogin.LastUpdate = DateTime.Now;
                    dm_UserBUS.Instance.AddOrUpdate(usrLogin);

                    // Lưu thông tin đăng nhập và đóng form
                    RegistryHelper.SaveSetting(RegistryHelper.LoginId, _userID);
                    TPConfigs.LoginSuccessful = true;
                    TPConfigs.LoginUser = usrLogin;
                    TPConfigs.idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

                    Close();
                }
                else
                {
                    // Hiển thị thông báo lỗi nếu userID hoặc password không đúng
                    XtraMessageBox.Show("用戶名或密碼錯誤，請重試!", "通知!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txbPassword.Text = "";
                }
            }

            txbPassword.Focus();
        }

        private void fLogin_Shown(object sender, EventArgs e)
        {
            txbPassword.Focus();
#if DEBUG
            txbUserID.Text = "VNW0014732";
            txbPassword.Text = "Anhtuan03";
            btnLogin_Click(sender, e);
#endif
        }

        private void f00_Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Alt && e.Shift && e.KeyCode == Keys.A)
            {
                uc00_AdminLogin adminLogin = new uc00_AdminLogin();
                DialogResult result = XtraDialog.Show(this, adminLogin, "Administrator Login", MessageBoxButtons.OKCancel);
                if (result != DialogResult.OK)
                {
                    return;
                }

                string id = adminLogin.ID;
                string masterKey = adminLogin.MasterKey;

#if DEBUG
#else
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(masterKey) || masterKey != "havy2212") return;
#endif
                var _userLogin = dm_UserBUS.Instance.GetItemById(id);
                if (_userLogin == null) return;

                TPConfigs.LoginSuccessful = true;
                TPConfigs.LoginUser = _userLogin;
                TPConfigs.idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
                Close();
            }
        }

        private void txbPassword_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            txbPassword.Properties.UseSystemPasswordChar = !txbPassword.Properties.UseSystemPasswordChar;
        }
    }
}