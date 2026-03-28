using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce.f310_UpdateLeaveUser_Info;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_EHSAssign_Info : DevExpress.XtraEditors.XtraForm
    {
        private const string ActionCreate = "Create";
        private const string ActionUpdate = "Update";
        private const string ActionDelete = "Delete";

        public f310_EHSAssign_Info()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
        }

        BindingSource sourceUnitEHSOrg = new BindingSource();
        BindingSource sourceEHSFunction = new BindingSource();
        BindingSource sourceArea5SResponsible = new BindingSource();

        List<dm_User> users;
        List<dt310_Role> roles;
        List<dt310_Function> funcs;
        List<dt310_Area5S> areas;
        List<string> editableDeptIds = new List<string>();

        bool isEHSAdmin = false;

        DXMenuItem itemAddUnit;
        DXMenuItem itemDeleteUnit;
        DXMenuItem itemAddFunc;
        DXMenuItem itemDeleteFunc;
        DXMenuItem itemAddArea;
        DXMenuItem itemDeleteArea;

        RepositoryItemComboBox repoUnitDept;
        RepositoryItemComboBox repoFuncDept;
        RepositoryItemComboBox repoAreaDept;
        RepositoryItemLookUpEdit repoUnitRole;
        RepositoryItemLookUpEdit repoFunc;
        RepositoryItemLookUpEdit repoArea;

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnClose.ImageOptions.SvgImage = TPSvgimages.Close;
        }

        private void InitializeMenuItems()
        {
            itemAddUnit = CreateMenuItem("新增", ItemAddUnit_Click, TPSvgimages.Add);
            itemDeleteUnit = CreateMenuItem("刪除/還原", ItemDeleteUnit_Click, TPSvgimages.Remove);
            itemAddFunc = CreateMenuItem("新增", ItemAddFunc_Click, TPSvgimages.Add);
            itemDeleteFunc = CreateMenuItem("刪除/還原", ItemDeleteFunc_Click, TPSvgimages.Remove);
            itemAddArea = CreateMenuItem("新增", ItemAddArea_Click, TPSvgimages.Add);
            itemDeleteArea = CreateMenuItem("刪除/還原", ItemDeleteArea_Click, TPSvgimages.Remove);
        }

        private DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            DXMenuItem menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
            return menuItem;
        }

        private static string NormalizeAction(string actionType)
        {
            if (string.Equals(actionType, ActionCreate, StringComparison.OrdinalIgnoreCase))
            {
                return ActionCreate;
            }

            if (string.Equals(actionType, ActionDelete, StringComparison.OrdinalIgnoreCase))
            {
                return ActionDelete;
            }

            return ActionUpdate;
        }

        private static string GetActionText(string actionType)
        {
            switch (NormalizeAction(actionType))
            {
                case ActionCreate:
                    return "新增";
                case ActionDelete:
                    return "刪除";
                default:
                    return "修改";
            }
        }

        private List<string> ResolveEditableDeptIds()
        {
            List<dm_GroupUser> currentUserGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            List<dm_Group> ehsGroups = dm_GroupBUS.Instance.GetListContainName("安衛環");

            dm_Group ehsAdminGroup = ehsGroups.FirstOrDefault(g => g.DisplayName.Trim() == "安衛環7");
            isEHSAdmin = ehsAdminGroup != null && currentUserGroups.Any(gu => gu.IdGroup == ehsAdminGroup.Id);

            List<string> deptIds = ehsGroups
                .Where(g => currentUserGroups.Any(gu => gu.IdGroup == g.Id))
                .Select(g => g.DisplayName.Replace("安衛環", "").Trim())
                .Where(r => !string.IsNullOrWhiteSpace(r) && r != "7" && r.All(char.IsDigit))
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            if (deptIds.Count == 0)
            {
                string loginDeptId = TPConfigs.LoginUser?.IdDepartment ?? "";
                if (!string.IsNullOrWhiteSpace(loginDeptId))
                {
                    deptIds.Add(loginDeptId);
                }
            }

            return deptIds;
        }

        private void ConfigureUserLookup(RepositoryItemSearchLookUpEdit repository, GridView popupView)
        {
            var userSource = users
                .Where(r => r.Status == 0)
                .Select(r => new
                {
                    DisplayName = $"{r.IdDepartment}/{r.DisplayName}",
                    Id = r.Id,
                    DeptId = r.IdDepartment
                })
                .ToList();

            repository.DataSource = userSource;
            repository.DisplayMember = "DisplayName";
            repository.ValueMember = "Id";
            repository.NullText = "【請選擇人員】";
            repository.PopupView = popupView;
        }

        private RepositoryItemComboBox CreateDeptRepository()
        {
            RepositoryItemComboBox repository = new RepositoryItemComboBox();
            repository.AutoHeight = false;
            repository.TextEditStyle = TextEditStyles.DisableTextEditor;
            repository.Items.AddRange(editableDeptIds.Cast<object>().ToArray());
            return repository;
        }

        private RepositoryItemLookUpEdit CreateLookupRepository(object dataSource, string displayMember, string valueMember, string nullText)
        {
            RepositoryItemLookUpEdit repository = new RepositoryItemLookUpEdit();
            repository.AutoHeight = false;
            repository.DataSource = dataSource;
            repository.DisplayMember = displayMember;
            repository.ValueMember = valueMember;
            repository.NullText = nullText;
            repository.ShowHeader = false;
            repository.TextEditStyle = TextEditStyles.DisableTextEditor;
            repository.Columns.Add(new LookUpColumnInfo(displayMember));
            return repository;
        }

        private GridColumn EnsureColumn(GridView view, string name, string fieldName, string caption, int visibleIndex, int width, RepositoryItem editor = null, bool allowEdit = true)
        {
            GridColumn column = view.Columns.Cast<GridColumn>().FirstOrDefault(r => r.Name == name);
            if (column == null)
            {
                column = new GridColumn { Name = name };
                view.Columns.Add(column);
            }

            column.FieldName = fieldName;
            column.Caption = caption;
            column.Visible = true;
            column.VisibleIndex = visibleIndex;
            column.Width = width;
            column.OptionsColumn.AllowEdit = allowEdit;
            if (editor != null)
            {
                column.ColumnEdit = editor;
            }

            return column;
        }

        private void ConfigureGridLayouts()
        {
            ConfigureUserLookup(itemCbbUser01, repositoryItemSearchLookUpEdit1View);
            ConfigureUserLookup(itemCbbUser02, gridView1);
            ConfigureUserLookup(itemCbbUser03, gridView2);

            repoUnitDept = CreateDeptRepository();
            repoFuncDept = CreateDeptRepository();
            repoAreaDept = CreateDeptRepository();
            repoUnitRole = CreateLookupRepository(
                roles.Select(r => new { r.Id, r.DisplayName }).ToList(),
                "DisplayName",
                "Id",
                "【請選擇類別】");
            repoFunc = CreateLookupRepository(
                funcs.Select(r => new { r.Id, r.DisplayName }).ToList(),
                "DisplayName",
                "Id",
                "【請選擇類別】");
            repoArea = CreateLookupRepository(
                areas.Select(r => new { r.Id, DisplayName = r.DisplayName ?? "" }).ToList(),
                "DisplayName",
                "Id",
                "【請選擇區域】");

            gcUnitEHSOrg.RepositoryItems.Add(repoUnitDept);
            gcUnitEHSOrg.RepositoryItems.Add(repoUnitRole);
            gcEHSFunction.RepositoryItems.Add(repoFuncDept);
            gcEHSFunction.RepositoryItems.Add(repoFunc);
            gcArea5SResponsible.RepositoryItems.Add(repoAreaDept);
            gcArea5SResponsible.RepositoryItems.Add(repoArea);

            gridColumn1.Visible = false;
            gridColumn2.Caption = "接任人";
            gridColumn2.FieldName = "UserId";
            gridColumn2.ColumnEdit = itemCbbUser01;
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 3;
            gridColumn2.Width = 180;
            gridColumn2.OptionsColumn.AllowEdit = true;

            gridColumn3.Caption = "類別";
            gridColumn3.FieldName = "RoleId";
            gridColumn3.ColumnEdit = repoUnitRole;
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 2;
            gridColumn3.Width = 180;
            gridColumn3.OptionsColumn.AllowEdit = true;

            EnsureColumn(gvUnitEHSOrg, "colUnitAction", "ActionText", "動作", 0, 80, null, false);
            EnsureColumn(gvUnitEHSOrg, "colUnitDept", "DeptId", "部門", 1, 100, repoUnitDept);

            gridColumn4.Visible = false;
            gridColumn5.Caption = "接任人";
            gridColumn5.FieldName = "UserId";
            gridColumn5.ColumnEdit = itemCbbUser02;
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 3;
            gridColumn5.Width = 180;
            gridColumn5.OptionsColumn.AllowEdit = true;

            gridColumn6.Caption = "類別";
            gridColumn6.FieldName = "FunctionId";
            gridColumn6.ColumnEdit = repoFunc;
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 2;
            gridColumn6.Width = 180;
            gridColumn6.OptionsColumn.AllowEdit = true;

            EnsureColumn(gvEHSFunction, "colFuncAction", "ActionText", "動作", 0, 80, null, false);
            EnsureColumn(gvEHSFunction, "colFuncDept", "DeptId", "部門", 1, 100, repoFuncDept);

            gridColumn7.Visible = false;
            gridColumn8.Caption = "責任人員";
            gridColumn8.FieldName = "EmployeeId";
            gridColumn8.ColumnEdit = itemCbbUser03;
            gridColumn8.Visible = true;
            gridColumn8.VisibleIndex = 4;
            gridColumn8.Width = 180;
            gridColumn8.OptionsColumn.AllowEdit = true;

            gridColumn9.Caption = "紅線區域";
            gridColumn9.FieldName = "AreaId";
            gridColumn9.ColumnEdit = repoArea;
            gridColumn9.Visible = true;
            gridColumn9.VisibleIndex = 2;
            gridColumn9.Width = 160;
            gridColumn9.OptionsColumn.AllowEdit = true;

            gridColumn10.Caption = "代理人";
            gridColumn10.FieldName = "AgentId";
            gridColumn10.ColumnEdit = itemCbbUser03;
            gridColumn10.Visible = true;
            gridColumn10.VisibleIndex = 5;
            gridColumn10.Width = 180;
            gridColumn10.OptionsColumn.AllowEdit = true;

            gridColumn11.Caption = "區域";
            gridColumn11.FieldName = "AreaName";
            gridColumn11.Visible = true;
            gridColumn11.VisibleIndex = 7;
            gridColumn11.Width = 140;
            gridColumn11.OptionsColumn.AllowEdit = true;

            gridColumn12.Caption = "編號";
            gridColumn12.FieldName = "AreaCode";
            gridColumn12.Visible = true;
            gridColumn12.VisibleIndex = 6;
            gridColumn12.Width = 100;
            gridColumn12.OptionsColumn.AllowEdit = true;

            EnsureColumn(gvArea5SResponsible, "colAreaAction", "ActionText", "動作", 0, 80, null, false);
            EnsureColumn(gvArea5SResponsible, "colAreaDept", "DeptId", "部門", 1, 100, repoAreaDept);
            EnsureColumn(gvArea5SResponsible, "colAreaBoss", "BossId", "督導主管", 8, 180, itemCbbUser03);

            gvUnitEHSOrg.PopupMenuShowing -= gvUnitEHSOrg_PopupMenuShowing;
            gvEHSFunction.PopupMenuShowing -= gvEHSFunction_PopupMenuShowing;
            gvArea5SResponsible.PopupMenuShowing -= gvArea5SResponsible_PopupMenuShowing;
            gvUnitEHSOrg.ShowingEditor -= GridView_ShowingEditor;
            gvEHSFunction.ShowingEditor -= GridView_ShowingEditor;
            gvArea5SResponsible.ShowingEditor -= GridView_ShowingEditor;
            gvUnitEHSOrg.CellValueChanged -= gvUnitEHSOrg_CellValueChanged;
            gvEHSFunction.CellValueChanged -= gvEHSFunction_CellValueChanged;
            gvArea5SResponsible.CellValueChanged -= gvArea5SResponsible_CellValueChanged;

            gvUnitEHSOrg.PopupMenuShowing += gvUnitEHSOrg_PopupMenuShowing;
            gvEHSFunction.PopupMenuShowing += gvEHSFunction_PopupMenuShowing;
            gvArea5SResponsible.PopupMenuShowing += gvArea5SResponsible_PopupMenuShowing;
            gvUnitEHSOrg.ShowingEditor += GridView_ShowingEditor;
            gvEHSFunction.ShowingEditor += GridView_ShowingEditor;
            gvArea5SResponsible.ShowingEditor += GridView_ShowingEditor;
            gvUnitEHSOrg.CellValueChanged += gvUnitEHSOrg_CellValueChanged;
            gvEHSFunction.CellValueChanged += gvEHSFunction_CellValueChanged;
            gvArea5SResponsible.CellValueChanged += gvArea5SResponsible_CellValueChanged;
        }

        private void LoadGridData()
        {
            List<dt310_UnitEHSOrg> unitData = editableDeptIds
                .SelectMany(r => dt310_UnitEHSOrgBUS.Instance.GetListByDeptId(r))
                .GroupBy(r => r.Id)
                .Select(g => g.First())
                .OrderBy(r => r.DeptId)
                .ThenBy(r => r.RoleId)
                .ToList();

            List<dt310_EHSFunction> funcData = editableDeptIds
                .SelectMany(r => dt310_EHSFunctionBUS.Instance.GetListByDeptId(r))
                .GroupBy(r => r.Id)
                .Select(g => g.First())
                .OrderBy(r => r.DeptId)
                .ThenBy(r => r.FunctionId)
                .ToList();

            List<dt310_Area5SResponsible> areaData = editableDeptIds
                .SelectMany(r => dt310_Area5SResponsibleBUS.Instance.GetListByDeptId(r))
                .GroupBy(r => r.Id)
                .Select(g => g.First())
                .OrderBy(r => r.DeptId)
                .ThenBy(r => r.AreaId)
                .ThenBy(r => r.AreaCode)
                .ToList();

            List<UpdateLeaveUserData> unitRows = unitData
                .Select(r => CreateUnitRow(r))
                .ToList();
            List<UpdateLeaveUserData> funcRows = funcData
                .Select(r => CreateFuncRow(r))
                .ToList();
            List<UpdateLeaveUserData> areaRows = areaData
                .Select(r => CreateAreaRow(r))
                .ToList();

            sourceUnitEHSOrg.DataSource = unitRows;
            sourceEHSFunction.DataSource = funcRows;
            sourceArea5SResponsible.DataSource = areaRows;

            gcUnitEHSOrg.DataSource = sourceUnitEHSOrg;
            gcEHSFunction.DataSource = sourceEHSFunction;
            gcArea5SResponsible.DataSource = sourceArea5SResponsible;

            gvUnitEHSOrg.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvEHSFunction.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvArea5SResponsible.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gvUnitEHSOrg.BestFitColumns();
            gvEHSFunction.BestFitColumns();
            gvArea5SResponsible.BestFitColumns();
        }

        private UpdateLeaveUserData CreateUnitRow(dt310_UnitEHSOrg data = null)
        {
            UpdateLeaveUserData row = new UpdateLeaveUserData
            {
                ActionType = data == null ? ActionCreate : ActionUpdate,
                DeptId = data?.DeptId ?? editableDeptIds.FirstOrDefault(),
                RoleId = data?.RoleId,
                UserId = data?.EmployeeId ?? "",
                StartDate = data?.StartDate ?? DateTime.Now.Date,
                UnitEHSOrgData = data
            };
            NormalizeUnitRow(row);
            return row;
        }

        private UpdateLeaveUserData CreateFuncRow(dt310_EHSFunction data = null)
        {
            UpdateLeaveUserData row = new UpdateLeaveUserData
            {
                ActionType = data == null ? ActionCreate : ActionUpdate,
                DeptId = data?.DeptId ?? editableDeptIds.FirstOrDefault(),
                FunctionId = data?.FunctionId,
                UserId = data?.EmployeeId ?? "",
                StartDate = data?.StartDate ?? DateTime.Now.Date,
                EHSFunctionData = data
            };
            NormalizeFuncRow(row);
            return row;
        }

        private UpdateLeaveUserData CreateAreaRow(dt310_Area5SResponsible data = null)
        {
            UpdateLeaveUserData row = new UpdateLeaveUserData
            {
                ActionType = data == null ? ActionCreate : ActionUpdate,
                DeptId = data?.DeptId ?? editableDeptIds.FirstOrDefault(),
                AreaId = data?.AreaId,
                EmployeeId = data?.EmployeeId ?? "",
                AgentId = data?.AgentId ?? "",
                BossId = data?.BossId ?? "",
                AreaCode = data?.AreaCode ?? "",
                AreaName = data?.AreaName ?? "",
                Area5SResponsibleData = data
            };
            NormalizeAreaRow(row);
            return row;
        }

        private void NormalizeUnitRow(UpdateLeaveUserData row)
        {
            if (row == null)
            {
                return;
            }

            row.ActionType = NormalizeAction(row.ActionType);
            row.ActionText = GetActionText(row.ActionType);
            row.Desc = BuildUnitDesc(row.DeptId, row.RoleId);
        }

        private void NormalizeFuncRow(UpdateLeaveUserData row)
        {
            if (row == null)
            {
                return;
            }

            row.ActionType = NormalizeAction(row.ActionType);
            row.ActionText = GetActionText(row.ActionType);
            row.Desc = BuildFuncDesc(row.DeptId, row.FunctionId);
        }

        private void NormalizeAreaRow(UpdateLeaveUserData row)
        {
            if (row == null)
            {
                return;
            }

            row.ActionType = NormalizeAction(row.ActionType);
            row.ActionText = GetActionText(row.ActionType);
            row.Desc = BuildAreaDesc(row.DeptId, row.AreaId);
        }

        private string BuildUnitDesc(string deptId, Nullable<int> roleId)
        {
            string roleName = roles.FirstOrDefault(r => r.Id == roleId)?.DisplayName ?? "";
            return $"{deptId}：{roleName}".Trim('：');
        }

        private string BuildFuncDesc(string deptId, Nullable<int> functionId)
        {
            string funcName = funcs.FirstOrDefault(r => r.Id == functionId)?.DisplayName ?? "";
            return $"{deptId}：{funcName}".Trim('：');
        }

        private string BuildAreaDesc(string deptId, Nullable<int> areaId)
        {
            string areaName = areas.FirstOrDefault(r => r.Id == areaId)?.DisplayName ?? "";
            return $"{deptId}：{areaName}".Trim('：');
        }

        private static bool IsDeleteRow(UpdateLeaveUserData row)
        {
            return string.Equals(NormalizeAction(row?.ActionType), ActionDelete, StringComparison.OrdinalIgnoreCase);
        }

        private string GetDefaultDeptId(GridView view)
        {
            UpdateLeaveUserData focusedRow = view?.GetFocusedRow() as UpdateLeaveUserData;
            string deptId = focusedRow?.DeptId;
            if (!string.IsNullOrWhiteSpace(deptId) && editableDeptIds.Contains(deptId))
            {
                return deptId;
            }

            return editableDeptIds.FirstOrDefault();
        }

        private void ItemAddUnit_Click(object sender, EventArgs e)
        {
            List<UpdateLeaveUserData> rows = sourceUnitEHSOrg.DataSource as List<UpdateLeaveUserData>;
            if (rows == null)
            {
                return;
            }

            UpdateLeaveUserData row = CreateUnitRow();
            row.DeptId = GetDefaultDeptId(gvUnitEHSOrg);
            NormalizeUnitRow(row);
            rows.Insert(0, row);
            sourceUnitEHSOrg.ResetBindings(false);
            gvUnitEHSOrg.FocusedRowHandle = 0;
            gvUnitEHSOrg.BestFitColumns();
        }

        private void ItemDeleteUnit_Click(object sender, EventArgs e)
        {
            ToggleDeleteRow(gvUnitEHSOrg, sourceUnitEHSOrg, r => r.UnitEHSOrgData == null && NormalizeAction(r.ActionType) == ActionCreate, NormalizeUnitRow);
        }

        private void ItemAddFunc_Click(object sender, EventArgs e)
        {
            List<UpdateLeaveUserData> rows = sourceEHSFunction.DataSource as List<UpdateLeaveUserData>;
            if (rows == null)
            {
                return;
            }

            UpdateLeaveUserData row = CreateFuncRow();
            row.DeptId = GetDefaultDeptId(gvEHSFunction);
            NormalizeFuncRow(row);
            rows.Insert(0, row);
            sourceEHSFunction.ResetBindings(false);
            gvEHSFunction.FocusedRowHandle = 0;
            gvEHSFunction.BestFitColumns();
        }

        private void ItemDeleteFunc_Click(object sender, EventArgs e)
        {
            ToggleDeleteRow(gvEHSFunction, sourceEHSFunction, r => r.EHSFunctionData == null && NormalizeAction(r.ActionType) == ActionCreate, NormalizeFuncRow);
        }

        private void ItemAddArea_Click(object sender, EventArgs e)
        {
            List<UpdateLeaveUserData> rows = sourceArea5SResponsible.DataSource as List<UpdateLeaveUserData>;
            if (rows == null)
            {
                return;
            }

            UpdateLeaveUserData row = CreateAreaRow();
            row.DeptId = GetDefaultDeptId(gvArea5SResponsible);
            NormalizeAreaRow(row);
            rows.Insert(0, row);
            sourceArea5SResponsible.ResetBindings(false);
            gvArea5SResponsible.FocusedRowHandle = 0;
            gvArea5SResponsible.BestFitColumns();
        }

        private void ItemDeleteArea_Click(object sender, EventArgs e)
        {
            ToggleDeleteRow(gvArea5SResponsible, sourceArea5SResponsible, r => r.Area5SResponsibleData == null && NormalizeAction(r.ActionType) == ActionCreate, NormalizeAreaRow);
        }

        private void ToggleDeleteRow(GridView view, BindingSource source, Func<UpdateLeaveUserData, bool> removeCondition, Action<UpdateLeaveUserData> normalizeAction)
        {
            List<UpdateLeaveUserData> rows = source.DataSource as List<UpdateLeaveUserData>;
            UpdateLeaveUserData row = view.GetFocusedRow() as UpdateLeaveUserData;
            if (rows == null || row == null)
            {
                return;
            }

            if (removeCondition(row))
            {
                rows.Remove(row);
            }
            else
            {
                row.ActionType = NormalizeAction(row.ActionType) == ActionDelete ? ActionUpdate : ActionDelete;
                normalizeAction(row);
            }

            source.ResetBindings(false);
            view.RefreshData();
        }

        private void gvUnitEHSOrg_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null)
            {
                return;
            }

            e.Menu.Items.Add(itemAddUnit);
            if (e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemDeleteUnit);
            }
        }

        private void gvEHSFunction_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null)
            {
                return;
            }

            e.Menu.Items.Add(itemAddFunc);
            if (e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemDeleteFunc);
            }
        }

        private void gvArea5SResponsible_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null)
            {
                return;
            }

            e.Menu.Items.Add(itemAddArea);
            if (e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemDeleteArea);
            }
        }

        private void GridView_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GridView view = sender as GridView;
            UpdateLeaveUserData row = view?.GetFocusedRow() as UpdateLeaveUserData;
            if (row == null)
            {
                return;
            }

            if (IsDeleteRow(row) || string.Equals(view.FocusedColumn?.FieldName, "ActionText", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;
            }
        }

        private void gvUnitEHSOrg_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            UpdateLeaveUserData row = gvUnitEHSOrg.GetRow(e.RowHandle) as UpdateLeaveUserData;
            NormalizeUnitRow(row);
            gvUnitEHSOrg.RefreshRow(e.RowHandle);
        }

        private void gvEHSFunction_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            UpdateLeaveUserData row = gvEHSFunction.GetRow(e.RowHandle) as UpdateLeaveUserData;
            NormalizeFuncRow(row);
            gvEHSFunction.RefreshRow(e.RowHandle);
        }

        private void gvArea5SResponsible_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            UpdateLeaveUserData row = gvArea5SResponsible.GetRow(e.RowHandle) as UpdateLeaveUserData;
            NormalizeAreaRow(row);
            gvArea5SResponsible.RefreshRow(e.RowHandle);
        }

        private void f310_EHSAssign_Info_Load(object sender, EventArgs e)
        {
            Text = "安衛環人員異動申請";

            users = dm_UserBUS.Instance.GetList();
            roles = dt310_RoleBUS.Instance.GetList();
            funcs = dt310_FunctionBUS.Instance.GetList();
            areas = dt310_Area5SBUS.Instance.GetList();
            editableDeptIds = ResolveEditableDeptIds();

            ConfigureGridLayouts();
            LoadGridData();
        }

        private bool HasUnitChange(UpdateLeaveUserData row)
        {
            if (row == null)
            {
                return false;
            }

            string actionType = NormalizeAction(row.ActionType);
            if (actionType == ActionCreate)
            {
                return !string.IsNullOrWhiteSpace(row.DeptId)
                    || row.RoleId.HasValue
                    || !string.IsNullOrWhiteSpace(row.UserId);
            }

            if (actionType == ActionDelete)
            {
                return row.UnitEHSOrgData != null;
            }

            return row.UnitEHSOrgData != null && (
                !string.Equals(row.DeptId ?? "", row.UnitEHSOrgData.DeptId ?? "", StringComparison.OrdinalIgnoreCase)
                || row.RoleId != row.UnitEHSOrgData.RoleId
                || !string.Equals(row.UserId ?? "", row.UnitEHSOrgData.EmployeeId ?? "", StringComparison.OrdinalIgnoreCase));
        }

        private bool HasFuncChange(UpdateLeaveUserData row)
        {
            if (row == null)
            {
                return false;
            }

            string actionType = NormalizeAction(row.ActionType);
            if (actionType == ActionCreate)
            {
                return !string.IsNullOrWhiteSpace(row.DeptId)
                    || row.FunctionId.HasValue
                    || !string.IsNullOrWhiteSpace(row.UserId);
            }

            if (actionType == ActionDelete)
            {
                return row.EHSFunctionData != null;
            }

            return row.EHSFunctionData != null && (
                !string.Equals(row.DeptId ?? "", row.EHSFunctionData.DeptId ?? "", StringComparison.OrdinalIgnoreCase)
                || row.FunctionId != row.EHSFunctionData.FunctionId
                || !string.Equals(row.UserId ?? "", row.EHSFunctionData.EmployeeId ?? "", StringComparison.OrdinalIgnoreCase));
        }

        private bool HasAreaChange(UpdateLeaveUserData row)
        {
            if (row == null)
            {
                return false;
            }

            string actionType = NormalizeAction(row.ActionType);
            if (actionType == ActionCreate)
            {
                return !string.IsNullOrWhiteSpace(row.DeptId)
                    || row.AreaId.HasValue
                    || !string.IsNullOrWhiteSpace(row.AreaCode)
                    || !string.IsNullOrWhiteSpace(row.AreaName)
                    || !string.IsNullOrWhiteSpace(row.EmployeeId)
                    || !string.IsNullOrWhiteSpace(row.AgentId)
                    || !string.IsNullOrWhiteSpace(row.BossId);
            }

            if (actionType == ActionDelete)
            {
                return row.Area5SResponsibleData != null;
            }

            return row.Area5SResponsibleData != null && (
                !string.Equals(row.DeptId ?? "", row.Area5SResponsibleData.DeptId ?? "", StringComparison.OrdinalIgnoreCase)
                || row.AreaId != row.Area5SResponsibleData.AreaId
                || !string.Equals(row.AreaCode ?? "", row.Area5SResponsibleData.AreaCode ?? "", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(row.AreaName ?? "", row.Area5SResponsibleData.AreaName ?? "", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(row.EmployeeId ?? "", row.Area5SResponsibleData.EmployeeId ?? "", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(row.AgentId ?? "", row.Area5SResponsibleData.AgentId ?? "", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(row.BossId ?? "", row.Area5SResponsibleData.BossId ?? "", StringComparison.OrdinalIgnoreCase));
        }

        private bool ValidateUnitRows(List<UpdateLeaveUserData> rows)
        {
            UpdateLeaveUserData invalidRow = rows.FirstOrDefault(r =>
                NormalizeAction(r.ActionType) != ActionDelete
                && (string.IsNullOrWhiteSpace(r.DeptId) || !r.RoleId.HasValue || string.IsNullOrWhiteSpace(r.UserId)));

            if (invalidRow == null)
            {
                return true;
            }

            XtraMessageBox.Show("安衛環組織表資料未填寫完整，請確認部門、類別與接任人。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private bool ValidateFuncRows(List<UpdateLeaveUserData> rows)
        {
            UpdateLeaveUserData invalidRow = rows.FirstOrDefault(r =>
                NormalizeAction(r.ActionType) != ActionDelete
                && (string.IsNullOrWhiteSpace(r.DeptId) || !r.FunctionId.HasValue || string.IsNullOrWhiteSpace(r.UserId)));

            if (invalidRow == null)
            {
                return true;
            }

            XtraMessageBox.Show("各機能設定表資料未填寫完整，請確認部門、類別與接任人。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private bool ValidateAreaRows(List<UpdateLeaveUserData> rows)
        {
            UpdateLeaveUserData invalidRow = rows.FirstOrDefault(r =>
                NormalizeAction(r.ActionType) != ActionDelete
                && (string.IsNullOrWhiteSpace(r.DeptId)
                    || !r.AreaId.HasValue
                    || string.IsNullOrWhiteSpace(r.AreaCode)
                    || string.IsNullOrWhiteSpace(r.AreaName)
                    || string.IsNullOrWhiteSpace(r.EmployeeId)
                    || string.IsNullOrWhiteSpace(r.AgentId)
                    || string.IsNullOrWhiteSpace(r.BossId)));

            if (invalidRow == null)
            {
                return true;
            }

            XtraMessageBox.Show("紅線區域負責人資料未填寫完整，請確認部門、區域、編號、人員與主管。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private static string ResolveDeptId(UpdateLeaveUserData row)
        {
            if (row == null)
            {
                return "";
            }

            if (!string.IsNullOrWhiteSpace(row.DeptId))
            {
                return row.DeptId;
            }

            return row.UnitEHSOrgData?.DeptId
                ?? row.EHSFunctionData?.DeptId
                ?? row.Area5SResponsibleData?.DeptId
                ?? "";
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvUnitEHSOrg.CloseEditor();
            gvEHSFunction.CloseEditor();
            gvArea5SResponsible.CloseEditor();
            ActiveControl = null;

            List<UpdateLeaveUserData> unitRows = sourceUnitEHSOrg.DataSource as List<UpdateLeaveUserData> ?? new List<UpdateLeaveUserData>();
            List<UpdateLeaveUserData> funcRows = sourceEHSFunction.DataSource as List<UpdateLeaveUserData> ?? new List<UpdateLeaveUserData>();
            List<UpdateLeaveUserData> areaRows = sourceArea5SResponsible.DataSource as List<UpdateLeaveUserData> ?? new List<UpdateLeaveUserData>();

            List<UpdateLeaveUserData> unitChanged = unitRows.Where(HasUnitChange).ToList();
            List<UpdateLeaveUserData> funcChanged = funcRows.Where(HasFuncChange).ToList();
            List<UpdateLeaveUserData> areaChanged = areaRows.Where(HasAreaChange).ToList();

            if (!unitChanged.Any() && !funcChanged.Any() && !areaChanged.Any())
            {
                XtraMessageBox.Show("請至少新增、刪除或修改一項資料！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateUnitRows(unitChanged) || !ValidateFuncRows(funcChanged) || !ValidateAreaRows(areaChanged))
            {
                return;
            }

            List<string> changedDeptIds = unitChanged
                .Concat(funcChanged)
                .Concat(areaChanged)
                .Select(ResolveDeptId)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct()
                .ToList();

            if (changedDeptIds.Count != 1)
            {
                XtraMessageBox.Show("一次只能送審同一個部門的資料，若涉及多個部門請分開送審。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string deptId = changedDeptIds[0];
            string psmDeptId = deptId.Length >= 2 ? deptId.Substring(0, 2) : deptId;

            int GetGroupId(string name) => dm_GroupBUS.Instance.GetItemByName(name)?.Id ?? -1;

            int level2GroupId = GetGroupId($"二級{deptId}");
            int psmGroupId = GetGroupId($"PSM專人{psmDeptId}");
            int level1GroupId = GetGroupId($"一級{psmDeptId}");

            if (level2GroupId <= 0 || psmGroupId <= 0 || level1GroupId <= 0)
            {
                XtraMessageBox.Show("簽核群組設定不完整，請先確認二級主管、PSM專人與一級主管群組。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UpdateLeaveUserJson jsonData = new UpdateLeaveUserJson
            {
                UnitEHSOrg = unitChanged,
                EHSFunction = funcChanged,
                Area5SResponsible = areaChanged
            };

            string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            dt310_UpdateLeaveUser updateRecord = new dt310_UpdateLeaveUser
            {
                IdUserLeave = TPConfigs.LoginUser.Id,
                DataType = "EHSAssign",
                DisplayName = "人員異動申請",
                IsProcess = true,
                IsCancel = false,
                IdGroupProcess = level2GroupId,
                DataJson = json,
                CreateBy = TPConfigs.LoginUser.Id,
                CreateAt = DateTime.Now
            };

            int idUpdateData = dt310_UpdateLeaveUserBUS.Instance.Add(updateRecord);
            if (idUpdateData <= 0)
            {
                XtraMessageBox.Show("建立人員異動申請失敗，請確認資料後再試！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var steps = new[] { (level2GroupId, 0), (psmGroupId, 1), (level1GroupId, 2) };

            foreach (var (groupId, index) in steps)
            {
                dt310_UpdateLeaveUser_detailBUS.Instance.Add(new dt310_UpdateLeaveUser_detail
                {
                    IdUpdateData = idUpdateData,
                    IdGroup = groupId,
                    IndexStep = index
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
