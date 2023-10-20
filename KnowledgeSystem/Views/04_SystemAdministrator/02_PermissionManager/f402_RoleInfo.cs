using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class f402_RoleInfo : DevExpress.XtraEditors.XtraForm
    {
        public f402_RoleInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public string _formName;
        public dm_Role _role = null;

        dm_FunctionRoleBUS _sysFunctionRoleBUS = new dm_FunctionRoleBUS();

        BindingSource _sourceFunc = new BindingSource();

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnEditRoleUser.ImageOptions.SvgImage = TPSvgimages.AddUserGroup;
        }

        private void EnabledController(bool _enable = true)
        {
            txbRole.Enabled = _enable;
            txbDescribe.Enabled = _enable;

            tlsFunction.OptionsView.CheckBoxStyle = _enable ? DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check : DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Default;
        }

        private void LockControl()
        {
            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEditRoleUser.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEditRoleUser.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEditRoleUser.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{_formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEditRoleUser.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }
        }

        private void f402_RoleInfo_Load(object sender, EventArgs e)
        {
            LockControl();

            var lsFunctions = dm_FunctionBUS.Instance.GetList();
            _sourceFunc.DataSource = lsFunctions;
            tlsFunction.DataSource = _sourceFunc;
            tlsFunction.KeyFieldName = "Id";
            tlsFunction.ParentFieldName = "IdParent";
            tlsFunction.CheckBoxFieldName = "Status";
            tlsFunction.BestFitColumns();

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _role = new dm_Role();
                    break;
                case EventFormInfo.View:
                    txbRole.EditValue = _role.DisplayName;
                    txbDescribe.EditValue = _role.Describe;

                    var lsFuncRoles = _sysFunctionRoleBUS.GetListByRole(_role.Id);
                    var lsFunctionDisplay = (from func in lsFunctions
                                             join funcRole in lsFuncRoles on func.Id equals funcRole.IdFunction into dtg
                                             from p in dtg.DefaultIfEmpty()
                                             select new dm_FunctionM
                                             {
                                                 Id = func.Id,
                                                 IdParent = func.IdParent,
                                                 DisplayName = func.DisplayName,
                                                 ControlName = func.ControlName,
                                                 Prioritize = func.Prioritize,
                                                 Status = p != null,
                                                 Images = func.Images,
                                                 ImageSvg = func.ImageSvg
                                             }).ToList();

                    _sourceFunc.DataSource = lsFunctionDisplay;
                    tlsFunction.RefreshDataSource();
                    break;
                case EventFormInfo.Update:
                    break;
                case EventFormInfo.Delete:
                    break;
                default:
                    break;
            }

            tlsFunction.FocusedNode = tlsFunction.Nodes.AutoFilterNode;
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
                var _oldDisplayName = _role.DisplayName;
                var _oldDescribe = _role.Describe;

                _role.DisplayName = txbRole.EditValue?.ToString();
                _role.Describe = txbDescribe.EditValue?.ToString();
                List<dm_FunctionM> lsDataSourch = _sourceFunc.DataSource as List<dm_FunctionM>;
                var lsFunctionUpdates = lsDataSourch.Where(r => r.Status == true).ToList();

                msg = $"{_role.Id} {_role.DisplayName} {_role.Describe}";
                switch (_eventInfo)
                {
                    case EventFormInfo.Create:
                        result = dm_RoleBUS.Instance.Add(_role);

                        foreach (var m in lsFunctionUpdates)
                        {
                            dm_FunctionRole functionRole = new dm_FunctionRole()
                            {
                                IdFunction = m.Id,
                                IdRole = _role.Id
                            };

                            _sysFunctionRoleBUS.Add(functionRole);
                        }
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        result = true;
                        var resultUpdate = false;
                        if (_oldDisplayName != _role.DisplayName || _oldDescribe != _role.Describe)
                        {
                            resultUpdate = dm_RoleBUS.Instance.AddOrUpdate(_role);
                            result = !resultUpdate ? false : result;
                        }

                        // Xoá các funcRole trước đó
                        resultUpdate = _sysFunctionRoleBUS.RemoveByIdRole(_role.Id);
                        result = !resultUpdate ? false : result;

                        foreach (var m in lsFunctionUpdates)
                        {
                            dm_FunctionRole functionRole = new dm_FunctionRole()
                            {
                                IdFunction = m.Id,
                                IdRole = _role.Id
                            };

                            resultUpdate = _sysFunctionRoleBUS.Add(functionRole);
                            result = !resultUpdate ? false : result;
                        }

                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"Bạn xác nhận muốn xoá quyền hạn: {_role.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        result = dm_RoleBUS.Instance.Remove(_role.Id);
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

        private void btnEditRoleUser_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f402_UserRole fInfo = new f402_UserRole();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._idRole = _role.Id;
            fInfo.ShowDialog();
        }
    }
}