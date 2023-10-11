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
            InitializeMenuItems();
        }

        #region parameters

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
                var lsFuncRole = db.dm_FunctionRole.Where(r => r.IdRole == roleSelect.Id).ToList();

                lsFunctions = (from data in db.dm_Function.OrderBy(r => r.Prioritize).ToList()
                               join funcs in lsFuncRole on data.Id equals funcs.IdFunction into dtg
                               from p in dtg.DefaultIfEmpty()
                               select new dm_FunctionM
                               {
                                   Id = data.Id,
                                   IdParent = data.IdParent,
                                   DisplayName = data.DisplayName,
                                   ControlName = data.ControlName,
                                   Prioritize = data.Prioritize,
                                   Status = p != null,
                                   Images = data.Images,
                               }).ToList();
                sourceFunc.DataSource = lsFunctions;

                lsRoles = db.dm_Role.ToList();
                sourceRole.DataSource = lsRoles;
                gcRoles.DataSource = sourceRole;

                gvRoles.BestFitColumns();
            }

            treeFunctions.RefreshDataSource();
            gcRoles.RefreshDataSource();
        }

        private void ClearForm()
        {
            roleSelect = new dm_Role();
            txbNameRole.Text = "";

            LoadData();
        }

        #endregion

        private void uc402_Functions_Load(object sender, EventArgs e)
        {
            LoadData();

            sourceFunc.DataSource = lsFunctions;
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

        DXMenuItem[] menuItems;
        void InitializeMenuItems()
        {
            DXMenuItem itemEdit = new DXMenuItem("Edit", RoleEdit_Click);
            DXMenuItem itemDelete = new DXMenuItem("Delete", RoleDelete_Click);
            DXMenuItem itemView = new DXMenuItem("View", RoleView_Click);
            DXMenuItem itemNew = new DXMenuItem("New", RoleNew_Click);
            menuItems = new DXMenuItem[] { itemView, itemEdit, itemDelete, itemNew };
        }

        private void RoleEdit_Click(object sender, System.EventArgs e)
        {
            gvRoles.ShowEditForm();
        }

        private void RoleDelete_Click(object sender, System.EventArgs e)
        {
            dm_Role roleDelete = gvRoles.GetRow(gvRoles.FocusedRowHandle) as dm_Role;

            var dlg = XtraMessageBox.Show($"Bạn có chắc chắn muốn xoá quyền hạn {roleDelete.DisplayName} ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dlg != DialogResult.Yes) return;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                int idRoleDel = roleDelete.Id;

                db.dm_Role.Remove(db.dm_Role.First(r => r.Id == idRoleDel));
                db.dm_FunctionRole.RemoveRange(db.dm_FunctionRole.Where(r => r.IdRole == idRoleDel));
                db.SaveChanges();
            }

            LoadData();
        }

        private void RoleView_Click(object sender, System.EventArgs e)
        {
            roleSelect = gvRoles.GetRow(gvRoles.FocusedRowHandle) as dm_Role;
            txbNameRole.Text = roleSelect.DisplayName;

            LoadData();
        }

        private void RoleNew_Click(object sender, System.EventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                if (db.dm_Role.Any(r => r.DisplayName == "NEW"))
                {
                    XtraMessageBox.Show($"Đã tồn tại quyền hạn NEW !", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dlg = XtraMessageBox.Show($"Bạn có chắc chắn muốn thêm quyền hạn mới ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dlg != DialogResult.Yes) return;

                dm_Role role = new dm_Role()
                {
                    DisplayName = "NEW"
                };

                db.dm_Role.Add(role);
                db.SaveChanges();
            }

            LoadData();
        }

        private void gvRoles_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow && e.HitInfo.RowHandle >= 0)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                foreach (DXMenuItem item in menuItems)
                    e.Menu.Items.Add(item);
            }
        }

        private void txbNameRole_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ClearForm();
        }

        private void btnUpdateRole_Click(object sender, EventArgs e)
        {
            if (roleSelect.Id == 0) return;

            List<dm_FunctionM> lsDataSourch = sourceFunc.DataSource as List<dm_FunctionM>;
            var lsFunctionUpdate = lsDataSourch.Where(r => r.Status == true).ToList();

            // Xóa các functionRole trước đó, sau đó thêm lại
            using (var db = new DBDocumentManagementSystemEntities())
            {
                db.dm_FunctionRole.RemoveRange(db.dm_FunctionRole.Where(r => r.IdRole == roleSelect.Id));

                foreach (var item in lsFunctionUpdate)
                {
                    db.dm_FunctionRole.Add(new dm_FunctionRole() { IdRole = roleSelect.Id, IdFunction = item.Id });
                }
                db.SaveChanges();
            }

            XtraMessageBox.Show("Cập nhật quyền hạn thành công !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearForm();
            LoadData();
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
            fInfo._formName = "權限";
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo.ShowDialog();

        }

        private void gvRoles_DoubleClick(object sender, EventArgs e)
        {
            int forcusRow = gvRoles.FocusedRowHandle;
            if (forcusRow < 0) return;

            dm_Role dataRow = gvRoles.GetRow(forcusRow) as dm_Role;

            f402_RoleInfo fInfo = new f402_RoleInfo();
            fInfo._formName = "權限";
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._role = dataRow;
            fInfo.ShowDialog();

            LoadData();
        }
    }
}
