using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Preview;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_PrintReport : DevExpress.XtraEditors.XtraForm
    {
        public f00_PrintReport()
        {
            InitializeComponent();
        }
        public DocumentViewer ViewerReport => viewerReport;
    }
}