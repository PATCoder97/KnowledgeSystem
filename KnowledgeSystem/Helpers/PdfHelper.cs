using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Pdf;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;

namespace KnowledgeSystem.Helpers
{
    public class PdfHelper
    {
        private static PdfHelper instance;

        public static PdfHelper Instance
        {
            get { if (instance == null) instance = new PdfHelper(); return instance; }
            private set { instance = value; }
        }

        private PdfHelper()
        {
        }

        public void AddWatermarkImage(string filePath, string outFilePath, Image image, int scale, int offsetHori, int offsetVert)
        {
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument(filePath);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(90, Color.Red)))
                {
                    foreach (var page in documentProcessor.Document.Pages)
                    {
                        DrawImageToPage(page, documentProcessor, image, scale, offsetHori, offsetVert);
                    }
                }

                PdfEncryptionOptions encryptionOptions = new PdfEncryptionOptions();
                encryptionOptions.ModificationPermissions = PdfDocumentModificationPermissions.NotAllowed;
                encryptionOptions.InteractivityPermissions = PdfDocumentInteractivityPermissions.NotAllowed;

                encryptionOptions.OwnerPasswordString = "fhspdf";
                encryptionOptions.Algorithm = PdfEncryptionAlgorithm.AES256;

                documentProcessor.SaveDocument(outFilePath, new PdfSaveOptions() { EncryptionOptions = encryptionOptions });
            }
        }

        static void DrawImageToPage(DevExpress.Pdf.PdfPage page, PdfDocumentProcessor documentProcessor, Image mark, int scale, int offsetHori, int offsetVert)
        {
            using (PdfGraphics graphics = documentProcessor.CreateGraphics())
            {
                int rt = page.Rotate;
                using (Bitmap image = new Bitmap(mark, mark.Width / 2, mark.Height / 2))
                {
                    PdfRectangle pdfRectangle = page.CropBox;
                    float cropBoxHeight = (float)pdfRectangle.Height;
                    float imageAspectRatio = (float)image.Width / image.Height;
                    float cropBoxWidth = cropBoxHeight * imageAspectRatio;

                    // Áp dụng scale cho kích thước rectangle
                    float scaledWidth = cropBoxWidth * (scale / 100f);
                    float scaledHeight = cropBoxHeight * (scale / 100f);

                    // Tính toán vị trí để giữ rectangle ở giữa trang và áp dụng offset
                    float x = ((float)pdfRectangle.Width - scaledWidth) / 2 + offsetHori;
                    float y = ((float)pdfRectangle.Height - scaledHeight) / 2 + offsetVert;

                    switch (page.Rotate)
                    {
                        case 90:
                        case 270:
                            scaledWidth = scaledHeight;
                            scaledHeight = scaledWidth;
                            break;
                    }

                    Rectangle rec = new Rectangle((int)x, (int)y, (int)scaledWidth, (int)scaledHeight);
                    //graphics.DrawRectangle(new Pen(System.Drawing.Color.Red), rec);
                    graphics.DrawImage(mark, rec);
                }
                graphics.AddToPageForeground(page, 72, 72);
            }
        }

        public static Image EditImage(Image image, float opacity = 0.2f, float rotationAngle = 0f)
        {
            try
            {
                // Tạo bitmap với opacity
                Bitmap bmp = new Bitmap(image.Width, image.Height);
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    gfx.Clear(System.Drawing.Color.Transparent);
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
                        gfx.Clear(System.Drawing.Color.Transparent);
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
    }
}
