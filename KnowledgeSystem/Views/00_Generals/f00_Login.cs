using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
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

                dm_User _userLogin = null;

                /* Kiểm tra xem có trong Domain fpg không (Không dùng OA)
                *  - Có thì dùng tài khoản OA để đăng nhập, nếu không có OA thì dùng mật khẩu phụ
                *  - Không thì dùng mật khẩu phụ */
                if (TPConfigs.DomainComputer == DomainVNFPG.domainVNFPG)
                {
                    string userNameByDomain = DomainVNFPG.Instance.GetAccountName(_userID);
                    if (!string.IsNullOrEmpty(userNameByDomain))
                    {
                        bool isLoginSuccessful = DomainVNFPG.Instance.CheckLoginDomain(_userID, _password);
                        if (isLoginSuccessful)
                        {
                            _userLogin = dm_UserBUS.Instance.GetItemById(_userID);
                            if (_userLogin == null)
                            {
                                string[] displayNameFHS = userNameByDomain.Split('/');
                                string idDeptFHS = displayNameFHS[0].Replace("LG", string.Empty);
                                string userNameFHS = displayNameFHS[1];

                                _userLogin = new dm_User()
                                {
                                    Id = _userID,
                                    IdDepartment = idDeptFHS,
                                    DisplayName = userNameFHS,
                                    DateCreate = default(DateTime),
                                };

                                dm_UserBUS.Instance.Add(_userLogin);
                            }
                        }
                    }
                    else
                    {
                        _userLogin = dm_UserBUS.Instance.CheckLogin(_userID, _password);
                    }
                }
                else
                {
                    _userLogin = dm_UserBUS.Instance.CheckLogin(_userID, _password);
                }

                if (_userLogin != null)
                {
                    // Lưu thông tin đăng nhập và đóng form
                    RegistryHelper.SaveSetting(RegistryHelper.LoginId, _userID);
                    TPConfigs.LoginSuccessful = true;
                    TPConfigs.LoginUser = _userLogin;

                    // Test
                    _userLogin.SecondaryPassword = encryptPass;
                    _userLogin.PCName = PCInfoHelper.Instance.GetPCName();
                    _userLogin.IPAddress = PCInfoHelper.Instance.GetIPAddress();
                    dm_UserBUS.Instance.AddOrUpdate(_userLogin);

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
            txbPassword.Text = "Anhtuan06";
            btnLogin_Click(sender, e);
#endif
        }
    }
}