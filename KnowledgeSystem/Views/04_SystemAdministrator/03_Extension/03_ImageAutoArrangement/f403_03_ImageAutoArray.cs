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

            // Phân loại ảnh theo tên
            var topImages = allImages.Where(r => r.Contains("-T-")).ToList();
            var midImages = allImages.Where(r => r.Contains("-M-")).ToList();
            var botImages = allImages.Where(r => r.Contains("-B-")).ToList();

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
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                InsertImagesToExcel();
            }
        }
    }
}