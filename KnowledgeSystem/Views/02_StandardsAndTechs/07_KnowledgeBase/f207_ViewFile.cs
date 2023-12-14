using BusinessLayer;
using DataAccessLayer;
using DevExpress.Pdf;
using DevExpress.Utils.CommonDialogs;
using DevExpress.XtraEditors;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraRichEdit;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraSpreadsheet;
using KnowledgeSystem.Helpers;
using Spire.Presentation;
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
//using Spire.Presentation;

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

        dt207_HistoryGetFileBUS _dt207_HistoryGetFileBUS = new dt207_HistoryGetFileBUS();

        string documentFile = "";
        string idKnowledgeBase = "";

        enum FileType
        {
            Pdf,
            Word,
            Excel,
            PowerPoint,
            Image,
            Unknown
        }

        static FileType GetFileType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".pdf":
                    return FileType.Pdf;
                case ".doc":
                case ".docx":
                    return FileType.Word;
                case ".xls":
                case ".xlsx":
                    return FileType.Excel;
                case ".ppt":
                case ".pptx":
                    return FileType.PowerPoint;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    return FileType.Image;
                default:
                    return FileType.Unknown;
            }
        }

        private void f207_ViewFile_Load(object sender, EventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                string fileName = Path.GetFileName(documentFile);
                Text = fileName;

                FileType fileType = GetFileType(documentFile);

                switch (fileType)
                {
                    case FileType.Pdf:
                        PdfViewer viewPDF = new PdfViewer();
                        viewPDF.ReadOnly = true;
                        viewPDF.Name = "viewPDF";
                        viewPDF.NavigationPanePageVisibility = PdfNavigationPanePageVisibility.None;
                        viewPDF.PopupMenuShowing += new PdfPopupMenuShowingEventHandler(viewPDF_PopupMenuShowing);
                        viewPDF.KeyDown += new KeyEventHandler(Viewer_KeyDown);
                        viewPDF.Dock = DockStyle.Fill;
                        viewPDF.DocumentFilePath = documentFile;
                        viewPDF.MenuManager = this.barManager1;

                        Controls.Add(viewPDF);
                        break;
                    case FileType.Word:
                        RichEditControl viewWord = new RichEditControl();
                        viewWord.Name = "viewWord";
                        viewWord.ReadOnly = true;
                        viewWord.Text = "viewWord";
                        viewWord.Dock = DockStyle.Fill;
                        viewWord.KeyDown += new KeyEventHandler(Viewer_KeyDown);
                        viewWord.PopupMenuShowing += ViewWord_PopupMenuShowing;
                        viewWord.Document.LoadDocument(documentFile);

                        Controls.Add(viewWord);
                        break;
                    case FileType.Excel:
                        SpreadsheetControl viewExcel = new SpreadsheetControl();
                        viewExcel.Name = "viewExcel";
                        viewExcel.ReadOnly = true;
                        viewExcel.Text = "viewExcel";
                        viewExcel.Dock = DockStyle.Fill;
                        viewExcel.PopupMenuShowing += ViewExcel_PopupMenuShowing;
                        viewExcel.KeyDown += new KeyEventHandler(Viewer_KeyDown);
                        viewExcel.Document.LoadDocument(documentFile);

                        Controls.Add(viewExcel);
                        break;
                    case FileType.PowerPoint:
                        Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);

                        string outputPath = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:MMddhhmmss} PPTConvertPDF.pdf");
                        using (Presentation presentation = new Presentation())
                        {
                            presentation.LoadFromFile(documentFile);
                            presentation.SaveToFile(outputPath, FileFormat.PDF);
                        }

                        documentFile = outputPath;
                        goto case FileType.Pdf;
                    case FileType.Image:
                        PictureBox viewPic = new PictureBox();
                        viewPic.Name = "viewPic";
                        viewPic.Text = "viewPic";
                        viewPic.Dock = DockStyle.Fill;
                        viewPic.KeyDown += new KeyEventHandler(Viewer_KeyDown);
                        viewPic.Image = Image.FromFile(documentFile);
                        viewPic.SizeMode = PictureBoxSizeMode.Zoom;

                        Controls.Add(viewPic);
                        break;
                    default:
                        string msg = "<font='Microsoft JhengHei UI' size=14>不支援文件預覽\r\nKhông hỗ trợ xem trước định dạng tệp tin</font>";
                        MsgTP.MsgShowInfomation(msg);
                        Close();
                        break;
                }
            }

            bar2.Visible = !string.IsNullOrEmpty(idKnowledgeBase);
        }

        private void ViewExcel_PopupMenuShowing(object sender, DevExpress.XtraSpreadsheet.PopupMenuShowingEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void ViewWord_PopupMenuShowing(object sender, DevExpress.XtraRichEdit.PopupMenuShowingEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void Viewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.P || e.KeyCode == Keys.S))
            {
                e.SuppressKeyPress = true;
                return;
            }
        }

        private void viewPDF_PopupMenuShowing(object sender, PdfPopupMenuShowingEventArgs e)
        {
            e.ItemLinks.Clear();
        }

        private void pdfViewerData_PopupMenuShowing(object sender, DevExpress.XtraPdfViewer.PdfPopupMenuShowingEventArgs e)
        {
            e.ItemLinks.Clear();
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CanSaveFile != true)
            {
                MsgTP.MsgNoPermission();
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

                var IsProcessing = dt207_DocProcessingBUS.Instance.CheckItemProcessing(idKnowledgeBase);
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