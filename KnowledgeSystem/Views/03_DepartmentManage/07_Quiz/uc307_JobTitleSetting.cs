using BusinessLayer;
using DataAccessLayer;
using DevExpress.Pdf;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._02_JFEnCSCDocs;
using KnowledgeSystem.Views._03_DepartmentManage._06_Signature;
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
    public partial class uc307_JobTitleSetting : DevExpress.XtraEditors.XtraUserControl
    {
        public uc307_JobTitleSetting()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();
            CreateRuleGV();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();

        DXMenuItem itemEditInfo;

        List<dm_User> lsUser = new List<dm_User>();
        List<dt202_Type> types;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            gvData.FormatRules.AddExpressionRule(gColQuesCount, new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Red }, "[Count] < [QuizData.QuesCount]");

            gvData.FormatRules.AddExpressionRule(gColMultiQues, new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Red }, "[MultiQuesCount] < [QuizData.MultiQues]");
        }

        private void InitializeMenuItems()
        {
            itemEditInfo = CreateMenuItem("更新", ItemEditInfo_Click, TPSvgimages.Edit);
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

        private void ItemEditInfo_Click(object sender, EventArgs e)
        {
            string idJob = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId).ToString();

            f307_JobSet_Info fInfo = new f307_JobSet_Info();
            fInfo.idJob = idJob;
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var baseData = dt307_JobQuesManageBUS.Instance.GetList();
                var jobs = dm_JobTitleBUS.Instance.GetList().Where(r => r.Id.EndsWith("J")).ToList();
                var depts = dm_DeptBUS.Instance.GetList();

                var ques = dt307_QuestionsBUS.Instance.GetList();
                var quesCount = from data in ques
                                group data by data.IdJob into dtg
                                select new
                                {
                                    IdJob = dtg.Key,
                                    QuestionCount = dtg.Count(),
                                    MultiQuesCount = dtg.Count(r => r.IsMultiAns == true),
                                };

                // First Join: jobs with baseData
                var jobWithBaseData = from job in jobs
                                      join data in baseData on job.Id equals data.JobId into dtg1
                                      from g1 in dtg1.DefaultIfEmpty()
                                      select new
                                      {
                                          JobTitle = job,
                                          QuizData = g1
                                      };

                // Second Join: result of the first join with depts
                var jobWithBaseDataAndDept = from jobData in jobWithBaseData
                                             join dept in depts on (jobData.QuizData != null ? jobData.QuizData.IdDept : default) equals dept.Id into dtg3
                                             from g3 in dtg3.DefaultIfEmpty()
                                             select new
                                             {
                                                 jobData.JobTitle,
                                                 jobData.QuizData,
                                                 Dept = g3 != null ? $"{g3.Id}\n{g3.DisplayName}" : ""
                                             };

                // Third Join: result of the second join with quesCount
                var jobWithAllData = from jobData in jobWithBaseDataAndDept
                                     join count in quesCount on (jobData.QuizData != null ? jobData.QuizData.JobId : default) equals count.IdJob into dtg2
                                     from g2 in dtg2.DefaultIfEmpty()
                                     select new
                                     {
                                         jobData.QuizData,
                                         jobData.JobTitle,
                                         jobData.Dept,
                                         Count = g2?.QuestionCount ?? 0,
                                         MultiQuesCount = g2?.MultiQuesCount ?? 0
                                     };

                // Convert to list
                var dataDisplays = jobWithAllData.ToList();

                sourceBases.DataSource = dataDisplays;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
            }
        }

        private void uc307_JobTitleSetting_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
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

            string filePath = Path.Combine(documentsPath, $"職務的考卷題目設定 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemEditInfo);
            }
        }
    }
}
