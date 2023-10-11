using BusinessLayer;
using DataAccessLayer;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class uc402_Functions : DevExpress.XtraEditors.XtraUserControl
    {
        public uc402_Functions()
        {
            InitializeComponent();
            InitializeIcon();
        }

        #region parameters

        dm_FunctionBUS _dm_FunctionBUS = new dm_FunctionBUS();

        BindingSource sourceFunc = new BindingSource();
        List<dm_FunctionM> lsFunctions = new List<dm_FunctionM>();

        BindingSource sourceRole = new BindingSource();
        List<dm_Role> lsRoles = new List<dm_Role>();
        dm_Role roleSelect = new dm_Role();

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnNewRole.ImageOptions.SvgImage = TPSvgimages.Add;
            btnNewFunc.ImageOptions.SvgImage = TPSvgimages.Add2;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadData()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsFuncRole = _dm_FunctionBUS.GetList().OrderBy(r => r.Prioritize).ToList();

                //lsFunctions = (from data in db.dm_Function.OrderBy(r => r.Prioritize).ToList()
                //               join funcs in lsFuncRole on data.Id equals funcs.IdFunction into dtg
                //               from p in dtg.DefaultIfEmpty()
                //               select new dm_FunctionM
                //               {
                //                   Id = data.Id,
                //                   IdParent = data.IdParent,
                //                   DisplayName = data.DisplayName,
                //                   ControlName = data.ControlName,
                //                   Prioritize = data.Prioritize,
                //                   Status = p != null,
                //                   Images = data.Images,
                //               }).ToList();

                sourceFunc.DataSource = lsFuncRole;

                lsRoles = dm_RoleBUS.Instance.GetList();
                sourceRole.DataSource = lsRoles;
                gcRoles.DataSource = sourceRole;
                gvRoles.BestFitColumns();
            }

            treeFunctions.RefreshDataSource();
            gcRoles.RefreshDataSource();
        }

        #endregion

        private void uc402_Functions_Load(object sender, EventArgs e)
        {
            LoadData();

            //sourceFunc.DataSource = lsFunctions;
            treeFunctions.DataSource = sourceFunc;
            treeFunctions.KeyFieldName = "Id";
            treeFunctions.ParentFieldName = "IdParent";
            treeFunctions.CheckBoxFieldName = "Status";
            treeFunctions.BestFitColumns();

            gvRoles.ReadOnlyGridView();
            treeFunctions.ReadOnlyTreelist();
        }

        private void treeFunctions_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            if (treeFunctions.DataSource == null)
                return;
            if (e.Node.Checked)
            {
                e.Appearance.ForeColor = Color.FromArgb(153, 0, 0);
            }
        }

        private void gvRoles_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            dm_Role roles = e.Row as dm_Role;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                db.dm_Role.AddOrUpdate(roles);
                db.SaveChanges();
            }

            XtraMessageBox.Show("Thao tác sửa thành công!", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadData();
        }

        private void btnNewRole_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f402_RoleInfo fInfo = new f402_RoleInfo();
            fInfo._formName = "角色";
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo.ShowDialog();

        }

        private void gvRoles_DoubleClick(object sender, EventArgs e)
        {
            int forcusRow = gvRoles.FocusedRowHandle;
            if (forcusRow < 0) return;

            dm_Role dataRow = gvRoles.GetRow(forcusRow) as dm_Role;

            f402_RoleInfo fInfo = new f402_RoleInfo();
            fInfo._formName = "角色";
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._role = dataRow;
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnNewFunc_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f402_FuncInfo fInfo = new f402_FuncInfo();
            fInfo._formName = "功能";
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo.ShowDialog();
        }

        private void treeFunctions_DoubleClick(object sender, EventArgs e)
        {
            TreeList treeList = treeFunctions;
            TreeListNode focusedNode = treeList.FocusedNode;
            TreeListHitInfo hitInfo = treeList.CalcHitInfo(treeList.PointToClient(MousePosition));

            if (focusedNode != null && focusedNode.Nodes != null && hitInfo.HitInfoType == HitInfoType.Cell)
            {
                dm_FunctionM rowData = treeList.GetRow(focusedNode.Id) as dm_FunctionM;

                f402_FuncInfo _info = new f402_FuncInfo();
                _info._formName = Text.ToLower();
                _info._eventInfo = EventFormInfo.View;
                _info._function = rowData;
                _info.ShowDialog();

                LoadData();
            }
        }
    }
}
