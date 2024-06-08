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
        public f00_VIewFile(string _filePath, bool isCanSave = true, bool isCanConfirm = false)
        {
            InitializeComponent();
            filePath = _filePath;
            sourceFile = _filePath;
            IsCanSave = isCanSave;
            IsCanConfirm = isCanConfirm;
        }

        public bool IsConfirm { get; set; }
        public string Describe { get; set; }

        enum FileType
        {
            Pdf,
            Word,
            Excel,
            PowerPoint,
            Image,
            Unknown
        }

        FileType fileType;
        PdfViewer viewPDF;
        RichEditControl viewWord;
        SpreadsheetControl viewExcel;
        PictureBox viewPic;

        bool IsCanSave = true;
        bool IsCanConfirm = false;
        string filePath = "";
        string sourceFile = "";

        private void f00_VIewFile_Load(object sender, EventArgs e)
        {
            if (!IsCanSave)
            {
                btnSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnPrint.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            if (!IsCanConfirm)
            {
                btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                string fileName = Path.GetFileName(filePath);
                Text = fileName;

                fileType = GetFileType(filePath);

                switch (fileType)
                {
                    case FileType.Pdf:
                        viewPDF = new PdfViewer();
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
                        viewWord = new RichEditControl();
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
                        viewExcel = new SpreadsheetControl();
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
                        viewPic = new PictureBox();
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
            if ((e.Control && (e.KeyCode == Keys.P || e.KeyCode == Keys.S)) && !IsCanSave)
            {
                e.SuppressKeyPress = true;
                return;
            }
        }

        private void viewPDF_PopupMenuShowing(object sender, PdfPopupMenuShowingEventArgs e)
        {
            e.ItemLinks.Clear();
        }

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!IsCanSave) return;

            switch (fileType)
            {
                case FileType.Pdf:
                case FileType.PowerPoint:
                    viewPDF.ShowPrintPageSetupDialog();
                    break;
                case FileType.Word:
                    viewWord.ShowPrintDialog();
                    break;
                case FileType.Excel:
                    viewExcel.ShowPrintDialog();
                    break;
                default:
                    string msg = "<font='Microsoft JhengHei UI' size=14>不支援文件打印\r\nKhông hỗ trợ in định dạng tệp tin</font>";
                    MsgTP.MsgShowInfomation(msg);
                    Close();
                    break;
            }
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!IsCanSave) return;

            string extension = Path.GetExtension(sourceFile).ToLower();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = $"{extension}|*{extension}";
            saveFileDialog.FileName = Path.GetFileName(sourceFile);

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            string destFile = saveFileDialog.FileName;

            File.Copy(sourceFile, destFile, true);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IsConfirm = true;
            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;
            Describe = result?.ToString() ?? "";
            IsConfirm = true;

            Close();
        }
    }
}