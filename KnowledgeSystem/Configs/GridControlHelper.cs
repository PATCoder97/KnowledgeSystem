using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Configs
{
    public static class GridControlHelper
    {
        public static void ReadOnlyGridView(this GridView gridView_)
        {
            gridView_.OptionsEditForm.ShowOnDoubleClick = DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsEditForm.ShowOnF2Key = DevExpress.Utils.DefaultBoolean.False;
            gridView_.OptionsBehavior.EditingMode = GridEditingMode.EditFormInplace;
        }
    }
}
