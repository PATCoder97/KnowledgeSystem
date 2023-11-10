using BusinessLayer;
using DataAccessLayer;
using DevExpress.Charts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
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
    public partial class f402_UserRole : DevExpress.XtraEditors.XtraForm
    {
        public f402_UserRole()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo _eventInfo = EventFormInfo.View;
        public int _idRole = -1;

        BindingSource _sourceAllUser = new BindingSource();
        BindingSource _sourceChooseUser = new BindingSource();

        List<dm_User> lsAllUsers = new List<dm_User>();
        List<dm_User> lsChooseUsers = new List<dm_User>();

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
        }

        private void LockControl()
        {
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            switch (_eventInfo)
            {
                case EventFormInfo.View:
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
                case EventFormInfo.Update:
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
            }
        }

        private void f402_UserRole_Load(object sender, EventArgs e)
        {
            gvAllUser.ReadOnlyGridView();
            gvChooseUser.ReadOnlyGridView();

            gcAllUser.DataSource = _sourceAllUser;
            gcChooseUser.DataSource = _sourceChooseUser;

            lsAllUsers = dm_UserBUS.Instance.GetList();
            _sourceAllUser.DataSource = lsAllUsers;
            _sourceChooseUser.DataSource = lsChooseUsers;

            switch (_eventInfo)
            {
                case EventFormInfo.View:
                    var lsUserRoles = dm_UserRoleBUS.Instance.GetListByRole(_idRole).Select(r => r.IdUser).ToList();
                    lsChooseUsers.AddRange(lsAllUsers.Where(a => lsUserRoles.Exists(b => b == a.Id)));
                    lsAllUsers.RemoveAll(a => lsUserRoles.Exists(b => b == a.Id));

                    gcAllUser.RefreshDataSource();
                    gcChooseUser.RefreshDataSource();
                    break;
                case EventFormInfo.Update:
                    break;
            }

            LockControl();
        }

        private void gvAllUser_DoubleClick(object sender, EventArgs e)
        {
            if (_eventInfo != EventFormInfo.Update) return;

            GridView view = gvAllUser;
            dm_User _user = view.GetRow(view.FocusedRowHandle) as dm_User;

            lsAllUsers.Remove(_user);
            view.RefreshData();
            lsChooseUsers.Add(_user);
            gvChooseUser.RefreshData();
        }

        private void gvChooseUser_DoubleClick(object sender, EventArgs e)
        {
            if (_eventInfo != EventFormInfo.Update) return;

            GridView view = gvChooseUser;
            dm_User _user = view.GetRow(view.FocusedRowHandle) as dm_User;

            lsChooseUsers.Remove(_user);
            view.RefreshData();
            lsAllUsers.Add(_user);
            gvAllUser.RefreshData();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<dm_UserRole> lsUserRolesAdd = lsChooseUsers.Select(r => new dm_UserRole { IdRole = _idRole, IdUser = r.Id }).ToList();
            var result1 = dm_UserRoleBUS.Instance.RemoveRangeByRole(_idRole);
            var result2 = dm_UserRoleBUS.Instance.AddRange(lsUserRolesAdd);

            if (result1 && result2)
            {
                Close();
            }
        }
    }
}