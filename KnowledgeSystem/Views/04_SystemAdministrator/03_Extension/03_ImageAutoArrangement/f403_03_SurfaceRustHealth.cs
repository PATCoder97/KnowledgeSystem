using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Spire.Presentation.Drawing;
using Spire.Presentation;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._03_ImageAutoArrangement
{
    public partial class f403_03_SurfaceRustHealth : DevExpress.XtraEditors.XtraForm
    {
        public f403_03_SurfaceRustHealth()
        {
            InitializeComponent();
        }

        public void InsertImagesToPttx()
        {
            Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);

            string folderPath = txbPath.Text.Trim();
            folderPath = @"C:\Users\Dell Alpha\Desktop\RÁC 1\RAC\TOAN\2783132112";
            if (!Directory.Exists(folderPath))
            {
                XtraMessageBox.Show("路徑不存在!");
                return;
            }

            // Đường dẫn đến file PPTX có sẵn
            string filePath = Path.Combine(TPConfigs.ResourcesPath, "403_03_SurfaceRustHealth.pptx");
            string savePath = Path.Combine(folderPath, $"Result-{DateTime.Now:yyyyMMddHHmmss}.pptx");

            //// Lấy danh sách file ảnh .jpg bằng điều kiện regex
            //var pattern = @"^(00|15|25)-[BTM]-(100X|200X)( DO)?$";

            //var allImages = Directory.GetFiles(folderPath, "*.jpg", SearchOption.AllDirectories)
            //    .Where(file => Regex.IsMatch(Path.GetFileNameWithoutExtension(file), pattern, RegexOptions.IgnoreCase))
            //    .GroupBy(file => new
            //    {
            //        Directory = Path.GetDirectoryName(file),  // Nhóm theo thư mục
            //        FileName = Path.GetFileNameWithoutExtension(file).Replace(" DO", "").ToLower()  // Nhóm theo tên file (loại bỏ " DO") và chuyển về chữ thường
            //    })
            //    .Select(group => group.OrderByDescending(file => Path.GetFileNameWithoutExtension(file).EndsWith(" DO")).First())
            //    .ToList();

            //string sampleId = allImages.FirstOrDefault() != null
            //    ? Path.GetFileName(Path.GetDirectoryName(allImages.FirstOrDefault()))?.Split('-').ElementAtOrDefault(1)
            //    : null;

            //// Hàm lấy tên thư mục cha
            //string GetParentFolderName(string path)
            //{
            //    var parts = path.Split(Path.DirectorySeparatorChar);
            //    return parts.Length > 1 ? parts[parts.Length - 2].Split('-')[0] : string.Empty;
            //}

            //// Hàm sắp xếp ảnh theo yêu cầu (1-4 A-Z, 5 Z-A)
            //List<string> SortImages(List<string> images)
            //{
            //    var group1to4 = images
            //        .Where(img => int.TryParse(GetParentFolderName(img), out int num) && num >= 1 && num <= 4)
            //        .OrderBy(img => img, StringComparer.OrdinalIgnoreCase)
            //        .ToList();

            //    var group5 = images
            //        .Where(img => int.TryParse(GetParentFolderName(img), out int num) && num == 5)
            //        .OrderByDescending(img => img, StringComparer.OrdinalIgnoreCase)
            //        .ToList();

            //    return group1to4.Concat(group5).ToList();
            //}

            //// Phân loại ảnh theo tên và sắp xếp
            //var topImages = SortImages(allImages.Where(r => r.Contains("-T-")).ToList());
            //var midImages = SortImages(allImages.Where(r => r.Contains("-M-")).ToList());
            //var botImages = SortImages(allImages.Where(r => r.Contains("-B-")).ToList());

            // Tạo đối tượng Presentation và mở file PPTX
            Presentation presentation = new Presentation();
            presentation.LoadFromFile(filePath);

            //// Lấy danh sách bộ ảnh theo thứ tự
            //var imageCollections = new List<List<string>> { topImages, midImages, botImages };

            // Đặt vị trí ban đầu của ảnh
            int x = 165;
            int y = 109;
            int dpi = 96;
            int newWidth = (int)(2.07 * dpi);

            // Lấy slide đầu tiên
            ISlide slide = presentation.Slides[0];

            //// Duyệt qua tất cả các shapes trong slide
            //foreach (IShape shape in slide.Shapes)
            //{
            //    // Kiểm tra nếu shape là bảng (ITable)
            //    if (shape is ITable table)
            //    {
            //        var cell = table[0, 0];

            //        // Kiểm tra nếu cell chứa text "SampleID"
            //        if (cell.TextFrame.Text.Contains("SampleID"))
            //        {
            //            // Sửa text trong cell này (ví dụ thay thế "SampleID" thành "NewSampleID")
            //            cell.TextFrame.Text = cell.TextFrame.Text.Replace("SampleID", sampleId);
            //        }
            //    }
            //}

            // Duyệt qua từng bộ ảnh (top, mid, bot)
            //for (int j = 0; j < imageCollections.Count; j++)
            for (int j = 0; j < 2; j++)
            {
                //    var images = imageCollections[j];

                //for (int i = 0; i < images.Count; i++)
                for (int i = 0; i < 3; i++)
                {
                    // Đường dẫn ảnh từ bộ sưu tập tương ứng
                    //string imagePath = images[i];
                    string imagePath = @"C:\Users\Dell Alpha\Desktop\RÁC 1\RAC\TOAN\2783132112\5000X-8.05-507.jpg";

                    // Add the image to the image collection of the document
                    Image img = Image.FromFile(imagePath);
                    IImageData image = presentation.Images.Append(img);

                    // Lấy tỷ lệ khung hình từ ảnh gốc
                    float aspectRatio = (float)image.Height / image.Width;

                    // Tính chiều cao mới theo tỷ lệ
                    int newHeight = (int)(newWidth * aspectRatio);

                    // Tính vị trí ảnh dựa trên chỉ số i và j
                    RectangleF rect = new RectangleF(x + 270 * i, y + j * 211, newWidth, newHeight);
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
            InsertImagesToPttx();
        }
    }
}