using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
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
    public partial class uc301_SafetyCertMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc301_SafetyCertMain()
        {
            InitializeComponent();
            InitializeIcon();
            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        List<BaseDisplay> lsBasesDisplay = new List<BaseDisplay>();
        BindingSource sourceBases = new BindingSource();

        private class BaseDisplay : dt301_Base
        {
            public string UserName { get; set; }
            public string JobName { get; set; }
            public string CourseName { get; set; }
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();
            var lsBases = dt301_BaseBUS.Instance.GetList();
            var lsUser = dm_UserBUS.Instance.GetList();
            var lsJobs = dm_JobTitleBUS.Instance.GetList();
            var lsCourses = dt301_CourseBUS.Instance.GetList();

            lsBasesDisplay = (from data in lsBases
                              join urs in lsUser on data.IdUser equals urs.Id
                              join job in lsJobs on data.IdJobTitle equals job.Id
                              join course in lsCourses on data.IdCourse equals course.Id
                              select new BaseDisplay()
                              {
                                  Id = data.Id,
                                  IdDept = data.IdDept,
                                  IdUser = data.IdUser,
                                  IdJobTitle = data.IdJobTitle,
                                  IdCourse = data.IdCourse,
                                  DateReceipt = data.DateReceipt,
                                  ExpDate = data.ExpDate,
                                  ValidLicense = data.ValidLicense,
                                  BackupLicense = data.BackupLicense,
                                  InvalidLicense = data.InvalidLicense,
                                  Describe = data.Describe,
                                  UserName = $"{data.IdUser} {urs.DisplayName}",
                                  JobName = $"{job.DisplayName}",
                                  CourseName = $"{course.DisplayName}",
                              }).ToList();

            sourceBases.DataSource = lsBasesDisplay;
            helper.LoadViewInfo();

            gvData.BestFitColumns();
        }

        private void uc301_SafetyCertMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.CustomSummaryCalculate += GvData_CustomSummaryCalculate;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        double countValid = 0;
        double countBackup = 0;
        double countInvalid = 0;
        double countTotal = 0;
        private void GvData_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.IsTotalSummary)
            {
                GridSummaryItem item = e.Item as GridSummaryItem;

                switch (e.SummaryProcess)
                {
                    case CustomSummaryProcess.Start:
                        switch (item.FieldName)
                        {
                            case "ValidLicense":
                                countValid = 0;
                                break;
                            case "BackupLicense":
                                countBackup = 0;
                                break;
                            case "InvalidLicense":
                                countInvalid = 0;
                                break;
                        }
                        break;
                    case CustomSummaryProcess.Calculate:
                        switch (item.FieldName)
                        {
                            case "ValidLicense":
                                if ((bool)view.GetRowCellValue(e.RowHandle, "ValidLicense")) countValid++;
                                break;
                            case "BackupLicense":
                                if ((bool)view.GetRowCellValue(e.RowHandle, "BackupLicense")) countBackup++;
                                break;
                            case "InvalidLicense":
                                if ((bool)view.GetRowCellValue(e.RowHandle, "InvalidLicense")) countInvalid++;
                                break;
                        }
                        break;
                    case CustomSummaryProcess.Finalize:
                        switch (item.FieldName)
                        {
                            case "ValidLicense":
                                e.TotalValue = countValid;
                                break;
                            case "BackupLicense":
                                e.TotalValue = countBackup;
                                break;
                            case "InvalidLicense":
                                e.TotalValue = countInvalid;
                                break;
                            case "Describe":
                                e.TotalValue = countValid + countBackup + countInvalid;
                                break;
                        }
                        break;
                }
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f301_CertInfo fInfo = new f301_CertInfo();
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo._formName = "證照";
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
            dt301_Base _base = view.GetRow(view.FocusedRowHandle) as dt301_Base;

            f301_CertInfo fInfo = new f301_CertInfo();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._formName = "證照";
            fInfo._base = _base;
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }


    }
}
