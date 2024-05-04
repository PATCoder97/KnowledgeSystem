using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using GridView = DevExpress.XtraGrid.Views.Grid.GridView;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_AddAttachment : DevExpress.XtraEditors.XtraForm
    {
        public f201_AddAttachment()
        {
            InitializeComponent();
        }

        class TestData
        {
            public string VNW { get; set; }
            public string DisplayName { get; set; }
        }

        private void f201_AddAttachment_Load(object sender, EventArgs e)
        {
            var data = new List<TestData>()
            {
                new TestData(){ VNW="VNW0014732"},
                new TestData()
            };

            gcProgress.DataSource = data;
        }

        private void gvProgress_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName != "VNW") return;
            string cellValue = e.Value.ToString() + " OK";
            view.SetRowCellValue(e.RowHandle, view.Columns["DisplayName"], cellValue);
        }
    }
}