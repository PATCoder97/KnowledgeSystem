using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal static class FixedAsset313GridHelper
    {
        public static void ConfigureReadOnlyView(GridView view)
        {
            view.ReadOnlyGridView();
            view.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            FixedAsset313UIHelper.ApplyGridStyle(view);
        }

        public static void HideColumn(GridView view, string fieldName)
        {
            var column = view.Columns.ColumnByFieldName(fieldName);
            if (column != null)
            {
                column.Visible = false;
            }
        }

        public static void SetColumn(GridView view, string fieldName, string caption, int width)
        {
            GridColumn column = view.Columns.ColumnByFieldName(fieldName);
            if (column == null)
            {
                return;
            }

            column.Caption = caption;
            column.Width = width;
            column.Visible = true;
        }

        public static void SetDateColumn(GridView view, string fieldName, string caption, int width, string format = "yyyy-MM-dd")
        {
            SetColumn(view, fieldName, caption, width);
            GridColumn column = view.Columns.ColumnByFieldName(fieldName);
            if (column == null)
            {
                return;
            }

            column.DisplayFormat.FormatType = FormatType.DateTime;
            column.DisplayFormat.FormatString = format;
        }
    }
}
