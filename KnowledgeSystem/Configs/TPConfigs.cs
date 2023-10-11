using DataAccessLayer;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public const string KeySpirePPT = "CtOzJs2BlzPokWgBAKMfmNxjRwLa3eqzrAvKtn54UDB/dWjIyGokcs+UQuYuvMY03wX56Ox75KV+U1r5H0PR++c1zc6i8e0QIOVuhMp9Qbg5A9bJJA7e7KvC4KMINTr4jnJy/yTGFwT1aEusw144kml/6oAttwEUoXBkDPLWGOsvNgH1iTYkTGWMXEV8Or4p4t4doNsl0Z7V5qWDKwB6sD/ZiH7l/Jum27FWevOlKIa2VG1rEKjtURYukbWXeSH54IKtmn7nmr0wKwnRgdu3q60aC/PdkxC0zX75EnbU5M6fa3pplU40f3LGOWcgZ2f+8oI7qpPXJ8/s7LrsxBqpQ2YGKfKuqx5ex9ALrXgjnwjcslmXPYun7flHGIkbvBsCjCpo4Ed+M658sZTGATak6gLmftEqhJ1ZZJJKFgXE5qa/TyCY7wIq1ll+z1VNhnSBZUc1RA4TwSBcFKvrZEHlj9o1WFZ1+QqNAcnzh/n+tG48B0wHLCl6D4hroCfWMoaw/23DRxx1WuWqfkazuz2H8ga1RC2XPs83nB7CHPFNs0sT5lsKbfA3P9jgtza5CEhfjAN/3TiwEP/tvnTZY+VABK97veB77h4LEiVMfQXzKfhm9cNW4ft/ofVU2OfqZ8GjtntoZdPxp1bIwTvI98SnQi/H81w19aHwUqNECTeJBjqqHMxdVKVSBAKJL0TM7RyzoOPKS19OfURAxlEgRUqJF/BM8eU0R+UicIM2h36sTuBKO4g3H6woDMlnx0QG0nqthauTB7oK6QFTwk44UQ1kTAu8LeOJwM2xNu5MLsPmoWwDvmIaTuZIW6VUX8C285c9KkrYAf79YKA3e3yxx6SSQdN/jLbtR7MaeGpxRzX0iEbqL9sG1m5USuYVByvVKQ4ntvfCMlLmUN9UCvJ/m63K27Z2dm6fTXIe/g0smYmnvEQ3JQVnldWOi1TKOMK8RbuU5un5mQZ96pLq0Q7g0NLQZh50UMT+OjAzXHPxmXfV6/deHeE8Gbb3ZYJSg7UXW2sty86uXwkj89x5yJTaMNtm6Kh2QQugn/Vd9n8C8QReNewYxjF827FBpMp9yf+vLf2FSyA50wiA9o9luoXYgRmGuUh+g9+KMWgMK5fxQ2h3cHqADzPcwsDhVfG6HuAgt81vH/M5hFLdQztXdvRKVuYOyyTOnQz9K93LZ2EvbeWz0YByRkGxnve+K8UNo3pyNgaPGRQWr5RbeURNJ4PhmM3dB2oMkwE//+s39ccgADdEJS8s35cjRrVEGs8JicRu6mDNqJfdHUNfLmiySMjG/ePwhYkiB2WhJ9AqpY9N7eQ3TBsAMkr34olS6eSNpaE1BjgJsljB27GDnmMAXNZeifyIYpBcqu6H9SLN5pGBF9WHcPVivjdNpMUrKQ==";
        public const string FilterFile = "Files|*.pdf;*.xlsx;*.xls;*.docx;*.doc;*.ppt;*.pptx";

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

    public enum EventFormInfo
    {
        Create,
        View,
        Update,
        Delete,
        ViewOnly
    }


    public static class DefaultMsg
    {
        public static void MsgErrorDB()
        {
            XtraMessageBox.Show("Dữ liệu đầu vào không đúng\r\nHoặc có lỗi xảy ra trong quá trình truy vấn\r\nLiên hệ người quản lý để được xử lý!", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void MsgConfirmDel()
        {
            XtraMessageBox.Show("Vui lòng xác nhận lại thông tin muốn xoá!\r\nSau đó ấn xác nhận để xoá.", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
    //public DBDocumentManagementSystemEntities()
    //        : base(SingleConnection.ConString)
    //{
    //}
}
