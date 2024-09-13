using BusinessLayer;
using DataAccessLayer;
using DevExpress.Pdf;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._06_Signature;
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
    public partial class uc307_ExamMgmt : DevExpress.XtraEditors.XtraUserControl
    {
        public uc307_ExamMgmt()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();

        List<dm_User> users = new List<dm_User>();
        List<dm_JobTitle> jobs;

        List<dt307_ExamMgmt> bases;
        List<dm_Attachment> attachments;

        DXMenuItem itemViewInfo;
        DXMenuItem itemStartExam;
        DXMenuItem itemFinishExam;
        DXMenuItem itemExportExam;

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看詳情", ItemViewInfo_Click, TPSvgimages.View);
            itemStartExam = CreateMenuItem("開始考試", ItemStartExam_Click, TPSvgimages.Start);
            itemFinishExam = CreateMenuItem("完成考試", ItemFinishExam_Click, TPSvgimages.Finish);
            itemExportExam = CreateMenuItem("導出結果", ItemExportExam_Click, TPSvgimages.Print);
        }

        private void ItemExportExam_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() != DialogResult.OK) return;
            string rootFolder = folderBrowser.SelectedPath;

            GridView view = gvData;
            string examCode = view.GetRowCellValue(view.FocusedRowHandle, gColExamCode).ToString();
            string saveFolder = rootFolder + $"//{examCode} {DateTime.Now:yyyyMMddHHmmss}";
            Directory.CreateDirectory(saveFolder);

            var bases = dt307_ExamUserBUS.Instance.GetListByExamCode(examCode);
            string dataTpl = File.ReadAllText(Path.Combine(TPConfigs.HtmlPath, "dt307_ResultExam.html"));

            var usrs = dm_UserBUS.Instance.GetList();
            var jobs = dm_JobTitleBUS.Instance.GetList();
            var depts = dm_DeptBUS.Instance.GetList();

            foreach (var item in bases)
            {
                if (string.IsNullOrEmpty(item.ExamData)) return;
                List<f307_DoExam.ExamResult> examDatas = JsonConvert.DeserializeObject<List<f307_DoExam.ExamResult>>(item.ExamData);

                var dataexam = (from data in examDatas
                                select new
                                {
                                    questionindex = data.QuestionIndex,
                                    question = data.Questions.DisplayText,
                                    questionimage = string.IsNullOrEmpty(data.Questions.ImageName) ? "" : ImageHelper.ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, data.Questions.ImageName)),
                                    answers = data.Answers.Select(r => new { id = r.Id, answertext = r.DisplayText, answerimage = string.IsNullOrEmpty(r.ImageName) ? "" : ImageHelper.ConvertImageToBase64DataUri(Path.Combine(TPConfigs.Folder307, r.ImageName)) }).ToList(),
                                    correctanswer = data.CorrectAnswer,
                                    useranswer = data.UserAnswer
                                }).ToList();

                var usr = usrs.FirstOrDefault(r => r.Id == item.IdUser);
                var dept = depts.FirstOrDefault(r => r.Id == usr.IdDepartment);
                var job = jobs.FirstOrDefault(r => r.Id == item.IdJob);

                var datahtml = new
                {
                    username = $"{usr.DisplayName}\r\n{usr.DisplayNameVN}",
                    userid = item.IdUser,
                    dept = $"{dept.Id}\r\n{dept.DisplayName}",
                    score = item.Score,
                    jobname = job.DisplayName,
                    dataexam = dataexam
                };

                var tpl = Template.Parse(dataTpl);

                var res = tpl.Render(datahtml);
                string fileName = Path.Combine(saveFolder, $"{item.IdUser} {item.IdJob}.html");

                File.WriteAllText(fileName, res);
            }

            MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>已導出完成！！！</font>");
        }

        private void ItemFinishExam_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            var itemUpdate = dt307_ExamMgmtBUS.Instance.GetItemById(idBase);
            itemUpdate.FinishTime = DateTime.Now;

            dt307_ExamMgmtBUS.Instance.AddOrUpdate(itemUpdate);

            LoadData();
        }

        private void ItemStartExam_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            var itemUpdate = dt307_ExamMgmtBUS.Instance.GetItemById(idBase);
            itemUpdate.StartTime = DateTime.Now;

            dt307_ExamMgmtBUS.Instance.AddOrUpdate(itemUpdate);

            LoadData();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            string examCode = view.GetRowCellValue(view.FocusedRowHandle, gColExamCode).ToString();

            f307_ExamDetail fDetail = new f307_ExamDetail();
            fDetail.examCode = examCode;
            fDetail.ShowDialog();

            LoadData();
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

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            gvData.FormatRules.AddExpressionRule(gColStatus, new DevExpress.Utils.AppearanceDefault() { BackColor = Color.Red, BackColor2 = Color.White }, $"IsNullOrEmpty([FinishTime])");
            gvData.FormatRules.AddExpressionRule(gColStatus, new DevExpress.Utils.AppearanceDefault() { BackColor = Color.LightGreen, BackColor2 = Color.White }, $"IsNullOrEmpty([StartTime])");
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                bases = dt307_ExamMgmtBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();
                jobs = dm_JobTitleBUS.Instance.GetList();

                sourceBases.DataSource = bases;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
            }
        }

        private void uc307_ExamMgmt_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gcData.ForceInitialize();
            gvData.CustomUnboundColumnData += gvData_CustomUnboundColumnData;
        }

        private void gvData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Column.FieldName == "Status" && e.IsGetData)
            {
                bool isStart = !string.IsNullOrEmpty(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "StartTime")?.ToString());
                bool isFinish = !string.IsNullOrEmpty(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "FinishTime")?.ToString());

                if (isFinish)
                {
                    e.Value = "考試完畢";
                }
                else if (isStart)
                {
                    e.Value = "考試中";
                }
                else
                {
                    e.Value = "還沒開始";
                }
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f307_Exam_Info fInfo = new f307_Exam_Info();
            fInfo.ShowDialog();

            LoadData();
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

            string filePath = Path.Combine(documentsPath, $"考試系統 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                bool isNotStart = string.IsNullOrEmpty(view.GetRowCellValue(view.FocusedRowHandle, "StartTime")?.ToString());
                bool isStart = string.IsNullOrEmpty(view.GetRowCellValue(view.FocusedRowHandle, "FinishTime")?.ToString());

                e.Menu.Items.Add(itemViewInfo);

                if (isStart)
                {
                    e.Menu.Items.Add(isNotStart ? itemStartExam : itemFinishExam);
                }
                else if (!isNotStart)
                {
                    e.Menu.Items.Add(itemExportExam);
                }
            }
        }
    }
}
