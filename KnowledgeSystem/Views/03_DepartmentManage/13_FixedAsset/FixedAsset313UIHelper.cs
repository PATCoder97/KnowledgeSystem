using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal static class FixedAsset313UIHelper
    {
        private static readonly Font MenuFont = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        static FixedAsset313UIHelper()
        {
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = MenuFont;
        }

        public static DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
            return menuItem;
        }
    }
}
