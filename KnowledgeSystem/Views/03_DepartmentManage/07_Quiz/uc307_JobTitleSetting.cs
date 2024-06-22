using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._02_JFEnCSCDocs;
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

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();

        DXMenuItem itemViewInfo;
        DXMenuItem itemViewFile;

        List<dm_User> lsUser = new List<dm_User>();
        List<dt202_Type> types;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = new DXMenuItem("編輯", ItemViewInfo_Click, TPSvgimages.Info, DXMenuItemPriority.Normal);
            itemViewFile = new DXMenuItem("讀取", ItemViewFile_Click, TPSvgimages.View, DXMenuItemPriority.Normal);

            itemViewInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewInfo.AppearanceHovered.ForeColor = Color.Blue;

            itemViewFile.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewFile.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void ItemViewFile_Click(object sender, EventArgs e)
        {
            //int idFile = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdFile));

            //var fileInfo = attachmentsInfo.FirstOrDefault(r => r.Id == idFile);

            //if (fileInfo == null) return;

            //string source = Path.Combine(TPConfigs.Folder202, fileInfo.EncryptionName);
            //string dest = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyMMddhhmmss} {fileInfo.ActualName}");
            //if (!Directory.Exists(TPConfigs.TempFolderData))
            //    Directory.CreateDirectory(TPConfigs.TempFolderData);

            //File.Copy(source, dest, true);

            //f00_VIewFile viewFile = new f00_VIewFile(dest);
            //viewFile.ShowDialog();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            string idBase = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId).ToString();

            f202_DocInfo fInfo = new f202_DocInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "文件";
            fInfo.idBase202 = idBase;
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var baseData = dt307_JobQuesManageBUS.Instance.GetList();
                var jobs = dm_JobTitleBUS.Instance.GetList();
                var depts = dm_DeptBUS.Instance.GetList();

                var ques = dt307_QuestionsBUS.Instance.GetList();
                var quesCount = from data in ques
                                group data by data.IdJob into dtg
                                select new
                                {
                                    IdJob = dtg.Key,
                                    QuestionCount = dtg.Count()
                                };

                var dataDisplays = (from data in baseData
                                    join job in jobs on data.JobId equals job.Id
                                    join dept in depts on data.IdDept equals dept.Id
                                    join count in quesCount on data.JobId equals count.IdJob into dtg
                                    from g in dtg.DefaultIfEmpty()
                                    select new
                                    {
                                        data,
                                        job,
                                        Dept = $"{dept.Id}\n{dept.DisplayName}",
                                        Count = g?.QuestionCount ?? 0
                                    }).ToList();

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
    }
}
