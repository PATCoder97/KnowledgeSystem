using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data.Helpers;
using DevExpress.DataAccess.Native.Sql.SqlParser;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
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
using static DevExpress.XtraEditors.Mask.MaskSettings;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_DoExam : DevExpress.XtraEditors.XtraForm
    {
        public f307_DoExam()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Bỏ khung viền
            this.WindowState = FormWindowState.Maximized; // Phóng to toàn màn hình
            //this.TopMost = true; // Đảm bảo form luôn ở trên cùng

            this.KeyDown += F307_DoExam_KeyDown;
            this.KeyPreview = true;  // Đảm bảo Form nhận được sự kiện KeyDown
        }

        public class ExamResult
        {
            public dt307_Questions Questions { get; set; }
            public List<dt307_Answers> Answers { get; set; }
            public int QuestionIndex { get; set; }
            public string CorrectAnswer { get; set; }
            public string UserAnswer { get; set; }
            public bool IsCorrect { get; set; }
            public bool IsMultiChoice { get; set; }
        }

        private Timer countdownTimer;
        private TimeSpan timeRemaining;

        private bool IsClose = false;

        public string idJob = "";
        public int idExamUser = -1;
        int testDuration = 0, passingScore = 0, quesCount = 0, multiQuesCount = 0, indexQues = 0;
        string templateContentSigner;

        List<dt307_Questions> ques = new List<dt307_Questions>();
        List<dt307_Answers> answers = new List<dt307_Answers>();
        List<ExamResult> examResults = new List<ExamResult>();

        BindingSource sourceResult = new BindingSource();

        // Phương pháp xáo trộn Fisher-Yates với salt
        public List<int> Shuffle(int maxIndex)
        {
            // Tạo danh sách số từ 1 đến 100
            List<int> numbers = Enumerable.Range(0, maxIndex).ToList();

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

        private void NextQues()
        {
            examResults[indexQues].UserAnswer = string.Join(",", txbUserAns.GetTokenList().Select(r => r.Description as string).ToList());
            gcData.RefreshDataSource();

            if (indexQues >= ques.Count - 1) return;

            indexQues++;
            InitializeWebView2(indexQues);

            txbUserAns.Focus();
        }

        private void PreviousQues()
        {
            examResults[indexQues].UserAnswer = string.Join(",", txbUserAns.GetTokenList().Select(r => r.Description as string).ToList());
            gcData.RefreshDataSource();

            if (indexQues <= 0) return;

            indexQues--;
            InitializeWebView2(indexQues);
            txbUserAns.Focus();
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = gvData;
            indexQues = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString()) - 1;

            InitializeWebView2(indexQues);
        }

        private void btnPreviousQues_Click(object sender, EventArgs e)
        {
            PreviousQues();
        }

        private void btnNextQues_Click(object sender, EventArgs e)
        {
            NextQues();
        }

        private void F307_DoExam_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                NextQues();
            }
            else if (e.KeyCode == Keys.Left)
            {
                PreviousQues();
            }

            if (e.Control && e.Alt && e.Shift && e.KeyCode == Keys.T)
            {
                foreach (var answer in examResults)
                {
                    answer.UserAnswer = answer.CorrectAnswer;
                }

                gcData.RefreshDataSource();
            }
        }

        private async void InitializeWebView2(int index)
        {
            lbQuesNo.Text = $"題目：{index + 1}/{ques.Count}";
            var counter = 1;
            var anses = answers.Where(r => r.QuesId == ques[index].Id)
                .Select(r => new
                {
                    id = counter++, // Đánh số thứ tự từ 1
                    disp = r.DisplayText,
                    img = string.IsNullOrEmpty(r.ImageName) ? "" :
                    ImageHelper.ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, r.ImageName)),
                    istrue = false
                }).ToList();

            var templateData = new
            {
                quesno = index + 1,
                ques = ques[index].DisplayText,
                quesimg = string.IsNullOrEmpty(ques[index].ImageName) ? "" :
                ImageHelper.ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, ques[index].ImageName)),
                answers = anses,
                ismultichoice = ques[index].IsMultiAns == true
            };

            // Thêm danh sách câu trả lời, và hiện các câu trả lời đã lưu
            txbUserAns.Properties.DataSource = Enumerable.Range(1, anses.Count).Select(num => num.ToString()).ToList();
            txbUserAns.EditValue = examResults[indexQues].UserAnswer;

            var templateSigner = Template.Parse(templateContentSigner);

            var pageContent = templateSigner.Render(templateData);
            
            webViewQues.CoreWebView2.NavigateToString(pageContent);

            // Thêm JavaScript để ngăn chặn chuột phải
            webViewQues.CoreWebView2.DOMContentLoaded += (sender, args) =>
            {
                webViewQues.CoreWebView2.ExecuteScriptAsync(@"
                document.addEventListener('contextmenu', function(e) {
                    e.preventDefault();
                });");
            };

            gvData.FocusedRowHandle = index;
        }

        private void f307_DoExam_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsClose)
            {
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>請按<color=red>「提交」</color>結束考試</font>");
                e.Cancel = true;
            }
        }

        private void f307_DoExam_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                countdownTimer.Stop();
            }
            catch { }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            foreach (var answer in examResults)
            {
                if (answer.UserAnswer == null) { answer.IsCorrect = false; continue; }

                // Chuyển TrueAns và UserAns thành HashSet để so sánh không thứ tự
                var trueAnsSet = answer.CorrectAnswer.Split(',').Select(x => x.Trim()).ToHashSet();
                var userAnsSet = answer.UserAnswer.Split(',').Select(x => x.Trim()).ToHashSet();

                // So sánh hai tập hợp và gán giá trị cho IsTrue
                answer.IsCorrect = trueAnsSet.SetEquals(userAnsSet);
            }

            int correctAnswers = examResults.Count(r => r.IsCorrect);
            int score = (int)Math.Round(correctAnswers * 100.0 / quesCount);
            int totalScore = 100;
            bool IsPass = score >= passingScore;
            string message = IsPass ? "恭喜您通過考試!!!" : "很遺憾你考試沒通過!";
            string json = JsonConvert.SerializeObject(examResults);

            // Nếu có thông tin của kỳ thi thì lưu kết quả lại
            if (idExamUser != -1)
            {
                dt307_ExamUser result = dt307_ExamUserBUS.Instance.GetItemById(idExamUser);
                result.IsPass = IsPass;
                result.Score = score;
                result.SubmitTime = DateTime.Now;
                result.ExamData = json;

                dt307_ExamUserBUS.Instance.AddOrUpdate(result);
            }

            string htmlString = $"<font='Microsoft JhengHei UI' size=24><color=red>{message}</color></font>\r\n" +
                $"<font='Microsoft JhengHei UI' size=14>結果：\r\n" +
                $"-正確：<color=blue>{correctAnswers}/{quesCount}</color>\r\n" +
                $"-成績：<color=blue>{score}/{totalScore}</color></font>";

            MsgTP.MsgShowInfomation(htmlString);

            IsClose = true;
            Close();
        }

        private string LoadData()
        {
            var setting = dt307_JobQuesManageBUS.Instance.GetItemByIdJob(idJob);

            testDuration = setting?.TestDuration ?? 0;
            passingScore = setting?.PassingScore ?? 0;
            quesCount = setting?.QuesCount ?? 0;
            multiQuesCount = setting?.MultiQues ?? 0;

            var dataQues = dt307_QuestionsBUS.Instance.GetListByJob(idJob);

            var normalQues = dataQues.Where(r => r.IsMultiAns == false).ToList();
            var multiChoiceQues = dataQues.Where(r => r.IsMultiAns == true).ToList();

            if (dataQues.Count <= quesCount)
                return "題目數量不夠！";

            if (dataQues.Count <= quesCount)
                return "題目數量不夠！";

            var numbersNormalQues = Shuffle(normalQues.Count).Take(quesCount - multiQuesCount).ToList();
            var numbersMultiChoiceQues = Shuffle(multiChoiceQues.Count).Take(multiQuesCount).ToList();

            ques = numbersNormalQues.Select(index => normalQues[index]).ToList();
            ques.AddRange(numbersMultiChoiceQues.Select(index => multiChoiceQues[index]).ToList());

            var numbers = Shuffle(quesCount);
            ques = numbers.Select(index => ques[index]).ToList();

            answers = dt307_AnswersBUS.Instance.GetListByListQues(ques.Select(r => r.Id).ToList())
                .GroupBy(r => r.QuesId)  // Group by QuesId
                .SelectMany(group => group.Select((answer, index) => new dt307_Answers
                {
                    Id = index + 1,
                    QuesId = answer.QuesId,
                    ImageName = answer.ImageName,
                    DisplayText = answer.DisplayText,
                    TrueAns = answer.TrueAns
                })).ToList();

            // Lấy danh sách câu trả lời đúng dưới dạng 1,2,3
            var correctAnsByIdQues = answers.Where(r => r.TrueAns).GroupBy(r => r.QuesId).Select(r => new
            {
                QuesId = r.Key,
                CorrectAnswer = string.Join(",", r.Select(g => g.Id.ToString()))
            }).ToList();

            // Lấy ra danh sách để lưu ra kết quả
            int indexQues = 1;
            examResults = (from question in ques
                           join correct in correctAnsByIdQues on question.Id equals correct.QuesId
                           select new ExamResult
                           {
                               QuestionIndex = indexQues++,
                               CorrectAnswer = correct.CorrectAnswer,
                               UserAnswer = "",
                               Questions = question,
                               Answers = answers.Where(r => r.QuesId == question.Id).ToList(),
                               IsMultiChoice = question.IsMultiAns == true
                           }).ToList();

            return "";
        }

        private async void f307_DoExam_Load(object sender, EventArgs e)
        {
            string result = LoadData();

            if (!string.IsNullOrEmpty(result))
            {
                IsClose = true;
                MsgTP.MsgError(result);
                Close();
                return;
            }

            string pathDataWebView = Path.Combine(TPConfigs.DocumentPath(), "WebView2UserData");
            var options = new CoreWebView2EnvironmentOptions();
            var env = await CoreWebView2Environment.CreateAsync(null, pathDataWebView, options);
            await webViewQues.EnsureCoreWebView2Async(env);

            string msg = $"<font='Microsoft JhengHei UI' size=14>點擊「<color=red>確定</color>」開始！</font>";
            MsgTP.MsgShowInfomation(msg);

            countdownTimer = new Timer();
            countdownTimer.Interval = 1000; // 1 giây
            countdownTimer.Tick += CountdownTimer_Tick;

            timeRemaining = TimeSpan.FromMinutes(testDuration);
            lbTime.Text = $"剩餘時間 {timeRemaining:mm\\:ss}";
            countdownTimer.Start();

            txbUserAns.Focus();

            sourceResult.DataSource = examResults;
            gcData.DataSource = sourceResult;

            templateContentSigner = File.ReadAllText(Path.Combine(TPConfigs.HtmlPath, "dt307_ConfirmQuestion.html"));
            InitializeWebView2(indexQues);
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            // Giảm thời gian còn lại đi 1 giây
            timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));

            // Cập nhật Label với thời gian còn lại
            lbTime.Text = $"剩餘時間 {timeRemaining:mm\\:ss}";

            if (timeRemaining <= TimeSpan.FromMinutes(5))
            {
                lbTime.ForeColor = System.Drawing.Color.Red;
            }

            // Nếu hết thời gian thì dừng Timer
            if (timeRemaining <= TimeSpan.Zero)
            {
                countdownTimer.Stop();
                lbTime.Text = "時間到了！";
                MessageBox.Show("Thời gian đã hết!");
            }
        }
    }
}