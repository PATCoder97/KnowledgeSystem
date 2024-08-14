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

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_JobSet_Info : DevExpress.XtraEditors.XtraForm
    {
        public f307_JobSet_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string idJob = "";

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f307_JobSet_Info_Load(object sender, EventArgs e)
        {
            var jobTitles = dm_JobTitleBUS.Instance.GetList();
            var depts = dm_DeptBUS.Instance.GetList().Where(r => r.Id.Length == 4)
               .Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id,-5}{r.DisplayName}" }).ToList();
            cbbDept.Properties.DataSource = depts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            var dataInfo = dt307_JobQuesManageBUS.Instance.GetItemByIdJob(idJob);

            txbJobTitle.Text = jobTitles.FirstOrDefault(r => r.Id == idJob).DisplayName;
            if (dataInfo != null)
            {
                cbbDept.EditValue = dataInfo.IdDept;
                txbTime.EditValue = dataInfo.TestDuration;
                txbPassScore.EditValue = dataInfo.PassingScore;
                txbTotalQues.EditValue = dataInfo.QuesCount;
            }
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(cbbDept.Text) || string.IsNullOrEmpty(txbTime.Text) || string.IsNullOrEmpty(txbPassScore.Text) || string.IsNullOrEmpty(txbTotalQues.Text))
            {
                return;
            }

            dt307_JobQuesManage data = new dt307_JobQuesManage()
            {
                JobId = idJob,
                IdDept = cbbDept.EditValue.ToString(),
                PassingScore = Convert.ToInt16(txbPassScore.EditValue),
                TestDuration = Convert.ToInt16(txbTime.EditValue),
                QuesCount = Convert.ToInt16(txbTotalQues.EditValue),
            };

            dt307_JobQuesManageBUS.Instance.AddOrUpdate(data);

            Close();
        }
    }
}