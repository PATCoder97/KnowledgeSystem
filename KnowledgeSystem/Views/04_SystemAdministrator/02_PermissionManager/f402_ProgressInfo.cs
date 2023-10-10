using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
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

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class f402_ProgressInfo : DevExpress.XtraEditors.XtraForm
    {
        public f402_ProgressInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo _eventInfo = EventFormInfo.Create;

        List<dm_Departments> lsDepts;

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
                case EventFormInfo.Create:
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
                case EventFormInfo.View:
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
                case EventFormInfo.Update:
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
                case EventFormInfo.Delete:
                    break;
                case EventFormInfo.ViewOnly:
                    break;
                default:
                    break;
            }
        }

        private void f402_ProgressInfo_Load(object sender, EventArgs e)
        {
            lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id} {r.DisplayName}" }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    cbbDept.EditValue = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
                    cbbDept.EditValue = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
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

            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cbbDept.EditValue ="7810";
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _eventInfo = EventFormInfo.Update;
            LockControl();
        }
    }
}