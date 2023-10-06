using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
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
    public partial class uc00_ChangePassword : XtraUserControl
    {
        public uc00_ChangePassword()
        {
            InitializeComponent();
        }

        dm_UserBUS _dm_UserBUS = new dm_UserBUS();

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            NewPassword = string.Empty;

            string oldPassword = txbOldPass.Text.Trim();
            string newPassword1 = txbNewPass1.Text.Trim();
            string newPassword2 = txbNewPass2.Text.Trim();

            if (newPassword1 != newPassword2) return;

            var _userCheck = _dm_UserBUS.CheckLogin(TPConfigs.LoginUser.Id, oldPassword);
            if (_userCheck != default)
            {
                NewPassword = newPassword2;
            }
        }

        public string NewPassword { get; set; }
    }
}
