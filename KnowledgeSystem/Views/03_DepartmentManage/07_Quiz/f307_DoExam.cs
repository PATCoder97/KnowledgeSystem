using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_DoExam : DevExpress.XtraEditors.XtraForm
    {
        public f307_DoExam()
        {
            InitializeComponent();
            //TopMost = true;
        }

        private class ResultExam
        {
            public int Index { get; set; }
            public string Answer { get; set; }
        }

        public string idJob = "";
        int testDuration = 0, passingScore = 0, quesCount = 0, indexQues = 0;
        string templateContentSigner;

        List<dt307_Questions> ques = new List<dt307_Questions>();
        List<dt307_Answers> answers = new List<dt307_Answers>();
        List<ResultExam> resultsExam = new List<ResultExam>();

        BindingSource sourceResult = new BindingSource();

        // Phương pháp xáo trộn Fisher-Yates với salt
        public List<int> Shuffle(int maxIndex)
        {
            // Tạo danh sách số từ 1 đến 100
            List<int> numbers = new List<int>();
            for (int i = 0; i < maxIndex; i++)
            {
                numbers.Add(i);
            }

            // Tạo giá trị salt từ Guid
            string salt = Guid.NewGuid().ToString();

            // Tạo Random từ hash của salt
            int seed = salt.GetHashCode();
            Random rng = new Random(seed);

            int n = numbers.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = numbers[k];
                numbers[k] = numbers[n];
                numbers[n] = value;
            }

            return numbers;
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

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = gvData;
            indexQues = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString()) - 1;

            InitializeWebView2(indexQues);
        }

        private async void InitializeWebView2(int index)
        {
            //lbPageNumber.Caption = $"{index + 1}/{ques.Count}";

            var counter = 1;
            var anses = answers.Where(r => r.QuesId == ques[index].Id)
                .Select(r => new
                {
                    id = counter++, // Đánh số thứ tự từ 1
                    disp = r.DisplayText,
                    img = string.IsNullOrEmpty(r.ImageName) ? "" :
                    ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, r.ImageName)),
                    istrue = false
                }).ToList();

            var templateData = new
            {
                ques = ques[index].DisplayText,
                quesimg = string.IsNullOrEmpty(ques[index].ImageName) ? "" :
                ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, ques[index].ImageName)),
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

        private void LoadData()
        {
            var setting = dt307_JobQuesManageBUS.Instance.GetItemByIdJob(idJob);

            testDuration = setting.TestDuration ?? 0;
            passingScore = setting.PassingScore ?? 0;
            quesCount = setting.QuesCount ?? 0;

            var dataQues = dt307_QuestionsBUS.Instance.GetListByJob(idJob);

            var numbers = Shuffle(dataQues.Count).Take(quesCount).ToList();

            ques = numbers.Select(index => dataQues[index]).ToList();
            answers = dt307_AnswersBUS.Instance.GetListByListQues(ques.Select(r => r.Id).ToList());

            resultsExam = Enumerable.Range(1, 20).Select(i => new ResultExam { Index = i }).ToList();



        }

        private void f307_DoExam_Load(object sender, EventArgs e)
        {
            LoadData();

            sourceResult.DataSource = resultsExam;
            gcData.DataSource = sourceResult;

            templateContentSigner = File.ReadAllText(Path.Combine(TPConfigs.HtmlPath, "dt307_ConfirmQuestion.html"));
            InitializeWebView2(indexQues);
        }
    }
}