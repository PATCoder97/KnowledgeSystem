using BusinessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._02_NewPersonnel
{
    public partial class uc302_NewPersonnelMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc302_NewPersonnelMain()
        {
            InitializeComponent();
            InitializeIcon();
            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        //List<BaseDisplay> lsBasesDisplay = new List<BaseDisplay>();
        BindingSource sourceBases = new BindingSource();

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnFilter.ImageOptions.SvgImage = TPSvgimages.Filter;

            btnValidCert.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnBackCert.ImageOptions.SvgImage = TPSvgimages.Num2;
            btnInvalidCert.ImageOptions.SvgImage = TPSvgimages.Num3;
            btnWaitCert.ImageOptions.SvgImage = TPSvgimages.Num4;
            btnExpCert.ImageOptions.SvgImage = TPSvgimages.Num5;
            btnClearFilter.ImageOptions.SvgImage = TPSvgimages.Close;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();
            //var lsBases = dt301_BaseBUS.Instance.GetList();
            //lsUser = dm_UserBUS.Instance.GetList();
            //lsJobs = dm_JobTitleBUS.Instance.GetList();
            //lsCourses = dt301_CourseBUS.Instance.GetList();
            //lsDept = dm_DeptBUS.Instance.GetList();

            //lsBasesDisplay = (from data in lsBases
            //                  join urs in lsUser on data.IdUser equals urs.Id
            //                  join job in lsJobs on data.IdJobTitle equals job.Id
            //                  join course in lsCourses on data.IdCourse equals course.Id
            //                  select new BaseDisplay()
            //                  {
            //                      Id = data.Id,
            //                      IdDept = data.IdDept,
            //                      IdUser = data.IdUser,
            //                      IdJobTitle = data.IdJobTitle,
            //                      IdCourse = data.IdCourse,
            //                      DateReceipt = data.DateReceipt,
            //                      ExpDate = data.ExpDate,
            //                      ValidLicense = data.ValidLicense,
            //                      BackupLicense = data.BackupLicense,
            //                      InvalidLicense = data.InvalidLicense,
            //                      Describe = data.Describe,
            //                      UserName = urs.DisplayName,
            //                      JobName = data.IdJobTitle + job.DisplayName,
            //                      CourseName = data.IdCourse + course.DisplayName,
            //                      CertSuspended = data.CertSuspended
            //                  }).ToList();

            //sourceBases.DataSource = lsBasesDisplay;
            helper.LoadViewInfo();

            gvData.BestFitColumns();
        }

        private void uc302_NewPersonnelMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f302_NewUserInfo fInfo = new f302_NewUserInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "大學新進人員";
            fInfo.ShowDialog();

            LoadData();
        }
    }
}
