using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Configs
{
    public class TempDatas
    {
        public static string SoftNameEN = "Document Management System";
        public static string SoftNameTW = "冶金文件管理系統";

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
