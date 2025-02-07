using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    public partial class uc308_HealthCheckMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc308_HealthCheckMain()
        {
            InitializeComponent();
            InitializeIcon();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.idDept2word;

        List<dm_User> users = new List<dm_User>();
        List<dm_JobTitle> jobs;

        List<dt308_CheckSession> dt308CheckSession;
        List<dt308_CheckDetail> dt308CheckDetail;
        List<dt308_Disease> dt308Diseases;

        Dictionary<string, string> DiseaseType = new Dictionary<string, string>()
        {
            { "1", "Thong thuowng" },
            { "2", "man tinh" },
            { "3", "nghe nghiep" }
        };


        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            //var rule = new GridFormatRule
            //{
            //    ApplyToRow = true,
            //    Name = "RuleNotify",
            //    Rule = new FormatConditionRuleExpression
            //    {
            //        Expression = "AddMonths(Today(),3) >= AddMonths([data.DeployDate], [data.PeriodNotify] + Iif(IsNullOrEmpty([data.PauseNotify]), 0, [data.PauseNotify]))",
            //        Appearance =
            //        {
            //            BackColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
            //            BackColor2 = Color.White,
            //            Options = { UseBackColor = true }
            //        }
            //    }
            //};

            //// Thêm quy tắc vào GridView
            //gvData.FormatRules.Add(rule);
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();
                dt308CheckSession = dt308_CheckSessionBUS.Instance.GetList();
                dt308CheckDetail = dt308_CheckDetailBUS.Instance.GetList();
                dt308Diseases = dt308_DiseaseBUS.Instance.GetList();

                sourceBases.DataSource = dt308CheckSession;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f308_CheckSession_Info fAdd = new f308_CheckSession_Info()
            {
                eventInfo = EventFormInfo.Create,
                formName = "健康檢查"
            };
            fAdd.ShowDialog();

            LoadData();
        }

        private void uc308_HealthCheckMain_Load(object sender, EventArgs e)
        {

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            //gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvSession.ReadOnlyGridView();
            gvSession.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvDetail.ReadOnlyGridView();
            gvDetail.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "CheckSession";
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idSession = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColIdSession));
            e.IsEmpty = !dt308CheckDetail.Any(r => r.SessionId == idSession);
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idSession = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColIdSession));
            e.ChildList = dt308CheckDetail.Where(r => r.SessionId == idSession).ToList();
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gvSession_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "CheckDetail";
        }

        private void gvSession_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvSession_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idDetail = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColIdDetail));

            var detail = dt308CheckDetail.FirstOrDefault(r => r.Id == idDetail);
            e.IsEmpty = detail == null || (string.IsNullOrEmpty(detail.Disease1) && string.IsNullOrEmpty(detail.Disease2) && string.IsNullOrEmpty(detail.Disease3));
        }

        private void gvSession_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idDetail = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColIdDetail));
            var detail = dt308CheckDetail.FirstOrDefault(r => r.Id == idDetail);

            var disease1 = (detail.Disease1 ?? "").Split(',').ToList();
            var disease2 = (detail.Disease2 ?? "").Split(',').ToList();
            var disease3 = (detail.Disease3 ?? "").Split(',').ToList();
            var disease = disease1.Concat(disease2).Concat(disease3).ToList();


            e.ChildList = dt308Diseases.Where(r => disease.Contains(r.Id.ToString())).Select(r=>new
            {
                r.Id,
                r.DisplayNameVN,
                r.DisplayNameTW,
                r.DiseaseType,
                DiseaseTypeName = DiseaseType[r.DiseaseType.ToString()]
            }).ToList();
        }


    }
}
