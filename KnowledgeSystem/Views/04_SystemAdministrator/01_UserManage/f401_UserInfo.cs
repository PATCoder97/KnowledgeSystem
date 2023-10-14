using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_UserManage
{
    public partial class f401_UserInfo : DevExpress.XtraEditors.XtraForm
    {
        public f401_UserInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string _formName = string.Empty;
        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public dm_User _user = null;
        string _oldUserInfo = "";
        bool IsSysAdmin = false;

        List<dm_Role> lsAllRoles;
        List<dm_Role> lsChooseRoles = new List<dm_Role>();

        BindingSource _sourceAllRole = new BindingSource();
        BindingSource _sourceChooseRole = new BindingSource();

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbUserNameVN.Enabled = _enable;
            txbUserNameTW.Enabled = _enable;
            cbbDept.Enabled = _enable;
            txbDOB.Enabled = _enable;
            txbCCCD.Enabled = _enable;
            cbbNationality.Enabled = _enable;
        }

        private void LockControl()
        {
            txbUserId.Enabled = false;
            txbCreate.Enabled = false;
            txbUserId.ReadOnly = false;
            lcRole.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Size = new Size(579, 230);

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{_formName}";

                    txbUserId.Enabled = true;
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{_formName}";

                    txbUserId.Enabled = true;
                    txbUserId.ReadOnly = true;

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    if (IsSysAdmin)
                    {
                        lcRole.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        Size = new Size(579, 530);
                    }

                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{_formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }

            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            int x = screenBounds.Width / 2 - Width / 2;
            int y = screenBounds.Height / 2 - Height / 2;
            Location = new Point(x, y);
        }

        private void f401_UserInfo_Load(object sender, EventArgs e)
        {
            IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);

            LockControl();

            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id,-5}{r.DisplayName}" }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            cbbNationality.Properties.Items.AddRange(new string[] { "VN", "TW", "CN" });

            lsAllRoles = dm_RoleBUS.Instance.GetList();
            _sourceAllRole.DataSource = lsAllRoles;
            gcAllRole.DataSource = _sourceAllRole;

            _sourceChooseRole.DataSource = lsChooseRoles;
            gcChooseRole.DataSource = _sourceChooseRole;

            gvAllRole.ReadOnlyGridView();
            gvChooseRole.ReadOnlyGridView();

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _user = new dm_User();
                    txbCreate.EditValue = DateTime.Today;
                    break;
                case EventFormInfo.View:
                    _user.DisplayName = _user.DisplayName.Split('\n')[0];
                    _user.DateCreate = DateTime.Parse(_user.DateCreate.ToShortDateString());

                    txbUserId.EditValue = _user.Id;
                    txbUserNameVN.EditValue = _user.DisplayNameVN;
                    txbUserNameTW.EditValue = _user.DisplayName.Split('\n')[0];
                    cbbDept.EditValue = _user.IdDepartment;
                    txbCreate.EditValue = _user.DateCreate;
                    txbDOB.EditValue = _user.DOB;
                    txbCCCD.EditValue = _user.CitizenID;
                    cbbNationality.EditValue = _user.Nationality;

                    _oldUserInfo = JsonConvert.SerializeObject(_user);

                    // Load Role
                    var lsUserRoles = dm_UserRoleBUS.Instance.GetListByUID(_user.Id).Select(r => r.IdRole).ToList();
                    lsChooseRoles.AddRange(lsAllRoles.Where(a => lsUserRoles.Exists(b => b == a.Id)));
                    lsAllRoles.RemoveAll(a => lsUserRoles.Exists(b => b == a.Id));

                    gcAllRole.RefreshDataSource();
                    gcChooseRole.RefreshDataSource();
                    break;
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DefaultMsg.MsgConfirmDel();

            _eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                _user.DisplayName = txbUserNameTW.EditValue?.ToString();
                _user.DisplayNameVN = txbUserNameVN.EditValue?.ToString();
                _user.IdDepartment = cbbDept.EditValue?.ToString();
                _user.DateCreate = DateTime.Parse(DateTime.Today.ToShortDateString());
                _user.DOB = txbDOB.DateTime;
                _user.CitizenID = txbCCCD.EditValue?.ToString();
                _user.Nationality = cbbNationality.EditValue?.ToString();

                string _newUserInfo = JsonConvert.SerializeObject(_user);

                msg = $"{_user.Id} {_user.DisplayName} {_user.DisplayNameVN}";
                switch (_eventInfo)
                {
                    case EventFormInfo.Create:
                        result = dm_UserBUS.Instance.Add(_user);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        result = true;
                        var resultUpdate = false;

                        if (_oldUserInfo != _newUserInfo)
                        {
                            resultUpdate = dm_UserBUS.Instance.AddOrUpdate(_user);
                            result = !resultUpdate ? false : result;
                        }

                        if (IsSysAdmin)
                        {
                            var resultDel = dm_UserRoleBUS.Instance.RemoveRangeByUID(_user.Id);

                            var lsUserRolesAdd = lsChooseRoles.Select(r => new dm_UserRole { IdUser = _user.Id, IdRole = r.Id }).ToList();
                            var resultAdd = dm_UserRoleBUS.Instance.AddRange(lsUserRolesAdd);
                        }
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"Bạn xác nhận muốn xoá quyền hạn: {_user.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        result = dm_UserBUS.Instance.Remove(_user.Id);
                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                //switch (_eventInfo)
                //{
                //    case EventFormInfo.Update:
                //        logger.Info(_eventInfo.ToString(), msg);
                //        break;
                //    case EventFormInfo.Delete:
                //        logger.Warning(_eventInfo.ToString(), msg);
                //        break;
                //}
                Close();
            }
            else
            {
                DefaultMsg.MsgErrorDB();
            }
        }

        private void gvAllRole_DoubleClick(object sender, EventArgs e)
        {
            if (_eventInfo != EventFormInfo.Update) return;

            GridView view = gvAllRole;
            dm_Role _role = view.GetRow(view.FocusedRowHandle) as dm_Role;

            lsAllRoles.Remove(_role);
            view.RefreshData();
            lsChooseRoles.Add(_role);
            gvChooseRole.RefreshData();
        }

        private void gvChooseRole_DoubleClick(object sender, EventArgs e)
        {
            if (_eventInfo != EventFormInfo.Update) return;

            GridView view = gvChooseRole;
            dm_Role _role = view.GetRow(view.FocusedRowHandle) as dm_Role;

            lsChooseRoles.Remove(_role);
            view.RefreshData();
            lsAllRoles.Add(_role);
            gvAllRole.RefreshData();
        }

        private void txbUserId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (TPConfigs.DomainComputer != DomainVNFPG.domainVNFPG) return;

            string userNameByDomain = DomainVNFPG.Instance.GetAccountName(txbUserId.Text.ToUpper());
            if (string.IsNullOrEmpty(userNameByDomain)) return;

            string[] displayNameFHS = userNameByDomain.Split('/');
            cbbDept.EditValue = displayNameFHS[0].Replace("LG", string.Empty);
            txbUserNameTW.EditValue = displayNameFHS[1];
        }

        private void cbbDept_AutoSearch(object sender, DevExpress.XtraEditors.Controls.LookUpEditAutoSearchEventArgs e)
        {
            e.ClearHighlight();
        }
    }
}