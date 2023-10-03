using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class TPConfigsDAL
    {
        public static string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
