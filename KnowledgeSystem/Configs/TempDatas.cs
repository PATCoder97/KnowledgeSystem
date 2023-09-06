using DataEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Configs
{
    public class TempDatas
    {
        // Static Value SQL
        public static string SoftNameEN { get; set; }
        public static string SoftNameTW { get; set; }
        public static string UrlUpdate { get; set; }
        public static string PathKnowledgeFile { get; set; }

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


        public static string NoPermission { get; set; } = "您沒有該功能的權限";
        public static string DocIsProcessing { get; set; } = "文件處理中，暫不可顯示！";


    }

    public class ChartDataSource
    {
        public string SeriesName { get; set; }
        public string XAxis { get; set; }
        public int YAxis { get; set; }
    }

    public class UpdateInfo
    {
        public string app { get; set; }
        public string version { get; set; }
        public string url { get; set; }
    }

    //public DBDocumentManagementSystemEntities()
    //        : base(SingleConnection.ConString)
    //{
    //}
}
