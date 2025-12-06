using BusinessLayer;
using DataAccessLayer;
using DevExpress.Charts.Native;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class f312_DoExam : DevExpress.XtraEditors.XtraForm
    {
        public f312_DoExam()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Bỏ khung viền
            this.WindowState = FormWindowState.Maximized; // Phóng to toàn màn hình
            //this.TopMost = true; // Đảm bảo form luôn ở trên cùng

            this.KeyDown += F312_DoExam_KeyDown;
            this.KeyPreview = true;  // Đảm bảo Form nhận được sự kiện KeyDown
        }

        public class ExamResult
        {
            public dt312_Questions Questions { get; set; }
            public List<dt312_Answers> Answers { get; set; }
            public int QuestionIndex { get; set; }
            public string CorrectAnswer { get; set; }
            public string UserAnswer { get; set; }
            public bool IsCorrect { get; set; }
            public bool IsMultiChoice { get; set; }
        }

        private Timer countdownTimer;
        private TimeSpan timeRemaining;

        private bool IsClose = false;

        public int idExamUser = -1;
        int testDuration = 0, passingScore = 0, numQues = 0, indexQues = 0;
        string templateContentSigner;

        List<dt312_Questions> ques = new List<dt312_Questions>();
        List<dt312_Answers> answers = new List<dt312_Answers>();
        List<ExamResult> examResults = new List<ExamResult>();

        BindingSource sourceResult = new BindingSource();

        public class GroupDistribution
        {
            public int GroupId { get; set; }
            public int MaxAvailable { get; set; } // Tổng số câu hỏi nhóm đang có
            public int CountToTake { get; set; }  // Số câu hỏi thuật toán quyết định sẽ lấy
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

        private void F312_DoExam_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                NextQues();
            }
            else if (e.KeyCode == Keys.Left)
            {
                PreviousQues();
            }

            if (e.Control && e.Alt && e.Shift && e.KeyCode == Keys.Z)
            {
                var groups = dm_GroupBUS.Instance.GetItemByName("評鑒係統");
                if (groups == null) return;

                var grpUsers = dm_GroupUserBUS.Instance.GetListByIdGroup(groups.Id);
                if (!grpUsers.Any(r => r.IdUser == TPConfigs.LoginUser.Id))
                    return;

                foreach (var answer in examResults)
                    answer.UserAnswer = answer.CorrectAnswer;

                gcData.RefreshDataSource();

                GridView view = gvData;
                indexQues = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString()) - 1;

                InitializeWebView2(indexQues);
            }
        }

        private void InitializeWebView2(int index)
        {
            lbQuesNo.Text = $"題目：{index + 1}/{ques.Count}";
            var counter = 1;
            var anses = answers.Where(r => r.QuesId == ques[index].Id)
                .Select(r => new
                {
                    id = counter++, // Đánh số thứ tự từ 1
                    disp = r.DisplayName,
                    img = string.IsNullOrEmpty(r.ImageName) ? "" :
                    ImageHelper.ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, r.ImageName)),
                    istrue = false
                }).ToList();

            var templateData = new
            {
                quesno = index + 1,
                ques = ques[index].DisplayName,
                quesimg = string.IsNullOrEmpty(ques[index].ImageName) ? "" :
                ImageHelper.ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, ques[index].ImageName)),
                answers = anses,
                ismultichoice = ques[index].IsmultiAns == true
            };

            // Thêm danh sách câu trả lời, và hiện các câu trả lời đã lưu
            txbUserAns.Properties.DataSource = Enumerable.Range(1, anses.Count).Select(num => num.ToString()).ToList();
            txbUserAns.EditValue = examResults[indexQues].UserAnswer;

            var templateSigner = Template.Parse(templateContentSigner);

            var pageContent = templateSigner.Render(templateData);

            webViewQues.DocumentText = pageContent;

            gvData.FocusedRowHandle = index;
        }

        private void f312_DoExam_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsClose)
            {
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>請按<color=red>「提交」</color>結束考試</font>");
                e.Cancel = true;
            }
        }

        private void f312_DoExam_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                countdownTimer.Stop();
            }
            catch { }
        }

        private void Submit_Exam()
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
            int score = (int)Math.Round(correctAnswers * 100.0 / numQues);
            int totalScore = 100;
            bool IsPass = score >= passingScore;
            string message = IsPass ? "恭喜您通過考試!" : "很遺憾你考試沒通過!";
            string json = JsonConvert.SerializeObject(examResults);

            // Nếu có thông tin của kỳ thi thì lưu kết quả lại
            if (idExamUser != -1)
            {
                dt312_ExamUser result = dt312_ExamUserBUS.Instance.GetItemById(idExamUser);
                result.IsPass = IsPass;
                result.Score = score;
                result.SubmitAt = DateTime.Now;
                result.ExamData = json;

                dt312_ExamUserBUS.Instance.AddOrUpdate(result);
            }

            string htmlString = $"<font='Microsoft JhengHei UI' size=24><color={(IsPass ? "blue" : "red")}>{message}</color></font>\r\n" +
                $"<font='Microsoft JhengHei UI' size=14>結果：\r\n" +
                $"-正確：<color=blue>{correctAnswers}/{numQues}</color>\r\n" +
                $"-成績：<color=blue>{score}/{totalScore}</color></font>";

            MsgTP.MsgShowInfomation(htmlString);

            IsClose = true;
            Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            var dialog = XtraMessageBox.Show("你確定要提交考試答案嗎", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog != DialogResult.Yes) return;

            Submit_Exam();
        }

        public List<GroupDistribution> DistributeQuestions(List<GroupDistribution> groups, int totalNeeded)
        {
            // 1. Validate: Nếu tổng cần lấy lớn hơn tổng số có sẵn -> Lấy tất cả
            int totalAvailable = groups.Sum(g => g.MaxAvailable);
            if (totalNeeded >= totalAvailable)
            {
                groups.ForEach(g => g.CountToTake = g.MaxAvailable);
                return groups;
            }

            // Reset số lượng sẽ lấy về 0 trước khi tính
            groups.ForEach(g => g.CountToTake = 0);

            // Tạo danh sách các nhóm đang chờ xét duyệt (ban đầu là tất cả)
            var pendingGroups = groups.ToList();

            while (totalNeeded > 0 && pendingGroups.Count > 0)
            {
                // Tính trung bình (dùng double để so sánh chính xác)
                double avgNeeded = (double)totalNeeded / pendingGroups.Count;

                // Tìm các nhóm "nghèo" (không đủ cung cấp mức trung bình)
                var insufficientGroups = pendingGroups.Where(g => g.MaxAvailable < avgNeeded).ToList();

                if (insufficientGroups.Count > 0)
                {
                    // TRƯỜNG HỢP 1: CÓ NHÓM THIẾU
                    // Lấy hết "vốn liếng" của các nhóm này
                    foreach (var group in insufficientGroups)
                    {
                        group.CountToTake = group.MaxAvailable; // Lấy hết
                        totalNeeded -= group.CountToTake;       // Giảm tổng cần lấy
                        pendingGroups.Remove(group);            // Loại khỏi danh sách chia tiếp theo
                    }
                    // Vòng lặp sẽ quay lại bước 1 để tính lại trung bình mới cho các nhóm còn lại
                }
                else
                {
                    // TRƯỜNG HỢP 2: TẤT CẢ NHÓM CÒN LẠI ĐỀU ĐỦ (Hoặc dư)
                    int baseCount = totalNeeded / pendingGroups.Count; // Phần nguyên
                    int remainder = totalNeeded % pendingGroups.Count; // Phần dư

                    foreach (var group in pendingGroups)
                    {
                        group.CountToTake = baseCount;

                        // Chia phần dư cho các nhóm đầu tiên trong danh sách pending
                        if (remainder > 0)
                        {
                            group.CountToTake += 1;
                            remainder--;
                        }
                    }

                    // Đã chia xong hết
                    break;
                }
            }

            return groups;
        }

        private string LoadData()
        {
            var setting = dt312_SettingBUS.Instance.GetItemById(1);

            testDuration = setting?.TestDuration ?? 0;
            passingScore = setting?.PassingScore ?? 0;
            numQues = setting?.QuesCount ?? 0;

            // --- PHẦN 1: LẤY DỮ LIỆU GỐC ---
            var dataQues = dt312_QuestionsBUS.Instance.GetList(); // Lấy tất cả câu hỏi

            if (dataQues.Count <= numQues)
                return "題目數量不夠！";

            // Group và đếm số lượng (Code cũ của bạn)
            var quesCountsQuery = from data in dataQues
                                  group data by data.GroupId into dtg
                                  select new
                                  {
                                      IdGroup = dtg.Key,
                                      QuestionCount = dtg.Count()
                                  };

            // --- PHẦN 2: CHUẨN BỊ DỮ LIỆU TÍNH TOÁN ---

            // Map sang object GroupDistribution
            var listForCalc = quesCountsQuery.Select(x => new GroupDistribution
            {
                GroupId = x.IdGroup,
                MaxAvailable = x.QuestionCount,
                CountToTake = 0
            }).ToList();

            // --- PHẦN 3: XỬ LÝ PHÂN BỔ (CODE MỚI THÊM VÀO) ---

            // BƯỚC 1: Trộn ngẫu nhiên thứ tự các nhóm 
            // (Để khi chia phần dư, nhóm nào được ưu tiên +1 sẽ là ngẫu nhiên)
            listForCalc = listForCalc.OrderBy(x => Guid.NewGuid()).ToList();

            // BƯỚC 2: Gọi hàm tính toán logic chia đều
            DistributeQuestions(listForCalc, numQues);

            // BƯỚC 3: Lấy danh sách câu hỏi thực tế dựa trên số lượng đã tính
            foreach (var groupInfo in listForCalc)
            {
                if (groupInfo.CountToTake > 0)
                {
                    // Lấy các câu hỏi thuộc nhóm này từ danh sách gốc dataQues
                    var questionsOfGroup = dataQues
                        .Where(q => q.GroupId == groupInfo.GroupId)
                        // Quan trọng: Trộn ngẫu nhiên câu hỏi TRONG nhóm để không luôn lấy các câu đầu tiên
                        .OrderBy(x => Guid.NewGuid())
                        .Take(groupInfo.CountToTake)
                        .ToList();

                    ques.AddRange(questionsOfGroup);
                }
            }

            ques = ques.OrderBy(x => Guid.NewGuid()).ToList();

            answers = dt312_AnswersBUS.Instance.GetListByListQues(ques.Select(r => r.Id).ToList())
                .GroupBy(r => r.QuesId)  // Group by QuesId
                .SelectMany(group => group.Select((answer, index) => new dt312_Answers
                {
                    Id = index + 1,
                    QuesId = answer.QuesId,
                    ImageName = answer.ImageName,
                    DisplayName = answer.DisplayName,
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
                               IsMultiChoice = question.IsmultiAns == true
                           }).ToList();

            return "";
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

                Submit_Exam();
            }
        }

        private void f312_DoExam_Load(object sender, EventArgs e)
        {
            string result = LoadData();

            if (!string.IsNullOrEmpty(result))
            {
                IsClose = true;
                MsgTP.MsgError(result);
                Close();
                return;
            }

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

            templateContentSigner = File.ReadAllText(Path.Combine(TPConfigs.ResourcesPath, "dt307_ConfirmQuestion.html"));
            InitializeWebView2(indexQues);
        }
    }
}