using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Pdf;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraSpreadsheet.Model;
using DocumentFormat.OpenXml.Wordprocessing;
using KnowledgeSystem.Helpers;
using Spire.Presentation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class f402_Watermark_Info : DevExpress.XtraEditors.XtraForm
    {
        public f402_Watermark_Info()
        {
            InitializeComponent();
            InitializeIcon();

            pdfPreview.NavigationPanePageVisibility = PdfNavigationPanePageVisibility.None;
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
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

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName;
        public dm_Watermark watermark = null;

        string sourcePdf = Path.Combine(TPConfigs.ResourcesPath, "blank.pdf");

        // Các thông tin để vẽ VM
        string picImage = "";
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

        void DisplayPdfWithWatermark(string inputPath)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(pdfPreview))
            {
                // Create a PDF Document Processor
                using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                {
                    processor.LoadDocument(inputPath);
                    var page = processor.Document.Pages[0];
                    Image mark = picVM.Image;
                    mark = SetImageOpacity(new Bitmap(mark, mark.Width / 2, mark.Height / 2), (float)opacity, (float)rotation);

                    using (PdfGraphics graphics = processor.CreateGraphics())
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

                            lcPreview.Text = $"預覽 - Rectangle: {pdfRectangle.Width:N1} - Image: {scaledWidth:N1}";
                            Rectangle rec = new Rectangle((int)x, (int)y, (int)scaledWidth, (int)scaledHeight);
                            graphics.DrawRectangle(new Pen(System.Drawing.Color.Red), rec);
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

        private void EnabledController(bool _enable = true)
        {
            txbName.Enabled = _enable;
            txbDesc.Enabled = _enable;
            cbbRotarion.Enabled = _enable;
            txbScale.Enabled = _enable;
            txbOpacity.Enabled = _enable;
            txbOffsetHoriz.Enabled = _enable;
            txbOffsetVert.Enabled = _enable;
        }

        private void LockControl()
        {
            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }
        }

        private void f402_Watermark_Info_Load(object sender, EventArgs e)
        {
            LockControl();

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    watermark = new dm_Watermark();
                    break;
                case EventFormInfo.View:
                    txbName.EditValue = watermark.DisplayName;
                    txbDesc.EditValue = watermark.Describe;
                    cbbRotarion.EditValue = watermark.Rotation;
                    txbScale.EditValue = watermark.Scale;
                    txbOpacity.EditValue = watermark.Opacity;
                    txbOffsetHoriz.EditValue = watermark.HoriDistance;
                    txbOffsetVert.EditValue = watermark.VertDistance;
                    picVM.Image = Image.FromFile(Path.Combine(TPConfigs.FolderWatermark, watermark.PicImage));

                    opacity = (float)watermark.Opacity / 100f;
                    rotation = (float)watermark.Rotation;
                    offsetVert = (float)watermark.VertDistance;
                    offsetHori = (float)watermark.HoriDistance;
                    scale = (float)watermark.Scale;

                    break;
                case EventFormInfo.Update:
                    break;
                case EventFormInfo.Delete:
                    break;
                default:
                    break;
            }
        }

        private void txbOpacity_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (int.TryParse(txbOpacity.Text, out int value))
            {
                opacity = value / 100f;
            }
        }

        private void cbbRotarion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(cbbRotarion.Text, out int value))
            {
                rotation = value / 1f;
            }
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

        private void btnBrowse_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            picImage = ofd.FileName;
            picVM.Image = Image.FromFile(picImage);
        }

        private void btnPasteImage_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();
                var imageFiles = new List<string>();
                string[] supportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };

                foreach (var file in files)
                {
                    if (supportedExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) && File.Exists(file))
                    {
                        imageFiles.Add(file);
                    }
                }

                if (imageFiles.Count != 1)
                {
                    XtraMessageBox.Show("請選擇一個照片檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                picImage = imageFiles.First();
                picVM.Image = Image.FromFile(picImage);
            }
            else
            {
                XtraMessageBox.Show("請選擇一個照片檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (picVM.Image == null) return;

            DisplayPdfWithWatermark(sourcePdf);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                watermark.DisplayName = txbName.EditValue?.ToString();
                watermark.Describe = txbDesc.EditValue?.ToString();
                watermark.Rotation = (int)rotation;
                watermark.Scale = (int)scale;
                watermark.Opacity = (int)(opacity * 100);
                watermark.VertDistance = (int)offsetVert;
                watermark.HoriDistance = (int)offsetHori;

                msg = $"{watermark.DisplayName} {watermark.Describe}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        if (string.IsNullOrEmpty(picImage)) return;

                        watermark.PicImage = EncryptionHelper.EncryptionFileName(Path.GetFileName(picImage));
                        if (!Directory.Exists(TPConfigs.FolderSign)) Directory.CreateDirectory(TPConfigs.FolderWatermark);
                        File.Copy(picImage, Path.Combine(TPConfigs.FolderWatermark, watermark.PicImage));

                        result = dm_WatermarkBUS.Instance.Add(watermark);

                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:

                        result = true;
                        if (!string.IsNullOrEmpty(picImage))
                        {
                            watermark.PicImage = EncryptionHelper.EncryptionFileName(Path.GetFileName(picImage));
                            if (!Directory.Exists(TPConfigs.FolderSign)) Directory.CreateDirectory(TPConfigs.FolderWatermark);
                            File.Copy(picImage, Path.Combine(TPConfigs.FolderWatermark, watermark.PicImage));
                        }

                        result = dm_WatermarkBUS.Instance.AddOrUpdate(watermark);

                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"Bạn xác nhận muốn xoá Watermark: {watermark.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        result = dm_WatermarkBUS.Instance.RemoveById(watermark.ID);
                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
        }
    }
}