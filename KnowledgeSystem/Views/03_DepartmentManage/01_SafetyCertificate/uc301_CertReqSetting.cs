using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
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
    public partial class uc301_CertReqSetting : DevExpress.XtraEditors.XtraUserControl
    {
        public uc301_CertReqSetting()
        {
            InitializeComponent();
            InitializeIcon();
            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        List<CertReqSetM> lsCertReqSetDisplay = new List<CertReqSetM>();
        BindingSource sourceCertReqSet = new BindingSource();

        private class CertReqSetM : dt301_CertReqSetting
        {
            public string JobName { get; set; }
            public string CourseName { get; set; }
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();
            var lsJobTitles = dm_JobTitleBUS.Instance.GetList();
            var lsCourses = dt301_CourseBUS.Instance.GetList();
            var lsCertReqs = dt301_CertReqSetBUS.Instance.GetList();

            lsCertReqSetDisplay = (from data in lsCertReqs
                                   join job in lsJobTitles on data.IdJobTitle equals job.Id
                                   join courses in lsCourses on data.IdCourse equals courses.Id
                                   select new CertReqSetM
                                   {
                                       Id = data.Id,
                                       IdDept = data.IdDept,
                                       IdJobTitle = data.IdJobTitle,
                                       IdCourse = data.IdCourse,
                                       NewHeadcount = data.NewHeadcount,
                                       ActualHeadcount = data.ActualHeadcount,
                                       ReqQuantity = data.ReqQuantity,
                                       JobName = $"{data.IdJobTitle} {job.DisplayName}",
                                       CourseName = $"{data.IdCourse} {courses.DisplayName}"
                                   }).ToList();

            sourceCertReqSet.DataSource = lsCertReqSetDisplay;
            helper.LoadViewInfo();

            gvData.BestFitColumns();
        }

        private void uc301_CertReqSetting_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            LoadData();

            gcData.DataSource = sourceCertReqSet;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f301_CertReqSetInfo fInfo = new f301_CertReqSetInfo();
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo._formName = "證照要求";
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            dt301_CertReqSetting _certReqSet = view.GetRow(view.FocusedRowHandle) as dt301_CertReqSetting;

            f301_CertReqSetInfo fInfo = new f301_CertReqSetInfo();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._formName = "證照要求";
            fInfo._certReq = _certReqSet;
            fInfo.ShowDialog();

            LoadData();
        }
    }
}
