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
using System.Windows.Media.Animation;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
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
            InitializeMenuItems();

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
            { "1", "Bệnh thông thường\r\n一般疾病" },
            { "2", "Bệnh mãn tính\r\n慢性病" },
            { "3", "Bệnh nghề nghiệp\r\n得職業病" }
        };

        DXMenuItem itemViewInfo;
        DXMenuItem itemCreateScript;
        DXMenuItem itemGetFile;
        DXMenuItem itemPauseNotify;
        DXMenuItem itemViewHistory;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            var rule = new GridFormatRule
            {
                ApplyToRow = true,
                Name = "RuleNotify",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[HealthRating] = -1",
                    Appearance =
                    {
                        BackColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
                        BackColor2 = Color.White,
                        Options = { UseBackColor = true }
                    }
                }
            };

            // Thêm quy tắc vào GridView
            gvSession.FormatRules.Add(rule);
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

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
            itemCreateScript = CreateMenuItem("導出GoogleForm", ItemCreateScript_Click, TPSvgimages.EmailSend);
            //itemGetFile = CreateMenuItem("下載檔案", ItemGetFile_Click, TPSvgimages.Attach);
            //itemPauseNotify = CreateMenuItem("暫停通知", ItemPauseNotify_Click, TPSvgimages.Suspension);
            //itemViewHistory = CreateMenuItem("版本歷史", ItemViewHistory_Click, TPSvgimages.Progress);
        }

        private void ItemCreateScript_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            dt308_CheckSession session = view.GetRow(view.FocusedRowHandle) as dt308_CheckSession;

            var diseaseTitles = new List<Tuple<string, string>>
            {
                Tuple.Create("Bạn mắc các bệnh thông thường nào sau đây nào sau đây?", "您患有以下哪些一般疾病？"),
                Tuple.Create("Bạn mắc các bệnh mãn tính nào sau đây nào sau đây?", "您患有以下哪些慢性疾病？"),
                Tuple.Create("Bạn mắc các bệnh nghề nghiệp nào sau đây nào sau đây?", "您患有以下哪些得職業病？")
            };

            string sourceScript = File.ReadAllText(Path.Combine(TPConfigs.HtmlPath, "f308_GoogleAppScript.txt"));
            var scriptGoogleForm = sourceScript.Replace("{{formname}}", $"{session.DisplayNameVN}/{session.DisplayNameTW}");

            for (int i = 0; i < diseaseTitles.Count; i++)
            {
                string checkboxName = $"checkboxDiseases{i + 1}";
                string diseasesCode = string.Join(",", dt308Diseases
                    .Where(r => r.DiseaseType == i + 1)
                    .Select(r => $"{checkboxName}.createChoice('({r.Id:D2}) {r.DisplayNameVN}/{r.DisplayNameTW}')")
                    .ToList());

                scriptGoogleForm = scriptGoogleForm.Replace($"{{{{diseases{i + 1}}}}}", $"{checkboxName}.setTitle('{diseaseTitles[i].Item1}\\n{diseaseTitles[i].Item2}').setChoices([{diseasesCode}]);");
            }

            Clipboard.SetText(scriptGoogleForm);
            MessageBox.Show("ok");

            Process.Start("https://script.google.com/");
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idSession = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdSession));
            f308_CheckSession_Info fAdd = new f308_CheckSession_Info()
            {
                eventInfo = EventFormInfo.View,
                formName = "健康檢查",
                idSession = idSession
            };
            fAdd.ShowDialog();

            LoadData();
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

                users = dm_UserBUS.Instance.GetList();
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
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvSession.ReadOnlyGridView();
            gvSession.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvSession.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
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
            e.ChildList = dt308CheckDetail
                .Where(r => r.SessionId == idSession)
                .Select(r =>
                {
                    var user = users.FirstOrDefault(u => u.Id == r.EmpId);
                    return new
                    {
                        r.SessionId,
                        r.Id,
                        r.EmpId,
                        r.HealthRating,
                        r.Disease1,
                        r.Disease2,
                        r.Disease3,
                        EmpName = user != null ? $"LG{user.IdDepartment} {user.Id} {user.DisplayName} {user.DisplayNameVN}" : "Unknown"
                    };
                })
                .ToList();

        }

        private void gv_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
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


            e.ChildList = dt308Diseases.Where(r => disease.Contains(r.Id.ToString())).Select(r => new
            {
                r.Id,
                r.DisplayNameVN,
                r.DisplayNameTW,
                r.DiseaseType,
                DiseaseTypeName = DiseaseType[r.DiseaseType.ToString()]
            }).ToList();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                //itemGetFile.Tag = "gvData";

                e.Menu.Items.Add(itemViewInfo);
                e.Menu.Items.Add(itemCreateScript);
                //    e.Menu.Items.Add(itemGetFile);
                //    itemPauseNotify.BeginGroup = true;
                //    e.Menu.Items.Add(itemPauseNotify);
                //    e.Menu.Items.Add(itemViewHistory);
            }
        }
    }
}
