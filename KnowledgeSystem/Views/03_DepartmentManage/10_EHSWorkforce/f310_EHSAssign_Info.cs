using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce.f310_UpdateLeaveUser_Info;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_EHSAssign_Info : DevExpress.XtraEditors.XtraForm
    {
        public f310_EHSAssign_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        BindingSource sourceUnitEHSOrg = new BindingSource();
        BindingSource sourceEHSFunction = new BindingSource();
        BindingSource sourceArea5SResponsible = new BindingSource();

        List<dm_User> users;

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnClose.ImageOptions.SvgImage = TPSvgimages.Close;
        }

        private void ConfigureGridEdit(DevExpress.XtraGrid.Views.Grid.GridView view, string editableField)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in view.Columns)
            {
                col.OptionsColumn.AllowEdit = col.FieldName.Equals(editableField, StringComparison.OrdinalIgnoreCase);
            }
        }

        private void f310_EHSAssign_Info_Load(object sender, EventArgs e)
        {
            Text = "安衛環人員異動申請";

            users = dm_UserBUS.Instance.GetList();

            // Danh sách nhân viên đang active để chọn
            var usrSource = users.Where(r => r.Status == 0).Select(r => new
            {
                DisplayName = $"{r.IdDepartment}/{r.DisplayName}",
                Id = r.Id
            }).ToList();

            // Gán ComboBox Repository cho từng grid
            itemCbbUser01.DataSource = usrSource;
            itemCbbUser01.DisplayMember = "DisplayName";
            itemCbbUser01.ValueMember = "Id";
            itemCbbUser01.NullText = "【請選擇接任人】";

            itemCbbUser02.DataSource = usrSource;
            itemCbbUser02.DisplayMember = "DisplayName";
            itemCbbUser02.ValueMember = "Id";
            itemCbbUser02.NullText = "【請選擇接任人】";

            itemCbbUser03.DataSource = usrSource;
            itemCbbUser03.DisplayMember = "DisplayName";
            itemCbbUser03.ValueMember = "Id";
            itemCbbUser03.NullText = "【請選擇接任人】";

            // Lấy DeptId user hiện tại để lọc dữ liệu theo bộ phận
            var loginUser = users.FirstOrDefault(r => r.Id == TPConfigs.LoginUser.Id);
            string deptId = loginUser?.IdDepartment ?? "";

            var roles = dt310_RoleBUS.Instance.GetList();
            var funcs = dt310_FunctionBUS.Instance.GetList();
            var areas = dt310_Area5SBUS.Instance.GetList();

            // --- UnitEHSOrg ---
            var unitEHSOrgData = dt310_UnitEHSOrgBUS.Instance.GetListByDeptId(deptId);
            var unitEHSOrgUpdate = (from data in unitEHSOrgData
                                    join role in roles on data.RoleId equals role.Id
                                    select new UpdateLeaveUserData
                                    {
                                        UnitEHSOrgData = data,
                                        RoleData = role,
                                        Desc = $"{data.DeptId}：{role.DisplayName}",
                                        UserId = data.EmployeeId   // hiển thị người cũ sẵn
                                    }).ToList();

            sourceUnitEHSOrg.DataSource = unitEHSOrgUpdate;
            gcUnitEHSOrg.DataSource = sourceUnitEHSOrg;
            ConfigureGridEdit(gvUnitEHSOrg, "UserId");
            gvUnitEHSOrg.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvUnitEHSOrg.BestFitColumns();

            // --- EHSFunction ---
            var ehsFunctionData = dt310_EHSFunctionBUS.Instance.GetListByDeptId(deptId);
            var ehsFunctionUpdate = (from data in ehsFunctionData
                                     join func in funcs on data.FunctionId equals func.Id
                                     select new UpdateLeaveUserData
                                     {
                                         EHSFunctionData = data,
                                         FuncData = func,
                                         Desc = $"{data.DeptId}：{func.DisplayName}",
                                         UserId = data.EmployeeId   // hiển thị người cũ sẵn
                                     }).ToList();

            sourceEHSFunction.DataSource = ehsFunctionUpdate;
            gcEHSFunction.DataSource = sourceEHSFunction;
            ConfigureGridEdit(gvEHSFunction, "UserId");
            gvEHSFunction.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvEHSFunction.BestFitColumns();

            // --- Area5SResponsible ---
            var area5SData = dt310_Area5SResponsibleBUS.Instance.GetListByDeptId(deptId);
            var area5SUpdate = new List<UpdateLeaveUserData>();
            foreach (var data in area5SData)
            {
                var area = areas.FirstOrDefault(a => a.Id == data.AreaId);
                area5SUpdate.Add(new UpdateLeaveUserData
                {
                    Area5SResponsibleData = data,
                    AreaData = area,
                    Desc = $"{data.DeptId}：{area?.DisplayName}",
                    FieldName = "EmployeeId",
                    ColName = "責任人員",
                    UserId = data.EmployeeId   // hiển thị người cũ sẵn
                });
                area5SUpdate.Add(new UpdateLeaveUserData
                {
                    Area5SResponsibleData = data,
                    AreaData = area,
                    Desc = $"{data.DeptId}：{area?.DisplayName}",
                    FieldName = "AgentId",
                    ColName = "代理人",
                    UserId = data.AgentId     // hiển thị người cũ sẵn
                });
            }

            sourceArea5SResponsible.DataSource = area5SUpdate;
            gcArea5SResponsible.DataSource = sourceArea5SResponsible;
            ConfigureGridEdit(gvArea5SResponsible, "UserId");
            gvArea5SResponsible.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvArea5SResponsible.BestFitColumns();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvUnitEHSOrg.CloseEditor();
            gvEHSFunction.CloseEditor();
            gvArea5SResponsible.CloseEditor();
            this.ActiveControl = null;

            var unitEHSOrgList  = sourceUnitEHSOrg.DataSource as List<UpdateLeaveUserData>;
            var ehsFunctionList = sourceEHSFunction.DataSource as List<UpdateLeaveUserData>;
            var area5SList      = sourceArea5SResponsible.DataSource as List<UpdateLeaveUserData>;

            // Chỉ lấy những dòng đã đổi sang người khác (khác với người cũ)
            var unitChanged = unitEHSOrgList?.Where(r =>
                !string.IsNullOrWhiteSpace(r.UserId) &&
                r.UserId != r.UnitEHSOrgData?.EmployeeId
            ).ToList() ?? new List<UpdateLeaveUserData>();

            var funcChanged = ehsFunctionList?.Where(r =>
                !string.IsNullOrWhiteSpace(r.UserId) &&
                r.UserId != r.EHSFunctionData?.EmployeeId
            ).ToList() ?? new List<UpdateLeaveUserData>();

            var areaChanged = area5SList?.Where(r =>
                !string.IsNullOrWhiteSpace(r.UserId) &&
                r.UserId != (r.FieldName == "EmployeeId"
                    ? r.Area5SResponsibleData?.EmployeeId
                    : r.Area5SResponsibleData?.AgentId)
            ).ToList() ?? new List<UpdateLeaveUserData>();

            if (!unitChanged.Any() && !funcChanged.Any() && !areaChanged.Any())
            {
                XtraMessageBox.Show("請至少選擇一項人員異動！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var loginUser = users.FirstOrDefault(r => r.Id == TPConfigs.LoginUser.Id);
            string deptId = loginUser?.IdDepartment ?? "";
            string psmDeptId = deptId.Length >= 2 ? deptId.Substring(0, 2) : deptId;

            int GetGroupId(string name) => dm_GroupBUS.Instance.GetItemByName(name)?.Id ?? -1;

            int level2GroupId = GetGroupId($"二級{deptId}");
            int psmGroupId    = GetGroupId($"PSM專人{psmDeptId}");
            int level1GroupId = GetGroupId($"一級{psmDeptId}");

            var jsonData = new
            {
                UnitEHSOrg        = unitChanged,
                EHSFunction       = funcChanged,
                Area5SResponsible = areaChanged
            };
            string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            var updateRecord = new dt310_UpdateLeaveUser
            {
                // Bảng yêu cầu IdUserLeave không được null, nên dùng mã người tạo đơn cho loại EHSAssign.
                IdUserLeave    = TPConfigs.LoginUser.Id,
                DataType       = "EHSAssign",
                DisplayName    = "人員異動申請",
                IsProcess      = true,
                IsCancel       = false,
                IdGroupProcess = level2GroupId,
                DataJson       = json,
                CreateBy       = TPConfigs.LoginUser.Id,
                CreateAt       = DateTime.Now
            };
            int idUpdateData = dt310_UpdateLeaveUserBUS.Instance.Add(updateRecord);
            if (idUpdateData <= 0)
            {
                XtraMessageBox.Show("建立人員異動申請失敗，請確認資料後再試！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 所有人員異動申請都必須經過：二級主管(0) → PSM專人(1) → 一級主管(2)
            var steps = new[] { (level2GroupId, 0), (psmGroupId, 1), (level1GroupId, 2) };

            foreach (var (groupId, index) in steps)
            {
                dt310_UpdateLeaveUser_detailBUS.Instance.Add(new dt310_UpdateLeaveUser_detail
                {
                    IdUpdateData = idUpdateData,
                    IdGroup      = groupId,
                    IndexStep    = index
                });
            }

            XtraMessageBox.Show("已成功送交二級主管審核！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }
    }
}
