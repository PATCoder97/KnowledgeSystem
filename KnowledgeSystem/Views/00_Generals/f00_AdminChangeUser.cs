using BusinessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_AdminChangeUser : DevExpress.XtraEditors.XtraForm
    {
        public f00_AdminChangeUser()
        {
            InitializeComponent();
        }

        private void f00_AdminChangeUser_Load(object sender, EventArgs e)
        {
            var users = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0).ToList();

            txbUser.Properties.DataSource = users;
            txbUser.Properties.DisplayMember = "DisplayName";
            txbUser.Properties.ValueMember = "Id";
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById(txbUser.EditValue.ToString());

            Close();
        }
    }
}