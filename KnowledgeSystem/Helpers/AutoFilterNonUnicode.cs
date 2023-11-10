using DevExpress.Data.Filtering;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem
{
    internal class AutoFilterNonUnicode
    {
        [ToolboxItem(true)]
        public class NonUnicode_GridView : GridControl
        {
            protected override BaseView CreateDefaultView()
            {
                MyGridView view = CreateView("NonUnicode_GridView") as MyGridView;
                return view;
            }
            protected override void RegisterAvailableViewsCore(InfoCollection collection)
            {
                base.RegisterAvailableViewsCore(collection);
                collection.Add(new MyGridViewInfoRegistrator());
            }
        }

        public class MyGridViewInfoRegistrator : GridInfoRegistrator
        {
            public override string ViewName { get { return "NonUnicode_GridView"; } }
            public override BaseView CreateView(GridControl grid) { return new MyGridView(grid as GridControl); }
        }



        public class MyGridView : GridView
        {
            public MyGridView() : this(null)
            {
                this.CustomUnboundColumnData += MyGridView_CustomUnboundColumnData;
            }

            void MyGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
            {
                if (e.Column.FieldName.Contains("Unb"))
                {
                    string field = e.Column.FieldName.Substring(0, e.Column.FieldName.IndexOf("Unb"));
                    string val = (e.Row as DataRowView)[field].ToString();
                    string processedString = MyGridView.RemoveDiacritics(val, true);
                    e.Value = val + processedString;
                }
            }

            protected override void RefreshVisibleColumnsList()
            {
                base.RefreshVisibleColumnsList();
                // add required unbound columns
                foreach (GridColumn column in VisibleColumns)
                {
                    string name = column.FieldName + "Unb";
                    GridColumn col = Columns.OfType<GridColumn>().Where(c => c.FieldName == name).FirstOrDefault();
                    if (col != null) continue;
                    GridColumn unboundCol = Columns.AddField(column.FieldName + "Unb");
                    unboundCol.UnboundType = DevExpress.Data.UnboundColumnType.String;
                    column.FieldNameSortGroup = unboundCol.FieldName;
                    column.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                }
            }

            public MyGridView(DevExpress.XtraGrid.GridControl grid) : base(grid) { }

            protected override string ViewName { get { return "NonUnicode_GridView"; } }

            public static IEnumerable<char> RemoveDiacriticsEnum(string src, bool compatNorm, Func<char, char> customFolding)
            {
                foreach (char c in src.Normalize(compatNorm ? NormalizationForm.FormKD : NormalizationForm.FormD))
                    switch (CharUnicodeInfo.GetUnicodeCategory(c))
                    {
                        case UnicodeCategory.NonSpacingMark:
                        case UnicodeCategory.SpacingCombiningMark:
                        case UnicodeCategory.EnclosingMark:
                            //do nothing
                            break;
                        default:
                            yield return customFolding(c);
                            break;
                    }
            }
            public static IEnumerable<char> RemoveDiacriticsEnum(string src, bool compatNorm)
            {
                return RemoveDiacritics(src, compatNorm, c => c);
            }
            public static string RemoveDiacritics(string src, bool compatNorm, Func<char, char> customFolding)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in RemoveDiacriticsEnum(src, compatNorm, customFolding))
                    sb.Append(c);
                return sb.ToString();
            }
            public static string RemoveDiacritics(string src, bool compatNorm)
            {
                return RemoveDiacritics(src, compatNorm, c => c);
            }

            protected override ColumnFilterInfo CreateFilterRowInfo(GridColumn column, object _value)
            {
                string strVal = _value == null ? null : _value.ToString();
                if (_value == null || strVal == string.Empty) return ColumnFilterInfo.Empty;
                strVal = MyGridView.RemoveDiacritics(strVal, true);
                AutoFilterCondition condition = ResolveAutoFilterCondition(column);
                CriteriaOperator op = CreateAutoFilterCriterion(column, condition, _value, strVal);
                return new ColumnFilterInfo(ColumnFilterType.AutoFilter, _value, op, string.Empty);
            }
        }
    }
}
