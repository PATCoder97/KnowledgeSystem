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
    public partial class uc307_ExamMgmt : DevExpress.XtraEditors.XtraUserControl
    {
        public uc307_ExamMgmt()
        {
            InitializeComponent();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f307_Exam_Info fInfo = new f307_Exam_Info();
            fInfo.ShowDialog();
        }
    }
}
