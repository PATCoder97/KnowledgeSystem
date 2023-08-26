using DevExpress.Pdf;
using DevExpress.Utils.CommonDialogs;
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

        public f207_ViewPdf(string documentFile_, string IdKnowledgeBase_)
        {
            InitializeComponent();
            documentFile = documentFile_;
            idKnowledgeBase = IdKnowledgeBase_;
        }

        string documentFile = "";
        string idKnowledgeBase = "";

        private void f207_ViewPdf_Load(object sender, EventArgs e)
        {
            pdfViewerData.DocumentFilePath = documentFile;

            bar2.Visible = !string.IsNullOrEmpty(idKnowledgeBase);
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
            if (CanSaveFile != true)
            {
                XtraMessageBox.Show(TempDatas.NoPermission, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            var dialogResult = pdfViewerData.ShowPrintPageSetupDialog();
            if (dialogResult == null) return;

            pdfViewerData.Print(dialogResult);

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var IsProcessing = db.dt207_DocProgress.Any(r => r.IdKnowledgeBase == idKnowledgeBase && !r.IsComplete);
                if (!IsProcessing)
                {
                    dt207_HistoryGetFile historyGetFile = new dt207_HistoryGetFile()
                    {
                        IdKnowledgeBase = idKnowledgeBase,
                        idTypeHisGetFile = 3,
                        KnowledgeAttachmentName = Text,
                        IdUser = TempDatas.LoginId,
                        TimeGet = DateTime.Now
                    };

                    db.dt207_HistoryGetFile.Add(historyGetFile);
                    db.SaveChanges();
                }
            }

            XtraMessageBox.Show("列印文件成功！", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CanSaveFile != true)
            {
                XtraMessageBox.Show(TempDatas.NoPermission, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                File.Copy(documentFile, saveFileDialog1.FileName, true);

                using (var db = new DBDocumentManagementSystemEntities())
                {
                    var IsProcessing = db.dt207_DocProgress.Any(r => r.IdKnowledgeBase == idKnowledgeBase && !r.IsComplete);
                    if (!IsProcessing)
                    {
                        dt207_HistoryGetFile historyGetFile = new dt207_HistoryGetFile()
                        {
                            IdKnowledgeBase = idKnowledgeBase,
                            idTypeHisGetFile = 2,
                            KnowledgeAttachmentName = Text,
                            IdUser = TempDatas.LoginId,
                            TimeGet = DateTime.Now
                        };

                        db.dt207_HistoryGetFile.Add(historyGetFile);
                        db.SaveChanges();
                    }
                }

                XtraMessageBox.Show("下載文件成功！", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}