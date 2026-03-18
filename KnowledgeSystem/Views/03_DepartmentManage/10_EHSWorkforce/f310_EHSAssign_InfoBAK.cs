using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

            // --- UnitEHSOrg: lấy tất cả record thuộc bộ phận của user ---
            var unitEHSOrgData = dt310_UnitEHSOrgBUS.Instance.GetListByDeptId(deptId);
            var unitEHSOrgUpdate = (from data in unitEHSOrgData
                                    join role in roles on data.RoleId equals role.Id
                                    select new UpdateLeaveUserData
                                    {
                                        UnitEHSOrgData = data,
                                        RoleData = role,
                                        Desc = $"{data.DeptId}：{role.DisplayName}"
                                    }).ToList();

            sourceUnitEHSOrg.DataSource = unitEHSOrgUpdate;
            gcUnitEHSOrg.DataSource = sourceUnitEHSOrg;
            ConfigureGridEdit(gvUnitEHSOrg, "UserId");
            gvUnitEHSOrg.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvUnitEHSOrg.BestFitColumns();

            // --- EHSFunction: lấy tất cả record thuộc bộ phận ---
            var ehsFunctionData = dt310_EHSFunctionBUS.Instance.GetListByDeptId(deptId);
            var ehsFunctionUpdate = (from data in ehsFunctionData
                                     join func in funcs on data.FunctionId equals func.Id
                                     select new UpdateLeaveUserData
                                     {
                                         EHSFunctionData = data,
                                         FuncData = func,
                                         Desc = $"{data.DeptId}：{func.DisplayName}"
                                     }).ToList();

            sourceEHSFunction.DataSource = ehsFunctionUpdate;
            gcEHSFunction.DataSource = sourceEHSFunction;
            ConfigureGridEdit(gvEHSFunction, "UserId");
            gvEHSFunction.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvEHSFunction.BestFitColumns();

            // --- Area5SResponsible: lấy tất cả record thuộc bộ phận ---
            var area5SData = dt310_Area5SResponsibleBUS.Instance.GetListByDeptId(deptId);
            var area5SUpdate = new List<UpdateLeaveUserData>();
            foreach (var data in area5SData)
            {
                var area = areas.FirstOrDefault(a => a.Id == data.AreaId);
                // Tạo 1 row cho EmployeeId
                area5SUpdate.Add(new UpdateLeaveUserData
                {
                    Area5SResponsibleData = data,
                    AreaData = area,
                    Desc = $"{data.DeptId}：{area?.DisplayName}",
                    FieldName = "EmployeeId",
                    ColName = "責任人員"
                });
                // Tạo 1 row cho AgentId
                area5SUpdate.Add(new UpdateLeaveUserData
                {
                    Area5SResponsibleData = data,
                    AreaData = area,
                    Desc = $"{data.DeptId}：{area?.DisplayName}",
                    FieldName = "AgentId",
                    ColName = "代理人"
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
            // Đóng editor để lưu giá trị đang sửa
            gvUnitEHSOrg.CloseEditor();
            gvEHSFunction.CloseEditor();
            gvArea5SResponsible.CloseEditor();
            this.ActiveControl = null;

            var unitEHSOrgList   = sourceUnitEHSOrg.DataSource as List<UpdateLeaveUserData>;
            var ehsFunctionList  = sourceEHSFunction.DataSource as List<UpdateLeaveUserData>;
            var area5SList       = sourceArea5SResponsible.DataSource as List<UpdateLeaveUserData>;

            // Lọc: chỉ lấy những dòng đã chọn người mới (UserId != null)
            var unitChanged  = unitEHSOrgList?.Where(r => !string.IsNullOrWhiteSpace(r.UserId)).ToList()  ?? new List<UpdateLeaveUserData>();
            var funcChanged  = ehsFunctionList?.Where(r => !string.IsNullOrWhiteSpace(r.UserId)).ToList() ?? new List<UpdateLeaveUserData>();
            var areaChanged  = area5SList?.Where(r => !string.IsNullOrWhiteSpace(r.UserId)).ToList()     ?? new List<UpdateLeaveUserData>();

            // Phải có ít nhất 1 thay đổi
            if (!unitChanged.Any() && !funcChanged.Any() && !areaChanged.Any())
            {
                XtraMessageBox.Show("請至少選擇一項人員異動！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy thông tin DeptId để xác định group ký
            var loginUser = dm_UserBUS.Instance.GetItemById(TPConfigs.LoginUser.Id);
            string deptId = loginUser?.IdDepartment ?? "";
            string psmDeptId = deptId.Length >= 2 ? deptId.Substring(0, 2) : deptId;

            int GetGroupId(string name) => dm_GroupBUS.Instance.GetItemByName(name)?.Id ?? -1;

            int level2GroupId = GetGroupId($"二級{deptId}");
            int psmGroupId    = GetGroupId($"PSM專人{psmDeptId}");
            int level1GroupId = GetGroupId($"一級{deptId}");

            bool hasArea5S = areaChanged.Any();

            // Tạo DataJson
            var jsonData = new
            {
                UnitEHSOrg       = unitChanged,
                EHSFunction      = funcChanged,
                Area5SResponsible = areaChanged
            };
            string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            // Tạo header
            var updateRecord = new dt310_UpdateLeaveUser
            {
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

            // Tạo steps có điều kiện
            // Không có Area5S: 二級(0) → PSM(1)
            // Có Area5S:       二級(0) → PSM(1) → 一級(2)
            var steps = hasArea5S
                ? new[] { (level2GroupId, 0), (psmGroupId, 1), (level1GroupId, 2) }
                : new[] { (level2GroupId, 0), (psmGroupId, 1) };

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
