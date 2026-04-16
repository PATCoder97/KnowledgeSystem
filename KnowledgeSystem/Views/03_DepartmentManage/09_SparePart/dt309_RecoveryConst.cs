using System;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public static class dt309_RecoveryConst
    {
        public const string RecoveryOptionNone = "none";
        public const string RecoveryOptionScrap = "scrap";
        public const string RecoveryOptionRestock = "restock";

        public const string RecoveryStatusScheduled = "scheduled";
        public const string RecoveryStatusAwaitManagerConfirm = "await_manager_confirm";
        public const string RecoveryStatusCompleted = "completed";
        public const string RecoveryStatusCancelled = "cancelled";

        public const string SparePartGroupName = "機邊庫";
        public const string SparePartManagerGroupName = "機邊庫【管理】";

        public static string GetRecoveryOptionDisplay(string recoveryOption)
        {
            switch ((recoveryOption ?? string.Empty).Trim().ToLowerInvariant())
            {
                case RecoveryOptionScrap:
                    return "報廢";
                case RecoveryOptionRestock:
                    return "回收入庫";
                default:
                    return string.Empty;
            }
        }

        public static string GetRecoveryStatusDisplay(string status)
        {
            switch ((status ?? string.Empty).Trim().ToLowerInvariant())
            {
                case RecoveryStatusScheduled:
                    return "已安排";
                case RecoveryStatusAwaitManagerConfirm:
                    return "待管理確認";
                case RecoveryStatusCompleted:
                    return "已完成";
                case RecoveryStatusCancelled:
                    return "已取消";
                default:
                    return string.Empty;
            }
        }

        public static bool IsScrap(string recoveryOption)
        {
            return string.Equals(recoveryOption, RecoveryOptionScrap, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsRestock(string recoveryOption)
        {
            return string.Equals(recoveryOption, RecoveryOptionRestock, StringComparison.OrdinalIgnoreCase);
        }
    }
}
