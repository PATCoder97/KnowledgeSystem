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
        }

        string idRoll = "12345";

        public void InsertImagesToPttx()
        {
            var measurements = InitializeMeasurements();
            string folderPath = @"C:\Users\Dell Alpha\Desktop\RÁC 1\RAC\TOAN";
            idRoll = "12345";

            if (!Directory.Exists(folderPath))
            {
                XtraMessageBox.Show("路徑不存在!");
                return;
            }

            string filePath = Path.Combine(TPConfigs.ResourcesPath, "403_03_SurfaceRustHealth.pptx");
            string savePath = Path.Combine(folderPath, $"Result-{DateTime.Now:yyyyMMddHHmmss}.pptx");
            var allImages = GetValidImages(folderPath, idRoll);
            var presentation = new Presentation();
            presentation.LoadFromFile(filePath);

            InsertImagesIntoSlides(presentation, measurements, allImages);
            presentation.SaveToFile(savePath, FileFormat.Pptx2010);
            presentation.Dispose();

            XtraMessageBox.Show("已將圖片新增至PPT檔案!");
            Process.Start(savePath);
        }

        private Dictionary<string, DataImage> InitializeMeasurements()
        {
            return new Dictionary<string, DataImage>
            {
                { "DC_NH", new DataImage(){ SlideIndex = 0 } },
                { "DC_NT", new DataImage(){ SlideIndex = 1 } },
                { "SEM_NH", new DataImage(){ SlideIndex = 2 } },
                { "SEM_NT", new DataImage(){ SlideIndex = 3 } },
                { "XRD_NH", new DataImage(){ SlideIndex = 4 } },
                { "XRD_NT", new DataImage(){ SlideIndex = 5 } },
            };
        }

        private List<string> GetValidImages(string folderPath, string idRoll)
        {
            var pattern = @"^(SEM|XRD|DC)?-\d+X-\d+(\.\d+)?-\d+(\.\d+)?$";
            var validSubfolders = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly)
                .Where(dir => Regex.IsMatch(Path.GetFileName(dir), $"^{idRoll}(NH|NT)[LPC]$", RegexOptions.IgnoreCase))
                .ToList();

            return validSubfolders
                .SelectMany(subfolder => Directory.GetFiles(subfolder, "*.jpg", SearchOption.AllDirectories))
                .Where(file => Regex.IsMatch(Path.GetFileNameWithoutExtension(file), pattern, RegexOptions.IgnoreCase))
                .ToList();
        }

        private void InsertImagesIntoSlides(Presentation presentation, Dictionary<string, DataImage> measurements, List<string> allImages)
        {
            int x = 165, y = 111, dpi = 96, newWidth = (int)(2.07 * dpi);
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
                    InsertImage(slide, item.Value.ImageTOP[i], x + 270 * i, y, newWidth);
                    InsertImage(slide, item.Value.ImageBOT[i], x + 270 * i, y + 211, newWidth);

                    InsertText(slide, item.Value.ImageTOP[i], $"T{i}");
                    InsertText(slide, item.Value.ImageBOT[i], $"B{i}");
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
                    Console.WriteLine("Bảng:");

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
                    Console.WriteLine();
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            switch (cbbType.SelectedIndex)
            {
                case 0:
                    
                    break;
                default:
                    InsertImagesToPttx();
                    break;
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
    }
}