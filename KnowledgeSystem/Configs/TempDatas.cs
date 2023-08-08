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

        public static string LoginId { get; set; }
        public static string DomainComputer { get; set; }
        public static bool LoginSuccessful { get; set; }

        public static string PahtDataFile = @"E:\01. Softwares Programming\24. Knowledge System\05. Data";

        public static string NoPermission { get; set; } = "Ban khong co quyen han cho chuc nang nay";
    }
    //public DBDocumentManagementSystemEntities()
    //        : base(SingleConnection.ConString)
    //{
    //}
}
