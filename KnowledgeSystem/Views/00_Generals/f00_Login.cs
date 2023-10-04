using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.IO;
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

        dm_UserBUS _dm_UserBUS = new dm_UserBUS();

        private void fLogin_Load(object sender, EventArgs e)
        {
            BackgroundImage = Image.FromFile(Path.Combine(TempDatas.ImagesPath, "loginscreen.png"));

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
                string _userID = txbUserID.Text.Trim().ToUpper();
                string _password = txbPassword.Text.Trim();

                dm_User _userLogin = null;

                // Kiểm tra userID và password trong cơ sở dữ liệu
                // Kiểm tra xem có nằm trong DoMain VNFPG không, nếu có thì check tk OA
                if (TempDatas.DomainComputer == DomainVNFPG.domainName)
                {
                    string userNameByDomain = DomainVNFPG.Instance.GetAccountName(_userID);
                    if (!string.IsNullOrEmpty(userNameByDomain))
                    {
                        bool isLoginSuccessful = DomainVNFPG.Instance.CheckLoginDomain(_userID, _password);
                        if (isLoginSuccessful)
                        {
                            _userLogin = _dm_UserBUS.GetUserByUID(_userID);
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
                                    DateCreate = DateTime.Now,
                                };

                                _dm_UserBUS.Create(_userLogin);
                            }
                        }
                    }
                    else
                    {
                        _userLogin = _dm_UserBUS.CheckLogin(_userID, _password);
                    }
                }
                else
                {
                    _userLogin = _dm_UserBUS.CheckLogin(_userID, _password);
                }

                if (_userLogin != null)
                {
                    // Lưu thông tin đăng nhập và đóng form
                    TempDatas.LoginId = _userID;
                    TempDatas.RoleUserLogin = _userLogin.IdRole ?? 0;
                    RegistryHelper.SaveSetting(RegistryHelper.LoginId, _userID);
                    TempDatas.LoginSuccessful = true;

                    // Test
                    _userLogin.SecondaryPassword = txbPassword.Text;
                    _dm_UserBUS.Update(_userLogin);

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
        }
    }
}