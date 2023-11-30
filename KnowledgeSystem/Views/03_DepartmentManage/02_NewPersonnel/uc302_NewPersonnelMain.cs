using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
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
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dm_User> lsUser = new List<dm_User>();
        List<dm_User> lsAllUser;
        List<dm_Departments> lsDept;
        List<dm_JobTitle> lsJobs;
        List<dt301_Course> lsCourses;
        List<dt301_Base> lsData51;

        List<dt302_ReportInfo> reportsInfo;

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

            var lsBases = dt302_NewPersonBaseBUS.Instance.GetList();
            lsUser = dm_UserBUS.Instance.GetList();
            lsJobs = dm_JobTitleBUS.Instance.GetList();
            //lsDept = dm_DeptBUS.Instance.GetList();
            reportsInfo = dt302_ReportInfoBUS.Instance.GetList();

            var lsBasesDisplay = (from data in lsBases
                                  join urs in lsUser on data.IdUser equals urs.Id
                                  join supvr in lsUser on data.Supervisor equals supvr.Id
                                  join job in lsJobs on urs.JobCode equals job.Id
                                  where urs.IdDepartment.StartsWith(idDept2word)
                                  select new
                                  {
                                      Id = data.Id,
                                      IdDept = urs.IdDepartment,
                                      IdUser = data.IdUser,
                                      IdJobTitle = urs.JobCode,
                                      EnterDate = urs.DateCreate,
                                      Describe = data.Describe,
                                      UserName = $"{urs.DisplayName} {urs.DisplayNameVN}",
                                      JobName = $"{job.Id} {job.DisplayName}",
                                      Supervisor = $"{supvr.DisplayName} {supvr.DisplayNameVN}",
                                  }).ToList();

            sourceBases.DataSource = lsBasesDisplay;
            helper.LoadViewInfo();

            gvData.BestFitColumns();
        }

        private void uc302_NewPersonnelMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvReport.ReadOnlyGridView();

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

        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColId));
            e.IsEmpty = !reportsInfo.Any(r => r.IdBase == idBase);
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColId));

            //var lsDeviceChild = (from data in lsDevicesSpareParts
            //                     where data.IdSparePart == sparePart.IdSparePart
            //                     join device in lsDevices on data.IdDevice equals device.IdDevice
            //                     select device).ToList();

            int index = 1;
            e.ChildList = reportsInfo.Where(r => r.IdBase == idBase).Select(r => new
            {
                Index = index++,
                r.Id,
                r.Content,
                r.ExpectedDate,
                r.UploadDate,
                r.Attachment
            }).ToList();
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "報告進度";
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));
            //if (_base == null) return;

            f302_NewUserInfo fInfo = new f302_NewUserInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "證照";
            fInfo.idBase302 = idBase;
            fInfo.ShowDialog();

            LoadData();
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }
    }
}
