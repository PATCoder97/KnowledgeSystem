using DevExpress.Utils.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Configs
{
    public class TPSvgimages
    {
        public static string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string ImagesPath = Path.Combine(StartupPath, "Images");

        public static SvgImage CheckedRadio = SvgImage.FromFile(Path.Combine(ImagesPath, "checked_radio_button.svg"));
        public static SvgImage UncheckedRadio = SvgImage.FromFile(Path.Combine(ImagesPath, "unchecked_radio_button.svg"));
        public static SvgImage Add = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_add.svg"));
        public static SvgImage Add2 = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_plus.svg"));
        public static SvgImage Edit = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_edit.svg"));
        public static SvgImage Reload = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_reload.svg"));
        public static SvgImage Remove = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_remove.svg"));
        public static SvgImage Cancel = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_cancel.svg"));
        public static SvgImage Confirm = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_ok.svg"));
    }
}
