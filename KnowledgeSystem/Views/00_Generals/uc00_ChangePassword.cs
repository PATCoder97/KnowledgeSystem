﻿using BusinessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class uc00_ChangePassword : XtraUserControl
    {
        public uc00_ChangePassword()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            NewPassword = string.Empty;

            string oldPassword = txbOldPass.Text.Trim();
            string newPassword1 = txbNewPass1.Text.Trim();
            string newPassword2 = txbNewPass2.Text.Trim();

            if (newPassword1 != newPassword2) return;

            var _userCheck = dm_UserBUS.Instance.CheckLogin(TPConfigs.LoginUser.Id, oldPassword);
            if (_userCheck != default)
            {
                NewPassword = newPassword2;
            }
        }

        public string NewPassword { get; set; }
    }
}
