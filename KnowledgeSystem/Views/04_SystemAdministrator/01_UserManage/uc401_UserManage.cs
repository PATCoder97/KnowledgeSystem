using BusinessLayer;
using DataAccessLayer;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._04_SystemAdministrator._01_UserManage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_UserManager
{
    public partial class uc401_UserManage : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc401_UserManage()
        {
            InitializeComponent();
            InitializeIcon();
        }

        #region parameters

        bool IsSysAdmin = false;

        Font fontDFKaiSB12 = new Font("DFKai-SB", 12.0f, FontStyle.Regular);
        BindingSource sourceUsers = new BindingSource();

        private class UserInfos : dm_User
        {
            public string RoleName { get; set; }
            public string DeptName { get; set; }
        }

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnCreate.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnUploadList.ImageOptions.SvgImage = TPSvgimages.UploadFile;
        }

        private void InitializeControl()
        {
            gColPCName.Visible = IsSysAdmin;
            gColIP.Visible = IsSysAdmin;
        }

        private void LoadUser()
        {
          var lsUsers = dm_UserBUS.Instance.GetList();
            var lsDepts = dm_DeptBUS.Instance.GetList();
            var lsRoles = dm_RoleBUS.Instance.GetList();

            helper.SaveViewInfo();

            var lsUserManage = (from data in lsUsers
                                join depts in lsDepts on data.IdDepartment equals depts.Id
                                join roles in lsRoles on data.IdRole equals roles.Id into dtg
                                from p in dtg.DefaultIfEmpty()
                                select new UserInfos()
                                {
                                    Id = data.Id,
                                    DisplayName = $"{data.DisplayName}{(!string.IsNullOrEmpty(data.DisplayNameVN) ? $"\n{data.DisplayNameVN}" : "")}",
                                    DisplayNameVN = data.DisplayNameVN,
                                    IdRole = data.IdRole,
                                    IdDepartment = data.IdDepartment,
                                    DateCreate = data.DateCreate,
                                    SecondaryPassword = data.SecondaryPassword,
                                    RoleName = p != null ? p.DisplayName : "",
                                    DeptName = $"{data.IdDepartment}\n{depts.DisplayName}",
                                    DOB = data.DOB,
                                    CitizenID = data.CitizenID,
                                    Nationality = data.Nationality,
                                    PCName = data.PCName,
                                    IPAddress = data.IPAddress
                                }).ToList();

            sourceUsers.DataSource = lsUserManage;

            gvData.BestFitColumns();
            helper.LoadViewInfo();
        }

        #endregion

        private void f401_UserManager_Load(object sender, EventArgs e)
        {
            IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
            InitializeControl();

            gvData.OptionsEditForm.ShowOnDoubleClick = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsEditForm.ShowOnF2Key = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsBehavior.EditingMode = GridEditingMode.EditFormInplace;

            gcData.DataSource = sourceUsers;
            helper = new RefreshHelper(gvData, "Id");
            LoadUser();
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f401_UserInfo fInfo = new f401_UserInfo();
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo._formName = "用戶";
            fInfo.ShowDialog();
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadUser();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            dm_User _userSelect = view.GetRow(view.FocusedRowHandle) as dm_User;

            f401_UserInfo fInfo = new f401_UserInfo();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._formName = "用戶";
            fInfo._user = _userSelect;
            fInfo.ShowDialog();
        }
    }
}