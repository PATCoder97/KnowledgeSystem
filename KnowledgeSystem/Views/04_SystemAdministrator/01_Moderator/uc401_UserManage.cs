using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data.Browsing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraSpreadsheet.Model;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml.Table.PivotTable;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.Util;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class uc401_UserManage : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc401_UserManage()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");
            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        #region parameters

        bool IsSysAdmin = false;

        BindingSource sourceUsers = new BindingSource();
        string sheetName = "DataUser";
        List<dm_User> users = new List<dm_User>();

        DXMenuItem itemEditRole;
        DXMenuItem itemEditSign;
        DXMenuItem itemEditGroup;

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeMenuItems()
        {
            itemEditRole = CreateMenuItem("設定用色", ItemEditRole_Click, TPSvgimages.Num1);
            itemEditSign = CreateMenuItem("設定簽名", ItemEditSign_Click, TPSvgimages.Num2);
            itemEditGroup = CreateMenuItem("設定群組", ItemEditGroup_Click, TPSvgimages.Num3);
        }

        private void ItemEditGroup_Click(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();
            var form = new f402_UserMapping() { mapData = "group", idUsr = idUser };
            form.ShowDialog();
        }

        private void ItemEditSign_Click(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();
            var form = new f402_UserMapping() { mapData = "sign", idUsr = idUser };
            form.ShowDialog();

            //string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();

            //f402_UserSigns fSetting = new f402_UserSigns();
            //fSetting.eventInfo = EventFormInfo.View;
            //fSetting.idUsr = idUser;
            //fSetting.ShowDialog();
        }

        private void ItemEditRole_Click(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();
            var form = new f402_UserMapping() { mapData = "role", idUsr = idUser };
            form.ShowDialog();

            //string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();

            //f402_UserRoles fSetting = new f402_UserRoles();
            //fSetting.eventInfo = EventFormInfo.View;
            //fSetting.idUsr = idUser;
            //fSetting.ShowDialog();
        }

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnCreate.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeControl()
        {
            gColPCName.Visible = IsSysAdmin;
            gColIP.Visible = IsSysAdmin;
            gColLastUpdate.Visible = IsSysAdmin;
        }

        private void CreateRuleGV()
        {
            // Quy tắc định dạng khi TransactionType = 'C'
            var ruleResignPlan = new GridFormatRule
            {
                ApplyToRow = true,
                Column = gColStatus,
                Name = "RuleResignPlan",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[StatusName] = '預報離職'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical, }
                }
            };
            gvData.FormatRules.Add(ruleResignPlan);
        }

        private void LoadUser()
        {
            helper.SaveViewInfo();

            string idDept2Word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

            if (TPConfigs.IdParentControl == AppPermission.SysAdmin || TPConfigs.IdParentControl == AppPermission.Mod)
            {
                users = dm_UserBUS.Instance.GetList();
            }
            else if (TPConfigs.IdParentControl == AppPermission.SafetyCertMain || TPConfigs.IdParentControl == AppPermission.WorkManagementMain)
            {
                users = dm_UserBUS.Instance.GetListByDept(idDept2Word);
            }

            List<dm_Departments> lsDepts = dm_DeptBUS.Instance.GetList();
            List<dm_Role> lsRoles = dm_RoleBUS.Instance.GetList();
            List<dm_JobTitle> lsJobTitles = dm_JobTitleBUS.Instance.GetList();

            var lsUserManage = (from data in users
                                join depts in lsDepts on data.IdDepartment equals depts.Id
                                join job in lsJobTitles on data.JobCode equals job.Id into dtg
                                from g in dtg.DefaultIfEmpty()
                                join actualJob in lsJobTitles on data.ActualJobCode equals actualJob.Id into atg
                                from a in atg.DefaultIfEmpty()
                                let displayName = $"{data.DisplayName}{(!string.IsNullOrEmpty(data.DisplayNameVN) ? $"\r\n{data.DisplayNameVN}" : "")}"
                                let deptName = $"{data.IdDepartment}\r\n{depts.DisplayName}"
                                let sexName = data.Sex == null ? "" : data.Sex.Value ? "男" : "女"
                                let statusName = (data.ResignPlan != null && data.Status == 0) ? "預報離職" : (data.Status == null ? "" : TPConfigs.lsUserStatus[data.Status.Value])
                                select new
                                {
                                    Data = data,
                                    Depts = depts,
                                    DisplayName = displayName,
                                    DeptName = deptName,
                                    JobName = g != null ? g.DisplayName : "",
                                    ActualJobName = a != null ? a.DisplayName : "",
                                    SexName = sexName,
                                    StatusName = statusName,
                                }).ToList();

            sourceUsers.DataSource = lsUserManage;

            helper.LoadViewInfo();
            gvData.BestFitColumns();
            gcData.RefreshDataSource();
        }

        #endregion

        private void f401_UserManager_Load(object sender, EventArgs e)
        {
            IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
            InitializeControl();

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceUsers;

            LoadUser();
            CreateRuleGV();

            string filterString = "[StatusName] In ('在職','留職停薪','預報離職')";
            gvData.Columns["StatusName"].FilterInfo = new ColumnFilterInfo(filterString);
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f401_UserInfo fInfo = new f401_UserInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "用戶";
            fInfo.ShowDialog();

            LoadUser();
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadUser();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();
            dm_User _userSelect = users.FirstOrDefault(r => r.Id == idUser);

            f401_UserInfo fInfo = new f401_UserInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "用戶";
            fInfo.userInfo = _userSelect;
            fInfo.ShowDialog();

            LoadUser();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"{Text} - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && IsSysAdmin)
            {
                e.Menu.Items.Add(itemEditRole);
                e.Menu.Items.Add(itemEditSign);
                e.Menu.Items.Add(itemEditGroup);
            }
        }
    }
}