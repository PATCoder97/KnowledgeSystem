using DataAccessLayer;
using System;
using System.Collections.Generic;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal static class FixedAsset313Const
    {
        public const string GroupManagerName = "固定資產【管理】";
        public const string GroupHandlerName = "固定資產【經辦】";

        public const string BatchTypeMonthly = "Monthly";
        public const string BatchTypeQuarterly = "Quarterly";

        public const string ResultPending = "Pending";
        public const string ResultNormal = "Normal";
        public const string ResultAbnormal = "Abnormal";

        public const string CorrectionOpen = "Open";
        public const string CorrectionClosed = "Closed";
        public const string CorrectionOverdue = "Overdue";

        public const string PhotoTypeCloseUp = "CloseUp";
        public const string PhotoTypeOverview = "Overview";
        public const string PhotoTypeInUse = "InUse";

        public const string InspectionPhotoPurposeAbnormal = "Abnormal";
        public const string InspectionPhotoPurposeCorrection = "Correction";
    }

    internal class LookupItem
    {
        public LookupItem(string value, string display)
        {
            Value = value;
            Display = display;
        }

        public string Value { get; }
        public string Display { get; }

        public override string ToString()
        {
            return Display;
        }
    }

    internal class AssetGridRow
    {
        public dt313_FixedAsset Entity { get; set; }
        public string AssetCode { get; set; }
        public string AssetNameTW { get; set; }
        public string AssetNameVN { get; set; }
        public string IdDept { get; set; }
        public string DeptName { get; set; }
        public string IdManager { get; set; }
        public string ManagerName { get; set; }
        public string AssetCategory { get; set; }
        public string AssetCategoryDisplay { get; set; }
        public string TypeName { get; set; }
        public string Location { get; set; }
        public string BrandSpec { get; set; }
        public string Origin { get; set; }
        public DateTime? AcquireDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public DateTime? LastMonthlyCheckDate { get; set; }
        public DateTime? LastQuarterlyAuditDate { get; set; }
        public string PhotoCompletion { get; set; }
        public bool HasCloseUp { get; set; }
        public bool HasOverview { get; set; }
        public bool HasInUse { get; set; }
    }

    internal class BatchGridRow
    {
        public dt313_InspectionBatch Entity { get; set; }
        public string BatchName { get; set; }
        public string BatchType { get; set; }
        public string BatchTypeDisplay { get; set; }
        public string PeriodKey { get; set; }
        public string DeptName { get; set; }
        public string AssignedUserName { get; set; }
        public int? SampleRate { get; set; }
        public int TargetQty { get; set; }
        public int CompletedQty { get; set; }
        public int AbnormalQty { get; set; }
        public string ProgressText { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public List<BatchDetailGridRow> Details { get; set; } = new List<BatchDetailGridRow>();
    }

    internal class BatchDetailGridRow
    {
        public dt313_InspectionBatchAsset Entity { get; set; }
        public dt313_InspectionBatch Batch { get; set; }
        public dt313_FixedAsset Asset { get; set; }
        public string AssetCode { get; set; }
        public string AssetNameTW { get; set; }
        public string DeptName { get; set; }
        public string ManagerName { get; set; }
        public string Result { get; set; }
        public string ResultDisplay { get; set; }
        public string AbnormalName { get; set; }
        public string AbnormalNote { get; set; }
        public DateTime? CheckedDate { get; set; }
        public DateTime? CorrectionDueDate { get; set; }
        public string CorrectionStatus { get; set; }
        public string CorrectionStatusDisplay { get; set; }
        public string CorrectionNote { get; set; }
        public int EvidencePhotoCount { get; set; }
        public int CorrectionPhotoCount { get; set; }
    }

    internal class AbnormalGridRow
    {
        public dt313_InspectionBatchAsset Entity { get; set; }
        public dt313_InspectionBatch Batch { get; set; }
        public dt313_FixedAsset Asset { get; set; }
        public string BatchName { get; set; }
        public string BatchTypeDisplay { get; set; }
        public string PeriodKey { get; set; }
        public string AssetCode { get; set; }
        public string AssetNameTW { get; set; }
        public string DeptName { get; set; }
        public string ManagerName { get; set; }
        public string AbnormalName { get; set; }
        public string AbnormalNote { get; set; }
        public DateTime? CheckedDate { get; set; }
        public DateTime? CorrectionDueDate { get; set; }
        public string CorrectionStatus { get; set; }
        public string CorrectionStatusDisplay { get; set; }
        public string CorrectionNote { get; set; }
    }

    internal class DepartmentSettingGridRow
    {
        public dt313_DepartmentSetting Entity { get; set; }
        public string IdDept { get; set; }
        public string DeptName { get; set; }
        public int QuarterlySampleRate { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    internal class AbnormalCatalogGridRow
    {
        public dt313_AbnormalCatalog Entity { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    internal class BatchCreateDialogResult
    {
        public string TargetId { get; set; }
        public string TargetDisplay { get; set; }
        public DateTime SelectedDate { get; set; }
        public string PeriodKey { get; set; }
        public int SampleRate { get; set; }
    }

    internal class InspectionResultDialogResult
    {
        public string Result { get; set; }
        public int? AbnormalId { get; set; }
        public string AbnormalNote { get; set; }
        public string CorrectionNote { get; set; }
        public bool MarkCorrectionClosed { get; set; }
        public List<int> DeletedPhotoIds { get; set; } = new List<int>();
        public List<string> NewAbnormalPhotoFiles { get; set; } = new List<string>();
        public List<string> NewCorrectionPhotoFiles { get; set; } = new List<string>();
    }

    internal class PhotoSelectionRow
    {
        public bool IsExisting { get; set; }
        public int ExistingId { get; set; }
        public string PhysicalPath { get; set; }
        public string ActualName { get; set; }

        public override string ToString()
        {
            return ActualName;
        }
    }
}
