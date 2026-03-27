using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_EquipmentInfo_Info : XtraForm
    {
        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;

        private dt310_EquipmentInfo equipmentInfo;
        private List<dm_GroupUser> userGroups = new List<dm_GroupUser>();
        private List<string> editableDeptIds = new List<string>();
        private bool isEHSAdmin = false;
        private bool canManage = false;

        private List<LayoutControlItem> lcControls;
        private List<LayoutControlItem> lcImpControls;

        public f310_EquipmentInfo_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool enable = true)
        {
            txbCode.Enabled = enable;
            txbNameVN.Enabled = enable;
            txbNameTW.Enabled = enable;
            cbbDept.Enabled = enable && (isEHSAdmin || editableDeptIds.Count != 1);
            cbbManager.Enabled = enable;
            txbNote.Enabled = enable;
        }

        private void ResolvePermission()
        {
            userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);

            List<dm_Group> ehsGroups = dm_GroupBUS.Instance.GetListContainName("安衛環");
            dm_Group adminGroup = ehsGroups.FirstOrDefault(r => string.Equals(r.DisplayName?.Trim(), "安衛環7", StringComparison.Ordinal));
            isEHSAdmin = adminGroup != null && userGroups.Any(r => r.IdGroup == adminGroup.Id);

            editableDeptIds = ehsGroups
                .Where(r => userGroups.Any(gu => gu.IdGroup == r.Id))
                .Select(r => (r.DisplayName ?? string.Empty).Replace("安衛環", string.Empty).Trim())
                .Where(r => !string.IsNullOrWhiteSpace(r) && r != "7" && r.All(char.IsDigit))
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            canManage = isEHSAdmin || editableDeptIds.Count > 0;
        }

        private bool CanManageDept(string deptId)
        {
            if (isEHSAdmin)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(deptId) && editableDeptIds.Contains(deptId);
        }

        private bool CanManageCurrentRecord()
        {
            if (eventInfo == EventFormInfo.Create)
            {
                return canManage;
            }

            return equipmentInfo != null && CanManageDept(equipmentInfo.DeptId);
        }

        private void LockControl()
        {
            bool canManageCurrentRecord = CanManageCurrentRecord();

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";
                    btnConfirm.Visibility = BarItemVisibility.Always;
                    btnEdit.Visibility = BarItemVisibility.Never;
                    btnDelete.Visibility = BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";
                    btnConfirm.Visibility = BarItemVisibility.Always;
                    btnEdit.Visibility = BarItemVisibility.Never;
                    btnDelete.Visibility = BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";
                    btnConfirm.Visibility = BarItemVisibility.Always;
                    btnEdit.Visibility = BarItemVisibility.Never;
                    btnDelete.Visibility = BarItemVisibility.Never;
                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";
                    btnConfirm.Visibility = BarItemVisibility.Never;
                    btnEdit.Visibility = BarItemVisibility.Always;
                    btnDelete.Visibility = BarItemVisibility.Always;
                    EnabledController(false);
                    break;
                default:
                    break;
            }

            if (!canManageCurrentRecord)
            {
                btnConfirm.Visibility = BarItemVisibility.Never;
                btnEdit.Visibility = BarItemVisibility.Never;
                btnDelete.Visibility = BarItemVisibility.Never;
                EnabledController(false);
            }

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
            }

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

        private void LoadLookupData(string currentDeptId = null)
        {
            var depts = dm_DeptBUS.Instance.GetAllChildren(0)
                .Where(r => r.IsGroup != true)
                .OrderBy(r => r.Id)
                .Select(r => new
                {
                    r.Id,
                    DisplayName = $"{r.Id} {r.DisplayName}"
                })
                .ToList();

            if (!isEHSAdmin && editableDeptIds.Count > 0)
            {
                HashSet<string> visibleDeptIds = new HashSet<string>(editableDeptIds);
                if (!string.IsNullOrWhiteSpace(currentDeptId))
                {
                    visibleDeptIds.Add(currentDeptId);
                }

                depts = depts
                    .Where(r => visibleDeptIds.Contains(r.Id))
                    .ToList();
            }

            cbbDept.Properties.DataSource = depts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";
            cbbDept.Properties.NullText = "";

            var usrs = dm_UserBUS.Instance.GetList()
                .Where(r => r.Status == 0)
                .OrderBy(r => r.IdDepartment)
                .ThenBy(r => r.DisplayName)
                .Select(r => new
                {
                    r.Id,
                    r.IdDepartment,
                    DisplayName = $"LG{r.IdDepartment}/{r.DisplayName} {r.DisplayNameVN}"
                })
                .ToList();

            if (!isEHSAdmin && editableDeptIds.Count > 0)
            {
                HashSet<string> visibleDeptIds = new HashSet<string>(editableDeptIds);
                if (!string.IsNullOrWhiteSpace(currentDeptId))
                {
                    visibleDeptIds.Add(currentDeptId);
                }

                usrs = usrs
                    .Where(r => visibleDeptIds.Contains(r.IdDepartment))
                    .ToList();
            }

            cbbManager.Properties.DataSource = usrs;
            cbbManager.Properties.DisplayMember = "DisplayName";
            cbbManager.Properties.ValueMember = "Id";
            cbbManager.Properties.NullText = "";
        }

        private void f310_EquipmentInfo_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem> { lcCode, lcNameVN, lcNameTW, lcDept, lcManager, lcNote };
            lcImpControls = new List<LayoutControlItem> { lcCode, lcNameVN, lcNameTW, lcDept, lcManager };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    equipmentInfo = new dt310_EquipmentInfo();
                    break;
                case EventFormInfo.View:
                    equipmentInfo = dt310_EquipmentInfoBUS.Instance.GetItemById(idBase);
                    if (equipmentInfo == null)
                    {
                        MsgTP.MsgError("查無資料！");
                        Close();
                        return;
                    }
                    break;
                default:
                    equipmentInfo = dt310_EquipmentInfoBUS.Instance.GetItemById(idBase) ?? new dt310_EquipmentInfo();
                    break;
            }

            ResolvePermission();
            LoadLookupData(equipmentInfo?.DeptId);

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    if (isEHSAdmin)
                    {
                        cbbDept.EditValue = TPConfigs.LoginUser.IdDepartment;
                    }
                    else
                    {
                        cbbDept.EditValue = editableDeptIds.FirstOrDefault()
                            ?? TPConfigs.LoginUser.IdDepartment;
                    }
                    cbbManager.EditValue = TPConfigs.LoginUser.Id;
                    break;
                case EventFormInfo.View:
                    txbCode.EditValue = equipmentInfo.Code;
                    txbNameVN.EditValue = equipmentInfo.DisplayNameVN;
                    txbNameTW.EditValue = equipmentInfo.DisplayNameTW;
                    cbbDept.EditValue = equipmentInfo.DeptId;
                    cbbManager.EditValue = equipmentInfo.ManagerId;
                    txbNote.EditValue = equipmentInfo.Note;
                    break;
                default:
                    if (equipmentInfo.Id > 0)
                    {
                        txbCode.EditValue = equipmentInfo.Code;
                        txbNameVN.EditValue = equipmentInfo.DisplayNameVN;
                        txbNameTW.EditValue = equipmentInfo.DisplayNameTW;
                        cbbDept.EditValue = equipmentInfo.DeptId;
                        cbbManager.EditValue = equipmentInfo.ManagerId;
                        txbNote.EditValue = equipmentInfo.Note;
                    }
                    break;
            }

            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!CanManageCurrentRecord())
            {
                MsgTP.MsgNoPermission();
                return;
            }

            bool isValidate = true;
            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (string.IsNullOrWhiteSpace(baseEdit.EditValue?.ToString()))
                    {
                        isValidate = false;
                        break;
                    }
                }
            }

            if (!isValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            string code = txbCode.EditValue?.ToString().Trim();
            string nameVN = txbNameVN.EditValue?.ToString().Trim();
            string nameTW = txbNameTW.EditValue?.ToString().Trim();
            string deptId = cbbDept.EditValue?.ToString();
            string managerId = cbbManager.EditValue?.ToString();
            string note = txbNote.EditValue?.ToString().Trim();

            if (!isEHSAdmin && !CanManageDept(deptId))
            {
                MsgTP.MsgError("只可管理所屬安衛環部門的設備資料！");
                return;
            }

            dt310_EquipmentInfo existingItem = dt310_EquipmentInfoBUS.Instance.GetItemByCode(code);
            if (existingItem != null && existingItem.Id != equipmentInfo.Id)
            {
                MsgTP.MsgError("設備編號已存在！");
                return;
            }

            if (eventInfo == EventFormInfo.Delete)
            {
                var dialogResult = XtraMessageBox.Show(
                    $"您確認要刪除{formName}\r\n{equipmentInfo.Code}：{equipmentInfo.DisplayNameTW}",
                    TPConfigs.SoftNameTW,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (dialogResult != DialogResult.Yes)
                {
                    return;
                }
            }

            bool result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                equipmentInfo.Code = code;
                equipmentInfo.DisplayNameVN = nameVN;
                equipmentInfo.DisplayNameTW = nameTW;
                equipmentInfo.DeptId = deptId;
                equipmentInfo.ManagerId = managerId;
                equipmentInfo.Note = note;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        equipmentInfo.CreatedAt = DateTime.Now;
                        equipmentInfo.CreatedBy = TPConfigs.LoginUser.Id;
                        result = dt310_EquipmentInfoBUS.Instance.Add(equipmentInfo);
                        break;
                    case EventFormInfo.Update:
                        result = dt310_EquipmentInfoBUS.Instance.AddOrUpdate(equipmentInfo);
                        break;
                    case EventFormInfo.Delete:
                        result = dt310_EquipmentInfoBUS.Instance.RemoveById(equipmentInfo.Id, TPConfigs.LoginUser.Id);
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

        private void btnEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!CanManageCurrentRecord())
            {
                MsgTP.MsgNoPermission();
                return;
            }

            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!CanManageCurrentRecord())
            {
                MsgTP.MsgNoPermission();
                return;
            }

            MsgTP.MsgConfirmDel();
            eventInfo = EventFormInfo.Delete;
            LockControl();
        }
    }
}
