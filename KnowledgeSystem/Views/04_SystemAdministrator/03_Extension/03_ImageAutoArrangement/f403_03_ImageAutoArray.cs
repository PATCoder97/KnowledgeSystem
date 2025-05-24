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
using System.Text.RegularExpressions;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._03_ImageAutoArrangement
{
    public partial class f403_03_ImageAutoArray : DevExpress.XtraEditors.XtraForm
    {
        public f403_03_ImageAutoArray()
        {
            InitializeComponent();
        }

        string folderPath = "", idRoll = "";
        int numImage = 7;

        private class DataImage
        {
            public int SlideIndex { get; set; }
            public string[] ImageTOP = new string[7];
            public string[] ImageBOT = new string[7];
            public string[] ImageMID = new string[7];
        }

        private List<string> GetValidImages(string folderPath)
        {
            // Lấy danh sách file ảnh .jpg bằng điều kiện regex
            var pattern = @"^(00|15|25)-[BTM]-(100X|200X)( DO)?$";

            var allImages = Directory.GetFiles(folderPath, "*.jpg", SearchOption.AllDirectories)
                .Where(file => Regex.IsMatch(Path.GetFileNameWithoutExtension(file), pattern, RegexOptions.IgnoreCase))
                .GroupBy(file => new
                {
                    Directory = Path.GetDirectoryName(file),  // Nhóm theo thư mục
                    FileName = Path.GetFileNameWithoutExtension(file).Replace(" DO", "").ToLower()  // Nhóm theo tên file (loại bỏ " DO") và chuyển về chữ thường
                })
                .Select(group => group.OrderByDescending(file => Path.GetFileNameWithoutExtension(file).EndsWith(" DO")).First())
                .ToList();

            return allImages;
        }

        private void InsertImagesIntoSlides(Presentation presentation, DataImage measurements, List<string> allImages)
        {
            string[] positions;
            if (numImage == 4)
            {
                positions = new string[] { "1-15", "1-25", "5-25", "5-15" };
            }
            else
            {
                positions = new string[] { "1-15", "1-25", "2-00", "3-00", "4-00", "5-25", "5-15" };
            }


            foreach (var imagepath in allImages)
            {
                var parts = imagepath.Split('\\');
                string folderName = parts[parts.Length - 2].Trim();
                char indexFolder = folderName[0];
                string fileName = Path.GetFileNameWithoutExtension(imagepath);
                string topmidbot = fileName.Split('-')[1];
                string indexFile = fileName.Split('-')[0];
                string pos = $"{indexFolder}-{indexFile}";

                int index = Array.IndexOf(positions, pos.ToString());

                switch (topmidbot)
                {
                    case "T":
                        measurements.ImageTOP[index] = imagepath;
                        break;
                    case "M":
                        measurements.ImageMID[index] = imagepath;
                        break;
                    case "B":
                        measurements.ImageBOT[index] = imagepath;
                        break;
                }
            }

            ISlide slide = presentation.Slides[measurements.SlideIndex];
            for (int i = 0; i < numImage; i++)
            {
                if (numImage == 4)
                {
                    int x = 59, y = 242, dpi = 96, newWidth = (int)(1.03 * dpi);
                    InsertImage(slide, measurements.ImageTOP[i], x + 101 * i, y, newWidth);
                    InsertImage(slide, measurements.ImageMID[i], x + 101 * i, y + 77, newWidth);
                    InsertImage(slide, measurements.ImageBOT[i], x + 101 * i, y + 77 * 2, newWidth);
                }
                else
                {
                    int x = 59, y = 242, dpi = 96, newWidth = (int)(1.03 * dpi);
                    InsertImage(slide, measurements.ImageTOP[i], x + 101 * i, y, newWidth);
                    InsertImage(slide, measurements.ImageMID[i], x + 101 * i, y + 78, newWidth);
                    InsertImage(slide, measurements.ImageBOT[i], x + 101 * i, y + 78 * 2, newWidth);
                }
            }

            foreach (IShape shape in slide.Shapes)
            {
                if (shape is IAutoShape autoShape)
                {
                    autoShape.TextFrame.Text = autoShape.TextFrame.Text.Replace("SampleID", idRoll);
                }
            }
        }

        private void InsertImage(ISlide slide, string imagePath, int x, int y, int newWidth)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            Image img = Image.FromFile(imagePath);
            float aspectRatio = (float)img.Height / img.Width;
            int newHeight = (int)(newWidth * aspectRatio);
            IImageData image = slide.Presentation.Images.Append(img);
            RectangleF rect = new RectangleF(x, y, newWidth, newHeight);
            IEmbedImage imageShape = slide.Shapes.AppendEmbedImage(ShapeType.Rectangle, image, rect);
            imageShape.Line.FillType = FillFormatType.None;
        }

        public void InsertImagesToPttx()
        {
            var measurements = new DataImage() { SlideIndex = 0 };
            string filePath = Path.Combine(TPConfigs.ResourcesPath, "403_03_Template.pptx");

            if (numImage == 4)
            {
                filePath = Path.Combine(TPConfigs.ResourcesPath, "403_03_MixedAnalysis.pptx");
            }

            string savePath = Path.Combine(folderPath, $"Result-{DateTime.Now:yyyyMMddHHmmss}.pptx");
            var allImages = GetValidImages(folderPath);
            var presentation = new Presentation();
            presentation.LoadFromFile(filePath);

            InsertImagesIntoSlides(presentation, measurements, allImages);
            presentation.SaveToFile(savePath, FileFormat.Pptx2010);
            presentation.Dispose();

            XtraMessageBox.Show("已將圖片新增至PPT檔案!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(savePath);
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


        private void btnSubmit_Click(object sender, EventArgs e)
        {
            folderPath = txbPath.Text.Trim();

            if (!Directory.Exists(folderPath))
            {
                XtraMessageBox.Show("路徑不存在!", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);

            InsertImagesToPttx();

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                switch (cbbTypeFile.SelectedIndex)
                {
                    case 1:
                        InsertImagesToPttx();
                        break;
                    default:
                        InsertImagesToPttx();
                        break;
                }
            }
        }
    }
}