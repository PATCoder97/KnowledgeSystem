using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class uc309_RecoveryMgmt : XtraUserControl
    {
        private readonly BindingSource sourceTickets = new BindingSource();
        private DXMenuItem itemViewIssue;
        private DXMenuItem itemUploadEvidence;
        private DXMenuItem itemViewEvidence;
        private DXMenuItem itemUpdateTime;
        private DXMenuItem itemConfirmComplete;
        private DXMenuItem itemCancelTicket;
        private RefreshHelper helper;

        private List<dm_User> users = new List<dm_User>();
        private bool isManager309;

        public uc309_RecoveryMgmt()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;

            gcData.DataSource = sourceTickets;
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.FocusedRowChanged += gvData_FocusedRowChanged;
        }

        private void gvData_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateActionStates();
        }

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnViewIssue.ImageOptions.SvgImage = TPSvgimages.View;
            btnDownloadGuide.ImageOptions.SvgImage = TPSvgimages.Attach;
            btnManageGuide.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnUploadEvidence.ImageOptions.SvgImage = TPSvgimages.UploadFile;
            btnViewEvidence.ImageOptions.SvgImage = TPSvgimages.Search;
            btnUpdateTime.ImageOptions.SvgImage = TPSvgimages.Schedule;
            btnConfirmComplete.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnCancelTicket.ImageOptions.SvgImage = TPSvgimages.Remove;
            barCbbDept.ImageOptions.SvgImage = TPSvgimages.Dept;
        }

        private void InitializeMenuItems()
        {
            itemViewIssue = CreateMenuItem("查看領用", ItemViewIssueMenu_Click, TPSvgimages.View);
            itemUploadEvidence = CreateMenuItem("上傳證明", ItemUploadEvidenceMenu_Click, TPSvgimages.UploadFile);
            itemViewEvidence = CreateMenuItem("查看證明", ItemViewEvidenceMenu_Click, TPSvgimages.Search);
            itemUpdateTime = CreateMenuItem("更新日期", ItemUpdateTimeMenu_Click, TPSvgimages.Schedule);
            itemConfirmComplete = CreateMenuItem("確認完成", ItemConfirmCompleteMenu_Click, TPSvgimages.Confirm);
            itemCancelTicket = CreateMenuItem("取消單", ItemCancelTicketMenu_Click, TPSvgimages.Remove);
        }

        private DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, DevExpress.Utils.Svg.SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        private void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void ItemViewIssueMenu_Click(object sender, EventArgs e) => btnViewIssue_ItemClick(sender, null);

        private void ItemUploadEvidenceMenu_Click(object sender, EventArgs e) => btnUploadEvidence_ItemClick(sender, null);

        private void ItemViewEvidenceMenu_Click(object sender, EventArgs e) => btnViewEvidence_ItemClick(sender, null);

        private void ItemUpdateTimeMenu_Click(object sender, EventArgs e) => btnUpdateTime_ItemClick(sender, null);

        private void ItemConfirmCompleteMenu_Click(object sender, EventArgs e) => btnConfirmComplete_ItemClick(sender, null);

        private void ItemCancelTicketMenu_Click(object sender, EventArgs e) => btnCancelTicket_ItemClick(sender, null);

        private void uc309_RecoveryMgmt_Load(object sender, EventArgs e)
        {
            try
            {
                InitializePermissions();
                LoadData();
            }
            catch (Exception ex)
            {
                Enabled = false;
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializePermissions()
        {

            var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var departments = dm_DeptBUS.Instance.GetList();
            var groups = dm_GroupBUS.Instance.GetListByName(dt309_RecoveryConst.SparePartGroupName);
            var managerGroups = dm_GroupBUS.Instance.GetListByName(dt309_RecoveryConst.SparePartManagerGroupName);

            var accessibleGroups = groups
                .Where(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id))
                .ToList();

            if (accessibleGroups.Any(r => r.IdDept == "7"))
            {
                accessibleGroups = groups;
            }

            isManager309 = managerGroups.Any(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id));
            btnManageGuide.Visibility = isManager309
                ? BarItemVisibility.Always
                : BarItemVisibility.Never;
            btnDownloadGuide.Visibility = BarItemVisibility.Always;

            cbbDept.Items.Clear();
            cbbDept.Items.AddRange(departments
                .Where(dept => accessibleGroups.Any(group => group.IdDept == dept.Id))
                .OrderBy(dept => dept.Id)
                .Select(dept => $"{dept.Id} {dept.DisplayName}")
                .ToArray());

            barCbbDept.EditValue = cbbDept.Items.Count > 0
                ? cbbDept.Items[0]?.ToString()
                : string.Empty;

            users = dm_UserBUS.Instance.GetList()
                .Where(r => (r.Status ?? 0) == 0)
                .OrderBy(r => r.IdDepartment)
                .ThenBy(r => r.DisplayName)
                .ToList();
        }

        private void LoadData()
        {
            string deptPrefix = ParseToken(barCbbDept.EditValue?.ToString());

            var tickets = dt309_RecoveryBUS.Instance.GetList(deptPrefix, string.Empty, null, null, string.Empty);
            sourceTickets.DataSource = BuildTicketRows(tickets);
            gvData.BestFitColumns();
            UpdateActionStates();
        }

        private List<dt309_RecoveryTicketGridRow> BuildTicketRows(List<dt309_RecoveryTickets> tickets)
        {
            tickets = tickets ?? new List<dt309_RecoveryTickets>();
            if (tickets.Count == 0)
            {
                return new List<dt309_RecoveryTicketGridRow>();
            }

            var materialIds = tickets
                .SelectMany(ticket => new int?[] { ticket.NewMaterialId, ticket.OldBaseMaterialId, ticket.OldRecoveryMaterialId })
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var storageIds = tickets
                .SelectMany(ticket => new int?[] { ticket.SourceStorageId, ticket.RestockStorageId })
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var materials = dt309_MaterialsBUS.Instance.GetListByIds(materialIds)
                .ToDictionary(item => item.Id, item => item);
            var storages = dt309_StoragesBUS.Instance.GetList()
                .Where(item => storageIds.Contains(item.Id))
                .ToDictionary(item => item.Id, item => item);
            var userMap = users.ToDictionary(item => item.Id, item => item);
            var evidenceCountMap = dt309_RecoveryBUS.Instance.GetEvidenceCountMap(tickets.Select(item => item.Id));

            return tickets.Select(ticket =>
            {
                materials.TryGetValue(ticket.NewMaterialId, out dt309_Materials newMaterial);
                materials.TryGetValue(ticket.OldBaseMaterialId, out dt309_Materials oldBaseMaterial);

                dt309_Materials oldRecoveryMaterial = null;
                if (ticket.OldRecoveryMaterialId.HasValue)
                {
                    materials.TryGetValue(ticket.OldRecoveryMaterialId.Value, out oldRecoveryMaterial);
                }

                storages.TryGetValue(ticket.SourceStorageId, out dt309_Storages sourceStorage);

                dt309_Storages restockStorage = null;
                if (ticket.RestockStorageId.HasValue)
                {
                    storages.TryGetValue(ticket.RestockStorageId.Value, out restockStorage);
                }

                userMap.TryGetValue(ticket.AssignedUserId ?? string.Empty, out dm_User assignedUser);
                userMap.TryGetValue(ticket.CreatedBy ?? string.Empty, out dm_User createdUser);

                return new dt309_RecoveryTicketGridRow
                {
                    Id = ticket.Id,
                    TicketNo = ticket.TicketNo,
                    IssueTransactionId = ticket.IssueTransactionId,
                    RestockInTransactionId = ticket.RestockInTransactionId,
                    NewMaterialId = ticket.NewMaterialId,
                    OldBaseMaterialId = ticket.OldBaseMaterialId,
                    OldRecoveryMaterialId = ticket.OldRecoveryMaterialId,
                    NewMaterialCode = newMaterial?.Code,
                    NewMaterialDisplayName = newMaterial?.DisplayName,
                    OldBaseMaterialCode = oldBaseMaterial?.Code,
                    OldBaseMaterialDisplayName = oldBaseMaterial?.DisplayName,
                    OldRecoveryMaterialCode = oldRecoveryMaterial?.Code,
                    OldRecoveryMaterialDisplayName = oldRecoveryMaterial?.DisplayName,
                    RecoveryOption = ticket.RecoveryOption,
                    Quantity = ticket.Quantity,
                    SourceStorageId = ticket.SourceStorageId,
                    SourceStorageName = sourceStorage?.DisplayName,
                    RestockStorageId = ticket.RestockStorageId,
                    RestockStorageName = restockStorage?.DisplayName,
                    AssignedUserId = ticket.AssignedUserId,
                    AssignedUserName = assignedUser?.DisplayName,
                    PlannedDisposeDate = ticket.PlannedDisposeDate,
                    ActualDisposeDate = ticket.ActualDisposeDate,
                    Status = ticket.Status,
                    Description = ticket.Description,
                    ResultNote = ticket.ResultNote,
                    EvidenceCount = evidenceCountMap.TryGetValue(ticket.Id, out int evidenceCount)
                        ? evidenceCount
                        : 0,
                    CreatedBy = ticket.CreatedBy,
                    CreatedByName = createdUser?.DisplayName,
                    CreatedDate = ticket.CreatedDate,
                    IdDept = newMaterial?.IdDept
                };
            }).ToList();
        }

        private string ParseToken(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            return raw.Split(' ')[0];
        }

        private dt309_RecoveryTicketGridRow GetFocusedRow()
        {
            if (gvData.FocusedRowHandle < 0)
            {
                return null;
            }

            return gvData.GetRow(gvData.FocusedRowHandle) as dt309_RecoveryTicketGridRow;
        }

        private void Filter_EditValueChanged(object sender, EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            LoadData();
        }

        private void btnReload_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadData();
        }

        private void UpdateActionStates()
        {
            var row = GetFocusedRow();
            bool hasRow = row != null;
            bool isScrap = hasRow && dt309_RecoveryConst.IsScrap(row.RecoveryOption);
            bool isRestock = hasRow && dt309_RecoveryConst.IsRestock(row.RecoveryOption);
            bool isScheduled = hasRow && string.Equals(row.Status, dt309_RecoveryConst.RecoveryStatusScheduled, StringComparison.OrdinalIgnoreCase);
            bool isAwaitManagerConfirm = hasRow && string.Equals(row.Status, dt309_RecoveryConst.RecoveryStatusAwaitManagerConfirm, StringComparison.OrdinalIgnoreCase);
            bool isCompleted = hasRow && string.Equals(row.Status, dt309_RecoveryConst.RecoveryStatusCompleted, StringComparison.OrdinalIgnoreCase);
            bool isCancelled = hasRow && string.Equals(row.Status, dt309_RecoveryConst.RecoveryStatusCancelled, StringComparison.OrdinalIgnoreCase);
            bool isCreator = hasRow && string.Equals(row.CreatedBy, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase);

            btnViewIssue.Enabled = hasRow;
            btnDownloadGuide.Enabled = true;
            btnUploadEvidence.Enabled = hasRow && isScrap && !isCompleted && !isCancelled;
            btnViewEvidence.Enabled = hasRow && row.EvidenceCount > 0;
            btnUpdateTime.Enabled = hasRow && isScrap && isScheduled;
            btnConfirmComplete.Enabled = hasRow && isScrap && isAwaitManagerConfirm && (isManager309 || isCreator);
            btnCancelTicket.Enabled = hasRow && !isCompleted && !isRestock;
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (!e.HitInfo.InRowCell || !e.HitInfo.InDataRow || e.Menu == null)
            {
                return;
            }

            gvData.FocusedRowHandle = e.HitInfo.RowHandle;
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            UpdateActionStates();

            itemViewIssue.Enabled = btnViewIssue.Enabled;
            itemUploadEvidence.Enabled = btnUploadEvidence.Enabled;
            itemViewEvidence.Enabled = btnViewEvidence.Enabled;
            itemUpdateTime.Enabled = btnUpdateTime.Enabled;
            itemConfirmComplete.Enabled = btnConfirmComplete.Enabled;
            itemCancelTicket.Enabled = btnCancelTicket.Enabled;

            itemViewIssue.BeginGroup = true;
            itemUploadEvidence.BeginGroup = true;
            itemViewEvidence.BeginGroup = false;
            itemUpdateTime.BeginGroup = false;
            itemConfirmComplete.BeginGroup = false;
            itemCancelTicket.BeginGroup = true;

            e.Menu.Items.Add(itemViewIssue);

            if (itemUploadEvidence.Enabled)
            {
                e.Menu.Items.Add(itemUploadEvidence);
            }

            if (itemViewEvidence.Enabled)
            {
                e.Menu.Items.Add(itemViewEvidence);
            }

            if (itemUpdateTime.Enabled)
            {
                e.Menu.Items.Add(itemUpdateTime);
            }

            if (itemConfirmComplete.Enabled)
            {
                e.Menu.Items.Add(itemConfirmComplete);
            }

            if (itemCancelTicket.Enabled)
            {
                e.Menu.Items.Add(itemCancelTicket);
            }
        }

        private void btnViewIssue_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            string message = $"回收單號: {row.TicketNo}\r\n" +
                $"新物料: {row.NewMaterialCode} / {row.NewMaterialDisplayName}\r\n" +
                $"舊物料: {row.OldBaseMaterialCode} / {row.OldBaseMaterialDisplayName}\r\n" +
                $"方式: {row.RecoveryOptionDisplay}\r\n" +
                $"數量: {row.Quantity}\r\n" +
                $"來源倉庫: {row.SourceStorageName}\r\n" +
                $"建立時間: {row.CreatedDate:yyyy/MM/dd HH:mm}\r\n" +
                $"備註: {row.Description}";

            XtraMessageBox.Show(message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDownloadGuide_ItemClick(object sender, ItemClickEventArgs e)
        {
            var guides = dt309_RecoveryBUS.Instance.GetGuideList();
            if (guides.Count == 0)
            {
                XtraMessageBox.Show("目前尚未上傳報廢指引。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Material309RecoveryHelper.OpenGuideFiles(guides);
        }

        private void btnManageGuide_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var form = new f309_RecoveryGuideMgmt())
            {
                form.ShowDialog();
            }
        }

        private void btnUploadEvidence_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            if (!dt309_RecoveryConst.IsScrap(row.RecoveryOption) ||
                string.Equals(row.Status, dt309_RecoveryConst.RecoveryStatusCancelled, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(row.Status, dt309_RecoveryConst.RecoveryStatusCompleted, StringComparison.OrdinalIgnoreCase))
            {
                MsgTP.MsgError("目前狀態不可上傳證明。");
                return;
            }

            using (var form = new f309_RecoveryEvidenceUpload(row.TicketNo, row.ActualDisposeDate, row.ResultNote))
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var evidenceItems = form.SelectedFiles.Select(file =>
                {
                    var saved = Material309RecoveryHelper.SaveEvidenceFile(row.Id, file);
                    return new dt309_RecoveryEvidence
                    {
                        RecoveryTicketId = row.Id,
                        ActualName = saved.actualName,
                        EncryptionName = saved.encryptionName,
                        FileExt = saved.extension,
                        UploadedBy = TPConfigs.LoginUser.Id,
                        UploadedDate = DateTime.Now,
                        IsActive = true
                    };
                }).ToList();

                if (!dt309_RecoveryBUS.Instance.UploadEvidence(
                    row.Id,
                    form.ActualDisposeDate,
                    form.ResultNote,
                    TPConfigs.LoginUser.Id,
                    evidenceItems,
                    out string message))
                {
                    MsgTP.MsgError(string.IsNullOrWhiteSpace(message) ? "上傳證明失敗。" : message);
                    return;
                }
            }

            LoadData();
        }

        private void btnViewEvidence_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            var evidences = dt309_RecoveryBUS.Instance.GetEvidenceListByTicketId(row.Id);
            if (evidences.Count == 0)
            {
                XtraMessageBox.Show("目前尚未上傳證明。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Material309RecoveryHelper.OpenEvidenceFiles(evidences);
        }

        private void btnUpdateTime_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            if (!dt309_RecoveryConst.IsScrap(row.RecoveryOption) ||
                !string.Equals(row.Status, dt309_RecoveryConst.RecoveryStatusScheduled, StringComparison.OrdinalIgnoreCase))
            {
                MsgTP.MsgError("只有已安排的報廢案件可更新日期。");
                return;
            }

            var availableUsers = users
                .Where(r => !string.IsNullOrWhiteSpace(r.IdDepartment) &&
                    (!string.IsNullOrWhiteSpace(row.IdDept)
                        ? r.IdDepartment.StartsWith(row.IdDept, StringComparison.OrdinalIgnoreCase)
                        : true))
                .ToList();

            using (var form = new f309_RecoverySchedule(availableUsers, row.AssignedUserId, row.PlannedDisposeDate))
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (!dt309_RecoveryBUS.Instance.UpdateSchedule(row.Id, form.AssignedUserId, form.PlannedDisposeDate, TPConfigs.LoginUser.Id, out string message))
                {
                    MsgTP.MsgError(string.IsNullOrWhiteSpace(message) ? "更新日期失敗。" : message);
                    return;
                }
            }

            LoadData();
        }

        private void btnConfirmComplete_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            if (!dt309_RecoveryBUS.Instance.ConfirmCompleted(row.Id, TPConfigs.LoginUser.Id, row.ResultNote, out string message))
            {
                MsgTP.MsgError(string.IsNullOrWhiteSpace(message) ? "確認完成失敗。" : message);
                return;
            }

            LoadData();
        }

        private void btnCancelTicket_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            string reason = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "請輸入取消原因",
                DefaultResponse = string.Empty,
                Editor = new TextEdit()
            })?.ToString();

            if (!dt309_RecoveryBUS.Instance.CancelTicket(row.Id, TPConfigs.LoginUser.Id, reason, out string message))
            {
                MsgTP.MsgError(string.IsNullOrWhiteSpace(message) ? "取消失敗。" : message);
                return;
            }

            LoadData();
        }
    }
}


