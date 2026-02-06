using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_EHSFunc_Info : DevExpress.XtraEditors.XtraForm
    {
        public f310_EHSFunc_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        public string idDeptGetData = TPConfigs.LoginUser.IdDepartment;

        dt310_EHSFunction EHSFunc;

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
            cbbFunc.Enabled = _enable;
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

        private void f310_EHSFunc_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcDept, lcUser, lcFunc, lcStartDate };
            lcImpControls = new List<LayoutControlItem>() { lcDept, lcUser, lcFunc, lcStartDate };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            var groupUser = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var groupEHSs = dm_GroupBUS.Instance.GetListContainName("安衛環");

            var ehsAdminGroup = groupEHSs.FirstOrDefault(g => g.DisplayName.Trim() == "安衛環7");
            bool isEHSAdmin = ehsAdminGroup != null && groupUser.Any(gu => gu.IdGroup == ehsAdminGroup.Id);

            var deptByGroups = groupEHSs
                .Where(g => groupUser.Any(gu => gu.IdGroup == g.Id))
                .Select(g => g.DisplayName.Replace("安衛環", "").Trim())
                .ToList();

            var usrs = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0 && (isEHSAdmin || deptByGroups.Contains(r.IdDepartment))).ToList();

            cbbUsr.Properties.DataSource = usrs;
            cbbUsr.Properties.DisplayMember = "DisplayName";
            cbbUsr.Properties.ValueMember = "Id";

            var depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true && (isEHSAdmin || deptByGroups.Contains(r.Id))).ToList();

            cbbDept.Properties.DataSource = depts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            var funcs = dt310_FunctionBUS.Instance.GetList();
            cbbFunc.Properties.DataSource = funcs;
            cbbFunc.Properties.DisplayMember = "DisplayName";
            cbbFunc.Properties.ValueMember = "Id";

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    EHSFunc = new dt310_EHSFunction();

                    break;
                case EventFormInfo.View:

                    EHSFunc = dt310_EHSFunctionBUS.Instance.GetItemById(idBase);

                    cbbDept.EditValue = EHSFunc.DeptId;
                    cbbUsr.EditValue = EHSFunc.EmployeeId;
                    cbbFunc.EditValue = EHSFunc.FunctionId;
                    txbStartDate.EditValue = EHSFunc.StartDate;

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
            var funcId = Convert.ToInt16(cbbFunc.EditValue);
            var startDate = txbStartDate.DateTime;

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                EHSFunc.DeptId = deptId;
                EHSFunc.EmployeeId = empId;
                EHSFunc.FunctionId = funcId;
                EHSFunc.StartDate = startDate;
                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        EHSFunc.CreatedAt = DateTime.Now;
                        EHSFunc.CreatedBy = TPConfigs.LoginUser.Id;
                        result = dt310_EHSFunctionBUS.Instance.Add(EHSFunc);

                        break;
                    case EventFormInfo.Update:

                        result = dt310_EHSFunctionBUS.Instance.AddOrUpdate(EHSFunc);

                        break;
                    case EventFormInfo.Delete:

                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}\r\n{cbbFunc.Text}：{cbbUsr.Text}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt310_EHSFunctionBUS.Instance.RemoveById(EHSFunc.Id, TPConfigs.LoginUser.Id);

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