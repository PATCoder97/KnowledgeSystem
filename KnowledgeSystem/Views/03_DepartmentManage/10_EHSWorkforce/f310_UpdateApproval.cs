using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_UpdateApproval : DevExpress.XtraEditors.XtraForm
    {
        public f310_UpdateApproval()
        {
            InitializeComponent();
        }

        private void f310_UpdateApproval_Load(object sender, EventArgs e)
        {
            webViewUpdateData.DocumentText = File.ReadAllText(@"C:\Users\Dell Alpha\Desktop\a thanh\updateleaveruser.html");
        }
    }
}