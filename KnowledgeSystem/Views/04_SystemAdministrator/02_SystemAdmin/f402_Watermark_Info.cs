using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Pdf;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraSpreadsheet.Model;
using Spire.Presentation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class f402_Watermark_Info : DevExpress.XtraEditors.XtraForm
    {
        public f402_Watermark_Info()
        {
            InitializeComponent();

            pdfPreview.NavigationPanePageVisibility = PdfNavigationPanePageVisibility.None;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MAXIMIZE = 0xF030;
            const int WM_NCLBUTTONDBLCLK = 0x00A3; // Nhấp đúp vào thanh tiêu đề (non-client double-click)

            // Chặn lệnh phóng to khi nhấp đúp vào thanh tiêu đề
            if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() == SC_MAXIMIZE))
            {
                return; // Bỏ qua lệnh phóng to
            }

            // Chặn nhấp đúp vào thanh tiêu đề (non-client area)
            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                return; // Bỏ qua thao tác nhấp đúp
            }

            base.WndProc(ref m); // Xử lý các thông điệp khác bình thường
        }


        string sourcePdf = @"C:\Users\Dell Alpha\Desktop\blank-pdf.pdf";
        string destPdf = @"C:\Users\Dell Alpha\Desktop\mofi-pdf.pdf";
        string vmPath = @"C:\Users\Dell Alpha\Desktop\RÁC 1\Test.jpg";

        // Các thông tin để vẽ VM
        float opacity = 0.1f;
        float rotation = 0;
        float offsetVert = 0;
        float offsetHori = 0;
        float scale = 100;

        public static Image SetImageOpacity(Image image, float opacity = 0.2f, float rotationAngle = 0f)
        {
            try
            {
                // Tạo bitmap với opacity
                Bitmap bmp = new Bitmap(image.Width, image.Height);
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    gfx.Clear(Color.Transparent);
                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gfx.SmoothingMode = SmoothingMode.AntiAlias;

                    // Tạo ColorMatrix để thiết lập độ mờ
                    ColorMatrix matrix = new ColorMatrix
                    {
                        Matrix33 = opacity
                    };
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    // Vẽ ảnh với opacity
                    gfx.DrawImage(
                        image,
                        new Rectangle(0, 0, image.Width, image.Height),
                        0,
                        0,
                        image.Width,
                        image.Height,
                        GraphicsUnit.Pixel,
                        attributes
                    );
                }

                if (rotationAngle != 0)
                {
                    // Tính kích thước cho bitmap xoay
                    int newWidth = (int)(image.Width * Math.Abs(Math.Cos(rotationAngle * Math.PI / 180)) + image.Height * Math.Abs(Math.Sin(rotationAngle * Math.PI / 180)));
                    int newHeight = (int)(image.Height * Math.Abs(Math.Cos(rotationAngle * Math.PI / 180)) + image.Width * Math.Abs(Math.Sin(rotationAngle * Math.PI / 180)));

                    // Tạo bitmap lớn hơn để chứa ảnh xoay
                    Bitmap bmp1 = new Bitmap(newWidth, newHeight);
                    using (Graphics gfx = Graphics.FromImage(bmp1))
                    {
                        gfx.Clear(Color.Transparent);
                        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        gfx.SmoothingMode = SmoothingMode.AntiAlias;

                        // Di chuyển đến tâm của bitmap mới
                        gfx.TranslateTransform(newWidth / 2f, newHeight / 2f);

                        // Xoay
                        gfx.RotateTransform(rotationAngle);

                        // Vẽ ảnh gốc vào giữa
                        gfx.DrawImage(bmp, -bmp.Width / 2f, -bmp.Height / 2f);
                    }

                    bmp.Dispose();
                    return bmp1;
                }

                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        void DisplayPdfWithWatermark(string inputPath, string imagePath)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(pdfPreview))
            {
                // Create a PDF Document Processor
                using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                {
                    processor.LoadDocument(inputPath);
                    var page = processor.Document.Pages[0];
                    Image mark = Image.FromFile(imagePath);

                    Bitmap image1 = new Bitmap(mark, mark.Width / 2, mark.Height / 2);

                    mark = SetImageOpacity(image1, (float)opacity, (float)rotation);

                    using (PdfGraphics graphics = processor.CreateGraphics())
                    {
                        int rt = page.Rotate;
                        using (Bitmap image = new Bitmap(mark, mark.Width / 2, mark.Height / 2))
                        {
                            PdfRectangle pdfRectangle = page.CropBox;
                            //float cropBoxWidth = (float)pdfRectangle.Width;
                            float cropBoxHeight = (float)pdfRectangle.Height;

                            float imageAspectRatio = (float)image.Width / image.Height;
                            float cropBoxWidth = cropBoxHeight * imageAspectRatio;

                            switch (page.Rotate)
                            {
                                case 90:
                                case 270:
                                    cropBoxWidth = (float)pdfRectangle.Height;
                                    cropBoxHeight = (float)pdfRectangle.Width;
                                    break;
                            }

                            // Áp dụng scale cho kích thước rectangle
                            float scaledWidth = cropBoxWidth * (scale / 100f);
                            float scaledHeight = cropBoxHeight * (scale / 100f);

                            // Tính toán vị trí để giữ rectangle ở giữa trang và áp dụng offset
                            float x = ((float)pdfRectangle.Width - scaledWidth) / 2 + offsetHori;
                            float y = ((float)pdfRectangle.Height - scaledHeight) / 2 + offsetVert;

                            layoutControlGroup1.Text = $"Image: {mark.Width}, Rec: {cropBoxWidth}";

                            Rectangle rec = new Rectangle((int)x, (int)y, (int)scaledWidth, (int)scaledHeight);

                            graphics.DrawRectangle(new Pen(Color.Red), rec);

                            graphics.DrawImage(mark, rec);
                        }
                        graphics.AddToPageForeground(page, 72, 72);
                    }
                    // Save the modified document to a MemoryStream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        processor.SaveDocument(memoryStream);
                        memoryStream.Position = 0;

                        // Load the PDF directly from MemoryStream to PdfViewer
                        pdfPreview.LoadDocument(memoryStream);
                        pdfPreview.Refresh();

                        SizeF currentPageSize = pdfPreview.GetPageSize(pdfPreview.CurrentPageNumber);
                        float dpi = 110f;
                        float pageHeightPixel = currentPageSize.Height * dpi;
                        float topBottomOffset = 40f;
                        pdfPreview.ZoomMode = PdfZoomMode.Custom;
                        pdfPreview.ZoomFactor = ((float)pdfPreview.ClientSize.Height - topBottomOffset) / pageHeightPixel * 100f;
                    }
                }
            }
        }

        private void f402_Watermark_Info_Load(object sender, EventArgs e)
        {
            picVM.Image = Image.FromFile(vmPath);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pdfPreview.CloseDocument();
        }

        private void txbOpacity_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (int.TryParse(txbOpacity.Text, out int value))
            {
                opacity = value / 100f;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            DisplayPdfWithWatermark(sourcePdf, vmPath);
        }

        private void cbbRotarion_SelectedIndexChanged(object sender, EventArgs e)
        {
            int.TryParse(cbbRotarion.Text, out int value);
            rotation = value / 1f;
        }

        private void txbOffsetVert_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (float.TryParse(txbOffsetVert.Text, out float value))
            {
                offsetVert = value;
            }
        }

        private void txbOffsetHoriz_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (float.TryParse(txbOffsetHoriz.Text, out float value))
            {
                offsetHori = value;
            }
        }

        private void txbScale_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (float.TryParse(txbScale.Text, out float value))
            {
                scale = value;
            }
        }
    }
}