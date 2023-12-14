using DevExpress.XtraEditors;
using DevExpress.XtraEditors.TextEditController.InputHandler;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraRichEdit;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraSpreadsheet;
using DocumentFormat.OpenXml.Drawing.Charts;
using KnowledgeSystem.Helpers;
using Spire.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_VIewFile : DevExpress.XtraEditors.XtraForm
    {
        public f00_VIewFile(string _filePath)
        {
            InitializeComponent();
            filePath = _filePath;
        }

        enum FileType
        {
            Pdf,
            Word,
            Excel,
            PowerPoint,
            Image,
            Unknown
        }

        string filePath = "";

        private void f00_VIewFile_Load(object sender, EventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                string fileName = Path.GetFileName(filePath);
                Text = fileName;

                FileType fileType = GetFileType(filePath);

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
                        viewPDF.DocumentFilePath = filePath;

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
                        viewWord.Document.LoadDocument(filePath);

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
                        viewExcel.Document.LoadDocument(filePath);

                        Controls.Add(viewExcel);
                        break;
                    case FileType.PowerPoint:
                        Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);

                        string outputPath = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:MMddhhmmss} PPTConvertPDF.pdf");
                        using (Presentation presentation = new Presentation())
                        {
                            presentation.LoadFromFile(filePath);
                            presentation.SaveToFile(outputPath, FileFormat.PDF);
                        }

                        filePath = outputPath;
                        goto case FileType.Pdf;
                    case FileType.Image:
                        PictureBox viewPic = new PictureBox();
                        viewPic.Name = "viewPic";
                        viewPic.Text = "viewPic";
                        viewPic.Dock = DockStyle.Fill;
                        viewPic.KeyDown += new KeyEventHandler(Viewer_KeyDown);
                        viewPic.Image = Image.FromFile(filePath);
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
    }
}