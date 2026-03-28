using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using KnowledgeSystem.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal static class FixedAsset313UIHelper
    {
        private static readonly Font menuFont = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static readonly Font headerFont = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static readonly Font rowFont = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        public static void InitializeMenuFont()
        {
            AppearanceObject.DefaultMenuFont = menuFont;
        }

        public static void ApplyBarStyle(Bar bar)
        {
            if (bar == null)
            {
                return;
            }

            bar.BarAppearance.Disabled.Font = new Font("Segoe UI", 12F);
            bar.BarAppearance.Disabled.Options.UseFont = true;

            bar.BarAppearance.Hovered.Font = headerFont;
            bar.BarAppearance.Hovered.ForeColor = Color.Black;
            bar.BarAppearance.Hovered.Options.UseFont = true;
            bar.BarAppearance.Hovered.Options.UseForeColor = true;

            bar.BarAppearance.Normal.Font = headerFont;
            bar.BarAppearance.Normal.ForeColor = Color.Black;
            bar.BarAppearance.Normal.Options.UseFont = true;
            bar.BarAppearance.Normal.Options.UseForeColor = true;

            bar.BarAppearance.Pressed.Font = headerFont;
            bar.BarAppearance.Pressed.ForeColor = Color.Black;
            bar.BarAppearance.Pressed.Options.UseFont = true;
            bar.BarAppearance.Pressed.Options.UseForeColor = true;

            bar.OptionsBar.AllowQuickCustomization = false;
            bar.OptionsBar.DrawDragBorder = false;
            bar.OptionsBar.MultiLine = true;
            bar.OptionsBar.UseWholeRow = true;
        }

        public static void ApplyGridStyle(GridView view)
        {
            if (view == null)
            {
                return;
            }

            view.Appearance.FooterPanel.Font = rowFont;
            view.Appearance.FooterPanel.Options.UseFont = true;
            view.Appearance.FooterPanel.Options.UseTextOptions = true;
            view.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            view.Appearance.HeaderPanel.Font = headerFont;
            view.Appearance.HeaderPanel.ForeColor = Color.Black;
            view.Appearance.HeaderPanel.Options.UseFont = true;
            view.Appearance.HeaderPanel.Options.UseForeColor = true;
            view.Appearance.HeaderPanel.Options.UseTextOptions = true;
            view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            view.Appearance.Row.Font = rowFont;
            view.Appearance.Row.ForeColor = Color.Black;
            view.Appearance.Row.Options.UseFont = true;
            view.Appearance.Row.Options.UseForeColor = true;

            view.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.True;
            view.OptionsView.ColumnAutoWidth = false;
            view.OptionsView.EnableAppearanceOddRow = true;
            view.OptionsView.ShowAutoFilterRow = true;
            view.OptionsView.ShowGroupPanel = false;
        }

        public static void ApplyUserControlStyle(XtraUserControl control, BarManager barManager, Bar mainBar)
        {
            if (control == null)
            {
                return;
            }

            InitializeMenuFont();
            ApplyBarStyle(mainBar);
            ApplyControlTree(control.Controls);
        }

        public static void ApplyFormStyle(XtraForm form, BarManager barManager, Bar mainBar)
        {
            if (form == null)
            {
                return;
            }

            InitializeMenuFont();
            ApplyBarStyle(mainBar);
            form.StartPosition = FormStartPosition.CenterParent;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            ApplyControlTree(form.Controls);
        }

        public static void ApplyFreeFormStyle(Control root)
        {
            if (root == null)
            {
                return;
            }

            InitializeMenuFont();
            ApplyControlTree(root.Controls);
        }

        public static void ApplyLayoutItemCaptions(params LayoutControlItem[] items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }

                item.AppearanceItemCaption.Font = headerFont;
                item.AppearanceItemCaption.ForeColor = Color.Black;
                item.AppearanceItemCaption.Options.UseFont = true;
                item.AppearanceItemCaption.Options.UseForeColor = true;
            }
        }

        public static DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
            return menuItem;
        }

        private static void ApplyControlTree(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                ApplyControlStyle(control);
                if (control.HasChildren)
                {
                    ApplyControlTree(control.Controls);
                }
            }
        }

        private static void ApplyControlStyle(Control control)
        {
            if (control is LayoutControl layoutControl)
            {
                layoutControl.AllowCustomization = false;
            }

            if (control is BaseEdit baseEdit)
            {
                baseEdit.Properties.Appearance.Font = headerFont;
                baseEdit.Properties.Appearance.ForeColor = Color.Black;
                baseEdit.Properties.Appearance.Options.UseFont = true;
                baseEdit.Properties.Appearance.Options.UseForeColor = true;

                if (baseEdit is ComboBoxEdit comboBoxEdit)
                {
                    comboBoxEdit.Properties.AppearanceDropDown.Font = headerFont;
                    comboBoxEdit.Properties.AppearanceDropDown.Options.UseFont = true;
                }
            }
            else if (control is SimpleButton button)
            {
                button.Appearance.Font = headerFont;
                button.Appearance.ForeColor = Color.Black;
                button.Appearance.Options.UseFont = true;
                button.Appearance.Options.UseForeColor = true;
                button.AppearanceHovered.ForeColor = Color.Blue;
                button.AppearanceHovered.Options.UseForeColor = true;
            }
            else if (control is GroupControl group)
            {
                group.AppearanceCaption.Font = headerFont;
                group.AppearanceCaption.ForeColor = Color.Black;
                group.AppearanceCaption.Options.UseFont = true;
                group.AppearanceCaption.Options.UseForeColor = true;
            }
            else if (control is CheckEdit checkEdit)
            {
                checkEdit.Properties.Appearance.Font = headerFont;
                checkEdit.Properties.Appearance.ForeColor = Color.Black;
                checkEdit.Properties.Appearance.Options.UseFont = true;
                checkEdit.Properties.Appearance.Options.UseForeColor = true;
            }
            else if (control is LabelControl labelControl)
            {
                labelControl.Appearance.Font = headerFont;
                labelControl.Appearance.ForeColor = Color.Black;
                labelControl.Appearance.Options.UseFont = true;
                labelControl.Appearance.Options.UseForeColor = true;
            }
            else if (control is Label label)
            {
                label.Font = headerFont;
                label.ForeColor = Color.Black;
            }
            else if (control is ListBox listBox)
            {
                listBox.Font = rowFont;
                listBox.ForeColor = Color.Black;
            }
        }
    }
}
