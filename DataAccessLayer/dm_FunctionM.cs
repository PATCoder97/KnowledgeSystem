using DevExpress.Utils.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class dm_FunctionM : dm_Function
    {
        public SvgImage ImageSvg
        {
            get
            {
                return File.Exists(Path.Combine(TPConfigsDAL.StartupPath, $@"Images\{Images}")) ?
                 SvgImage.FromFile(Path.Combine(TPConfigsDAL.StartupPath, $@"Images\{Images}")) :
                 SvgImage.FromFile(Path.Combine(TPConfigsDAL.StartupPath, $@"Images\none.svg"));
            }
            set { }
        }

        public List<dm_FunctionM> Children { get; set; } = new List<dm_FunctionM>();
    }
}
