using BusinessLayer;
using DataAccessLayer;
using DevExpress.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class f401_GroupInfo : DevExpress.XtraEditors.XtraForm
    {
        public f401_GroupInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string _formName = string.Empty;
        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public dm_Group _group = null;
        string _oldGroupInfo = "";
        bool IsSysAdmin = false;

        #region parameters

        List<dm_User> lsUserData = new List<dm_User>();
        List<dm_User> lsUserChoose = new List<dm_User>();

        BindingSource sourceData = new BindingSource();
        BindingSource sourceChoose = new BindingSource();

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbName.Enabled = _enable;
            txbPrioritize.Enabled = _enable;
            cbbDept.Enabled = _enable;
            txbDescribe.Enabled = _enable;
        }

        private void LockControl()
        {
            txbName.Enabled = false;
            txbPrioritize.Enabled = false;
            cbbDept.ReadOnly = false;
            txbDescribe.ReadOnly = false;
            lcUserGroup.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Size = new Size(855, 165);

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    lcUserGroup.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    Size = new Size(855, 525);

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
                    lcUserGroup.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    Size = new Size(855, 525);

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

        #endregion

        private void f401_GroupManage_Info_Load(object sender, EventArgs e)
        {
            LockControl();

            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id} {r.DisplayName}" }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            lsUserData = dm_UserBUS.Instance.GetList();

            gvData.ReadOnlyGridView();
            gvChoose.ReadOnlyGridView();

            sourceData.DataSource = lsUserData;
            sourceChoose.DataSource = lsUserChoose;

            gcData.DataSource = sourceData;
            gcChoose.DataSource = sourceChoose;

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _group = new dm_Group();
                    break;
                case EventFormInfo.View:
                    txbName.EditValue = _group.DisplayName;
                    cbbDept.EditValue = _group.IdDept;
                    txbPrioritize.EditValue = _group.Prioritize;
                    txbDescribe.EditValue = _group.Describe;

                    _oldGroupInfo = JsonConvert.SerializeObject(_group);

                    // Load Role
                    var lsUserGroups = dm_GroupUserBUS.Instance.GetListByIdGroup(_group.Id).Select(r => r.IdUser).ToList();
                    lsUserChoose.AddRange(lsUserData.Where(a => lsUserGroups.Exists(b => b == a.Id)));
                    lsUserData.RemoveAll(a => lsUserGroups.Exists(b => b == a.Id));

                    gcData.RefreshDataSource();
                    gcChoose.RefreshDataSource();
                    break;
            }
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string name = txbName.EditValue?.ToString();
            int prioritize = Convert.ToInt16(txbPrioritize.EditValue);

            if (string.IsNullOrEmpty(name))
            {
                XtraMessageBox.Show("請填寫所有信息", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                _group.DisplayName = name;
                _group.IdDept = cbbDept.EditValue?.ToString();
                _group.Prioritize = prioritize;
                _group.Describe = txbDescribe.EditValue?.ToString();

                string _newGroupInfo = JsonConvert.SerializeObject(_group);

                msg = $"{_group.Id} {_group.DisplayName} {_group.Describe}";
                switch (_eventInfo)
                {
                    case EventFormInfo.Create:
                        result = dm_GroupBUS.Instance.Add(_group);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        result = true;
                        var resultUpdate = false;

                        if (_oldGroupInfo != _newGroupInfo)
                        {
                            resultUpdate = dm_GroupBUS.Instance.AddOrUpdate(_group);
                            result = !resultUpdate ? false : result;
                        }

                        var resultDel = dm_GroupUserBUS.Instance.RemoveRangeByIdGroup(_group.Id);

                        var lsUserRolesAdd = lsUserChoose.Select(r => new dm_GroupUser { IdUser = r.Id, IdGroup = _group.Id }).ToList();
                        var resultAdd = dm_GroupUserBUS.Instance.AddRange(lsUserRolesAdd);

                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"Bạn xác nhận muốn xoá quyền hạn: {_group.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        result = dm_GroupBUS.Instance.Remove(_group.Id);
                        dm_GroupUserBUS.Instance.RemoveRangeByIdGroup(_group.Id);
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

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DefaultMsg.MsgConfirmDel();

            _eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            if (_eventInfo != EventFormInfo.Update) return;

            GridView view = gvData;
            dm_User _role = view.GetRow(view.FocusedRowHandle) as dm_User;
            if (_role == null) return;

            lsUserData.Remove(_role);
            view.RefreshData();
            lsUserChoose.Add(_role);
            gvChoose.RefreshData();
        }

        private void gvChoose_DoubleClick(object sender, EventArgs e)
        {
            if (_eventInfo != EventFormInfo.Update) return;

            GridView view = gvData;
            dm_User _role = view.GetRow(view.FocusedRowHandle) as dm_User;
            if (_role == null) return;

            lsUserData.Remove(_role);
            view.RefreshData();
            lsUserData.Add(_role);
            gvData.RefreshData();
        }

        private void cbbDept_AutoSearch(object sender, LookUpEditAutoSearchEventArgs e)
        {
            e.ClearHighlight();
        }
    }
}