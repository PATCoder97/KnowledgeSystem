using BusinessLayer;
using DataAccessLayer;
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
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._04_BorrVehicle
{
    public partial class uc304_ChangeUsr : DevExpress.XtraEditors.XtraUserControl
    {
        public uc304_ChangeUsr()
        {
            InitializeComponent();

            cbbUser.DataBindings.Add("EditValue", this, "IdSelectUser");
        }

        public string IdSelectUser { get; set; }

        private void uc304_ChangeUsr_Load(object sender, EventArgs e)
        {
            string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
            var users = dm_UserBUS.Instance.GetList()
                .Where(r => r.Status == 0 && r.IdDepartment.StartsWith(idDept2word)).ToList();

            cbbUser.Properties.DataSource = users;
            cbbUser.Properties.DisplayMember = "DisplayName";
            cbbUser.Properties.ValueMember = "Id";
        }
    }
}
