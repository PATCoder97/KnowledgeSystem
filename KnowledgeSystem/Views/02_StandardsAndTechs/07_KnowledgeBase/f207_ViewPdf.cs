using DevExpress.Pdf;
using DevExpress.XtraEditors;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
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
            if (e.Control && (e.KeyCode == Keys.P || e.KeyCode == Keys.S))
            {
                e.SuppressKeyPress = true;
                return;
            }
        }

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CanPrintFile!=true)
            {
                XtraMessageBox.Show(TempDatas.NoPermission, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            pdfViewerData.ShowPrintPageSetupDialog();
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CanSaveFile != true)
            {
                XtraMessageBox.Show(TempDatas.NoPermission, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Xuất dữ liệu chính chủ ra file pdf";
            saveFileDialog1.DefaultExt = "xlsx";
            saveFileDialog1.Filter = "PDF Files|*.pdf";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = $"{DateTime.Now:yyyyMMddHHmmss}-{Text}";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.Copy(documentFile, saveFileDialog1.FileName,true);
            }
        }
    }
}