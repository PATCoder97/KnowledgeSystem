using BusinessLayer;
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
using Spire.Presentation;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_ViewFile : DevExpress.XtraEditors.XtraForm
    {
        public bool? CanSaveFile { get; set; }

        public f207_ViewFile(string documentFile_, string IdKnowledgeBase_)
        {
            InitializeComponent();
            documentFile = documentFile_;
            idKnowledgeBase = IdKnowledgeBase_;
        }

        dt207_DocProgressBUS _dt207_DocProgress = new dt207_DocProgressBUS();
        dt207_HistoryGetFileBUS _dt207_HistoryGetFileBUS = new dt207_HistoryGetFileBUS();

        string documentFile = "";
        string idKnowledgeBase = "";


        private void f207_ViewFile_Load(object sender, EventArgs e)
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
                    case ".pptx":
                    case ".ppt":
                        Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);

                        string outputPath = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:MMddhhmmss} PPTConvertPDF.pdf");
                        // Load the PowerPoint presentation
                        using (Presentation presentation = new Presentation())
                        {
                            presentation.LoadFromFile(documentFile);

                            // Convert the presentation to PDF
                            presentation.SaveToFile(outputPath, FileFormat.PDF);
                        }

                        viewPDF.DocumentFilePath = outputPath;
                        lcPDF.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
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

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CanSaveFile != true)
            {
                XtraMessageBox.Show(TPConfigs.NoPermission, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = TPConfigs.SoftNameTW;
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.Filter = TPConfigs.FilterFile;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = $"{DateTime.Now:yyyyMMddHHmmss}-{Text}";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.Copy(documentFile, saveFileDialog1.FileName, true);

                var IsProcessing = _dt207_DocProgress.CheckItemProcessing(idKnowledgeBase);
                if (!IsProcessing)
                {
                    dt207_HistoryGetFile historyGetFile = new dt207_HistoryGetFile()
                    {
                        IdKnowledgeBase = idKnowledgeBase,
                        TypeGet = TPConfigs.strSaveFile,
                        KnowledgeAttachmentName = Text,
                        IdUser = TPConfigs.LoginUser.Id,
                        TimeGet = DateTime.Now
                    };
                    _dt207_HistoryGetFileBUS.Create(historyGetFile);
                }

                XtraMessageBox.Show("下載文件成功！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}