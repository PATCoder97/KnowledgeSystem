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

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            NewPassword = string.Empty;

            string oldPassword = txbOldPass.Text.Trim();
            string newPassword1 = txbNewPass1.Text.Trim();
            string newPassword2 = txbNewPass2.Text.Trim();

            if (newPassword1 != newPassword2) return;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                bool IsExistUser = db.Users.Any(r => r.Id == TempDatas.LoginId && r.SecondaryPassword == oldPassword);
                if (IsExistUser)
                {
                    NewPassword = newPassword2;
                }
            }
        }

        public string NewPassword { get; set; }
    }
}
