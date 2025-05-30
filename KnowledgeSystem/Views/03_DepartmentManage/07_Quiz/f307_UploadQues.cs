﻿using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_UploadQues : DevExpress.XtraEditors.XtraForm
    {
        public f307_UploadQues()
        {
            InitializeComponent();
        }

        public DataTable dtBase;
        public string pathImages = "";
        public string idJob = "";

        List<dt307_Questions> ques;
        List<dt307_Answers> answers;

        static void CleanDataTable(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (row[column] is string)
                    {
                        row[column] = CleanText(row[column].ToString());
                    }
                }
            }
        }

        static string CleanText(string input)
        {
            string cleaned = Regex.Replace(input, @"[\t\n\r\s]+", match =>
            {
                if (match.Value.Contains("\n"))
                {
                    return "\r\n";
                }
                else
                {
                    return " ";
                }
            }).Trim();

            return cleaned;
        }

        private void f307_UploadQues_Load(object sender, EventArgs e)
        {
            CleanDataTable(dtBase);
            gcData.DataSource = dtBase;
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ques = new List<dt307_Questions>();
            answers = new List<dt307_Answers>();

            int currentQuesId = 0;
            int answerId = 1; // Biến đếm để theo dõi ID hiện tại cho câu trả lời

            for (int row = 0; row < dtBase.Rows.Count; row++)
            {
                int quesId = 0;
                int.TryParse(dtBase.Rows[row][0].ToString(), out quesId);

                if (quesId != currentQuesId && quesId != 0)
                {
                    // Thêm câu hỏi mới
                    ques.Add(new dt307_Questions()
                    {
                        DisplayText = dtBase.Rows[row][1].ToString(),
                        ImageName = string.IsNullOrEmpty(dtBase.Rows[row][2].ToString()) ? "" : Path.Combine(pathImages, dtBase.Rows[row][2].ToString()),
                    });
                    currentQuesId = quesId;
                    answerId = 1; // Đặt lại biến đếm answerId khi câu hỏi đổi
                }

                int resultAns = 0;
                int.TryParse(dtBase.Rows[row][6].ToString(), out resultAns);
                bool IsTrueAns = resultAns == 1;

                // Thêm câu trả lời mới
                answers.Add(new dt307_Answers()
                {
                    Id = answerId++, // Tăng ID mỗi khi thêm câu trả lời mới
                    QuesId = currentQuesId,
                    DisplayText = dtBase.Rows[row][4].ToString(),
                    ImageName = string.IsNullOrEmpty(dtBase.Rows[row][5].ToString()) ? "" : Path.Combine(pathImages, dtBase.Rows[row][5].ToString()),
                    TrueAns = IsTrueAns
                });
            }

            f307_ConfirmQues fConfirm = new f307_ConfirmQues();
            fConfirm.ques = ques;
            fConfirm.answers = answers;
            fConfirm.ShowDialog();

            // Nếu xác nhận thành công thì cập nhật vào CSDL
            bool IsConfirmed = fConfirm.IsConfirmed;
            if (!IsConfirmed) return;

            if (!Directory.Exists(TPConfigs.Folder307))
                Directory.CreateDirectory(TPConfigs.Folder307);

            int index = 1;
            foreach (var item in ques)
            {
                var data = new dt307_Questions()
                {
                    IdJob = idJob,
                    DisplayText = item.DisplayText,
                    ImageName = string.IsNullOrEmpty(item.ImageName) ? ""
                    : $"{EncryptionHelper.EncryptionFileName(Path.GetFileName(item.ImageName))}{Path.GetExtension(item.ImageName)}"
                };

                int idQues = dt307_QuestionsBUS.Instance.Add(data);
                answers.Where(r => r.QuesId == index).ToList().ForEach(r => r.QuesId = idQues);
                index++;

                if (string.IsNullOrEmpty(item.ImageName)) continue;
                File.Copy(item.ImageName, Path.Combine(TPConfigs.Folder307, data.ImageName), true);
            }

            List<dt307_Answers> anwsAdd = new List<dt307_Answers>();
            foreach (var item in answers)
            {
                var data = new dt307_Answers()
                {
                    QuesId = item.QuesId,
                    DisplayText = item.DisplayText,
                    TrueAns = item.TrueAns,
                    ImageName = string.IsNullOrEmpty(item.ImageName) ? ""
                    : $"{EncryptionHelper.EncryptionFileName(Path.GetFileName(item.ImageName))}{Path.GetExtension(item.ImageName)}"
                };

                anwsAdd.Add(data);

                if (string.IsNullOrEmpty(item.ImageName)) continue;
                File.Copy(item.ImageName, Path.Combine(TPConfigs.Folder307, data.ImageName), true);
            }

            var b = dt307_AnswersBUS.Instance.AddRange(anwsAdd);

            Close();
        }
    }
}