using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
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

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class uc402_Functions : DevExpress.XtraEditors.XtraUserControl
    {
        public uc402_Functions()
        {
            InitializeComponent();
            InitializeMenuItems();
        }

        #region parameters

        BindingSource sourceFunc = new BindingSource();
        List<Function> lsFunctions = new List<Function>();

        BindingSource sourceRole = new BindingSource();
        List<Role> lsRoles = new List<Role>();

        #endregion

        #region methods

        private void LoadData()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsFunctions = db.Functions.ToList();
                lsRoles = db.Roles.ToList();
            }

            treeFunctions.RefreshDataSource();
            gcRoles.RefreshDataSource();
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

            sourceRole.DataSource = lsRoles;
            gcRoles.DataSource = sourceRole;
            gvRoles.BestFitColumns();

            treeFunctions.CollapseAll();
            treeFunctions.BestFitColumns();

            gvRoles.ReadOnlyGridView();
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
            DXMenuItem itemEdit = new DXMenuItem("修改", RoleEdit_Click);
            DXMenuItem itemDelete = new DXMenuItem("刪除", RoleDelete_Click);
            DXMenuItem itemView = new DXMenuItem("看權限", RoleView_Click);
            menuItems = new DXMenuItem[] { itemView, itemEdit, itemDelete };
        }

        private void RoleEdit_Click(object sender, System.EventArgs e)
        {
            // gvRoles.ShowEditor();
        }

        private void RoleDelete_Click(object sender, System.EventArgs e)
        {
            // gvRoles.DeleteRow(gvRoles.FocusedRowHandle);
        }

        private void RoleView_Click(object sender, System.EventArgs e)
        {
            // gvRoles.DeleteRow(gvRoles.FocusedRowHandle);
        }

        private void FunctionEdit_Click(object sender, System.EventArgs e)
        {
            // gvRoles.DeleteRow(gvRoles.FocusedRowHandle);
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

        private void treeFunctions_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            DXMenuItem itemEdit = new DXMenuItem("修改", FunctionEdit_Click);
            //itemEdit.ImageOptions.SvgImage = CommonSvgImages.Get(CommonSvgImages.Sorting.Ascending);
            if (e.HitInfo.InRow && e.HitInfo.Node.Id >= 0)
                e.Menu.Items.Add(itemEdit);
        }
    }
}
