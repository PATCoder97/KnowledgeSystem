using BusinessLayer;
using DataAccessLayer;
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

        List<dm_User> lsUser = new List<dm_User>();
        List<dm_User> lsAllUser;
        List<dm_Departments> lsDept;
        List<dm_JobTitle> lsJobs;
        List<dt301_Course> lsCourses;
        List<dt301_Base> lsData51;

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

            var lsBasesDisplay = (from data in lsBases
                                  join urs in lsUser on data.IdUser equals urs.Id
                                  join supvr in lsUser on data.Supervisor equals supvr.Id
                                  join job in lsJobs on urs.JobCode equals job.Id
                                  select new
                                  {
                                      Id = data.Id,
                                      IdDept = urs.IdDepartment,
                                      IdUser = data.IdUser,
                                      IdJobTitle = urs.JobCode,
                                      EnterDate = urs.DateCreate,
                                      Describe = data.Describe,
                                      UserName = $"{urs.DisplayName} {urs.DisplayNameVN}",
                                      JobName = job.Id + job.DisplayName,
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
