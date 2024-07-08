using DataAccessLayer;
using DevExpress.XtraEditors;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using KnowledgeSystem.Helpers;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Image = System.Drawing.Image;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_ConfirmQues : DevExpress.XtraEditors.XtraForm
    {
        public f307_ConfirmQues()
        {
            InitializeComponent();
        }

        public List<dt307_Questions> ques;
        public List<dt307_Answers> answers;
        string templateContentSigner;
        int indexQues = 0;


        private async void InitializeWebView2(int index)
        {
            lbPageNumber.Caption = $"{index + 1}/{ques.Count}";

            var anses = answers.Where(r => r.QuesId == index + 1).Select(r => new
            {
                id = r.Id,
                disp = r.DisplayText,
                img = ConvertImageToBase64DataUri(r.ImageName),
                istrue = r.TrueAns
            }).ToList();

            var templateData = new
            {
                ques = ques[index].DisplayText,
                answers = anses
            };

            var templateSigner = Template.Parse(templateContentSigner);

            var pageContent = templateSigner.Render(templateData);

            await webViewQues.EnsureCoreWebView2Async(null);
            webViewQues.CoreWebView2.NavigateToString(pageContent);

            // Thêm JavaScript để ngăn chặn chuột phải
            webViewQues.CoreWebView2.DOMContentLoaded += (sender, args) =>
            {
                webViewQues.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('contextmenu', function(e) {
                    e.preventDefault();
                });");
            };
        }

        public static string ConvertImageToBase64DataUri(string imagePath)
        {
            // Load the image
            using (Image image = Image.FromFile(imagePath))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Determine the image format and MIME type
                    ImageFormat imageFormat = GetImageFormat(image, out string mimeType);

                    // Save the image to the memory stream
                    image.Save(memoryStream, imageFormat);

                    // Convert byte array to Base64 string
                    string base64String = Convert.ToBase64String(memoryStream.ToArray());

                    // Construct the full data URI
                    string dataUri = $"data:{mimeType};base64,{base64String}";

                    return dataUri;
                }
            }
        }

        private static ImageFormat GetImageFormat(Image image, out string mimeType)
        {
            if (ImageFormat.Jpeg.Equals(image.RawFormat))
            {
                mimeType = "image/jpeg";
                return ImageFormat.Jpeg;
            }
            if (ImageFormat.Png.Equals(image.RawFormat))
            {
                mimeType = "image/png";
                return ImageFormat.Png;
            }
            if (ImageFormat.Gif.Equals(image.RawFormat))
            {
                mimeType = "image/gif";
                return ImageFormat.Gif;
            }
            if (ImageFormat.Bmp.Equals(image.RawFormat))
            {
                mimeType = "image/bmp";
                return ImageFormat.Bmp;
            }
            if (ImageFormat.Tiff.Equals(image.RawFormat))
            {
                mimeType = "image/tiff";
                return ImageFormat.Tiff;
            }
            if (ImageFormat.Icon.Equals(image.RawFormat))
            {
                mimeType = "image/x-icon";
                return ImageFormat.Icon;
            }

            // Default to PNG if format is unknown
            mimeType = "image/png";
            return ImageFormat.Png;
        }

        private void f307_ConfirmQues_Load(object sender, EventArgs e)
        {
            templateContentSigner = System.IO.File.ReadAllText(Path.Combine(TPConfigs.HtmlPath, @"C:\Users\ANHTUAN\Desktop\DataShift\307Question.html"));
            InitializeWebView2(indexQues);
        }

        private void btnPrevius_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (indexQues <= 0)
            {
                return;
            }

            indexQues--;
            InitializeWebView2(indexQues);
        }

        private void btnNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (indexQues >= ques.Count - 1)
            {
                return;
            }

            indexQues++;
            InitializeWebView2(indexQues);
        }
    }
}