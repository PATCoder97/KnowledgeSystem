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

namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    public partial class uc301_SelectOutputFile : DevExpress.XtraEditors.XtraUserControl
    {
        public uc301_SelectOutputFile()
        {
            InitializeComponent();
        }

        private void uc301_SelectOutputFile_Load(object sender, EventArgs e)
        {
            List<int> lsQuarters = new List<int>() { 1, 2, 3, 4 };
            cbbQuarter.Properties.Items.AddRange(lsQuarters);

            DateTime today = DateTime.Today;
           int currentQuarter = (today.Month - 1) / 3 + 1;
            cbbQuarter.SelectedItem = currentQuarter;
        }

        private void Change_CheckFile(object sender, EventArgs e)
        {
            fileMappings = new Dictionary<string, string>();

            List<CheckEdit> lsCheckBoxs = new List<CheckEdit>() { ckFile1, ckFile2, ckFile3, ckFile4, ckFile5_1, ckFile5_2, ckFile6 };
            foreach (CheckEdit ck in lsCheckBoxs)
            {
                if (ck.Checked)
                {
                    fileMappings.Add(ck.Name.Replace("ck", ""), ck.Text);
                }
            }
        }

        public int quarter { get; set; }
        public Dictionary<string, string> fileMappings { get; set; }

        private void cbbQuarter_SelectedIndexChanged(object sender, EventArgs e)
        {
            quarter = Convert.ToInt16(cbbQuarter.EditValue);
        }
    }
}
