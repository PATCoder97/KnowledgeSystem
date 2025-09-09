using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Html.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_UnitEHSOrg_Info : DevExpress.XtraEditors.XtraForm
    {
        public f310_UnitEHSOrg_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        public string idDeptGetData = TPConfigs.LoginUser.IdDepartment;

        dt310_UnitEHSOrg unitEHSOrg;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            cbbDept.Enabled = _enable;
            cbbUsr.Enabled = _enable;
            cbbRole.Enabled = _enable;
            txbStartDate.Enabled = _enable;
        }

        private void LockControl()
        {
            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();

                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
            }

            // Các thông tin phải điền có thêm dấu * màu đỏ
            foreach (var item in lcImpControls)
            {
                if (item.Control.Enabled)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }

        private void f310_UnitEHSOrg_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcDept, lcUser, lcRole, lcStartDate };
            lcImpControls = new List<LayoutControlItem>() { lcDept, lcUser, lcRole, lcStartDate };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            var usrs = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0).ToList();
            cbbUsr.Properties.DataSource = usrs;
            cbbUsr.Properties.DisplayMember = "DisplayName";
            cbbUsr.Properties.ValueMember = "Id";

            var depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true).ToList();
            cbbDept.Properties.DataSource = depts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            var roles = dt310_RoleBUS.Instance.GetList();
            cbbRole.Properties.DataSource = roles;
            cbbRole.Properties.DisplayMember = "DisplayName";
            cbbRole.Properties.ValueMember = "Id";

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    unitEHSOrg = new dt310_UnitEHSOrg();

                    break;
                case EventFormInfo.View:

                    unitEHSOrg = dt310_UnitEHSOrgBUS.Instance.GetItemById(idBase);
                    //oldMaterial = dt309_MaterialsBUS.Instance.GetItemById(idBase);

                    cbbDept.EditValue = unitEHSOrg.DeptId;
                    cbbUsr.EditValue = unitEHSOrg.EmployeeId;
                    cbbRole.EditValue = unitEHSOrg.RoleId;
                    txbStartDate.EditValue = unitEHSOrg.StartDate;
                    //txbLocation.EditValue = material.Location;
                    //cbbUsr.EditValue = material.IdManager;
                    //cbbTypeUse.EditValue = material.TypeUse;
                    //tokenMachine.EditValue = string.Join(",", dt309_MachineMaterialsBUS.Instance.GetListByIdMaterial(material.Id).Select(r => r.MachineId));
                    //txbExpDate.EditValue = material.ExpDate;
                    //txbMinQuantity.EditValue = material.MinQuantity;

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
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;

            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (string.IsNullOrEmpty(baseEdit.EditValue?.ToString()))
                    {
                        IsValidate = false;
                        break; // Dừng vòng lặp ngay khi phát hiện lỗi
                    }
                }
            }

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            var deptId = cbbDept.EditValue?.ToString();
            var empId = cbbUsr.EditValue?.ToString();
            var roleId = Convert.ToInt16(cbbRole.EditValue);
            var startDate = txbStartDate.DateTime;

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                unitEHSOrg.DeptId = deptId;
                unitEHSOrg.EmployeeId = empId;
                unitEHSOrg.RoleId = roleId;
                unitEHSOrg.StartDate = startDate;
                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        unitEHSOrg.CreatedAt = DateTime.Now;
                        unitEHSOrg.CreatedBy = TPConfigs.LoginUser.Id;
                        result = dt310_UnitEHSOrgBUS.Instance.Add(unitEHSOrg);

                        break;
                    case EventFormInfo.Update:

                        result = dt310_UnitEHSOrgBUS.Instance.AddOrUpdate(unitEHSOrg);

                        break;
                    case EventFormInfo.Delete:

                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}\r\n{cbbRole.Text}：{cbbUsr.Text}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt310_UnitEHSOrgBUS.Instance.RemoveById(unitEHSOrg.Id, TPConfigs.LoginUser.Id);

                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
        }
    }
}