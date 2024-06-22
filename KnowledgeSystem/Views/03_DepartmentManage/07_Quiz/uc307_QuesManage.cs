using BusinessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
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
    public partial class uc307_QuesManage : DevExpress.XtraEditors.XtraUserControl
    {
        public uc307_QuesManage()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                var baseData = dt307_JobQuesManageBUS.Instance.GetList();
                var jobs = dm_JobTitleBUS.Instance.GetList();
                var depts = dm_DeptBUS.Instance.GetList();

                var dataDisplays = (from data in baseData
                                    join job in jobs on data.JobId equals job.Id
                                    join dept in depts on data.IdDept equals dept.Id
                                    select new
                                    {
                                        Id = data.JobId,
                                        DisplayName = job.DisplayName,
                                        Dept = $"{dept.Id}\n{dept.DisplayName}",
                                    }).ToList();

                txbJob.DataSource = dataDisplays;
                txbJob.DisplayMember = "DisplayName";
                txbJob.ValueMember = "Id";

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
            }
        }

        private void uc307_QuesManage_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
