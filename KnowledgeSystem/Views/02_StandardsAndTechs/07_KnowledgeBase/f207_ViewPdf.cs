using DevExpress.XtraEditors;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_ViewPdf : DevExpress.XtraEditors.XtraForm
    {
        public bool? CanSaveFile { get; set; }
        public bool? CanPrintFile { get; set; }

        public f207_ViewPdf(string documentFile_)
        {
            InitializeComponent();
            documentFile = documentFile_;
        }

        string documentFile = "";

        private void f207_ViewPdf_Load(object sender, EventArgs e)
        {
            //pdfViewerData.PreviewKeyDown = false;

            pdfViewerData.DocumentFilePath = documentFile;
        }

        private void pdfViewerData_PopupMenuShowing(object sender, DevExpress.XtraPdfViewer.PdfPopupMenuShowingEventArgs e)
        {
            e.ItemLinks.Clear();
        }

        private void pdfViewerData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.P && CanPrintFile != true) //detect key ctrl+p
            {
                e.SuppressKeyPress = true; //<= Set it to true.
                MessageBox.Show(TempDatas.NoPermission, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (e.Control && e.KeyCode == Keys.S && CanSaveFile != true) //detect key ctrl+s
            {
                e.SuppressKeyPress = true; //<= Set it to true.
                MessageBox.Show(TempDatas.NoPermission, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
    }
}