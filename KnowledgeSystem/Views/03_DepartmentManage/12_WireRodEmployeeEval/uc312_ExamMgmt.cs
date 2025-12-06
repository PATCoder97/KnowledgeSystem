using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class uc312_ExamMgmt : DevExpress.XtraEditors.XtraUserControl
    {
        public uc312_ExamMgmt()
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

        List<dt312_ExamMgmt> examMgmt;
        List<dt312_ExamUser> examUsers;

        DXMenuItem itemViewInfo;
        DXMenuItem itemStartExam;
        DXMenuItem itemFinishExam;
        DXMenuItem itemExportExam;
        DXMenuItem itemExportStatistical;

        private void InitializeMenuItems()
        {
            //itemViewInfo = CreateMenuItem("查看詳情", ItemViewInfo_Click, TPSvgimages.View);
            //itemStartExam = CreateMenuItem("開始考試", ItemStartExam_Click, TPSvgimages.Start);
            //itemFinishExam = CreateMenuItem("完成考試", ItemFinishExam_Click, TPSvgimages.Finish);
            //itemExportExam = CreateMenuItem("導出結果", ItemExportExam_Click, TPSvgimages.Print);
            //itemExportStatistical = CreateMenuItem("導出匯總表", ItemExportStatistical_Click, TPSvgimages.Excel);
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
            //gvData.FormatRules.AddExpressionRule(gColStatus, new DevExpress.Utils.AppearanceDefault() { BackColor = Color.Red, BackColor2 = Color.White }, $"IsNullOrEmpty([FinishTime])");
            //gvData.FormatRules.AddExpressionRule(gColStatus, new DevExpress.Utils.AppearanceDefault() { BackColor = Color.LightGreen, BackColor2 = Color.White }, $"IsNullOrEmpty([StartTime])");
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                users = dm_UserBUS.Instance.GetList();
                examMgmt = dt312_ExamMgmtBUS.Instance.GetList();
                examUsers = dt312_ExamUserBUS.Instance.GetList().Select(r => new dt312_ExamUser()
                {
                    Id = r.Id,
                    ExamId = r.ExamId,
                    SubmitAt = r.SubmitAt,
                    Score = r.Score,
                    IsPass = r.IsPass,
                    ExamData = null,
                    UserId = $"{r.UserId} {users.FirstOrDefault(x => x.Id == r.UserId)?.DisplayName}"
                }).ToList();

                sourceBases.DataSource = examMgmt;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
            }
        }

        private void uc312_ExamMgmt_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvEmp.ReadOnlyGridView();
            gvEmp.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvEmp.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gcData.ForceInitialize();
            //gvData.CustomUnboundColumnData += gvData_CustomUnboundColumnData;
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = (int)view.GetRowCellValue(e.RowHandle, gColId);
            e.ChildList = examUsers.Where(r => r.ExamId == idBase).ToList();
        }

        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = (int)view.GetRowCellValue(e.RowHandle, gColId);
            e.IsEmpty = !examUsers.Any(r => r.ExamId == idBase);
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "emp";
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
