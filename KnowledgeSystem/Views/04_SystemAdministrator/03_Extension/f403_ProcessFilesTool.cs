using DevExpress.ChartRangeControlClient.Core;
using DevExpress.Export.Xl;
using DevExpress.Pdf;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Core.Model;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension
{
    public partial class f403_ProcessFilesTool : DevExpress.XtraEditors.XtraForm
    {
        public f403_ProcessFilesTool()
        {
            InitializeComponent();

            layoutStatus.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder containing files";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() != DialogResult.OK) return;

                FOLDER_PATH = folderDialog.SelectedPath;

                cbbFunction.Properties.Items.AddRange(funcs);
                cbbFunction.SelectedIndex = 0;
            }
        }

        List<string> funcs = new List<string>() { "FHS.Watermark" };

        string FOLDER_PATH;
        bool ISSTOP = true;

        List<string> files;

        void AddWatermarkImage(string fileName, string resultFileName)
        {
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(fileName);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(90, Color.Red)))
                {
                    foreach (var page in documentProcessor.Document.Pages)
                    {
                        DrawImageToPage(page, documentProcessor);
                    }
                }
                documentProcessor.SaveDocument(resultFileName);
            }
        }

        static void DrawImageToPage(DevExpress.Pdf.PdfPage page, PdfDocumentProcessor documentProcessor)
        {
            using (PdfGraphics graphics = documentProcessor.CreateGraphics())
            {
                Image mark = Image.FromFile(Path.Combine(TPConfigs.Folder403, "watermark FHS.png"));

                int rt = page.Rotate;
                using (Bitmap image = new Bitmap(mark, mark.Width / 2, mark.Height / 2))
                {
                    PdfRectangle pdfRectangle = page.CropBox;
                    float cropBoxWidth = (float)pdfRectangle.Width;
                    float cropBoxHeight = (float)pdfRectangle.Height;

                    switch (page.Rotate)
                    {
                        case 90:
                        case 270:
                            cropBoxWidth = (float)pdfRectangle.Height;
                            cropBoxHeight = (float)pdfRectangle.Width;
                            break;
                    }

                    Rectangle rec = new Rectangle(0, 0, (int)cropBoxWidth, (int)cropBoxHeight);
                    graphics.DrawImage(SetImageOpacity(image, (float)0.1), rec);
                }
                graphics.AddToPageForeground(page, 72, 72);
            }
        }

        public static Image SetImageOpacity(Image image, float opacity = (float)0.2)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);
                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();
                    //set the opacity  
                    matrix.Matrix33 = opacity;
                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();
                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void ProcessWaterMark()
        {
            string resultFolder = Path.Combine(Path.GetDirectoryName(files.First()), $"Result-{DateTime.Now:yyMMddHHmmss}");
            if (!Directory.Exists(resultFolder))
                Directory.CreateDirectory(resultFolder);

            int index = 0;
            foreach (var item in files)
            {
                if (ISSTOP) return;

                string fileName = Path.GetFileName(item);
                string resultPath = Path.Combine(resultFolder, fileName);

                if (IsHandleCreated)
                {
                    progressBar.Invoke(new Action(() => { layoutStatus.Text = $"{index + 1}/{files.Count}: {fileName}"; }));
                }

                try
                {
                    AddWatermarkImage(item, resultPath);
                }
                catch { }

                index++;

                if (IsHandleCreated)
                {
                    progressBar.Invoke(new Action(() =>
                    {
                        progressBar.PerformStep();
                        progressBar.Update();
                    }));

                    lsFileComplete.Invoke(new Action(() => { lsFileComplete.Items.Add($"{index}: {fileName}"); }));
                }
            }

            ISSTOP = true;
        }

        private void f403_ProcessFilesTool_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FOLDER_PATH))
            {
                Close();
                return;
            }

            string searchPattern = "*.*";
            switch (cbbFunction.SelectedIndex)
            {
                case 0:
                    searchPattern = "*.pdf";
                    break;
            }
            files = Directory.GetFiles(FOLDER_PATH, searchPattern, SearchOption.TopDirectoryOnly).ToList();

            btnStart.Text = $"執行<color=red>「{files.Count()}」</color>檔案";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!ISSTOP)
            {
                DialogResult dialogResult = XtraMessageBox.Show("Chương trình đang chạy bạn muốn dừng không ?", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult != DialogResult.Yes) return;

                ISSTOP = true;
                return;
            }

            if (files.Any(r => r.StartsWith("~")))
            {
                XtraMessageBox.Show("Vui lòng tắt hết các tệp Excel trong thư mục !", TPConfigs.SoftNameTW);
                return;
            }

            lsFileComplete.Items.Clear();
            layoutStatus.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

            progressBar.EditValue = 0;
            progressBar.Properties.Step = 1;
            progressBar.Properties.PercentView = true;
            progressBar.Properties.Maximum = files.Count;
            progressBar.Properties.Minimum = 0;
            progressBar.ShowProgressInTaskBar = true;

            System.Threading.Thread thrd = new System.Threading.Thread(() =>
            {
                switch (cbbFunction.SelectedIndex)
                {
                    case 0:
                        ProcessWaterMark();
                        break;
                }
            });
            thrd.Name = "Process";
            thrd.Priority = ThreadPriority.Highest;
            thrd.IsBackground = true;
            thrd.SetApartmentState(ApartmentState.STA);

            ISSTOP = false;
            thrd.Start();
        }
    }
}