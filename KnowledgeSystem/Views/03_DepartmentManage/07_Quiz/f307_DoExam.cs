using BusinessLayer;
using DataAccessLayer;
using DevExpress.DataAccess.Native.Sql.SqlParser;
using DevExpress.XtraCharts;
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
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_DoExam : DevExpress.XtraEditors.XtraForm
    {
        public f307_DoExam()
        {
            InitializeComponent();
            //TopMost = true;

            this.KeyDown += F307_DoExam_KeyDown;
            this.KeyPreview = true;  // Đảm bảo Form nhận được sự kiện KeyDown
        }

        private class ResultExam
        {
            public int Index { get; set; }
            public string Answer { get; set; }
        }

        private Timer countdownTimer;
        private TimeSpan timeRemaining;

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
            List<int> numbers = Enumerable.Range(0, maxIndex - 1).ToList();

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
            resultsExam[indexQues].Answer = string.Join(",", txbUserAns.GetTokenList().Select(r => r.Description as string).ToList());
            gcData.RefreshDataSource();

            if (indexQues >= ques.Count - 1) return;

            indexQues++;
            InitializeWebView2(indexQues);

            txbUserAns.Focus();
        }

        private void PreviousQues()
        {
            resultsExam[indexQues].Answer = string.Join(",", txbUserAns.GetTokenList().Select(r => r.Description as string).ToList());
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
                answers = anses
            };

            // Thêm danh sách câu trả lời, và hiện các câu trả lời đã lưu
            txbUserAns.Properties.DataSource = Enumerable.Range(1, anses.Count).Select(num => num.ToString()).ToList();
            txbUserAns.EditValue = resultsExam[indexQues].Answer;

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

        private string LoadData()
        {
            var setting = dt307_JobQuesManageBUS.Instance.GetItemByIdJob(idJob);

            testDuration = setting?.TestDuration ?? 0;
            passingScore = setting?.PassingScore ?? 0;
            quesCount = setting?.QuesCount ?? 0;

            var dataQues = dt307_QuestionsBUS.Instance.GetListByJob(idJob);

            if (dataQues.Count <= quesCount)
                return "題目數量不夠！";

            var numbers = Shuffle(dataQues.Count).Take(quesCount).ToList();

            ques = numbers.Select(index => dataQues[index]).ToList();
            answers = dt307_AnswersBUS.Instance.GetListByListQues(ques.Select(r => r.Id).ToList());

            resultsExam = Enumerable.Range(1, 20).Select(i => new ResultExam { Index = i }).ToList();

            return "";
        }

        private void f307_DoExam_Load(object sender, EventArgs e)
        {
            string result = LoadData();

            if (!string.IsNullOrEmpty(result))
            {
                MsgTP.MsgError(result);
                Close();
                return;
            }

            string msg= $"<font='Microsoft JhengHei UI' size=14>點擊「<color=red>確定</color>」開始！</font>";
            MsgTP.MsgShowInfomation(msg);

            countdownTimer = new Timer();
            countdownTimer.Interval = 1000; // 1 giây
            countdownTimer.Tick += CountdownTimer_Tick;

            timeRemaining = TimeSpan.FromMinutes(testDuration);
            lbTime.Text = $"剩餘時間 {timeRemaining:mm\\:ss}";
            countdownTimer.Start();

            txbUserAns.Focus();

            sourceResult.DataSource = resultsExam;
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

            if (timeRemaining <= TimeSpan.FromMinutes(1))
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