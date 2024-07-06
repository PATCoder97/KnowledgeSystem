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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                ImgCCITT = r.ImageName,
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