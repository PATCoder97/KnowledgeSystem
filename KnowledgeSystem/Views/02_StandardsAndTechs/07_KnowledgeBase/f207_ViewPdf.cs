using DataAccessLayer;
using DevExpress.Pdf;
using DevExpress.Utils.CommonDialogs;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
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
            using (var handle = SplashScreenManager.ShowOverlayForm(layoutControl1))
            {
                lcPDF.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcWord.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcExcel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcCanntView.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                string fileExtension = Path.GetExtension(documentFile).ToLower();

                switch (fileExtension)
                {
                    case ".pdf":
                        viewPDF.DocumentFilePath = documentFile;
                        lcPDF.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;
                    case ".xlsx":
                    case ".xls":
                        viewExcel.Document.LoadDocument(documentFile);
                        lcExcel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;
                    case ".docx":
                    case ".doc":
                        viewWord.Document.LoadDocument(documentFile);
                        lcWord.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;
                    default:
                        lcCanntView.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lbCanntView.Text = "目前系統無法讀取該類型文件，\n請下載已閱讀。謝謝！";
                        break;
                }
            }

            bar2.Visible = !string.IsNullOrEmpty(idKnowledgeBase);
        }

        private void pdfViewerData_PopupMenuShowing(object sender, DevExpress.XtraPdfViewer.PdfPopupMenuShowingEventArgs e)
        {
            e.ItemLinks.Clear();
        }

        private void Viewer_KeyDown(object sender, KeyEventArgs e)
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
                XtraMessageBox.Show(TPConfigs.NoPermission, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            var dialogResult = viewPDF.ShowPrintPageSetupDialog();
            if (dialogResult == null) return;

            viewPDF.Print(dialogResult);

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var IsProcessing = db.dt207_DocProgress.Any(r => r.IdKnowledgeBase == idKnowledgeBase && !(r.IsComplete));
                if (!IsProcessing)
                {
                    dt207_HistoryGetFile historyGetFile = new dt207_HistoryGetFile()
                    {
                        IdKnowledgeBase = idKnowledgeBase,
                        idTypeHisGetFile = 3,
                        KnowledgeAttachmentName = Text,
                        IdUser = TPConfigs.LoginId,
                        TimeGet = DateTime.Now
                    };

                    db.dt207_HistoryGetFile.Add(historyGetFile);
                    db.SaveChanges();
                }
            }

            XtraMessageBox.Show("列印文件成功！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CanSaveFile != true)
            {
                XtraMessageBox.Show(TPConfigs.NoPermission, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                    var IsProcessing = db.dt207_DocProgress.Any(r => r.IdKnowledgeBase == idKnowledgeBase && !(r.IsComplete));
                    if (!IsProcessing)
                    {
                        dt207_HistoryGetFile historyGetFile = new dt207_HistoryGetFile()
                        {
                            IdKnowledgeBase = idKnowledgeBase,
                            idTypeHisGetFile = 2,
                            KnowledgeAttachmentName = Text,
                            IdUser = TPConfigs.LoginId,
                            TimeGet = DateTime.Now
                        };

                        db.dt207_HistoryGetFile.Add(historyGetFile);
                        db.SaveChanges();
                    }
                }

                XtraMessageBox.Show("下載文件成功！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}