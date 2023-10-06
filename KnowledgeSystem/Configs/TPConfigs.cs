using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Configs
{
    public class TPConfigs
    {
        public static string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string ImagesPath = Path.Combine(StartupPath, "Images");
        public static string TempFolderData = Path.Combine(Path.GetTempPath(), "TPTempData");

        // Static Value SQL
        public static string SoftNameEN { get; set; }
        public static string SoftNameTW { get; set; }
        public static string UrlUpdate { get; set; }
        public static string PathKnowledgeFile { get; set; }

        // CONST Value
        public const string strReadFile = "讀取";
        public const string strSaveFile = "下載";
        public const string NoPermission  = "您沒有該功能的權限";
        public const string DocIsProcessing = "文件處理中，暫不可顯示！";

        public static dm_User LoginUser { get; set; }
        public static string DomainComputer { get; set; }
        public static bool LoginSuccessful { get; set; }
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

    public enum Event207DocInfo
    {
        [Description("新增文件")]
        Create,
        View,
        [Description("更新文件")]
        Update,
        [Description("刪除文件")]
        Delete,
        Approval,
        Check
    }

    //public DBDocumentManagementSystemEntities()
    //        : base(SingleConnection.ConString)
    //{
    //}
}
