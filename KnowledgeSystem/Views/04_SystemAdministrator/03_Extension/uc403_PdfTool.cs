using DevExpress.Pdf;
using DevExpress.XtraPdfViewer;
using KnowledgeSystem.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension
{
    public partial class uc403_PdfTool : DevExpress.XtraEditors.XtraUserControl
    {
        public uc403_PdfTool()
        {
            InitializeComponent();
        }

        string urlFile;

        PdfViewer pdfViewer1 = new PdfViewer();

        async void LoadPDFFile(string pdf_url)
        {
            using (WebClient client = new WebClient())
            {
                var data = await client.DownloadDataTaskAsync(new Uri(pdf_url));
                pdfViewer1.DetachStreamAfterLoadComplete = true;
                using (MemoryStream ms = new MemoryStream(data))
                {
                    pdfViewer1.LoadDocument(ms);
                    urlFile = pdf_url;
                }
            }
        }

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
                LoadPDFFile(resultFileName);
            }
        }

        static void DrawImageToPage(DevExpress.Pdf.PdfPage page, PdfDocumentProcessor documentProcessor)
        {
            using (PdfGraphics graphics = documentProcessor.CreateGraphics())
            {
                Image mark = Image.FromFile(Path.Combine(TPConfigs.Folder403, "no-image.png"));

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


        private void AddWatermark(string text, string fileName, string resultFileName)
        {
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                string fontName = "Arial Black";
                int fontSize = 12;
                PdfStringFormat stringFormat = PdfStringFormat.GenericTypographic;
                stringFormat.Alignment = PdfStringAlignment.Center;
                stringFormat.LineAlignment = PdfStringAlignment.Center;
                documentProcessor.LoadDocument(fileName);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(90, Color.Red)))
                {
                    using (Font font = new Font(fontName, fontSize))
                    {
                        foreach (var page in documentProcessor.Document.Pages)
                        {
                            var watermarkSize = page.CropBox.Width * 0.75;
                            using (PdfGraphics graphics = documentProcessor.CreateGraphics())
                            {
                                SizeF stringSize = graphics.MeasureString(text, font);
                                Single scale = Convert.ToSingle(watermarkSize / stringSize.Width);
                                graphics.TranslateTransform(Convert.ToSingle(page.CropBox.Width * 0.5), Convert.ToSingle(page.CropBox.Height * 0.5));
                                graphics.RotateTransform(-45);
                                graphics.TranslateTransform(Convert.ToSingle(-stringSize.Width * scale * 0.5), Convert.ToSingle(-stringSize.Height * scale * 0.5));
                                using (Font actualFont = new Font(fontName, fontSize * scale))
                                {
                                    RectangleF rect = new RectangleF(0, 0, stringSize.Width * scale, stringSize.Height * scale);
                                    graphics.DrawString(text, actualFont, brush, rect, stringFormat);
                                }
                                graphics.AddToPageForeground(page, 72, 72);
                            }
                        }
                    }
                }
                documentProcessor.SaveDocument(resultFileName);
            }
        }

        public static Stream CopyStream(Stream input)
        {
            Stream output = new MemoryStream();
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
            return output;
        }

        private void btnWatermark_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddWatermarkImage(urlFile, urlFile);
        }

        private void uc403_PdfTool_Load(object sender, EventArgs e)
        {
            LoadPDFFile(@"C:\Users\ANHTUAN\Desktop\New folder\A1 - Copy.pdf");
        }
    }
}
