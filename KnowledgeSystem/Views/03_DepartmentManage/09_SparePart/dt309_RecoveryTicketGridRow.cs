using System;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public class dt309_RecoveryTicketGridRow
    {
        public int Id { get; set; }
        public string TicketNo { get; set; }
        public int IssueTransactionId { get; set; }
        public int? RestockInTransactionId { get; set; }
        public int NewMaterialId { get; set; }
        public int OldBaseMaterialId { get; set; }
        public int? OldRecoveryMaterialId { get; set; }
        public string NewMaterialCode { get; set; }
        public string NewMaterialDisplayName { get; set; }
        public string OldBaseMaterialCode { get; set; }
        public string OldBaseMaterialDisplayName { get; set; }
        public string OldRecoveryMaterialCode { get; set; }
        public string OldRecoveryMaterialDisplayName { get; set; }
        public string RecoveryOption { get; set; }
        public double Quantity { get; set; }
        public int SourceStorageId { get; set; }
        public string SourceStorageName { get; set; }
        public int? RestockStorageId { get; set; }
        public string RestockStorageName { get; set; }
        public string AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }
        public DateTime? PlannedDisposeDate { get; set; }
        public DateTime? ActualDisposeDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string ResultNote { get; set; }
        public int EvidenceCount { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IdDept { get; set; }

        public string RecoveryOptionDisplay => dt309_RecoveryConst.GetRecoveryOptionDisplay(RecoveryOption);
        public string StatusDisplay => dt309_RecoveryConst.GetRecoveryStatusDisplay(Status);
    }
}
