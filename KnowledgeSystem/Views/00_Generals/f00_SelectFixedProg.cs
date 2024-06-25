using BusinessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
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

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_SelectFixedProg : DevExpress.XtraEditors.XtraForm
    {
        public f00_SelectFixedProg()
        {
            InitializeComponent();
        }

        public int Id { get; set; }

        private void LoadData()
        {
            var fixedProgress = dm_FixedProgressBUS.Instance.GetListByOwner(TPConfigs.LoginUser.Id);

            txbProgress.Properties.DataSource = fixedProgress;
            txbProgress.Properties.DisplayMember = "DisplayName";
            txbProgress.Properties.ValueMember = "Id";
        }

        private void f00_SelectFixedProg_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (txbProgress.EditValue == null) return;
            
            Id = (int)txbProgress.EditValue;
            Close();
        }

        private void txbProgManager_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            f00_FixedProgMain fixedProgMain = new f00_FixedProgMain();
            fixedProgMain.ShowDialog();

            LoadData();
        }
    }
}