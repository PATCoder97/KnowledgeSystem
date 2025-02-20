using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class uc309_SparePartMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_SparePartMain()
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

        List<dt309_Materials> materials;
        List<dt308_CheckDetail> dt308CheckDetail;
        List<dt308_Disease> dt308Diseases;

        DXMenuItem itemViewInfo;
        DXMenuItem itemCreateScript;
        DXMenuItem itemEditDetail;
        DXMenuItem itemExcelUploadDetail;
        DXMenuItem itemGoogleSheetUploadDetail;

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
            itemCreateScript = CreateMenuItem("導出GoogleForm程式碼", ItemCreateScript_Click, TPSvgimages.GgForm);
            itemEditDetail = CreateMenuItem("更新檢查表", ItemEditDetail_Click, TPSvgimages.Edit);
            itemExcelUploadDetail = CreateMenuItem("上傳Excel檔案", ItemExcelUploadDetail_Click, TPSvgimages.Excel);
            itemGoogleSheetUploadDetail = CreateMenuItem("上傳GoogleSheet路徑", ItemGoogleSheetUploadDetail_Click, TPSvgimages.GgSheet);
        }

        private void ShowDataDetailRaw(int idSession)
        {
        }

        private void ItemGoogleSheetUploadDetail_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            dt308_CheckSession session = view.GetRow(view.FocusedRowHandle) as dt308_CheckSession;

            //string result = XtraInputBox.Show("Nhập đường đãn Google Sheet của bài khảo sát:", "Điền liên kết google sheet", "");
            //if (string.IsNullOrEmpty(result?.ToString())) return;

            //var rawlink = result.ToString();

            //var match = Regex.Match(rawlink, @"/d/([a-zA-Z0-9-_]+)");
            //if (!match.Success) return;

            //string sheetId = match.Groups[1].Value;
            //var google_sheet_url = $@"https://docs.google.com/spreadsheets/d/{sheetId}/gviz/tq?tqx=out:html&tq&gid=1";

            //dtDetailExcel = GetGoogleSheetAsDataTable(google_sheet_url);

            //ShowDataDetailRaw(session.Id);
        }

        private void ItemExcelUploadDetail_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFile = new OpenFileDialog()
            //{
            //    Filter = "Excel |*.xlsx"
            //};

            //if (openFile.ShowDialog() != DialogResult.OK) return;

            //GridView view = gvData;
            //dt308_CheckSession session = view.GetRow(view.FocusedRowHandle) as dt308_CheckSession;

            //using (var stream = File.Open(openFile.FileName, FileMode.Open, FileAccess.Read))
            //{
            //    IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            //    DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
            //    {
            //        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
            //        {
            //            UseHeaderRow = true
            //        }
            //    });

            //    reader.Close();

            //    dtDetailExcel = ds.Tables[0];
            //}

            //ShowDataDetailRaw(session.Id);
        }

        private void ItemEditDetail_Click(object sender, EventArgs e)
        {
            //GridView detailGridView = gvData.GetDetailView(gvData.FocusedRowHandle, 0) as GridView;
            //if (detailGridView != null)
            //{
            //    int detailFocusedRowHandle = detailGridView.FocusedRowHandle;
            //    if (detailFocusedRowHandle < 0) return;

            //    var idReport = Convert.ToInt16(detailGridView.GetRowCellValue(detailFocusedRowHandle, gColIdDetail));

            //    f308_CheckData fData = new f308_CheckData()
            //    {
            //        eventInfo = EventFormInfo.Update,
            //        formName = "健康檢查",
            //        idDetail = idReport
            //    };
            //    fData.ShowDialog();

            //    LoadData();
            //}
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

            string sourceScript = File.ReadAllText(Path.Combine(TPConfigs.ResourcesPath, "f308_GoogleAppScript.txt"));
            var scriptGoogleForm = sourceScript.Replace("{{formname}}", $"{session.DisplayNameVN}/{session.DisplayNameTW} - {System.DateTime.Now:yyyyMMddHHmmss}");

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
            XtraMessageBox.Show("Đã lưu Code vào bộ nhớ tạm, Làm theo SOP để tạo được khảo sát google form !", "Thông báo!");

            Process.Start("https://script.google.com/");
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idMaterial = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdMaterial));
            f309_Material_Info fInfo = new f309_Material_Info()
            {
                eventInfo = EventFormInfo.View,
                formName = "備品",
                idBase = idMaterial
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                materials = dt309_MaterialsBUS.Instance.GetListByIdDept(TPConfigs.LoginUser.IdDepartment);


                sourceBases.DataSource = materials;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
                //gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;

                users = dm_UserBUS.Instance.GetList();
            }
        }

        private void uc309_SparePartMain_Load(object sender, EventArgs e)
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

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f309_Material_Info finfo = new f309_Material_Info();
            finfo.ShowDialog();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);

                //itemCreateScript.BeginGroup = true;
                //e.Menu.Items.Add(itemCreateScript);
                //e.Menu.Items.Add(itemGoogleSheetUploadDetail);
                //e.Menu.Items.Add(itemExcelUploadDetail);
            }
        }
    }
}
