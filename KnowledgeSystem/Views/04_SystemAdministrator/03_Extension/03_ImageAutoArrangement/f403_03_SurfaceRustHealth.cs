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
using static KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._03_ImageAutoArrangement.f403_03_SurfaceRustHealth;
using DevExpress.Data.Extensions;
using DocumentFormat.OpenXml.Presentation;
using Presentation = Spire.Presentation.Presentation;
using DevExpress.Utils.Extensions;
using KnowledgeSystem.Views._00_Generals;
using DevExpress.XtraSplashScreen;
using System.IO.Compression;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._03_ImageAutoArrangement
{
    public partial class f403_03_SurfaceRustHealth : DevExpress.XtraEditors.XtraForm
    {
        public f403_03_SurfaceRustHealth()
        {
            InitializeComponent();
            Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);
        }

        private class DataImage
        {
            public int SlideIndex { get; set; }
            public string[] ImageTOP = new string[3];
            public string[] ImageBOT = new string[3];
            public string[] ImageMID = new string[3];
        }

        string folderPath = "", idRoll = "";

        public void InsertImagesToPttx()
        {
            if (string.IsNullOrEmpty(idRoll))
            {
                XtraMessageBox.Show("請輸入鋼捲ID!", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var measurements = InitializeMeasurements();

            string filePath = Path.Combine(TPConfigs.ResourcesPath, "403_03_SurfaceRustHealth.pptx");
            string savePath = Path.Combine(folderPath, $"Result-{DateTime.Now:yyyyMMddHHmmss}.pptx");
            var allImages = GetValidImages(folderPath, idRoll);
            var presentation = new Presentation();
            presentation.LoadFromFile(filePath);

            InsertImagesIntoSlides(presentation, measurements, allImages);
            presentation.SaveToFile(savePath, FileFormat.Pptx2010);
            presentation.Dispose();

            XtraMessageBox.Show("已將圖片新增至PPT檔案!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(savePath);
        }

        private Dictionary<string, DataImage> InitializeMeasurements()
        {
            return new Dictionary<string, DataImage>
            {
                { "XRD_NH", new DataImage(){ SlideIndex = 0 } },
                { "DC_NH", new DataImage(){ SlideIndex = 1 } },
                { "SEM_NH", new DataImage(){ SlideIndex = 2 } },
                { "XRD_NT", new DataImage(){ SlideIndex = 3 } },
                { "DC_NT", new DataImage(){ SlideIndex = 4 } },
                { "SEM_NT", new DataImage(){ SlideIndex = 5 } },
            };
        }

        private List<string> GetValidImages(string folderPath, string idRoll)
        {
            var pattern = @"^(SEM|XRD|DC)?-\d+X-\d+(\.\d+)?-\d+(\.\d+)?$";
            var validSubfolders = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly)
                .Where(dir => Regex.IsMatch(Path.GetFileName(dir), $"^{idRoll}(NH|NT)[LPC]$", RegexOptions.IgnoreCase))
                .ToList();

            // Thêm DC/SEM
            var dcsemImages = validSubfolders
                .SelectMany(subfolder => Directory.GetFiles(subfolder, "*.jpg", SearchOption.AllDirectories))
                .Where(file => Regex.IsMatch(Path.GetFileNameWithoutExtension(file), pattern, RegexOptions.IgnoreCase))
                .ToList();

            var xrdImages = validSubfolders
                .SelectMany(subfolder => Directory.GetFiles(subfolder, "*.wmf", SearchOption.AllDirectories))
                .Where(file => Regex.IsMatch(Path.GetFileNameWithoutExtension(file), @"^XRD-\d+(\.\d+)?-\d+(\.\d+)?_1$", RegexOptions.IgnoreCase))
                .ToList();

            var allImages = new List<string>(dcsemImages);
            allImages.AddRange(xrdImages);

            return allImages;
        }

        private void InsertImagesIntoSlides(Presentation presentation, Dictionary<string, DataImage> measurements, List<string> allImages)
        {
            string[] positions = { "L", "P", "C" };

            foreach (var imagepath in allImages)
            {
                var parts = imagepath.Split('\\');
                string folderName = parts[parts.Length - 3];
                string botTop = parts[parts.Length - 2];
                string fileName = Path.GetFileNameWithoutExtension(imagepath);
                string typeName = fileName.Split('-')[0];
                string sampleKey = folderName.Contains("NH") ? "NH" : "NT";
                char pos = folderName[folderName.Length - 1];

                var itemType = measurements[$"{typeName}_{sampleKey}"];
                int index = Array.IndexOf(positions, pos.ToString());

                if (botTop == "TOP")
                    itemType.ImageTOP[index] = imagepath;
                else
                    itemType.ImageBOT[index] = imagepath;
            }

            foreach (var item in measurements)
            {
                ISlide slide = presentation.Slides[item.Value.SlideIndex];
                for (int i = 0; i < 3; i++)
                {
                    if (item.Value.SlideIndex == 0 || item.Value.SlideIndex == 3)
                    {
                        int x = 143, y = 111, dpi = 96, newWidth = (int)(2.7 * dpi);
                        InsertImage(slide, item.Value.ImageTOP[i], x + 270 * i, y, newWidth);
                        InsertImage(slide, item.Value.ImageBOT[i], x + 270 * i, y + 211, newWidth);
                    }
                    else
                    {
                        int x = 207, y = 111, dpi = 96, newWidth = (int)(2.07 * dpi);
                        InsertImage(slide, item.Value.ImageTOP[i], x + 256 * i, y, newWidth);
                        InsertImage(slide, item.Value.ImageBOT[i], x + 256 * i, y + 211, newWidth);

                        InsertText(slide, item.Value.ImageTOP[i], $"T{i}");
                        InsertText(slide, item.Value.ImageBOT[i], $"B{i}");
                    }
                }

                foreach (IShape shape in slide.Shapes)
                {
                    if (shape is IAutoShape autoShape)
                    {
                        autoShape.TextFrame.Text = autoShape.TextFrame.Text.Replace("idroll", idRoll);
                    }
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

        private void InsertText(ISlide slide, string imagePath, string keyReplace)
        {
            string fileName = Path.GetFileNameWithoutExtension(imagePath);
            if (string.IsNullOrEmpty(fileName)) return;

            string value1 = fileName.Split('-')[2];
            string value2 = fileName.Split('-')[3];
            // Duyệt qua từng shape trên slide
            foreach (IShape shape in slide.Shapes)
            {
                // Kiểm tra nếu shape là bảng
                if (shape is ITable table)
                {
                    // Duyệt qua từng ô trong bảng
                    for (int row = 0; row < table.TableRows.Count; row++)
                    {
                        for (int col = 0; col < table.TableRows[row].Count; col++)
                        {
                            var cell = table.TableRows[row][col];
                            if (cell.TextFrame != null && cell.TextFrame.Text.StartsWith($"{keyReplace}-1"))
                            {
                                cell.TextFrame.Text = cell.TextFrame.Text.Replace($"{keyReplace}-1", value1);
                            }
                            else if (cell.TextFrame != null && cell.TextFrame.Text.StartsWith($"{keyReplace}-2"))
                            {
                                cell.TextFrame.Text = cell.TextFrame.Text.Replace($"{keyReplace}-2", value2);
                            }
                        }
                    }
                }
            }
        }

        private void InsertBlendingTestImage()
        {
            var measurements = new Dictionary<string, DataImage>
            {
                { "UON_H", new DataImage(){ SlideIndex = 0 } },
                { "UON_T", new DataImage(){ SlideIndex = 1 } },
            };

            string filePath = Path.Combine(TPConfigs.ResourcesPath, "403_03_BendingTest.pptx");
            string savePath = Path.Combine(folderPath, $"Result-{DateTime.Now:yyyyMMddHHmmss}.pptx");

            idRoll = Path.GetFileName(folderPath);
            var pattern = @"^\d+-(H|T)[LPC]-\d+$";
            var allImages = Directory.GetFiles(folderPath, "*.jpg", SearchOption.AllDirectories)
                .Where(file => Regex.IsMatch(Path.GetFileNameWithoutExtension(file), pattern, RegexOptions.IgnoreCase))
                .ToList();

            var presentation = new Presentation();
            presentation.LoadFromFile(filePath);

            InsertImagesBledingTestIntoSlides(presentation, measurements, allImages);
            presentation.SaveToFile(savePath, FileFormat.Pptx2010);
            presentation.Dispose();

            XtraMessageBox.Show("已將圖片新增至PPT檔案!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(savePath);
        }

        private void InsertImagesBledingTestIntoSlides(Presentation presentation, Dictionary<string, DataImage> measurements, List<string> allImages)
        {
            int x = 142, y = 125, dpi = 96, newWidth = (int)(1.80 * dpi);
            string[] positions = { "C", "P", "L" };

            foreach (var imagepath in allImages)
            {
                string fileName = Path.GetFileNameWithoutExtension(imagepath);
                var parts = fileName.Split('-');
                var topmidbot = parts[0];
                char pos = parts[1].Last();
                char sampleKey = parts[1].First();

                var itemType = measurements[$"UON_{sampleKey}"];
                int index = Array.IndexOf(positions, pos.ToString());
                switch (topmidbot)
                {
                    case "0":
                        itemType.ImageTOP[index] = imagepath;
                        break;
                    case "45":
                        itemType.ImageMID[index] = imagepath;
                        break;
                    case "90":
                        itemType.ImageBOT[index] = imagepath;
                        break;
                }
            }

            foreach (var item in measurements)
            {
                ISlide slide = presentation.Slides[item.Value.SlideIndex];
                for (int i = 0; i < 3; i++)
                {
                    InsertImage(slide, item.Value.ImageTOP[i], x + 191 * i, y, newWidth);
                    InsertImage(slide, item.Value.ImageMID[i], x + 191 * i, y + 137, newWidth);
                    InsertImage(slide, item.Value.ImageBOT[i], x + 191 * i, y + 137 * 2, newWidth);

                    InsertTextBlendingTest(slide, item.Value.ImageTOP[i], $"T{i}");
                    InsertTextBlendingTest(slide, item.Value.ImageMID[i], $"M{i}");
                    InsertTextBlendingTest(slide, item.Value.ImageBOT[i], $"B{i}");
                }
            }
        }

        private void InsertTextBlendingTest(ISlide slide, string imagePath, string keyReplace)
        {
            string fileName = Path.GetFileNameWithoutExtension(imagePath);
            if (string.IsNullOrEmpty(fileName)) return;

            var parts = fileName.Split('-');
            string value1 = parts.Last();
            // Duyệt qua từng shape trên slide
            foreach (IShape shape in slide.Shapes)
            {
                // Kiểm tra nếu shape là bảng
                if (shape is ITable table)
                {
                    // Duyệt qua từng ô trong bảng
                    for (int row = 0; row < table.TableRows.Count; row++)
                    {
                        for (int col = 0; col < table.TableRows[row].Count; col++)
                        {
                            var cell = table.TableRows[row][col];
                            if (cell.TextFrame != null && cell.TextFrame.Text.StartsWith($"{keyReplace}"))
                            {
                                cell.TextFrame.Text = cell.TextFrame.Text.Replace($"{keyReplace}", value1);
                            }

                            if (cell.TextFrame != null && cell.TextFrame.Text.StartsWith($"idroll"))
                            {
                                cell.TextFrame.Text = cell.TextFrame.Text.Replace($"idroll", idRoll);
                            }
                        }
                    }
                }
            }
        }

        private void ExtractXRDImage()
        {
            var xrdDocFiles = Directory.GetFiles(folderPath, "*.docx", SearchOption.AllDirectories);

            foreach (var docFile in xrdDocFiles)
            {
                string docFileNameWithoutExt = Path.GetFileNameWithoutExtension(docFile);
                string docDirectory = Path.GetDirectoryName(docFile);

                using (ZipArchive archive = ZipFile.OpenRead(docFile))
                {
                    int imageIndex = 0;

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.StartsWith("word/media/") &&
                            (entry.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                             entry.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                             entry.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                             entry.Name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                             entry.Name.EndsWith(".wmf", StringComparison.OrdinalIgnoreCase)))
                        {
                            string extension = Path.GetExtension(entry.Name);
                            string outputFileName = $"{docFileNameWithoutExt}_{imageIndex}{extension}";
                            string outputPath = Path.Combine(docDirectory, outputFileName);

                            entry.ExtractToFile(outputPath, overwrite: true);
                            Console.WriteLine($"→ Đã trích: {outputFileName}");

                            imageIndex++;
                        }
                    }
                }
            }
        }


        private void cbbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txbId.Text = "";
            switch (cbbType.SelectedIndex)
            {
                case 0:
                    txbId.Enabled = true;
                    break;
                default:
                    txbId.Enabled = false;
                    break;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            folderPath = txbPath.Text.Trim();
            idRoll = txbId.Text.Trim();

            if (!Directory.Exists(folderPath))
            {
                XtraMessageBox.Show("路徑不存在!", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                switch (cbbType.SelectedIndex)
                {
                    case 1:
                        InsertBlendingTestImage();
                        break;
                    default:
                        ExtractXRDImage();
                        InsertImagesToPttx();
                        break;
                }
            }
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            var editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) };

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "輸入路徑",
                Editor = editor,
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;
            var folderPath = Directory.GetFiles(result.ToString().Trim(), "*.jpg", SearchOption.TopDirectoryOnly).ToList();

            f00_CropImage cropImage = new f00_CropImage(folderPath);
            cropImage.ShowDialog();
        }

        private void f403_03_SurfaceRustHealth_Load(object sender, EventArgs e)
        {
            cbbType.SelectedIndex = 0;

#if DEBUG
            txbPath.Text = @"C:\Users\Dell Alpha\Desktop\RÁC 1\RAC\TOAN";
            txbId.Text = "12345";
#endif
        }
    }
}