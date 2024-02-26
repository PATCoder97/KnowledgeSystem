using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using KnowledgeSystem.Helpers;
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

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class uc402_SignManage : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc402_SignManage()
        {
            InitializeComponent();
            InitializeIcon();
            helper = new RefreshHelper(gvData, "Id");
        }

        BindingSource sourceSigns = new BindingSource();

        private void InitializeIcon()
        {
            btnCreate.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }


        private void LoadSign()
        {
            helper.SaveViewInfo();

            List<dm_Sign> signs = dm_SignBUS.Instance.GetList();

            //string idDept2Word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
            //List<dm_User> lsUsers = new List<dm_User>();
            //if (TPConfigs.IdParentControl == AppPermission.SysAdmin || TPConfigs.IdParentControl == AppPermission.Mod)
            //{
            //    lsUsers = dm_UserBUS.Instance.GetList();
            //}
            //else if (TPConfigs.IdParentControl == AppPermission.SafetyCertMain || TPConfigs.IdParentControl == AppPermission.WorkManagementMain)
            //{
            //    lsUsers = dm_UserBUS.Instance.GetListByDept(idDept2Word);
            //}

            //List<dm_Departments> lsDepts = dm_DeptBUS.Instance.GetList();
            //List<dm_Role> lsRoles = dm_RoleBUS.Instance.GetList();
            //List<dm_JobTitle> lsJobTitles = dm_JobTitleBUS.Instance.GetList();

            //var lsUserManage = (from data in lsUsers
            //                    join depts in lsDepts on data.IdDepartment equals depts.Id
            //                    join job in lsJobTitles on data.JobCode equals job.Id into dtg
            //                    from g in dtg.DefaultIfEmpty()
            //                    select new dmUserM()
            //                    {
            //                        Id = data.Id,
            //                        DisplayName = $"{data.DisplayName}{(!string.IsNullOrEmpty(data.DisplayNameVN) ? $"\r\n{data.DisplayNameVN}" : "")}",
            //                        DisplayNameVN = data.DisplayNameVN,
            //                        IdDepartment = data.IdDepartment,
            //                        DateCreate = data.DateCreate,
            //                        SecondaryPassword = data.SecondaryPassword,
            //                        DeptName = $"{data.IdDepartment}\r\n{depts.DisplayName}",
            //                        DOB = data.DOB,
            //                        CitizenID = data.CitizenID,
            //                        Nationality = data.Nationality,
            //                        PCName = data.PCName,
            //                        IPAddress = data.IPAddress,
            //                        JobCode = data.JobCode,
            //                        JobName = g != null ? g.DisplayName : "",
            //                        Addr = data.Addr,
            //                        PhoneNum1 = data.PhoneNum1,
            //                        PhoneNum2 = data.PhoneNum2,
            //                        Sex = data.Sex,
            //                        SexName = data.Sex == null ? "" : data.Sex.Value ? "男" : "女",
            //                        Status = data.Status,
            //                        StatusName = data.Status == null ? "" : TPConfigs.lsUserStatus[data.Status.Value],
            //                    }).ToList();

            sourceSigns.DataSource = signs;

            helper.LoadViewInfo();
            gvData.BestFitColumns();
            gcData.RefreshDataSource();
        }

        private void uc402_SignManage_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceSigns;

            LoadSign();
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f402_SignInfo fInfo = new f402_SignInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "用戶";
            fInfo.ShowDialog();

            LoadSign();
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            dm_Sign signSelect = view.GetRow(view.FocusedRowHandle) as dm_Sign;

            f402_SignInfo fInfo = new f402_SignInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "用戶";
            fInfo.signInfo = signSelect;
            fInfo.ShowDialog();

            LoadSign();
        }
    }
}
