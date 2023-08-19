using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Configs
{
    public class TempDatas
    {
        public const string SoftNameEN = "Document Management System";
        public const string SoftNameTW = "冶金文件管理系統";

        public const string EventNew = "新增文件";
        public const string EventEdit = "更新文件";
        public const string EventDel = "刪除文件";

        public static dt207_TypeHisGetFile typeViewFile = default(dt207_TypeHisGetFile);
        public static dt207_TypeHisGetFile typeSaveFile = default(dt207_TypeHisGetFile);
        public static dt207_TypeHisGetFile typePrintFile = default(dt207_TypeHisGetFile);

        public static string LoginId { get; set; }
        public static int RoleUserLogin { get; set; }

        public static string DomainComputer { get; set; }
        public static bool LoginSuccessful { get; set; }

        public static string PathKnowledgeFile = @"E:\01. Softwares Programming\24. Knowledge System\05. Data";

        public static string NoPermission { get; set; } = "您沒有該功能的權限";
        public static string DocIsProcessing { get; set; } = "文件處理中，暫不可顯示！";
    }

    public class ChartDataSource
    {
        public string SeriesName { get; set; }
        public string XAxis { get; set; }
        public int YAxis { get; set; }
    }

    //public DBDocumentManagementSystemEntities()
    //        : base(SingleConnection.ConString)
    //{
    //}
}
