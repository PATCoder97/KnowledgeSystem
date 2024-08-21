using DataAccessLayer;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_Exam_Info : DevExpress.XtraEditors.XtraForm
    {
        public f307_Exam_Info()
        {
            InitializeComponent();
        }

        List<dm_User> usrs = new List<dm_User>();


        private void btnSelectUsr_Click(object sender, EventArgs e)
        {
            f307_UsersData fData = new f307_UsersData();
            fData.UsersInput = usrs;
            fData.ShowDialog();
        }
    }
}