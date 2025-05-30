﻿using BusinessLayer;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraRichEdit.Model;
using DocumentFormat.OpenXml.Drawing.Charts;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_ExamDetail : DevExpress.XtraEditors.XtraForm
    {
        public f307_ExamDetail()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();
        }

        public string examCode = "";


        DXMenuItem itemViewInfo;

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看詳情", ItemViewInfo_Click, TPSvgimages.View);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            string deptName = view.GetRowCellValue(view.FocusedRowHandle, "DeptName")?.ToString() ?? "";
            string usrName = view.GetRowCellValue(view.FocusedRowHandle, "DisplayName")?.ToString() ?? "";
            string usrId = view.GetRowCellValue(view.FocusedRowHandle, "usr.Id")?.ToString() ?? "";
            string jobName = view.GetRowCellValue(view.FocusedRowHandle, "job.DisplayName")?.ToString() ?? "";
            string score = view.GetRowCellValue(view.FocusedRowHandle, "data.Score")?.ToString() ?? "";
            string json = view.GetRowCellValue(view.FocusedRowHandle, "data.ExamData")?.ToString() ?? "";

            if (string.IsNullOrEmpty(json)) return;

            List<f307_DoExam.ExamResult> examResults = JsonConvert.DeserializeObject<List<f307_DoExam.ExamResult>>(json);

            string templateContentSigner = File.ReadAllText(Path.Combine(TPConfigs.ResourcesPath, "dt307_ResultExam.html"));

            var dataexam = examResults
                .Select(data => new
                {
                    questionindex = data.QuestionIndex,
                    question = data.Questions.DisplayText,
                    questionimage = string.IsNullOrEmpty(data.Questions.ImageName) ? "" : ImageHelper.ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, data.Questions.ImageName)),
                    answers = data.Answers.Select(r => new
                    {
                        id = r.Id,
                        answertext = r.DisplayText,
                        answerimage = string.IsNullOrEmpty(r.ImageName) ? "" : ImageHelper.ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, r.ImageName))
                    }).ToList(),
                    correctanswer = data.CorrectAnswer,
                    useranswer = data.UserAnswer,
                    ismultichoice = data.IsMultiChoice,
                    iscorrect = data.IsCorrect
                }).ToList();

            var datahtml = new
            {
                username = usrName,
                userid = usrId,
                dept = deptName,
                score = score,
                jobname = jobName,
                dataexam = dataexam
            };

            var templateSigner = Template.Parse(templateContentSigner);
            var pageContent = templateSigner.Render(datahtml);

            WebBrowser webView = new WebBrowser
            {
                Dock = DockStyle.Fill
            };

            XtraForm formView = new XtraForm
            {
                Text = "考試結果",
                WindowState = FormWindowState.Maximized,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            formView.Controls.Add(webView);

            formView.Load += async (o, args) =>
            {
                webView.DocumentText = pageContent;
            };

            formView.ShowDialog();
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void LoadData()
        {
            var bases = dt307_ExamUserBUS.Instance.GetListByExamCode(examCode);
            var usrs = dm_UserBUS.Instance.GetList();
            var jobs = dm_JobTitleBUS.Instance.GetList();
            var depts = dm_DeptBUS.Instance.GetList();

            var datas = (from data in bases
                         join usr in usrs on data.IdUser equals usr.Id
                         join job in jobs on data.IdJob equals job.Id
                         join dept in depts on usr.IdDepartment equals dept.Id
                         let DeptName = $"{dept.Id}\r\n{dept.DisplayName}"
                         let DisplayName = $"{usr.DisplayName}\r\n{usr.DisplayNameVN}"
                         select new
                         {
                             data,
                             usr,
                             job,
                             DeptName,
                             DisplayName
                         }).ToList();

            gcData.DataSource = datas;
            gvData.BestFitColumns();
        }

        private void f307_ExamDetail_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                bool isComplete = !string.IsNullOrEmpty(view.GetRowCellValue(view.FocusedRowHandle, "data.SubmitTime")?.ToString());

                if (isComplete)
                {
                    e.Menu.Items.Add(itemViewInfo);
                }
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"考試結果 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }
    }
}