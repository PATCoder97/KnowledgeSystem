using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    public partial class uc313_FixedAssetMain
    {
        private List<dm_User> users = new List<dm_User>();
        private List<dm_Departments> departments = new List<dm_Departments>();
        private List<dm_Group> groups = new List<dm_Group>();
        private List<dm_GroupUser> currentUserGroupLinks = new List<dm_GroupUser>();

        private List<dt313_FixedAsset> fixedAssets = new List<dt313_FixedAsset>();
        private List<dt313_FixedAssetPhoto> fixedAssetPhotos = new List<dt313_FixedAssetPhoto>();
        private List<dt313_DepartmentSetting> departmentSettings = new List<dt313_DepartmentSetting>();
        private List<dt313_AbnormalCatalog> abnormalCatalogs = new List<dt313_AbnormalCatalog>();
        private List<dt313_InspectionBatch> inspectionBatches = new List<dt313_InspectionBatch>();
        private List<dt313_InspectionBatchAsset> inspectionBatchAssets = new List<dt313_InspectionBatchAsset>();
        private List<dt313_InspectionPhoto> inspectionPhotos = new List<dt313_InspectionPhoto>();

        private bool isManager313;
        private HashSet<string> accessibleDeptPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, dm_User> userMap = new Dictionary<string, dm_User>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, dm_Departments> departmentMap = new Dictionary<string, dm_Departments>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<int, dt313_AbnormalCatalog> abnormalCatalogMap = new Dictionary<int, dt313_AbnormalCatalog>();

        private void InitializeModule()
        {
            try
            {
                FixedAsset313Helper.EnsureBaseFolder();
                LoadLookupsAndPermissions();
                ReloadAllData();
                ApplyPermissions();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"固定資產模組初始化失敗\r\n{ex.Message}", TPConfigs.SoftNameTW,
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void LoadLookupsAndPermissions()
        {
            users = dm_UserBUS.Instance.GetList().OrderBy(r => r.IdDepartment).ThenBy(r => r.DisplayName).ToList();
            departments = dm_DeptBUS.Instance.GetList().OrderBy(r => r.Id).ToList();
            groups = dm_GroupBUS.Instance.GetList();
            currentUserGroupLinks = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);

            userMap = users.GroupBy(r => r.Id)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            departmentMap = departments.GroupBy(r => r.Id)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var handlerGroups = dm_GroupBUS.Instance.GetListByName(GroupHandlerName);
            var managerGroups = dm_GroupBUS.Instance.GetListByName(GroupManagerName);
            isManager313 = managerGroups.Any(group => currentUserGroupLinks.Any(link => link.IdGroup == group.Id));

            accessibleDeptPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var accessibleGroups = handlerGroups
                .Where(group => currentUserGroupLinks.Any(link => link.IdGroup == group.Id))
                .ToList();

            if (isManager313 || accessibleGroups.Any(r => r.IdDept == "7"))
            {
                foreach (var dept in departments.Where(r => !string.IsNullOrWhiteSpace(r.Id)))
                {
                    accessibleDeptPrefixes.Add(dept.Id.Trim());
                }
            }
            else
            {
                foreach (string dept in accessibleGroups.Where(r => !string.IsNullOrWhiteSpace(r.IdDept)).Select(r => r.IdDept.Trim()).Distinct())
                {
                    accessibleDeptPrefixes.Add(dept);
                }
            }

            UpdateScopeLabel();
        }

        private void UpdateScopeLabel()
        {
            string roleText = isManager313 ? GroupManagerName : GroupHandlerName;
            string scopeText;

            if (isManager313)
            {
                scopeText = "全部部門";
            }
            else if (accessibleDeptPrefixes.Count == 0)
            {
                scopeText = "本人負責資產";
            }
            else
            {
                scopeText = string.Join(", ", accessibleDeptPrefixes.OrderBy(r => r).Take(8));
                if (accessibleDeptPrefixes.Count > 8)
                {
                    scopeText += " ...";
                }
            }

            lblScope.Text = $"登入者: {TPConfigs.LoginUser.DisplayName} ({TPConfigs.LoginUser.Id})    權限: {roleText}    範圍: {scopeText}";
        }

        private void ReloadAllData()
        {
            UseWaitCursor = true;
            try
            {
                fixedAssets = dt313_FixedAssetBUS.Instance.GetAll().OrderBy(r => r.AssetCode).ToList();
                fixedAssetPhotos = dt313_FixedAssetPhotoBUS.Instance.GetList();
                departmentSettings = dt313_DepartmentSettingBUS.Instance.GetList();
                abnormalCatalogs = dt313_AbnormalCatalogBUS.Instance.GetList();
                inspectionBatches = dt313_InspectionBatchBUS.Instance.GetList();
                inspectionBatchAssets = dt313_InspectionBatchAssetBUS.Instance.GetList();
                inspectionPhotos = dt313_InspectionPhotoBUS.Instance.GetList();
                abnormalCatalogMap = abnormalCatalogs.GroupBy(r => r.Id).ToDictionary(g => g.Key, g => g.First());

                BindAssetGrid();
                BindBatchGrid();
                BindAbnormalGrid();
                BindSettingGrids();
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void ApplyPermissions()
        {
            btnAssetAdd.Visible = isManager313;
            btnAssetDelete.Visible = isManager313;
            btnAssetImport.Visible = isManager313;

            btnCreateMonthlyBatch.Visible = isManager313;
            btnCreateQuarterlyBatch.Visible = isManager313;
            btnCloseBatch.Visible = isManager313;

            btnDeptSettingAdd.Visible = isManager313;
            btnDeptSettingEdit.Visible = isManager313;
            btnCatalogAdd.Visible = isManager313;
            btnCatalogEdit.Visible = isManager313;

            if (!isManager313)
            {
                tabMain.TabPages.RemoveByKey("Settings");
            }
        }

        private void BindAssetGrid()
        {
            var rows = fixedAssets
                .Where(CanAccessAsset)
                .Where(r => !r.IsDeleted)
                .Select(asset =>
                {
                    var activePhotos = fixedAssetPhotos.Where(r => r.FixedAssetId == asset.Id && r.IsActive).ToList();
                    return new AssetGridRow
                    {
                        Entity = asset,
                        AssetCode = asset.AssetCode,
                        AssetNameTW = asset.AssetNameTW,
                        AssetNameVN = asset.AssetNameVN,
                        IdDept = asset.IdDept,
                        DeptName = GetDeptDisplay(asset.IdDept),
                        IdManager = asset.IdManager,
                        ManagerName = GetUserDisplay(asset.IdManager),
                        AssetCategory = asset.AssetCategory,
                        AssetCategoryDisplay = GetAssetCategoryDisplay(asset.AssetCategory),
                        TypeName = asset.TypeName,
                        Location = asset.Location,
                        BrandSpec = asset.BrandSpec,
                        Origin = asset.Origin,
                        AcquireDate = asset.AcquireDate,
                        Status = asset.Status,
                        Remarks = asset.Remarks,
                        LastMonthlyCheckDate = asset.LastMonthlyCheckDate,
                        LastQuarterlyAuditDate = asset.LastQuarterlyAuditDate,
                        PhotoCompletion = $"{activePhotos.Count} / 3",
                        HasCloseUp = activePhotos.Any(r => r.PhotoType == PhotoTypeCloseUp),
                        HasOverview = activePhotos.Any(r => r.PhotoType == PhotoTypeOverview),
                        HasInUse = activePhotos.Any(r => r.PhotoType == PhotoTypeInUse)
                    };
                })
                .OrderBy(r => r.AssetCode)
                .ToList();

            assetSource.DataSource = rows;
            gcAssets.RefreshDataSource();
            gvAssets.PopulateColumns();
            HideColumn(gvAssets, nameof(AssetGridRow.Entity));
            HideColumn(gvAssets, nameof(AssetGridRow.IdDept));
            HideColumn(gvAssets, nameof(AssetGridRow.IdManager));
            SetColumn(gvAssets, nameof(AssetGridRow.AssetCode), "資產編號", 120);
            SetColumn(gvAssets, nameof(AssetGridRow.AssetNameTW), "資產中文名稱", 200);
            SetColumn(gvAssets, nameof(AssetGridRow.AssetNameVN), "資產越文名稱", 220);
            SetColumn(gvAssets, nameof(AssetGridRow.DeptName), "部門", 160);
            SetColumn(gvAssets, nameof(AssetGridRow.ManagerName), "經辦", 150);
            SetColumn(gvAssets, nameof(AssetGridRow.AssetCategoryDisplay), "分類", 120);
            SetColumn(gvAssets, nameof(AssetGridRow.TypeName), "類別", 130);
            SetColumn(gvAssets, nameof(AssetGridRow.Location), "位置", 140);
            SetColumn(gvAssets, nameof(AssetGridRow.Status), "狀態", 100);
            SetColumn(gvAssets, nameof(AssetGridRow.PhotoCompletion), "照片完整度", 100);
            SetColumn(gvAssets, nameof(AssetGridRow.BrandSpec), "廠牌規格", 150);
            SetColumn(gvAssets, nameof(AssetGridRow.Origin), "產地", 120);
            SetColumn(gvAssets, nameof(AssetGridRow.Remarks), "備註", 220);
            SetDateColumn(gvAssets, nameof(AssetGridRow.AcquireDate), "取得日期", 100);
            SetDateColumn(gvAssets, nameof(AssetGridRow.LastMonthlyCheckDate), "上次月檢", 100);
            SetDateColumn(gvAssets, nameof(AssetGridRow.LastQuarterlyAuditDate), "上次季檢", 100);
            HideColumn(gvAssets, nameof(AssetGridRow.HasCloseUp));
            HideColumn(gvAssets, nameof(AssetGridRow.HasOverview));
            HideColumn(gvAssets, nameof(AssetGridRow.HasInUse));
            gvAssets.BestFitColumns();
        }

        private void BindBatchGrid()
        {
            var batchRows = inspectionBatches
                .Where(CanAccessBatch)
                .Select(batch =>
                {
                    var items = inspectionBatchAssets.Where(r => r.BatchId == batch.Id).ToList();
                    int completedCount = items.Count(r => r.Result != ResultPending);
                    int abnormalCount = items.Count(r => r.Result == ResultAbnormal);
                    return new BatchGridRow
                    {
                        Entity = batch,
                        BatchName = batch.BatchName,
                        BatchType = batch.BatchType,
                        BatchTypeDisplay = GetBatchTypeDisplay(batch.BatchType),
                        PeriodKey = batch.PeriodKey,
                        DeptName = GetDeptDisplay(batch.IdDept),
                        AssignedUserName = GetUserDisplay(batch.AssignedUserId),
                        SampleRate = batch.SampleRate,
                        TargetQty = batch.TargetQty,
                        CompletedQty = completedCount,
                        AbnormalQty = abnormalCount,
                        ProgressText = $"{completedCount} / {Math.Max(batch.TargetQty, items.Count)}",
                        Status = batch.Status,
                        CreatedDate = batch.CreatedDate,
                        ClosedDate = batch.ClosedDate
                    };
                })
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            batchSource.DataSource = batchRows;
            gcBatches.RefreshDataSource();
            gvBatches.PopulateColumns();
            HideColumn(gvBatches, nameof(BatchGridRow.Entity));
            HideColumn(gvBatches, nameof(BatchGridRow.BatchType));
            SetColumn(gvBatches, nameof(BatchGridRow.BatchName), "批次名稱", 240);
            SetColumn(gvBatches, nameof(BatchGridRow.BatchTypeDisplay), "類型", 110);
            SetColumn(gvBatches, nameof(BatchGridRow.PeriodKey), "期間", 90);
            SetColumn(gvBatches, nameof(BatchGridRow.DeptName), "部門", 150);
            SetColumn(gvBatches, nameof(BatchGridRow.AssignedUserName), "經辦", 150);
            SetColumn(gvBatches, nameof(BatchGridRow.SampleRate), "抽樣%", 70);
            SetColumn(gvBatches, nameof(BatchGridRow.TargetQty), "目標數", 70);
            SetColumn(gvBatches, nameof(BatchGridRow.CompletedQty), "已檢數", 70);
            SetColumn(gvBatches, nameof(BatchGridRow.AbnormalQty), "異常數", 70);
            SetColumn(gvBatches, nameof(BatchGridRow.ProgressText), "進度", 80);
            SetColumn(gvBatches, nameof(BatchGridRow.Status), "狀態", 80);
            SetDateColumn(gvBatches, nameof(BatchGridRow.CreatedDate), "建立日期", 120, "yyyy-MM-dd HH:mm");
            SetDateColumn(gvBatches, nameof(BatchGridRow.ClosedDate), "結案日期", 120, "yyyy-MM-dd HH:mm");
            gvBatches.BestFitColumns();

            BindBatchDetailGrid(GetFocusedBatch());
        }

        private void BindBatchDetailGrid(BatchGridRow batchRow)
        {
            var rows = new List<BatchDetailGridRow>();
            if (batchRow != null)
            {
                rows = inspectionBatchAssets
                    .Where(r => r.BatchId == batchRow.Entity.Id)
                    .Join(fixedAssets, detail => detail.FixedAssetId, asset => asset.Id, (detail, asset) => new { detail, asset })
                    .Where(r => CanAccessAsset(r.asset))
                    .Select(r =>
                    {
                        string abnormalName = r.detail.AbnormalId.HasValue && abnormalCatalogMap.ContainsKey(r.detail.AbnormalId.Value)
                            ? abnormalCatalogMap[r.detail.AbnormalId.Value].DisplayName
                            : string.Empty;
                        var photos = inspectionPhotos.Where(p => p.BatchAssetId == r.detail.Id).ToList();
                        return new BatchDetailGridRow
                        {
                            Entity = r.detail,
                            Batch = batchRow.Entity,
                            Asset = r.asset,
                            AssetCode = r.asset.AssetCode,
                            AssetNameTW = r.asset.AssetNameTW,
                            DeptName = GetDeptDisplay(r.asset.IdDept),
                            ManagerName = GetUserDisplay(r.asset.IdManager),
                            Result = r.detail.Result,
                            ResultDisplay = GetResultDisplay(r.detail.Result),
                            AbnormalName = abnormalName,
                            AbnormalNote = r.detail.AbnormalNote,
                            CheckedDate = r.detail.CheckedDate,
                            CorrectionDueDate = r.detail.CorrectionDueDate,
                            CorrectionStatus = r.detail.CorrectionStatus,
                            CorrectionStatusDisplay = GetCorrectionStatusDisplay(r.detail.CorrectionStatus, r.detail.CorrectionDueDate),
                            CorrectionNote = r.detail.CorrectionNote,
                            EvidencePhotoCount = photos.Count(p => p.PhotoPurpose == InspectionPhotoPurposeAbnormal),
                            CorrectionPhotoCount = photos.Count(p => p.PhotoPurpose == InspectionPhotoPurposeCorrection)
                        };
                    })
                    .OrderBy(r => r.AssetCode)
                    .ToList();
            }

            batchDetailSource.DataSource = rows;
            gcBatchDetails.RefreshDataSource();
            gvBatchDetails.PopulateColumns();
            HideColumn(gvBatchDetails, nameof(BatchDetailGridRow.Entity));
            HideColumn(gvBatchDetails, nameof(BatchDetailGridRow.Batch));
            HideColumn(gvBatchDetails, nameof(BatchDetailGridRow.Asset));
            HideColumn(gvBatchDetails, nameof(BatchDetailGridRow.Result));
            HideColumn(gvBatchDetails, nameof(BatchDetailGridRow.CorrectionStatus));
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.AssetCode), "資產編號", 120);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.AssetNameTW), "資產名稱", 180);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.DeptName), "部門", 140);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.ManagerName), "經辦", 140);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.ResultDisplay), "結果", 90);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.AbnormalName), "異常項目", 140);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.AbnormalNote), "異常說明", 200);
            SetDateColumn(gvBatchDetails, nameof(BatchDetailGridRow.CheckedDate), "檢查時間", 120, "yyyy-MM-dd HH:mm");
            SetDateColumn(gvBatchDetails, nameof(BatchDetailGridRow.CorrectionDueDate), "改善期限", 100);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.CorrectionStatusDisplay), "改善狀態", 100);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.CorrectionNote), "改善說明", 200);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.EvidencePhotoCount), "異常照片", 80);
            SetColumn(gvBatchDetails, nameof(BatchDetailGridRow.CorrectionPhotoCount), "改善照片", 80);
            gvBatchDetails.BestFitColumns();
        }

        private void BindAbnormalGrid()
        {
            var batchMap = inspectionBatches.ToDictionary(r => r.Id, r => r);
            var assetMap = fixedAssets.ToDictionary(r => r.Id, r => r);

            var rows = inspectionBatchAssets
                .Where(r => r.Result == ResultAbnormal)
                .Where(r => batchMap.ContainsKey(r.BatchId) && assetMap.ContainsKey(r.FixedAssetId))
                .Select(r =>
                {
                    var batch = batchMap[r.BatchId];
                    var asset = assetMap[r.FixedAssetId];
                    if (!CanAccessBatch(batch) && !CanAccessAsset(asset))
                    {
                        return null;
                    }

                    string abnormalName = r.AbnormalId.HasValue && abnormalCatalogMap.ContainsKey(r.AbnormalId.Value)
                        ? abnormalCatalogMap[r.AbnormalId.Value].DisplayName
                        : string.Empty;

                    return new AbnormalGridRow
                    {
                        Entity = r,
                        Batch = batch,
                        Asset = asset,
                        BatchName = batch.BatchName,
                        BatchTypeDisplay = GetBatchTypeDisplay(batch.BatchType),
                        PeriodKey = batch.PeriodKey,
                        AssetCode = asset.AssetCode,
                        AssetNameTW = asset.AssetNameTW,
                        DeptName = GetDeptDisplay(asset.IdDept),
                        ManagerName = GetUserDisplay(asset.IdManager),
                        AbnormalName = abnormalName,
                        AbnormalNote = r.AbnormalNote,
                        CheckedDate = r.CheckedDate,
                        CorrectionDueDate = r.CorrectionDueDate,
                        CorrectionStatus = r.CorrectionStatus,
                        CorrectionStatusDisplay = GetCorrectionStatusDisplay(r.CorrectionStatus, r.CorrectionDueDate),
                        CorrectionNote = r.CorrectionNote
                    };
                })
                .Where(r => r != null)
                .OrderByDescending(r => r.CheckedDate)
                .ThenBy(r => r.AssetCode)
                .ToList();

            abnormalSource.DataSource = rows;
            gcAbnormals.RefreshDataSource();
            gvAbnormals.PopulateColumns();
            HideColumn(gvAbnormals, nameof(AbnormalGridRow.Entity));
            HideColumn(gvAbnormals, nameof(AbnormalGridRow.Batch));
            HideColumn(gvAbnormals, nameof(AbnormalGridRow.Asset));
            HideColumn(gvAbnormals, nameof(AbnormalGridRow.CorrectionStatus));
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.BatchName), "批次名稱", 220);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.BatchTypeDisplay), "批次類型", 100);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.PeriodKey), "期間", 80);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.AssetCode), "資產編號", 120);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.AssetNameTW), "資產名稱", 180);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.DeptName), "部門", 140);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.ManagerName), "經辦", 140);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.AbnormalName), "異常項目", 140);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.AbnormalNote), "異常說明", 180);
            SetDateColumn(gvAbnormals, nameof(AbnormalGridRow.CheckedDate), "檢查時間", 120, "yyyy-MM-dd HH:mm");
            SetDateColumn(gvAbnormals, nameof(AbnormalGridRow.CorrectionDueDate), "改善期限", 100);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.CorrectionStatusDisplay), "改善狀態", 100);
            SetColumn(gvAbnormals, nameof(AbnormalGridRow.CorrectionNote), "改善說明", 180);
            gvAbnormals.BestFitColumns();
        }

        private void BindSettingGrids()
        {
            var deptRows = departmentSettings
                .Select(r => new DepartmentSettingGridRow
                {
                    Entity = r,
                    IdDept = r.IdDept,
                    DeptName = GetDeptDisplay(r.IdDept),
                    QuarterlySampleRate = r.QuarterlySampleRate,
                    IsActive = r.IsActive,
                    UpdatedBy = r.UpdatedBy,
                    UpdatedDate = r.UpdatedDate
                })
                .OrderBy(r => r.IdDept)
                .ToList();

            deptSettingSource.DataSource = deptRows;
            gcDeptSettings.RefreshDataSource();
            gvDeptSettings.PopulateColumns();
            HideColumn(gvDeptSettings, nameof(DepartmentSettingGridRow.Entity));
            SetColumn(gvDeptSettings, nameof(DepartmentSettingGridRow.IdDept), "部門編號", 100);
            SetColumn(gvDeptSettings, nameof(DepartmentSettingGridRow.DeptName), "部門名稱", 180);
            SetColumn(gvDeptSettings, nameof(DepartmentSettingGridRow.QuarterlySampleRate), "抽樣率%", 80);
            SetColumn(gvDeptSettings, nameof(DepartmentSettingGridRow.IsActive), "啟用", 60);
            SetColumn(gvDeptSettings, nameof(DepartmentSettingGridRow.UpdatedBy), "更新者", 100);
            SetDateColumn(gvDeptSettings, nameof(DepartmentSettingGridRow.UpdatedDate), "更新時間", 120, "yyyy-MM-dd HH:mm");
            gvDeptSettings.BestFitColumns();

            var catalogRows = abnormalCatalogs
                .Select(r => new AbnormalCatalogGridRow
                {
                    Entity = r,
                    Code = r.Code,
                    DisplayName = r.DisplayName,
                    SortOrder = r.SortOrder,
                    IsActive = r.IsActive,
                    Remarks = r.Remarks,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = r.CreatedDate
                })
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.DisplayName)
                .ToList();

            abnormalCatalogSource.DataSource = catalogRows;
            gcAbnormalCatalogs.RefreshDataSource();
            gvAbnormalCatalogs.PopulateColumns();
            HideColumn(gvAbnormalCatalogs, nameof(AbnormalCatalogGridRow.Entity));
            SetColumn(gvAbnormalCatalogs, nameof(AbnormalCatalogGridRow.Code), "代碼", 100);
            SetColumn(gvAbnormalCatalogs, nameof(AbnormalCatalogGridRow.DisplayName), "異常名稱", 180);
            SetColumn(gvAbnormalCatalogs, nameof(AbnormalCatalogGridRow.SortOrder), "排序", 70);
            SetColumn(gvAbnormalCatalogs, nameof(AbnormalCatalogGridRow.IsActive), "啟用", 60);
            SetColumn(gvAbnormalCatalogs, nameof(AbnormalCatalogGridRow.Remarks), "備註", 220);
            SetColumn(gvAbnormalCatalogs, nameof(AbnormalCatalogGridRow.CreatedBy), "建立者", 90);
            SetDateColumn(gvAbnormalCatalogs, nameof(AbnormalCatalogGridRow.CreatedDate), "建立時間", 120, "yyyy-MM-dd HH:mm");
            gvAbnormalCatalogs.BestFitColumns();
        }

        private void HideColumn(GridView view, string fieldName)
        {
            var column = view.Columns.ColumnByFieldName(fieldName);
            if (column != null)
            {
                column.Visible = false;
            }
        }

        private void SetColumn(GridView view, string fieldName, string caption, int width)
        {
            GridColumn column = view.Columns.ColumnByFieldName(fieldName);
            if (column == null) return;
            column.Caption = caption;
            column.Width = width;
            column.Visible = true;
        }

        private void SetDateColumn(GridView view, string fieldName, string caption, int width, string format = "yyyy-MM-dd")
        {
            SetColumn(view, fieldName, caption, width);
            GridColumn column = view.Columns.ColumnByFieldName(fieldName);
            if (column == null) return;
            column.DisplayFormat.FormatType = FormatType.DateTime;
            column.DisplayFormat.FormatString = format;
        }

        private AssetGridRow GetFocusedAsset() => gvAssets.GetFocusedRow() as AssetGridRow;
        private BatchGridRow GetFocusedBatch() => gvBatches.GetFocusedRow() as BatchGridRow;
        private BatchDetailGridRow GetFocusedBatchDetail() => gvBatchDetails.GetFocusedRow() as BatchDetailGridRow;
        private AbnormalGridRow GetFocusedAbnormal() => gvAbnormals.GetFocusedRow() as AbnormalGridRow;
        private DepartmentSettingGridRow GetFocusedDeptSetting() => gvDeptSettings.GetFocusedRow() as DepartmentSettingGridRow;
        private AbnormalCatalogGridRow GetFocusedAbnormalCatalog() => gvAbnormalCatalogs.GetFocusedRow() as AbnormalCatalogGridRow;

        private bool CanAccessAsset(dt313_FixedAsset asset)
        {
            if (asset == null) return false;
            if (isManager313) return true;
            if (!string.IsNullOrWhiteSpace(asset.IdManager) &&
                string.Equals(asset.IdManager, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsDeptAccessible(asset.IdDept);
        }

        private bool CanAccessBatch(dt313_InspectionBatch batch)
        {
            if (batch == null) return false;
            if (isManager313) return true;

            if (batch.BatchType == BatchTypeMonthly &&
                !string.IsNullOrWhiteSpace(batch.AssignedUserId) &&
                string.Equals(batch.AssignedUserId, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsDeptAccessible(batch.IdDept);
        }

        private bool IsDeptAccessible(string idDept)
        {
            if (string.IsNullOrWhiteSpace(idDept)) return false;
            if (isManager313) return true;
            return accessibleDeptPrefixes.Any(prefix => idDept.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        private bool CanEditAsset(dt313_FixedAsset asset)
        {
            return asset != null && CanAccessAsset(asset);
        }

        private bool CanEditBatchResult(BatchDetailGridRow row)
        {
            if (row == null) return false;
            if (isManager313) return row.Batch.Status != "Closed";

            if (row.Batch.BatchType == BatchTypeMonthly)
            {
                return row.Batch.Status != "Closed" &&
                    string.Equals(row.Batch.AssignedUserId, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase);
            }

            return row.Batch.Status != "Closed" && (IsDeptAccessible(row.Asset.IdDept) || IsDeptAccessible(row.Batch.IdDept));
        }

        private bool CanUpdateCorrection(BatchDetailGridRow row)
        {
            if (row == null || row.Entity.Result != ResultAbnormal) return false;
            if (isManager313) return true;
            if (!string.IsNullOrWhiteSpace(row.Asset.IdManager) &&
                string.Equals(row.Asset.IdManager, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsDeptAccessible(row.Asset.IdDept);
        }

        private bool CanUpdateCorrection(AbnormalGridRow row)
        {
            if (row == null) return false;
            if (isManager313) return true;
            if (!string.IsNullOrWhiteSpace(row.Asset.IdManager) &&
                string.Equals(row.Asset.IdManager, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsDeptAccessible(row.Asset.IdDept);
        }

        private string NormalizeDeptId(string deptId)
        {
            if (string.IsNullOrWhiteSpace(deptId)) return string.Empty;
            string value = deptId.Trim();
            if (departmentMap.ContainsKey(value)) return value;
            return value.Length >= 4 ? value.Substring(0, 4) : value;
        }

        private string GetDeptDisplay(string idDept)
        {
            if (string.IsNullOrWhiteSpace(idDept)) return string.Empty;
            if (departmentMap.ContainsKey(idDept))
            {
                return $"{idDept} {departmentMap[idDept].DisplayName}";
            }

            return idDept;
        }

        private string GetUserDisplay(string idUser)
        {
            if (string.IsNullOrWhiteSpace(idUser)) return string.Empty;
            if (userMap.ContainsKey(idUser))
            {
                return $"{idUser} {userMap[idUser].DisplayName}";
            }

            return idUser;
        }

        private string GetAssetCategoryDisplay(string category)
        {
            return string.Equals(category, "DutyFreeImported", StringComparison.OrdinalIgnoreCase)
                ? "免稅進口設備"
                : "一般設備";
        }

        private string GetBatchTypeDisplay(string batchType)
        {
            if (batchType == BatchTypeMonthly) return "每月自檢";
            if (batchType == BatchTypeQuarterly) return "季度稽核";
            return batchType;
        }

        private string GetResultDisplay(string result)
        {
            if (result == ResultNormal) return "正常";
            if (result == ResultAbnormal) return "異常";
            return "待處理";
        }

        private string GetCorrectionStatusDisplay(string status, DateTime? dueDate)
        {
            if (status == CorrectionClosed) return "已完成";
            if (dueDate.HasValue && DateTime.Today > dueDate.Value.Date) return "逾期";
            if (status == CorrectionOpen || !string.IsNullOrWhiteSpace(status)) return "處理中";
            return string.Empty;
        }

        private void UpdateAssetLastCheck(dt313_FixedAsset asset, dt313_InspectionBatch batch, DateTime checkedDate)
        {
            asset.UpdatedBy = TPConfigs.LoginUser.Id;
            asset.UpdatedDate = DateTime.Now;
            if (batch.BatchType == BatchTypeMonthly)
            {
                asset.LastMonthlyCheckDate = checkedDate;
            }
            else if (batch.BatchType == BatchTypeQuarterly)
            {
                asset.LastQuarterlyAuditDate = checkedDate;
            }
        }

        private static void OpenPhotoFile(string physicalPath, string actualName)
        {
            if (!System.IO.File.Exists(physicalPath))
            {
                XtraMessageBox.Show("找不到對應的照片檔案。", TPConfigs.SoftNameTW,
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                return;
            }

            string tempFile = FixedAsset313Helper.CopyToTemp(physicalPath, actualName);
            using (var form = new Views._00_Generals.f00_VIewFile(tempFile, true, false))
            {
                form.ShowDialog();
            }
        }
    }
}
