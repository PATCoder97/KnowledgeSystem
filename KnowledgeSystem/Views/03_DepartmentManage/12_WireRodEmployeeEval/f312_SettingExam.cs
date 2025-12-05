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

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class f312_SettingExam : DevExpress.XtraEditors.XtraForm
    {
        public f312_SettingExam()
        {
            InitializeComponent();
            InitializeIcon();
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(txbCountEmp.Text) || string.IsNullOrEmpty(txbTime.Text) || string.IsNullOrEmpty(txbPassScore.Text) || string.IsNullOrEmpty(txbTotalQues.Text))
            {
                return;
            }

            dt312_Setting settingData = new dt312_Setting()
            {
                id = 1,
                CountEmp = Convert.ToInt16(txbCountEmp.EditValue),
                PassingScore = Convert.ToInt16(txbPassScore.EditValue),
                TestDuration = Convert.ToInt16(txbTime.EditValue),
                QuesCount = Convert.ToInt16(txbTotalQues.EditValue),
                CycleCreate = 1
            };

            dt312_SettingBUS.Instance.AddOrUpdate(settingData);

            Close();
        }

        private void f312_SettingExam_Load(object sender, EventArgs e)
        {
            var dataInfo = dt312_SettingBUS.Instance.GetItemById(1);

            if (dataInfo != null)
            {
                txbTime.EditValue = dataInfo.TestDuration;
                txbPassScore.EditValue = dataInfo.PassingScore;
                txbTotalQues.EditValue = dataInfo.QuesCount;
                txbCountEmp.EditValue = dataInfo.CountEmp;
            }
        }
    }
}