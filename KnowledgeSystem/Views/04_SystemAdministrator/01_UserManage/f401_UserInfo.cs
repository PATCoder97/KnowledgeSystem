using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
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

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{_formName}";

                    txbUserId.Enabled = true;
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    lcRole.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    Size = new Size(579, 200);
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{_formName}";

                    _oldUserInfo = JsonConvert.SerializeObject(_user);
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
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
        }

        private void f401_UserInfo_Load(object sender, EventArgs e)
        {
            LockControl();

            var lsDepts = dm_DeptBUS.Instance.GetList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            cbbNationality.Properties.Items.AddRange(new string[] { "VN", "TW", "CN" });

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _user = new dm_User();
                    txbCreate.EditValue = DateTime.Today;
                    break;
                case EventFormInfo.View:
                    txbUserId.EditValue = _user.Id;
                    txbUserNameVN.EditValue = _user.DisplayNameVN;
                    txbUserNameTW.EditValue = _user.DisplayName.Split('\n')[0];
                    cbbDept.EditValue = _user.IdDepartment;
                    txbCreate.EditValue = _user.DateCreate;
                    txbDOB.EditValue = _user.DOB;
                    txbCCCD.EditValue = _user.CitizenID;
                    cbbNationality.EditValue = _user.Nationality;
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
            //var result = false;
            //string msg = "";
            //using (var handle = SplashScreenManager.ShowOverlayForm(this))
            //{
            //    var _oldDisplayName = _role.DisplayName;
            //    var _oldDescribe = _role.Describe;

            //    _role.DisplayName = txbRole.EditValue?.ToString();
            //    _role.Describe = txbDescribe.EditValue?.ToString();
            //    List<dm_FunctionM> lsDataSourch = _sourceFunc.DataSource as List<dm_FunctionM>;
            //    var lsFunctionUpdates = lsDataSourch.Where(r => r.Status == true).ToList();

            //    msg = $"{_role.Id} {_role.DisplayName} {_role.Describe}";
            //    switch (_eventInfo)
            //    {
            //        case EventFormInfo.Create:
            //            result = dm_RoleBUS.Instance.Add(_role);

            //            foreach (var m in lsFunctionUpdates)
            //            {
            //                dm_FunctionRole functionRole = new dm_FunctionRole()
            //                {
            //                    IdFunction = m.Id,
            //                    IdRole = _role.Id
            //                };

            //                _sysFunctionRoleBUS.Add(functionRole);
            //            }
            //            break;
            //        case EventFormInfo.View:
            //            break;
            //        case EventFormInfo.Update:
            //            result = true;
            //            var resultUpdate = false;
            //            if (_oldDisplayName != _role.DisplayName || _oldDescribe != _role.Describe)
            //            {
            //                resultUpdate = dm_RoleBUS.Instance.AddOrUpdate(_role);
            //                result = !resultUpdate ? false : result;
            //            }

            //            // Xoá các funcRole trước đó
            //            resultUpdate = _sysFunctionRoleBUS.RemoveByIdRole(_role.Id);
            //            result = !resultUpdate ? false : result;

            //            foreach (var m in lsFunctionUpdates)
            //            {
            //                dm_FunctionRole functionRole = new dm_FunctionRole()
            //                {
            //                    IdFunction = m.Id,
            //                    IdRole = _role.Id
            //                };

            //                resultUpdate = _sysFunctionRoleBUS.Add(functionRole);
            //                result = !resultUpdate ? false : result;
            //            }

            //            break;
            //        case EventFormInfo.Delete:
            //            var dialogResult = XtraMessageBox.Show($"Bạn xác nhận muốn xoá quyền hạn: {_role.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //            if (dialogResult != DialogResult.Yes) return;

            //            result = dm_RoleBUS.Instance.Remove(_role.Id);
            //            break;
            //        default:
            //            break;
            //    }
            //}

            //if (result)
            //{
            //    //switch (_eventInfo)
            //    //{
            //    //    case EventFormInfo.Update:
            //    //        logger.Info(_eventInfo.ToString(), msg);
            //    //        break;
            //    //    case EventFormInfo.Delete:
            //    //        logger.Warning(_eventInfo.ToString(), msg);
            //    //        break;
            //    //}
            //    Close();
            //}
            //else
            //{
            //    DefaultMsg.MsgErrorDB();
            //}
        }
    }
}