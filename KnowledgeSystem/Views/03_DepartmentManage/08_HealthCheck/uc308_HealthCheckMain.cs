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
using System.Windows.Media.Animation;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
//using static DevExpress.Data.Mask.Internal.MaskSettings<T>;

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

        public static Dictionary<string, string> DiseaseType = new Dictionary<string, string>()
        {
            { "1", "Bệnh thông thường\r\n一般疾病" },
            { "2", "Bệnh mãn tính\r\n慢性病" },
            { "3", "Bệnh nghề nghiệp\r\n得職業病" }
        };

        DataTable dtDetailExcel = new DataTable();

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
            btnSummaryTable.ImageOptions.SvgImage = TPSvgimages.Word;
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

        public static DataTable GetGoogleSheetAsDataTable(string sheetUrl)
        {
            string htmlContent = GetHtmlContent(sheetUrl);
            return ParseHtmlToDataTable(htmlContent);
        }

        private static string GetHtmlContent(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

#if DEBUG
            // Lấy proxy mặc định từ cấu hình hệ thống (Internet Options)
            IWebProxy systemProxy = WebRequest.GetSystemWebProxy();
            Uri proxyUri = systemProxy.GetProxy(new Uri(url)); // Lấy địa chỉ proxy đang dùng

            // Tạo proxy mới với URL từ cấu hình hệ thống
            WebProxy proxy = new WebProxy(proxyUri)
            {
                // Cấu hình thông tin xác thực proxy (gồm tên, mật khẩu, domain)
                Credentials = new NetworkCredential("VNW0014732", "Anhtuan02", "VNFPG")
            };

            request.Proxy = proxy; // Gán proxy vào request
#endif

            // Gửi request và đọc dữ liệu phản hồi
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                XtraMessageBox.Show("Đường link sai hoặc không có quyền truy cập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private static DataTable ParseHtmlToDataTable(string htmlContent)
        {
            DataTable dt = new DataTable();

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var table = htmlDoc.DocumentNode.SelectSingleNode("//table");

            if (table == null)
            {
                return dt;
            }

            var headerRow = table.SelectSingleNode(".//thead/tr");
            if (headerRow != null)
            {
                foreach (var th in headerRow.SelectNodes("th"))
                {
                    dt.Columns.Add(WebUtility.HtmlDecode(th.InnerText.Trim()));
                }
            }
            else
            {
                var firstRow = table.SelectSingleNode(".//tr");
                if (firstRow != null)
                {
                    foreach (var td in firstRow.SelectNodes("td"))
                    {
                        dt.Columns.Add(td.InnerText.Trim());
                    }
                }
            }

            var rows = table.SelectNodes(".//tr");

            foreach (var row in rows.Skip(1))
            {
                DataRow dr = dt.NewRow();
                var cells = row.SelectNodes("td");

                if (cells != null)
                {
                    for (int i = 0; i < cells.Count() && i < dt.Columns.Count; i++)
                    {
                        dr[i] = WebUtility.HtmlDecode(cells[i].InnerText.Trim());
                    }

                    dt.Rows.Add(dr);
                }
            }

            return dt;
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
            if (dtDetailExcel.Rows.Count <= 0) return;

            List<dt308_CheckDetail> detailsView = new List<dt308_CheckDetail>();
            var columnName = dtDetailExcel.Columns.Cast<DataColumn>().ToList();
            int RomanToInt(string s) => Array.IndexOf(new[] { "I", "II", "III", "IV", "V" }, s) + 1;

            string ExtractDiseaseIds(string input)
            {
                return string.Join(",",
                    Regex.Matches(input ?? "", @"\((\d+)\)")
                         .Cast<Match>()
                         .Select(m => int.Parse(m.Groups[1].Value)) // Chuyển thành int
                         .Distinct()
                );
            }

            int empIdIndex = columnName.FindIndex(c => c.ColumnName.Contains("輸入您的員工代碼"));
            int healthRatingIndex = columnName.FindIndex(c => c.ColumnName.Contains("您的健康屬於哪一類"));
            int disease1Index = columnName.FindIndex(c => c.ColumnName.Contains("您患有以下哪些一般疾病"));
            int disease2Index = columnName.FindIndex(c => c.ColumnName.Contains("您患有以下哪些慢性疾病"));
            int disease3Index = columnName.FindIndex(c => c.ColumnName.Contains("您患有以下哪些得職業病"));

            int index = 1;
            foreach (DataRow row in dtDetailExcel.Rows)
            {
                int minute = (int)(Convert.ToDateTime(row[0]).Ticks / TimeSpan.TicksPerMinute);

                string empId = row[empIdIndex]?.ToString().ToUpper();
                int healthRating = RomanToInt(row[healthRatingIndex]?.ToString());
                string disease1 = ExtractDiseaseIds(row[disease1Index]?.ToString());
                string disease2 = ExtractDiseaseIds(row[disease2Index]?.ToString());
                string disease3 = ExtractDiseaseIds(row[disease3Index]?.ToString());

                detailsView.Add(new dt308_CheckDetail
                {
                    Id = minute,
                    SessionId = idSession,
                    EmpId = empId,
                    HealthRating = healthRating,
                    Disease1 = disease1,
                    Disease2 = disease2,
                    Disease3 = disease3
                });

                index++;
            }

            // Nhóm lại và lấy câu trả lời mới nhất của từng người
            detailsView = detailsView.GroupBy(d => d.EmpId).Select(g => g.OrderByDescending(d => d.Id).First()).ToList();

            f308_DetailDataReCheck fDataReCheck = new f308_DetailDataReCheck()
            {
                details = detailsView,
                idSession = idSession
            };

            fDataReCheck.ShowDialog();

            LoadData();
        }

        private void ItemGoogleSheetUploadDetail_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            dt308_CheckSession session = view.GetRow(view.FocusedRowHandle) as dt308_CheckSession;

            string result = XtraInputBox.Show("Nhập đường đãn Google Sheet của bài khảo sát:", "Điền liên kết google sheet", "");
            if (string.IsNullOrEmpty(result?.ToString())) return;

            var rawlink = result.ToString();

            var match = Regex.Match(rawlink, @"/d/([a-zA-Z0-9-_]+)");
            if (!match.Success) return;

            string sheetId = match.Groups[1].Value;
            var google_sheet_url = $@"https://docs.google.com/spreadsheets/d/{sheetId}/gviz/tq?tqx=out:html&tq&gid=1";

            dtDetailExcel = GetGoogleSheetAsDataTable(google_sheet_url);

            ShowDataDetailRaw(session.Id);
        }

        private void ItemExcelUploadDetail_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "Excel |*.xlsx"
            };

            if (openFile.ShowDialog() != DialogResult.OK) return;

            GridView view = gvData;
            dt308_CheckSession session = view.GetRow(view.FocusedRowHandle) as dt308_CheckSession;

            using (var stream = File.Open(openFile.FileName, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                reader.Close();

                dtDetailExcel = ds.Tables[0];
            }

            ShowDataDetailRaw(session.Id);
        }

        private void ItemEditDetail_Click(object sender, EventArgs e)
        {
            GridView detailGridView = gvData.GetDetailView(gvData.FocusedRowHandle, 0) as GridView;
            if (detailGridView != null)
            {
                int detailFocusedRowHandle = detailGridView.FocusedRowHandle;
                if (detailFocusedRowHandle < 0) return;

                var idReport = Convert.ToInt16(detailGridView.GetRowCellValue(detailFocusedRowHandle, gColIdDetail));

                f308_CheckData fData = new f308_CheckData()
                {
                    eventInfo = EventFormInfo.Update,
                    formName = "健康檢查",
                    idDetail = idReport
                };
                fData.ShowDialog();

                LoadData();
            }
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
            var scriptGoogleForm = sourceScript.Replace("{{formname}}", $"{session.DisplayNameVN}/{session.DisplayNameTW} - {DateTime.Now:yyyyMMddHHmmss}");

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

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
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
                }).ToList();
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
            }).OrderBy(r => r.DiseaseType).ToList();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);

                itemCreateScript.BeginGroup = true;
                e.Menu.Items.Add(itemCreateScript);
                e.Menu.Items.Add(itemGoogleSheetUploadDetail);
                e.Menu.Items.Add(itemExcelUploadDetail);
            }
        }

        private void gvSession_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemEditDetail);
            }
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = gvData;

            var pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo hitInfo = view.CalcHitInfo(pt);
            if (!hitInfo.InRowCell) return;

            gvData.ExpandMasterRow(hitInfo.RowHandle);
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"健康檢查 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void btnSummaryTable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {



        }
    }
}
