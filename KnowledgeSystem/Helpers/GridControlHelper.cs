using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Helpers
{
    public static class GridControlHelper
    {
        public static void ReadOnlyGridView(this GridView gridView_, bool active = true)
        {
            gridView_.OptionsEditForm.ShowOnDoubleClick = active ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsEditForm.ShowOnEnterKey = active ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsEditForm.ShowOnF2Key = active ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsBehavior.EditingMode = active ? GridEditingMode.EditFormInplace : GridEditingMode.Inplace;
        }

        public static void ReadOnlyTreelist(this TreeList treeList_)
        {
            treeList_.OptionsEditForm.ShowOnDoubleClick = DevExpress.Utils.DefaultBoolean.False;
            treeList_.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
            treeList_.OptionsEditForm.ShowOnF2Key = DevExpress.Utils.DefaultBoolean.False;
            treeList_.OptionsBehavior.EditingMode = TreeListEditingMode.EditForm;

            treeList_.OptionsSelection.SelectNodesOnRightClick = true;
        }

        public static void GridViewCopyCellData_KeyDown(object sender, KeyEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn) != null && view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn).ToString() != String.Empty)
                    Clipboard.SetText(view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn).ToString());
                e.Handled = true;
            }
        }

        public static void TreeViewCopyCellData_KeyDown(object sender, KeyEventArgs e)
        {
            TreeList view = sender as TreeList;
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (view.GetRowCellValue(view.FocusedNode, view.FocusedColumn) != null && view.GetRowCellValue(view.FocusedNode, view.FocusedColumn).ToString() != String.Empty)
                    Clipboard.SetText(view.GetRowCellValue(view.FocusedNode, view.FocusedColumn).ToString());
                e.Handled = true;
            }
        }
    }
}
