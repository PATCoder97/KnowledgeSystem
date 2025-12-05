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

        public class CheckQuestion
        {
            public string DisplayText { get; set; }
            public string ImageName { get; set; }
        }

        public class CheckAnswer
        {
            public int Id { get; set; }
            public int QuesId { get; set; }
            public string DisplayText { get; set; }
            public string ImageName { get; set; }
            public bool TrueAns { get; set; }
        }

        public List<CheckQuestion> ques;
        public List<CheckAnswer> answers;
        string templateContentSigner;
        int indexQues = 0;

        public bool IsConfirmed { get; set; }

        private void InitializeWebView2(int index)
        {
            lbPageNumber.Caption = $"{index + 1}/{ques.Count}";

            var anses = answers.Where(r => r.QuesId == index + 1).Select(r => new
            {
                id = r.Id,
                disp = r.DisplayText,
                img = string.IsNullOrEmpty(r.ImageName) ? "" : ImageHelper.ConvertImageToBase64DataUri(r.ImageName),
                istrue = r.TrueAns
            }).ToList();

            var templateData = new
            {
                ques = ques[index].DisplayText,
                quesimg = string.IsNullOrEmpty(ques[index].ImageName) ? "" : ImageHelper.ConvertImageToBase64DataUri(ques[index].ImageName),
                answers = anses
            };

            var templateSigner = Template.Parse(templateContentSigner);

            webViewQues.DocumentText = templateSigner.Render(templateData);
        }

        private void f307_ConfirmQues_Load(object sender, EventArgs e)
        {
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            templateContentSigner = System.IO.File.ReadAllText(Path.Combine(TPConfigs.ResourcesPath, "dt307_ConfirmQuestion.html"));
            InitializeWebView2(indexQues);
        }

        private void btnPrevius_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (indexQues <= 0)
            {
                return;
            }

            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            indexQues--;
            InitializeWebView2(indexQues);
        }

        private void btnNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (indexQues >= ques.Count - 1)
            {
                btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                return;
            }

            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            indexQues++;
            InitializeWebView2(indexQues);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IsConfirmed = true;
            Close();
        }
    }
}