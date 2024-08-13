using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    using System;

    public class AppCopyRight
    {
        public static string version;
        public static DateTime dateDeploy;
        public static string ownerSoft = "潘英俊";
        public static string supporter = "阮林山、潘成台、潘高孟雄";

        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
                // Automatically update dateDeploy when version is set
                dateDeploy = ConvertVersionToDateDeploy(value);
            }
        }

        public DateTime DateDeploy
        {
            get
            {
                return dateDeploy;
            }
            set
            {
                dateDeploy = value;
            }
        }

        public string MadeBy
        {
            get
            {
                return ownerSoft;
            }
            set
            {
                ownerSoft = value;
            }
        }

        public static string CopyRightString()
        {
            return $" - 版本: {version} - 發布: {dateDeploy:yyyy.MM.dd} - 作者: {ownerSoft}";
        }

        private static DateTime ConvertVersionToDateDeploy(string version)
        {
            string[] versionParts = version.Split('.');

            if (versionParts.Length != 3)
            {
                throw new ArgumentException("Version format is incorrect. It should be in 'YY.MM.DD' format.");
            }

            int year = int.Parse("20" + versionParts[0]);
            int month = int.Parse(versionParts[1]);
            int day = int.Parse(versionParts[2]);

            return new DateTime(year, month, day);
        }
    }
}
