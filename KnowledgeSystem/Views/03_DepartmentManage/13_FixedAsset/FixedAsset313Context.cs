using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal sealed class FixedAsset313Context
    {
        private Dictionary<string, dm_User> userMap = new Dictionary<string, dm_User>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, dm_Departments> departmentMap = new Dictionary<string, dm_Departments>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<int, dt313_AbnormalCatalog> abnormalCatalogMap = new Dictionary<int, dt313_AbnormalCatalog>();

        public List<dm_User> Users { get; private set; } = new List<dm_User>();
        public List<dm_Departments> Departments { get; private set; } = new List<dm_Departments>();
        public List<dm_GroupUser> CurrentUserGroupLinks { get; private set; } = new List<dm_GroupUser>();

        public List<dt313_FixedAsset> FixedAssets { get; private set; } = new List<dt313_FixedAsset>();
        public List<dt313_FixedAssetPhoto> FixedAssetPhotos { get; private set; } = new List<dt313_FixedAssetPhoto>();
        public List<dt313_DepartmentSetting> DepartmentSettings { get; private set; } = new List<dt313_DepartmentSetting>();
        public List<dt313_AbnormalCatalog> AbnormalCatalogs { get; private set; } = new List<dt313_AbnormalCatalog>();
        public List<dt313_InspectionBatch> InspectionBatches { get; private set; } = new List<dt313_InspectionBatch>();
        public List<dt313_InspectionBatchAsset> InspectionBatchAssets { get; private set; } = new List<dt313_InspectionBatchAsset>();
        public List<dt313_InspectionPhoto> InspectionPhotos { get; private set; } = new List<dt313_InspectionPhoto>();

        public bool IsManager313 { get; private set; }
        public HashSet<string> AccessibleDeptPrefixes { get; private set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public string ScopeSummary { get; private set; } = string.Empty;

        public void Initialize()
        {
            FixedAsset313Helper.EnsureBaseFolder();
            LoadLookupsAndPermissions();
            ReloadAllData();
        }

        public void ReloadAllData()
        {
            FixedAssets = dt313_FixedAssetBUS.Instance.GetAll().OrderBy(r => r.AssetCode).ToList();
            FixedAssetPhotos = dt313_FixedAssetPhotoBUS.Instance.GetList();
            DepartmentSettings = dt313_DepartmentSettingBUS.Instance.GetList();
            AbnormalCatalogs = dt313_AbnormalCatalogBUS.Instance.GetList();
            InspectionBatches = dt313_InspectionBatchBUS.Instance.GetList();
            InspectionBatchAssets = dt313_InspectionBatchAssetBUS.Instance.GetList();
            InspectionPhotos = dt313_InspectionPhotoBUS.Instance.GetList();
            abnormalCatalogMap = AbnormalCatalogs
                .GroupBy(r => r.Id)
                .ToDictionary(g => g.Key, g => g.First());
        }

        private void LoadLookupsAndPermissions()
        {
            Users = dm_UserBUS.Instance.GetList().OrderBy(r => r.IdDepartment).ThenBy(r => r.DisplayName).ToList();
            Departments = dm_DeptBUS.Instance.GetList().OrderBy(r => r.Id).ToList();
            CurrentUserGroupLinks = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);

            userMap = Users.GroupBy(r => r.Id)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            departmentMap = Departments.GroupBy(r => r.Id)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var handlerGroups = dm_GroupBUS.Instance.GetListByName(FixedAsset313Const.GroupHandlerName);
            var managerGroups = dm_GroupBUS.Instance.GetListByName(FixedAsset313Const.GroupManagerName);

            IsManager313 = managerGroups.Any(group => CurrentUserGroupLinks.Any(link => link.IdGroup == group.Id));

            AccessibleDeptPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var accessibleGroups = handlerGroups
                .Where(group => CurrentUserGroupLinks.Any(link => link.IdGroup == group.Id))
                .ToList();

            if (IsManager313 || accessibleGroups.Any(r => r.IdDept == "7"))
            {
                foreach (var dept in Departments.Where(r => !string.IsNullOrWhiteSpace(r.Id)))
                {
                    AccessibleDeptPrefixes.Add(dept.Id.Trim());
                }
            }
            else
            {
                foreach (string dept in accessibleGroups.Where(r => !string.IsNullOrWhiteSpace(r.IdDept)).Select(r => r.IdDept.Trim()).Distinct())
                {
                    AccessibleDeptPrefixes.Add(dept);
                }
            }

            ScopeSummary = BuildScopeSummary();
        }

        private string BuildScopeSummary()
        {
            if (IsManager313)
            {
                return $"{TPConfigs.LoginUser.DisplayName} / {FixedAsset313Const.GroupManagerName} / 全部部門";
            }

            if (AccessibleDeptPrefixes.Count == 0)
            {
                return $"{TPConfigs.LoginUser.DisplayName} / {FixedAsset313Const.GroupHandlerName} / 本人負責資產";
            }

            string scope = string.Join(", ", AccessibleDeptPrefixes.OrderBy(r => r).Take(8));
            if (AccessibleDeptPrefixes.Count > 8)
            {
                scope += " ...";
            }

            return $"{TPConfigs.LoginUser.DisplayName} / {FixedAsset313Const.GroupHandlerName} / {scope}";
        }

        public List<AssetGridRow> BuildAssetRows()
        {
            return FixedAssets
                .Where(CanAccessAsset)
                .Where(r => !r.IsDeleted)
                .Select(asset =>
                {
                    var activePhotos = FixedAssetPhotos.Where(r => r.FixedAssetId == asset.Id && r.IsActive).ToList();
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
                        HasCloseUp = activePhotos.Any(r => r.PhotoType == FixedAsset313Const.PhotoTypeCloseUp),
                        HasOverview = activePhotos.Any(r => r.PhotoType == FixedAsset313Const.PhotoTypeOverview),
                        HasInUse = activePhotos.Any(r => r.PhotoType == FixedAsset313Const.PhotoTypeInUse)
                    };
                })
                .OrderBy(r => r.AssetCode)
                .ToList();
        }

        public List<BatchGridRow> BuildBatchRows()
        {
            return InspectionBatches
                .Where(CanAccessBatch)
                .Select(batch =>
                {
                    var details = BuildBatchDetailRows(batch);
                    int completedCount = details.Count(r => r.Result != FixedAsset313Const.ResultPending);
                    int abnormalCount = details.Count(r => r.Result == FixedAsset313Const.ResultAbnormal);

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
                        ProgressText = $"{completedCount} / {Math.Max(batch.TargetQty, details.Count)}",
                        Status = batch.Status,
                        CreatedDate = batch.CreatedDate,
                        ClosedDate = batch.ClosedDate,
                        Details = details
                    };
                })
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public List<BatchDetailGridRow> BuildBatchDetailRows(dt313_InspectionBatch batch)
        {
            if (batch == null)
            {
                return new List<BatchDetailGridRow>();
            }

            return InspectionBatchAssets
                .Where(r => r.BatchId == batch.Id)
                .Join(FixedAssets, detail => detail.FixedAssetId, asset => asset.Id, (detail, asset) => new { detail, asset })
                .Where(r => CanAccessAsset(r.asset))
                .Select(r =>
                {
                    string abnormalName = r.detail.AbnormalId.HasValue && abnormalCatalogMap.ContainsKey(r.detail.AbnormalId.Value)
                        ? abnormalCatalogMap[r.detail.AbnormalId.Value].DisplayName
                        : string.Empty;

                    var photos = InspectionPhotos.Where(p => p.BatchAssetId == r.detail.Id).ToList();
                    return new BatchDetailGridRow
                    {
                        Entity = r.detail,
                        Batch = batch,
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
                        EvidencePhotoCount = photos.Count(p => p.PhotoPurpose == FixedAsset313Const.InspectionPhotoPurposeAbnormal),
                        CorrectionPhotoCount = photos.Count(p => p.PhotoPurpose == FixedAsset313Const.InspectionPhotoPurposeCorrection)
                    };
                })
                .OrderBy(r => r.AssetCode)
                .ToList();
        }

        public List<AbnormalGridRow> BuildAbnormalRows()
        {
            var batchMap = InspectionBatches.ToDictionary(r => r.Id, r => r);
            var assetMap = FixedAssets.ToDictionary(r => r.Id, r => r);

            return InspectionBatchAssets
                .Where(r => r.Result == FixedAsset313Const.ResultAbnormal)
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
        }

        public List<DepartmentSettingGridRow> BuildDepartmentSettingRows()
        {
            return DepartmentSettings
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
        }

        public List<AbnormalCatalogGridRow> BuildAbnormalCatalogRows()
        {
            return AbnormalCatalogs
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
        }

        public List<LookupItem> GetDepartmentLookupItems(bool includeAll = false)
        {
            var items = Departments
                .Where(r => !string.IsNullOrWhiteSpace(r.Id))
                .Select(r => new LookupItem(r.Id, $"{r.Id} {r.DisplayName}"))
                .OrderBy(r => r.Display)
                .ToList();

            if (includeAll || IsManager313)
            {
                return items;
            }

            return items.Where(r => IsDeptAccessible(r.Value)).ToList();
        }

        public List<LookupItem> GetUserLookupItems(bool includeAll = false)
        {
            IEnumerable<dm_User> query = Users;
            if (!includeAll && !IsManager313)
            {
                query = query.Where(r =>
                    string.Equals(r.Id, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase) ||
                    IsDeptAccessible(NormalizeDeptId(r.IdDepartment)));
            }

            var items = query
                .Select(r => new LookupItem(r.Id, $"{r.Id} {r.DisplayName}"))
                .OrderBy(r => r.Display)
                .ToList();

            items.Insert(0, new LookupItem(string.Empty, string.Empty));
            return items;
        }

        public List<dt313_FixedAssetPhoto> GetAssetPhotos(int fixedAssetId)
        {
            return FixedAssetPhotos
                .Where(r => r.FixedAssetId == fixedAssetId)
                .OrderBy(r => r.PhotoType)
                .ThenByDescending(r => r.UploadedDate)
                .ToList();
        }

        public List<dt313_InspectionPhoto> GetInspectionPhotos(int batchAssetId)
        {
            return InspectionPhotos
                .Where(r => r.BatchAssetId == batchAssetId)
                .OrderBy(r => r.PhotoPurpose)
                .ThenBy(r => r.DisplayOrder)
                .ToList();
        }

        public List<dt313_AbnormalCatalog> GetActiveAbnormalCatalogs()
        {
            return AbnormalCatalogs.Where(r => r.IsActive).OrderBy(r => r.SortOrder).ThenBy(r => r.DisplayName).ToList();
        }

        public bool CanAccessAsset(dt313_FixedAsset asset)
        {
            if (asset == null)
            {
                return false;
            }

            if (IsManager313)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(asset.IdManager) &&
                string.Equals(asset.IdManager, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsDeptAccessible(asset.IdDept);
        }

        public bool CanAccessBatch(dt313_InspectionBatch batch)
        {
            if (batch == null)
            {
                return false;
            }

            if (IsManager313)
            {
                return true;
            }

            if (batch.BatchType == FixedAsset313Const.BatchTypeMonthly &&
                !string.IsNullOrWhiteSpace(batch.AssignedUserId) &&
                string.Equals(batch.AssignedUserId, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsDeptAccessible(batch.IdDept);
        }

        public bool CanEditAsset(dt313_FixedAsset asset)
        {
            return asset != null && CanAccessAsset(asset);
        }

        public bool CanEditBatchResult(BatchDetailGridRow row)
        {
            if (row == null)
            {
                return false;
            }

            if (IsManager313)
            {
                return row.Batch.Status != "Closed";
            }

            if (row.Batch.BatchType == FixedAsset313Const.BatchTypeMonthly)
            {
                return row.Batch.Status != "Closed" &&
                    string.Equals(row.Batch.AssignedUserId, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase);
            }

            return row.Batch.Status != "Closed" && (IsDeptAccessible(row.Asset.IdDept) || IsDeptAccessible(row.Batch.IdDept));
        }

        public bool CanUpdateCorrection(BatchDetailGridRow row)
        {
            if (row == null || row.Entity.Result != FixedAsset313Const.ResultAbnormal)
            {
                return false;
            }

            if (IsManager313)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(row.Asset.IdManager) &&
                string.Equals(row.Asset.IdManager, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsDeptAccessible(row.Asset.IdDept);
        }

        public bool CanUpdateCorrection(AbnormalGridRow row)
        {
            if (row == null)
            {
                return false;
            }

            if (IsManager313)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(row.Asset.IdManager) &&
                string.Equals(row.Asset.IdManager, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IsDeptAccessible(row.Asset.IdDept);
        }

        public bool IsDeptAccessible(string idDept)
        {
            if (string.IsNullOrWhiteSpace(idDept))
            {
                return false;
            }

            if (IsManager313)
            {
                return true;
            }

            return AccessibleDeptPrefixes.Any(prefix => idDept.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        public string NormalizeDeptId(string deptId)
        {
            if (string.IsNullOrWhiteSpace(deptId))
            {
                return string.Empty;
            }

            string value = deptId.Trim();
            if (departmentMap.ContainsKey(value))
            {
                return value;
            }

            return value.Length >= 4 ? value.Substring(0, 4) : value;
        }

        public string GetDeptDisplay(string idDept)
        {
            if (string.IsNullOrWhiteSpace(idDept))
            {
                return string.Empty;
            }

            if (departmentMap.ContainsKey(idDept))
            {
                return $"{idDept} {departmentMap[idDept].DisplayName}";
            }

            return idDept;
        }

        public string GetUserDisplay(string idUser)
        {
            if (string.IsNullOrWhiteSpace(idUser))
            {
                return string.Empty;
            }

            if (userMap.ContainsKey(idUser))
            {
                return $"{idUser} {userMap[idUser].DisplayName}";
            }

            return idUser;
        }

        public string GetAssetCategoryDisplay(string category)
        {
            return string.Equals(category, "DutyFreeImported", StringComparison.OrdinalIgnoreCase)
                ? "免稅進口設備"
                : "一般設備";
        }

        public string GetBatchTypeDisplay(string batchType)
        {
            if (batchType == FixedAsset313Const.BatchTypeMonthly)
            {
                return "每月自檢";
            }

            if (batchType == FixedAsset313Const.BatchTypeQuarterly)
            {
                return "季度稽核";
            }

            return batchType;
        }

        public string GetResultDisplay(string result)
        {
            if (result == FixedAsset313Const.ResultNormal)
            {
                return "正常";
            }

            if (result == FixedAsset313Const.ResultAbnormal)
            {
                return "異常";
            }

            return "待處理";
        }

        public string GetCorrectionStatusDisplay(string status, DateTime? dueDate)
        {
            if (status == FixedAsset313Const.CorrectionClosed)
            {
                return "已完成";
            }

            if (dueDate.HasValue && DateTime.Today > dueDate.Value.Date)
            {
                return "逾期";
            }

            if (status == FixedAsset313Const.CorrectionOpen || !string.IsNullOrWhiteSpace(status))
            {
                return "處理中";
            }

            return string.Empty;
        }

        public string NormalizeCategory(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return "General";
            }

            string value = raw.Trim();
            if (value.IndexOf("免", StringComparison.OrdinalIgnoreCase) >= 0 ||
                value.IndexOf("duty", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "DutyFreeImported";
            }

            return "General";
        }

        public string ResolveDeptId(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            string value = raw.Trim();
            if (departmentMap.ContainsKey(value))
            {
                return value;
            }

            string token = value.Split(new[] { ' ', '-', '\t' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(token) && departmentMap.ContainsKey(token))
            {
                return token;
            }

            var match = Departments.FirstOrDefault(r =>
                string.Equals(r.DisplayName, value, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.DisplayNameVN, value, StringComparison.OrdinalIgnoreCase));

            return match?.Id ?? string.Empty;
        }

        public string ResolveUserId(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            string value = raw.Trim();
            if (userMap.ContainsKey(value))
            {
                return value;
            }

            string token = value.Split(new[] { ' ', '-', '\t' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(token) && userMap.ContainsKey(token))
            {
                return token;
            }

            var match = Users.FirstOrDefault(r =>
                string.Equals(r.DisplayName, value, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.DisplayNameVN, value, StringComparison.OrdinalIgnoreCase));

            return match?.Id ?? string.Empty;
        }

        public dt313_FixedAsset CloneAsset(dt313_FixedAsset asset)
        {
            if (asset == null)
            {
                return null;
            }

            return new dt313_FixedAsset
            {
                Id = asset.Id,
                AssetCode = asset.AssetCode,
                AssetNameTW = asset.AssetNameTW,
                AssetNameVN = asset.AssetNameVN,
                IdDept = asset.IdDept,
                IdManager = asset.IdManager,
                AssetCategory = asset.AssetCategory,
                TypeName = asset.TypeName,
                Location = asset.Location,
                BrandSpec = asset.BrandSpec,
                Origin = asset.Origin,
                AcquireDate = asset.AcquireDate,
                Status = asset.Status,
                Remarks = asset.Remarks,
                LastMonthlyCheckDate = asset.LastMonthlyCheckDate,
                LastQuarterlyAuditDate = asset.LastQuarterlyAuditDate,
                IsDeleted = asset.IsDeleted,
                DeletedBy = asset.DeletedBy,
                DeletedDate = asset.DeletedDate,
                CreatedBy = asset.CreatedBy,
                CreatedDate = asset.CreatedDate,
                UpdatedBy = asset.UpdatedBy,
                UpdatedDate = asset.UpdatedDate
            };
        }

        public dt313_DepartmentSetting CloneDepartmentSetting(dt313_DepartmentSetting setting)
        {
            if (setting == null)
            {
                return null;
            }

            return new dt313_DepartmentSetting
            {
                Id = setting.Id,
                IdDept = setting.IdDept,
                QuarterlySampleRate = setting.QuarterlySampleRate,
                IsActive = setting.IsActive,
                UpdatedBy = setting.UpdatedBy,
                UpdatedDate = setting.UpdatedDate
            };
        }

        public dt313_AbnormalCatalog CloneAbnormalCatalog(dt313_AbnormalCatalog catalog)
        {
            if (catalog == null)
            {
                return null;
            }

            return new dt313_AbnormalCatalog
            {
                Id = catalog.Id,
                Code = catalog.Code,
                DisplayName = catalog.DisplayName,
                SortOrder = catalog.SortOrder,
                IsActive = catalog.IsActive,
                Remarks = catalog.Remarks,
                CreatedBy = catalog.CreatedBy,
                CreatedDate = catalog.CreatedDate
            };
        }

        public bool ImportAssetsFromExcel()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Excel Files|*.xls;*.xlsx";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }

                var parsedRows = new List<dt313_FixedAsset>();
                var errors = new List<string>();
                var duplicateCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    var table = ReadFirstWorksheet(dialog.FileName);
                    int rowIndex = 1;
                    foreach (DataRow row in table.Rows)
                    {
                        rowIndex++;
                        if (IsRowEmpty(row))
                        {
                            continue;
                        }

                        string assetCode = ReadCell(table, row, "AssetCode", "資產編號", "資產编号", "編碼", "Code");
                        string assetNameTw = ReadCell(table, row, "AssetNameTW", "資產中文名稱", "中文名稱");
                        string assetNameVn = ReadCell(table, row, "AssetNameVN", "資產越文名稱", "越文名稱");
                        string deptRaw = ReadCell(table, row, "IdDept", "Dept", "使用部門", "部門");
                        string managerRaw = ReadCell(table, row, "IdManager", "Manager", "管理員", "經辦");
                        string categoryRaw = ReadCell(table, row, "AssetCategory", "Category", "資產分類", "分類");
                        string typeName = ReadCell(table, row, "TypeName", "資產類別", "類別");
                        string location = ReadCell(table, row, "Location", "位置");
                        string brandSpec = ReadCell(table, row, "BrandSpec", "廠牌規格", "規格");
                        string origin = ReadCell(table, row, "Origin", "製造產地", "產地");
                        string acquireRaw = ReadCell(table, row, "AcquireDate", "取得日期");
                        string status = ReadCell(table, row, "Status", "狀態");
                        string remarks = ReadCell(table, row, "Remarks", "備註");

                        if (string.IsNullOrWhiteSpace(assetCode))
                        {
                            errors.Add($"Row {rowIndex}: AssetCode is required.");
                            continue;
                        }

                        if (!duplicateCheck.Add(assetCode.Trim()))
                        {
                            errors.Add($"Row {rowIndex}: duplicated AssetCode {assetCode}.");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(assetNameTw))
                        {
                            errors.Add($"Row {rowIndex}: AssetNameTW is required.");
                            continue;
                        }

                        string deptId = ResolveDeptId(deptRaw);
                        if (string.IsNullOrWhiteSpace(deptId))
                        {
                            errors.Add($"Row {rowIndex}: invalid department {deptRaw}.");
                            continue;
                        }

                        string managerId = ResolveUserId(managerRaw);
                        if (!string.IsNullOrWhiteSpace(managerRaw) && string.IsNullOrWhiteSpace(managerId))
                        {
                            errors.Add($"Row {rowIndex}: invalid manager {managerRaw}.");
                            continue;
                        }

                        DateTime acquireDateValue;
                        DateTime? acquireDate = DateTime.TryParse(acquireRaw, out acquireDateValue)
                            ? acquireDateValue
                            : (DateTime?)null;

                        var existing = FixedAssets.FirstOrDefault(r => string.Equals(r.AssetCode, assetCode.Trim(), StringComparison.OrdinalIgnoreCase));
                        var item = existing != null ? CloneAsset(existing) : new dt313_FixedAsset
                        {
                            CreatedBy = TPConfigs.LoginUser.Id,
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        };

                        item.AssetCode = assetCode.Trim();
                        item.AssetNameTW = assetNameTw.Trim();
                        item.AssetNameVN = assetNameVn?.Trim();
                        item.IdDept = deptId;
                        item.IdManager = managerId;
                        item.AssetCategory = NormalizeCategory(categoryRaw);
                        item.TypeName = typeName?.Trim();
                        item.Location = location?.Trim();
                        item.BrandSpec = brandSpec?.Trim();
                        item.Origin = origin?.Trim();
                        item.AcquireDate = acquireDate;
                        item.Status = string.IsNullOrWhiteSpace(status) ? "Active" : status.Trim();
                        item.Remarks = remarks?.Trim();
                        item.UpdatedBy = TPConfigs.LoginUser.Id;
                        item.UpdatedDate = DateTime.Now;

                        parsedRows.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Excel 讀取失敗\r\n{ex.Message}", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (errors.Count > 0)
                {
                    string errorText = string.Join(Environment.NewLine, errors.Take(20));
                    if (errors.Count > 20)
                    {
                        errorText += $"{Environment.NewLine}...";
                    }

                    XtraMessageBox.Show(errorText, TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (parsedRows.Count == 0)
                {
                    XtraMessageBox.Show("Excel 中沒有可匯入的資料。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (XtraMessageBox.Show($"即將匯入/更新 {parsedRows.Count} 筆資產資料，是否繼續？", TPConfigs.SoftNameTW,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return false;
                }

                foreach (var item in parsedRows)
                {
                    int id = dt313_FixedAssetBUS.Instance.AddOrUpdateByCode(item);
                    if (id <= 0)
                    {
                        XtraMessageBox.Show($"匯入失敗: {item.AssetCode}", TPConfigs.SoftNameTW,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                XtraMessageBox.Show("匯入完成。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
        }

        public void ExportGrid(GridControl grid, string filePrefix)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel File|*.xlsx";
                dialog.FileName = $"{filePrefix}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                grid.ExportToXlsx(dialog.FileName);
                Process.Start(dialog.FileName);
            }
        }

        public bool CreateMonthlyBatch(BatchCreateDialogResult data)
        {
            string periodKey = data.TargetId == null ? string.Empty : data.PeriodKey;
            string userId = data.TargetId;
            var targetAssets = FixedAssets
                .Where(r => !r.IsDeleted && string.Equals(r.IdManager, userId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.AssetCode)
                .ToList();

            if (targetAssets.Count == 0)
            {
                XtraMessageBox.Show("該經辦名下沒有可建立月檢批次的資產。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            using (var context = new DBDocumentManagementSystemEntities())
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    bool duplicated = context.dt313_InspectionBatch.Any(r =>
                        r.BatchType == FixedAsset313Const.BatchTypeMonthly &&
                        r.PeriodKey == periodKey &&
                        r.AssignedUserId == userId);
                    if (duplicated)
                    {
                        throw new Exception("同一期間與經辦的月檢批次已存在。");
                    }

                    var user = Users.FirstOrDefault(r => r.Id == userId);
                    string deptId = NormalizeDeptId(user?.IdDepartment);

                    var batch = new dt313_InspectionBatch
                    {
                        BatchName = $"【每月自檢】{periodKey}-{data.TargetDisplay}",
                        BatchType = FixedAsset313Const.BatchTypeMonthly,
                        PeriodKey = periodKey,
                        IdDept = deptId,
                        AssignedUserId = userId,
                        TargetQty = targetAssets.Count,
                        Status = "Open",
                        CreatedBy = TPConfigs.LoginUser.Id,
                        CreatedDate = DateTime.Now
                    };

                    context.dt313_InspectionBatch.Add(batch);
                    context.SaveChanges();

                    context.dt313_InspectionBatchAsset.AddRange(targetAssets.Select(asset => new dt313_InspectionBatchAsset
                    {
                        BatchId = batch.Id,
                        FixedAssetId = asset.Id,
                        Result = FixedAsset313Const.ResultPending,
                        CreatedDate = DateTime.Now
                    }));
                    context.SaveChanges();
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        public bool CreateQuarterlyBatch(BatchCreateDialogResult data)
        {
            string periodKey = data.PeriodKey;
            string deptId = data.TargetId;
            int sampleRate = Math.Max(1, data.SampleRate);

            var targetAssets = FixedAssets
                .Where(r => !r.IsDeleted &&
                    !string.IsNullOrWhiteSpace(r.IdDept) &&
                    r.IdDept.StartsWith(deptId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.LastQuarterlyAuditDate ?? DateTime.MinValue)
                .ThenBy(r => r.AssetCode)
                .ToList();

            if (targetAssets.Count == 0)
            {
                XtraMessageBox.Show("該部門沒有可建立季檢批次的資產。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            int sampleCount = Math.Max(1, (int)Math.Ceiling(targetAssets.Count * sampleRate / 100D));
            var selectedAssets = targetAssets.Take(sampleCount).ToList();

            using (var context = new DBDocumentManagementSystemEntities())
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    bool duplicated = context.dt313_InspectionBatch.Any(r =>
                        r.BatchType == FixedAsset313Const.BatchTypeQuarterly &&
                        r.PeriodKey == periodKey &&
                        r.IdDept == deptId);
                    if (duplicated)
                    {
                        throw new Exception("同一季度與部門的稽核批次已存在。");
                    }

                    var batch = new dt313_InspectionBatch
                    {
                        BatchName = $"【季度稽核】{periodKey}-{data.TargetDisplay}",
                        BatchType = FixedAsset313Const.BatchTypeQuarterly,
                        PeriodKey = periodKey,
                        IdDept = deptId,
                        SampleRate = sampleRate,
                        TargetQty = selectedAssets.Count,
                        Status = "Open",
                        CreatedBy = TPConfigs.LoginUser.Id,
                        CreatedDate = DateTime.Now
                    };

                    context.dt313_InspectionBatch.Add(batch);
                    context.SaveChanges();

                    context.dt313_InspectionBatchAsset.AddRange(selectedAssets.Select(asset => new dt313_InspectionBatchAsset
                    {
                        BatchId = batch.Id,
                        FixedAssetId = asset.Id,
                        Result = FixedAsset313Const.ResultPending,
                        CreatedDate = DateTime.Now
                    }));
                    context.SaveChanges();
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        public bool SaveInspectionDetail(int batchAssetId, InspectionResultDialogResult dialogResult, bool allowResultEdit, bool allowCorrectionEdit)
        {
            using (var context = new DBDocumentManagementSystemEntities())
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var item = context.dt313_InspectionBatchAsset.First(r => r.Id == batchAssetId);
                    var batch = context.dt313_InspectionBatch.First(r => r.Id == item.BatchId);
                    var asset = context.dt313_FixedAsset.First(r => r.Id == item.FixedAssetId);
                    var photoList = context.dt313_InspectionPhoto.Where(r => r.BatchAssetId == batchAssetId).ToList();

                    if (allowResultEdit)
                    {
                        if (dialogResult.Result == FixedAsset313Const.ResultPending)
                        {
                            item.Result = FixedAsset313Const.ResultPending;
                            item.AbnormalId = null;
                            item.AbnormalNote = null;
                            item.CheckedBy = null;
                            item.CheckedDate = null;
                            item.CorrectionDueDate = null;
                            item.CorrectionStatus = null;
                            item.CorrectionNote = null;
                            item.ClosedBy = null;
                            item.ClosedDate = null;
                            context.dt313_InspectionPhoto.RemoveRange(photoList);
                        }
                        else if (dialogResult.Result == FixedAsset313Const.ResultNormal)
                        {
                            DateTime checkDate = item.CheckedDate ?? DateTime.Now;
                            item.Result = FixedAsset313Const.ResultNormal;
                            item.AbnormalId = null;
                            item.AbnormalNote = null;
                            item.CheckedBy = item.CheckedBy ?? TPConfigs.LoginUser.Id;
                            item.CheckedDate = checkDate;
                            item.CorrectionDueDate = null;
                            item.CorrectionStatus = null;
                            item.CorrectionNote = null;
                            item.ClosedBy = null;
                            item.ClosedDate = null;
                            context.dt313_InspectionPhoto.RemoveRange(photoList);
                            UpdateAssetLastCheck(asset, batch, checkDate);
                        }
                        else
                        {
                            DateTime checkDate = item.CheckedDate ?? DateTime.Now;
                            item.Result = FixedAsset313Const.ResultAbnormal;
                            item.AbnormalId = dialogResult.AbnormalId;
                            item.AbnormalNote = dialogResult.AbnormalNote?.Trim();
                            item.CheckedBy = item.CheckedBy ?? TPConfigs.LoginUser.Id;
                            item.CheckedDate = checkDate;
                            item.CorrectionDueDate = item.CorrectionDueDate ?? checkDate.Date.AddDays(5);
                            UpdateAssetLastCheck(asset, batch, checkDate);
                        }
                    }

                    foreach (int deleteId in dialogResult.DeletedPhotoIds.Distinct())
                    {
                        var photoDel = photoList.FirstOrDefault(r => r.Id == deleteId);
                        if (photoDel != null)
                        {
                            context.dt313_InspectionPhoto.Remove(photoDel);
                        }
                    }

                    context.SaveChanges();

                    if (allowResultEdit && dialogResult.Result == FixedAsset313Const.ResultAbnormal)
                    {
                        int abnormalDisplayOrder = context.dt313_InspectionPhoto
                            .Where(r => r.BatchAssetId == batchAssetId && r.PhotoPurpose == FixedAsset313Const.InspectionPhotoPurposeAbnormal)
                            .Select(r => (int?)r.DisplayOrder).Max() ?? 0;

                        foreach (string file in dialogResult.NewAbnormalPhotoFiles)
                        {
                            var saved = FixedAsset313Helper.SaveInspectionPhoto(batchAssetId, file);
                            abnormalDisplayOrder++;
                            context.dt313_InspectionPhoto.Add(new dt313_InspectionPhoto
                            {
                                BatchAssetId = batchAssetId,
                                PhotoPurpose = FixedAsset313Const.InspectionPhotoPurposeAbnormal,
                                EncryptionName = saved.encryptionName,
                                ActualName = saved.actualName,
                                UploadedBy = TPConfigs.LoginUser.Id,
                                UploadedDate = DateTime.Now,
                                DisplayOrder = abnormalDisplayOrder
                            });
                        }
                    }

                    if (allowCorrectionEdit && item.Result == FixedAsset313Const.ResultAbnormal)
                    {
                        int correctionDisplayOrder = context.dt313_InspectionPhoto
                            .Where(r => r.BatchAssetId == batchAssetId && r.PhotoPurpose == FixedAsset313Const.InspectionPhotoPurposeCorrection)
                            .Select(r => (int?)r.DisplayOrder).Max() ?? 0;

                        foreach (string file in dialogResult.NewCorrectionPhotoFiles)
                        {
                            var saved = FixedAsset313Helper.SaveInspectionPhoto(batchAssetId, file);
                            correctionDisplayOrder++;
                            context.dt313_InspectionPhoto.Add(new dt313_InspectionPhoto
                            {
                                BatchAssetId = batchAssetId,
                                PhotoPurpose = FixedAsset313Const.InspectionPhotoPurposeCorrection,
                                EncryptionName = saved.encryptionName,
                                ActualName = saved.actualName,
                                UploadedBy = TPConfigs.LoginUser.Id,
                                UploadedDate = DateTime.Now,
                                DisplayOrder = correctionDisplayOrder
                            });
                        }

                        item.CorrectionNote = dialogResult.CorrectionNote?.Trim();
                        DateTime dueDate = (item.CorrectionDueDate ?? DateTime.Today).Date;

                        if (IsManager313 && dialogResult.MarkCorrectionClosed)
                        {
                            item.CorrectionStatus = FixedAsset313Const.CorrectionClosed;
                            item.ClosedBy = TPConfigs.LoginUser.Id;
                            item.ClosedDate = DateTime.Now;
                        }
                        else
                        {
                            item.CorrectionStatus = DateTime.Today > dueDate
                                ? FixedAsset313Const.CorrectionOverdue
                                : FixedAsset313Const.CorrectionOpen;
                            item.ClosedBy = null;
                            item.ClosedDate = null;
                        }
                    }

                    if (allowResultEdit && item.Result == FixedAsset313Const.ResultAbnormal)
                    {
                        int abnormalPhotoCount = context.dt313_InspectionPhoto.Count(r =>
                            r.BatchAssetId == batchAssetId &&
                            r.PhotoPurpose == FixedAsset313Const.InspectionPhotoPurposeAbnormal);

                        if (abnormalPhotoCount == 0)
                        {
                            throw new Exception("異常結果至少需要一張異常照片。");
                        }
                    }

                    context.SaveChanges();
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        public bool CloseBatch(BatchGridRow row)
        {
            if (row == null)
            {
                return false;
            }

            using (var context = new DBDocumentManagementSystemEntities())
            {
                var batch = context.dt313_InspectionBatch.First(r => r.Id == row.Entity.Id);
                if (batch.Status == "Closed")
                {
                    XtraMessageBox.Show("此批次已結案。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                bool hasPending = context.dt313_InspectionBatchAsset.Any(r =>
                    r.BatchId == batch.Id &&
                    r.Result == FixedAsset313Const.ResultPending);
                if (hasPending)
                {
                    XtraMessageBox.Show("仍有待處理項目，無法結案。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                batch.Status = "Closed";
                batch.ClosedBy = TPConfigs.LoginUser.Id;
                batch.ClosedDate = DateTime.Now;
                context.SaveChanges();
                return true;
            }
        }

        private void UpdateAssetLastCheck(dt313_FixedAsset asset, dt313_InspectionBatch batch, DateTime checkedDate)
        {
            asset.UpdatedBy = TPConfigs.LoginUser.Id;
            asset.UpdatedDate = DateTime.Now;
            if (batch.BatchType == FixedAsset313Const.BatchTypeMonthly)
            {
                asset.LastMonthlyCheckDate = checkedDate;
            }
            else if (batch.BatchType == FixedAsset313Const.BatchTypeQuarterly)
            {
                asset.LastQuarterlyAuditDate = checkedDate;
            }
        }

        public static void OpenPhotoFile(string physicalPath, string actualName)
        {
            if (!File.Exists(physicalPath))
            {
                XtraMessageBox.Show("找不到對應的照片檔案。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tempFile = FixedAsset313Helper.CopyToTemp(physicalPath, actualName);
            using (var form = new Views._00_Generals.f00_VIewFile(tempFile, true, false))
            {
                form.ShowDialog();
            }
        }

        private DataTable ReadFirstWorksheet(string filePath)
        {
            string extension = Path.GetExtension(filePath)?.ToLowerInvariant();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = extension == ".xls"
                ? ExcelReaderFactory.CreateBinaryReader(stream)
                : ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });

                if (dataSet.Tables.Count == 0)
                {
                    throw new Exception("Excel 中沒有工作表。");
                }

                return dataSet.Tables[0];
            }
        }

        private bool IsRowEmpty(DataRow row)
        {
            return row.ItemArray.All(r => r == null || string.IsNullOrWhiteSpace(r.ToString()));
        }

        private string ReadCell(DataTable table, DataRow row, params string[] aliases)
        {
            string columnName = FindColumnName(table, aliases);
            return string.IsNullOrWhiteSpace(columnName) ? string.Empty : row[columnName]?.ToString().Trim() ?? string.Empty;
        }

        private string FindColumnName(DataTable table, params string[] aliases)
        {
            var normalizedColumns = table.Columns.Cast<DataColumn>()
                .ToDictionary(c => NormalizeHeader(c.ColumnName), c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

            foreach (string alias in aliases)
            {
                string key = NormalizeHeader(alias);
                if (normalizedColumns.ContainsKey(key))
                {
                    return normalizedColumns[key];
                }
            }

            return string.Empty;
        }

        private string NormalizeHeader(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? string.Empty
                : input.Replace(" ", string.Empty)
                    .Replace("_", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace("　", string.Empty)
                    .Trim()
                    .ToLowerInvariant();
        }
    }
}
