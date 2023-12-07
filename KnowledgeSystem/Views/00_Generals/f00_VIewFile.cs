using DevExpress.XtraEditors;
using DevExpress.XtraEditors.TextEditController.InputHandler;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraRichEdit;
using DevExpress.XtraSpreadsheet;
using DocumentFormat.OpenXml.Drawing.Charts;
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

        string filePath = "";

        private void f00_VIewFile_Load(object sender, EventArgs e)
        {
            string fileName = Path.GetFileName(filePath);
            string extension = Path.GetExtension(filePath).ToLower();

            Text = fileName;

            switch (extension)
            {
                case ".pdf":
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
                case ".xlsx":
                case ".xls":
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
                case ".docx":
                case ".doc":
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
                case ".pptx":
                case ".ppt":
                    //Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);

                    //string outputPath = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:MMddhhmmss} PPTConvertPDF.pdf");
                    //// Load the PowerPoint presentation
                    //using (Presentation presentation = new Presentation())
                    //{
                    //    presentation.LoadFromFile(documentFile);

                    //    // Convert the presentation to PDF
                    //    presentation.SaveToFile(outputPath, FileFormat.PDF);
                    //}

                    //viewPDF.DocumentFilePath = outputPath;
                    //lcPDF.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    break;
                default:
                    PictureBox viewPic = new PictureBox();
                    viewPic.Name = "viewPic";
                    viewPic.Text = "viewPic";
                    viewPic.Dock = DockStyle.Fill;
                    viewPic.KeyDown += new KeyEventHandler(Viewer_KeyDown);
                    viewPic.Image = Image.FromFile(filePath);
                    viewPic.SizeMode = PictureBoxSizeMode.Zoom;

                    Controls.Add(viewPic);
                    break;
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