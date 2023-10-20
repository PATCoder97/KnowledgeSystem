using BusinessLayer;
using DataAccessLayer;
using DevExpress.Internal;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using KnowledgeSystem.Configs;
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
using System.Windows.Media;

namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    public partial class f301_CertInfo : DevExpress.XtraEditors.XtraForm
    {
        public f301_CertInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public string _formName = "";
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        private enum CertStatus
        {
            應取證照,
            備援證照,
            無效證照
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            cbbDept.Enabled = false;
            //txbFunction.Enabled = _enable;
            //cbbControl.Enabled = _enable;
            //txbPrioritize.Enabled = _enable;
            //cbbPicture.Enabled = _enable;
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
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{_formName}";

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

        private void f301_CertInfo_Load(object sender, EventArgs e)
        {
            LockControl();

            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments
            {
                Id = r.Id,
                DisplayName = $"{r.Id} {r.DisplayName}"
            }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            var lsUsers = dm_UserBUS.Instance.GetListByDept(idDept2word).Select(r => new dm_User
            {
                Id = r.Id,
                IdDepartment = r.IdDepartment,
                DisplayName = $"{r.DisplayName} {r.DisplayNameVN}"
            }).ToList(); ;
            cbbUser.Properties.DataSource = lsUsers;
            cbbUser.Properties.DisplayMember = "DisplayName";
            cbbUser.Properties.ValueMember = "Id";
            cbbUser.Properties.BestFitWidth = 110;

            cbbCertStatus.Properties.Items.AddRange((CertStatus[])Enum.GetValues(typeof(CertStatus)));
            cbbCertStatus.SelectedIndex = 0;

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    cbbDept.EditValue = idDept2word;
                    break;
                case EventFormInfo.View:
                    break;
                case EventFormInfo.Update:
                    break;
                case EventFormInfo.Delete:
                    break;
                case EventFormInfo.ViewOnly:
                    break;
                default:
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
            var aaa = EnumHelper.GetEnumByDescription<CertStatus>(cbbCertStatus.EditValue?.ToString());
        }
    }
}