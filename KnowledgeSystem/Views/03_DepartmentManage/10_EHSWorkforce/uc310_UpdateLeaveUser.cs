using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class uc310_UpdateLeaveUser : DevExpress.XtraEditors.XtraUserControl
    {
        public uc310_UpdateLeaveUser()
        {
            InitializeComponent();
        }

        private void uc310_UpdateLeaveUser_Load(object sender, EventArgs e)
        {
            f310_UpdateLeaveUser_Info fUpdate = new f310_UpdateLeaveUser_Info();
            fUpdate.ShowDialog();
        }
    }
}
