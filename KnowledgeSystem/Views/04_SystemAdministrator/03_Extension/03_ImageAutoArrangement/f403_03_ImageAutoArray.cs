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
using DevExpress.XtraEditors;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using DevExpress.XtraSplashScreen;
using Spire.Presentation.Drawing;
using Spire.Presentation;
using KnowledgeSystem.Helpers;
using System.Diagnostics;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._03_ImageAutoArrangement
{
    public partial class f403_03_ImageAutoArray : DevExpress.XtraEditors.XtraForm
    {
        public f403_03_ImageAutoArray()
        {
            InitializeComponent();
        }

        public void InsertImagesToExcel()
        {
            string folderPath = txbPath.Text.Trim();
            if (!Directory.Exists(folderPath))
            {
                XtraMessageBox.Show("路徑不存在!");
                return;
            }

            string filePath = Path.Combine(folderPath, $"Result-{DateTime.Now:yyyyMMddHHmmss}.xlsx");

            // Lấy danh sách file ảnh .jpg kết thúc bằng "200X"
            var allImages = Directory.GetFiles(folderPath, "*.jpg", SearchOption.AllDirectories)
                .Where(file => Path.GetFileNameWithoutExtension(file)
                .EndsWith(cbbZoom.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Hàm lấy tên thư mục cha
            string GetParentFolderName(string path)
            {
                var parts = path.Split(Path.DirectorySeparatorChar);
                return parts.Length > 1 ? parts[parts.Length - 2].Split('-')[0] : string.Empty;
            }

            // Hàm sắp xếp ảnh theo yêu cầu (1-4 A-Z, 5 Z-A)
            List<string> SortImages(List<string> images)
            {
                var group1to4 = images
                    .Where(img => int.TryParse(GetParentFolderName(img), out int num) && num >= 1 && num <= 4)
                    .OrderBy(img => img, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var group5 = images
                    .Where(img => int.TryParse(GetParentFolderName(img), out int num) && num == 5)
                    .OrderByDescending(img => img, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return group1to4.Concat(group5).ToList();
            }

            // Phân loại ảnh theo tên và sắp xếp
            var topImages = SortImages(allImages.Where(r => r.Contains("-T-")).ToList());
            var midImages = SortImages(allImages.Where(r => r.Contains("-M-")).ToList());
            var botImages = SortImages(allImages.Where(r => r.Contains("-B-")).ToList());

            string[] positions = new string[]
            {
                "Edge(WS) 15 mm",
                "Edge(WS) 25 mm",
                "Edge(WS) 35 mm",
                "1/4",
                "Center",
                "3/4",
                "Edge(DS) 35 mm",
                "Edge(DS) 25 mm",
                "Edge(DS) 15 mm"
            };

            // Khởi tạo file Excel
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var ws = package.Workbook.Worksheets.Add("Hình ảnh");

                ws.Cells["A2"].Value = "Position";
                ws.Cells["A3"].Value = "FDT(⁰C)";
                ws.Cells["A4"].Value = "混晶率";
                ws.Cells["A5"].Value = "Top side";
                ws.Cells["A6"].Value = "Medium";
                ws.Cells["A7"].Value = "Bottom side";

                ws.Cells["B3:J3"].Merge = true;

                // Thiết lập kích thước ảnh chuẩn (2 inch cao)
                var sampleImage = Image.FromFile(allImages.First());
                int pixelHeight = 192;
                double ratio = (double)sampleImage.Width / sampleImage.Height;
                int pixelWidth = (int)(pixelHeight * ratio);

                // Thiết lập chiều cao các hàng chứa ảnh (hàng 2, 3, 4)
                for (int row = 5; row <= 7; row++)
                    ws.Row(row).Height = pixelHeight * 0.75 + 3;

                // Thiết lập chiều rộng các cột (B đến J = 9 cột)
                for (int col = 2; col <= 10; col++)
                {
                    ws.Column(col).Width = pixelWidth / 7.0;
                    ws.Cells[2, col].Value = positions[col - 2];
                }

                // Chọn vùng A2 đến J7
                var range = ws.Cells["A2:J7"];

                range.Style.WrapText = true;

                // Áp dụng căn giữa ngang và dọc
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Áp dụng khung viền mỏng cho tất cả các ô
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                // Font Times New Roman, cỡ 12
                range.Style.Font.Name = "Times New Roman";
                range.Style.Font.Size = 12;

                // Chèn ảnh theo hàng: top (row 2), mid (row 3), bot (row 4)
                InsertImageRow(ws, topImages, 4, pixelWidth, pixelHeight); // Row 2
                InsertImageRow(ws, midImages, 5, pixelWidth, pixelHeight); // Row 3
                InsertImageRow(ws, botImages, 6, pixelWidth, pixelHeight); // Row 4

                package.Save();
            }

            XtraMessageBox.Show("已將圖片新增至Excel檔案!");
            Close();
        }

        private void InsertImageRow(ExcelWorksheet ws, List<string> images, int rowIndex, int width, int height)
        {
            for (int col = 0; col < Math.Min(images.Count, 9); col++)
            {
                var picture = ws.Drawings.AddPicture(Guid.NewGuid().ToString("N"), images[col]);
                picture.SetSize(width, height);
                picture.SetPosition(rowIndex, 3, col + 1, 0);
            }
        }

        public void InsertImagesToPttx()
        {
            Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);

            string folderPath = txbPath.Text.Trim();
            if (!Directory.Exists(folderPath))
            {
                XtraMessageBox.Show("路徑不存在!");
                return;
            }

            // Đường dẫn đến file PPTX có sẵn
            string filePath = Path.Combine(TPConfigs.ResourcesPath, "403_03_Template.pptx");
            string savePath = Path.Combine(folderPath, $"Result-{DateTime.Now:yyyyMMddHHmmss}.pptx");

            // Lấy danh sách file ảnh .jpg kết thúc bằng "100X/200X" nhưng không bắt đầu bằng "35"
            var allImages = Directory.GetFiles(folderPath, "*.jpg", SearchOption.AllDirectories)
                .Where(file =>
                    !Path.GetFileNameWithoutExtension(file).StartsWith("35", StringComparison.OrdinalIgnoreCase) &&
                    Path.GetFileNameWithoutExtension(file).EndsWith(cbbZoom.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

            string sampleId = allImages.FirstOrDefault()?.Split('-').ElementAtOrDefault(1);

            // Hàm lấy tên thư mục cha
            string GetParentFolderName(string path)
            {
                var parts = path.Split(Path.DirectorySeparatorChar);
                return parts.Length > 1 ? parts[parts.Length - 2].Split('-')[0] : string.Empty;
            }

            // Hàm sắp xếp ảnh theo yêu cầu (1-4 A-Z, 5 Z-A)
            List<string> SortImages(List<string> images)
            {
                var group1to4 = images
                    .Where(img => int.TryParse(GetParentFolderName(img), out int num) && num >= 1 && num <= 4)
                    .OrderBy(img => img, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var group5 = images
                    .Where(img => int.TryParse(GetParentFolderName(img), out int num) && num == 5)
                    .OrderByDescending(img => img, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return group1to4.Concat(group5).ToList();
            }

            // Phân loại ảnh theo tên và sắp xếp
            var topImages = SortImages(allImages.Where(r => r.Contains("-T-")).ToList());
            var midImages = SortImages(allImages.Where(r => r.Contains("-M-")).ToList());
            var botImages = SortImages(allImages.Where(r => r.Contains("-B-")).ToList());

            // Tạo đối tượng Presentation và mở file PPTX
            Presentation presentation = new Presentation();
            presentation.LoadFromFile(filePath);

            // Lấy danh sách bộ ảnh theo thứ tự
            var imageCollections = new List<List<string>> { topImages, midImages, botImages };

            // Đặt vị trí ban đầu của ảnh
            int x = 59;
            int y = 242;
            int dpi = 96;
            int newWidth = (int)(1.03 * dpi);

            // Lấy slide đầu tiên
            ISlide slide = presentation.Slides[0];

            // Duyệt qua tất cả các shapes trong slide
            foreach (IShape shape in slide.Shapes)
            {
                // Kiểm tra nếu shape là bảng (ITable)
                if (shape is ITable table)
                {
                    var cell = table[0, 0];

                    // Kiểm tra nếu cell chứa text "SampleID"
                    if (cell.TextFrame.Text.Contains("SampleID"))
                    {
                        // Sửa text trong cell này (ví dụ thay thế "SampleID" thành "NewSampleID")
                        cell.TextFrame.Text = cell.TextFrame.Text.Replace("SampleID", sampleId);
                    }
                }
            }

            // Duyệt qua từng bộ ảnh (top, mid, bot)
            for (int j = 0; j < imageCollections.Count; j++)
            {
                var images = imageCollections[j];

                for (int i = 0; i < images.Count; i++)
                {
                    // Đường dẫn ảnh từ bộ sưu tập tương ứng
                    string imagePath = images[i];

                    // Add the image to the image collection of the document
                    Image img = Image.FromFile(imagePath);
                    IImageData image = presentation.Images.Append(img);

                    // Lấy tỷ lệ khung hình từ ảnh gốc
                    float aspectRatio = (float)image.Height / image.Width;

                    // Tính chiều cao mới theo tỷ lệ
                    int newHeight = (int)(newWidth * aspectRatio);

                    // Tính vị trí ảnh dựa trên chỉ số i và j
                    RectangleF rect = new RectangleF(x + 101 * i, y + j * 78, newWidth, newHeight);
                    IEmbedImage imageShape = slide.Shapes.AppendEmbedImage(ShapeType.Rectangle, image, rect);

                    // Set the line fill type of the image shape to none
                    imageShape.Line.FillType = FillFormatType.None;
                }
            }

            // Lưu file PPTX
            presentation.SaveToFile(savePath, FileFormat.Pptx2010);
            presentation.Dispose();

            XtraMessageBox.Show("已將圖片新增至PPT檔案!");
            Process.Start(savePath);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                switch (cbbTypeFile.Text)
                {
                    case "Excel":
                        InsertImagesToExcel();
                        break;
                    default:
                        InsertImagesToPttx();
                        break;
                }
            }
        }
    }
}