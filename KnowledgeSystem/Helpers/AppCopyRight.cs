using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    public class AppCopyRight
    {
        public static string version;
        public static string dateDeploy;
        public static string ownerSoft = "潘英俊";
        public static string supporter = "阮林山、潘成台";

        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        public string DateDeploy
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
            return " - 版本:" + version + " - 發布:" + dateDeploy + " - 作者:" + ownerSoft;
        }
    }
}
