using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
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
    public partial class f402_UserRoles : DevExpress.XtraEditors.XtraForm
    {
        public f402_UserRoles()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.View;
        public string idUsr = "";

        BindingSource _sourceAllRole = new BindingSource();
        BindingSource _sourceSelectRole = new BindingSource();

        List<dm_Role> roles = new List<dm_Role>();
        List<dm_Role> selectRoles = new List<dm_Role>();

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
        }

        private void LockControl()
        {
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            switch (eventInfo)
            {
                case EventFormInfo.View:
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
                case EventFormInfo.Update:
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
            }
        }

        private void f402_UserRoles_Load(object sender, EventArgs e)
        {
            gvAllRole.ReadOnlyGridView();
            gvSelectRole.ReadOnlyGridView();

            gcAllRole.DataSource = _sourceAllRole;
            gcSelectRole.DataSource = _sourceSelectRole;

            roles = dm_RoleBUS.Instance.GetList();
            _sourceAllRole.DataSource = roles;
            _sourceSelectRole.DataSource = selectRoles;

            switch (eventInfo)
            {
                case EventFormInfo.View:
                    var userRoles = dm_UserRoleBUS.Instance.GetListByUID(idUsr).Select(r => r.IdRole).ToList();
                    selectRoles.AddRange(roles.Where(a => userRoles.Exists(b => b == a.Id)));
                    roles.RemoveAll(a => userRoles.Exists(b => b == a.Id));

                    gcAllRole.RefreshDataSource();
                    gcSelectRole.RefreshDataSource();
                    break;
                case EventFormInfo.Update:
                    break;
            }

            LockControl();
        }

        private void gvAllRole_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (!(info.InRow || info.InRowCell) || eventInfo != EventFormInfo.Update) return;

            dm_Role role = view.GetRow(view.FocusedRowHandle) as dm_Role;

            roles.Remove(role);
            view.RefreshData();
            selectRoles.Add(role);
            gvSelectRole.RefreshData();
        }

        private void gvSelectRole_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (!(info.InRow || info.InRowCell) || eventInfo != EventFormInfo.Update) return;

            dm_Role role = view.GetRow(view.FocusedRowHandle) as dm_Role;

            selectRoles.Remove(role);
            view.RefreshData();
            roles.Add(role);
            gvAllRole.RefreshData();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                List<dm_UserRole> userRolesAdd = selectRoles.Select(r => new dm_UserRole { IdUser = idUsr, IdRole = r.Id }).ToList();
                var result1 = dm_UserRoleBUS.Instance.RemoveRangeByUID(idUsr);
                var result2 = dm_UserRoleBUS.Instance.AddRange(userRolesAdd);

                if (result1 && result2)
                {
                    Close();
                }
            }
        }
    }
}