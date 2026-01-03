using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Charts.Heatmap.Native;
using DevExpress.Data.Platform.Compatibility;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Drawing.Charts;
using ExcelDataReader;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs;
using KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt;
using MiniSoftware;
using OfficeOpenXml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static DevExpress.XtraEditors.Filtering.DataItemsExtension;
using DataTable = System.Data.DataTable;
using Path = System.IO.Path;
using DevExpress.Spreadsheet;
using CellRange = ExcelDataReader.CellRange;
using System.IO.Packaging;
using System.Globalization;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

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
        DXMenuItem itemDetailNull;

        private class SickData
        {
            public string Id { get; set; }
            public string Month { get; set; }
            public List<string> Data { get; set; }
            public double TotalTime { get; set; }
            public double Count { get; set; }
            public double CountUser { get; set; }
            public double Percent { get; set; }
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnSummaryTable.ImageOptions.SvgImage = TPSvgimages.Excel;
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
                Credentials = new NetworkCredential("VNW0014732", "Anhtuan03", "VNFPG")
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
            itemDetailNull = CreateMenuItem("人員未更新資料", ItemDetailNull_Click, TPSvgimages.Filter);
        }

        private void ItemDetailNull_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            dt308_CheckSession session = view.GetRow(view.FocusedRowHandle) as dt308_CheckSession;

            string dataNullString = string.Join("\n", dt308CheckDetail.Where(r => r.SessionId == session.Id)
                .Where(r => r.HealthRating == -1)
                .Select(r =>
                {
                    var user = users.FirstOrDefault(u => u.Id == r.EmpId);
                    return user != null
                        ? $"LG{user.IdDepartment} {user.Id} {user.DisplayName} {user.DisplayNameVN}"
                        : $"Unknown-{r.EmpId}";
                })
            );

            Clipboard.SetText(dataNullString);
            XtraMessageBox.Show("Đã lưu danh sách vào bộ nhớ tạm, Ctrl + V để dán dữ liệu!", "Thông báo!");
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
                string dateString = row[0].ToString(); // Giả sử giá trị từ row[0]
                DateTime dateTimeValue = DateTime.ParseExact(dateString, "d/M/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                int minute = (int)(dateTimeValue.Ticks / TimeSpan.TicksPerMinute);

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

            if (!string.IsNullOrEmpty(session.DataCollectionDate.ToString()))
            {
                if (XtraMessageBox.Show("Hiện tại đã có một khảo sát google form, Bạn muốn tạo lại không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }

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

            session.DataCollectionDate = DateTime.Now;
            var result = dt308_CheckSessionBUS.Instance.AddOrUpdate(session);

            if (result)
            {
                Clipboard.SetText(scriptGoogleForm);
                XtraMessageBox.Show("Đã lưu Code vào bộ nhớ tạm, Làm theo SOP để tạo được khảo sát google form !", "Thông báo!");

                Process.Start("https://script.google.com/");
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
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
                dt308CheckSession = dt308_CheckSessionBUS.Instance.GetListByIdDept(TPConfigs.idDept2word);
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
                        EmpName = user != null ? $"LG{user.IdDepartment} {user.Id} {user.DisplayName} {user.DisplayNameVN}" : $"Unknown-{r.EmpId}"
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

                itemDetailNull.BeginGroup = true;
                e.Menu.Items.Add(itemDetailNull);
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
            uc308_ExportReport ucInfo = new uc308_ExportReport();
            if (XtraDialog.Show(ucInfo, "輸入年份、請假明細表", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            int yearStatistic = ucInfo.Year;
            List<string> sickFiles = ucInfo.SickFiles;

            SaveFileDialog saveFile = new SaveFileDialog()
            {
                RestoreDirectory = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = $"健康檢查報告-{DateTime.Now:yyyyMMddHHmmss}.xlsx",
                Filter = "Excel| *.xlsx"
            };
            if (saveFile.ShowDialog() != DialogResult.OK) return;

            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                // Đường dẫn đến temp và file kết quả
                string PATH_EXPORT = saveFile.FileName;

                string PATH_TEMPLATE = Path.Combine(TPConfigs.ResourcesPath, "308_Statistic.xlsx");

                // Lấy dữ liệu để xuất báo cáo , phải lọc theo thời gian
                var sessionFilter = dt308CheckSession.Where(r => r.DateSession.Year == yearStatistic).ToList();

                // Làm dữ liệu gốc để tính toán cho các "Biểu mẫu" cần xuất ra
                var dataBases = (from data in sessionFilter
                                 select new
                                 {
                                     data,
                                     detail = (from detail in dt308CheckDetail
                                               join user in users on detail.EmpId equals user.Id
                                               where detail.SessionId == data.Id
                                               select new
                                               {
                                                   detail,
                                                   user
                                               }).ToList()
                                 }).ToList();

                // Biểu mẫu 1: QUẢN LÝ SỨC KHỎE TRƯỚC KHI BỐ TRÍ VIỆC LÀM
                // Biểu mẫu 2: QUẢN LÝ SỨC KHỎE NGƯỜI LAO ĐỘNG THÔNG QUA KHÁM SỨC KHỎE ĐỊNH KỲ
                var dataForm12 = dataBases.Where(r => r.data.CheckType == f308_CheckSession_Info.KSK_TruocViecLam).ToList();
                var statistics12 = dataBases.Select(db => new
                {
                    db.data,
                    db.detail,

                    // Thống kê Nam và Nữ
                    maleCount = db.detail.Count(x => x.user.Sex == true),
                    femaleCount = db.detail.Count(x => x.user.Sex == false),

                    // Tổng số người
                    totalCount = db.detail.Count(),

                    // Thống kê sức khỏe
                    healthRatingStats = db.detail
                    .GroupBy(x => x.detail.HealthRating)
                    .Select(g => new
                    {
                        HealthRating = g.Key,
                        Count = g.Count()
                    }).ToList()
                }).ToList();

                // Biểu mẫu 1: QUẢN LÝ SỨC KHỎE TRƯỚC KHI BỐ TRÍ VIỆC LÀM
                // Biểu mẫu 2: QUẢN LÝ SỨC KHỎE NGƯỜI LAO ĐỘNG THÔNG QUA KHÁM SỨC KHỎE ĐỊNH KỲ
                var dt12 = statistics12.Select(stat =>
                {
                    var healthDict = stat.healthRatingStats?
                        .ToDictionary(r => r.HealthRating, r => r.Count.ToString()) ?? new Dictionary<int, string>();

                    return new
                    {
                        checktype = stat.data.CheckType,
                        datetime = stat.data?.DateSession,
                        male_female_vn = $"Nam: {stat.maleCount}\r\nNữ: {stat.femaleCount}",
                        male_female_tw = $"男：{stat.maleCount}\r\n女：{stat.femaleCount}",
                        total = stat.totalCount,
                        h1 = healthDict.GetValueOrDefault(1, ""),
                        h2 = healthDict.GetValueOrDefault(2, ""),
                        h3 = healthDict.GetValueOrDefault(3, ""),
                        h4 = healthDict.GetValueOrDefault(4, ""),
                        h5 = healthDict.GetValueOrDefault(5, ""),
                        checkName_vn = $"KSK {(stat.data?.CheckType.Split('/')[0] ?? "N/A")}",
                        checkName_tw = $"{(stat.data?.CheckType.Split('/')[1] ?? "N/A")}"
                    };
                }).ToList();

                // Biểu mẫu 3: TÌNH HÌNH BỆNH TẬT TRONG THỜI GIAN BÁO CÁO
                // Hàm xử lý thống kê bệnh (chuyển sang int)
                Func<IEnumerable<dynamic>, string, List<dynamic>> GetDiseaseStats = (details, diseaseField) => details
                .SelectMany(d =>
                {
                    var detail = d.detail; // Lấy detail từ d
                    var propertyInfo = detail?.GetType().GetProperty(diseaseField);
                    var valueInt = propertyInfo?.GetValue(detail) as string;

                    return (valueInt ?? "")
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => int.TryParse(v.Trim(), out int diseaseId) ? (int?)diseaseId : null);
                })
                .Where(diseaseId => diseaseId.HasValue)
                .GroupBy(diseaseId => diseaseId.Value)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .Cast<dynamic>()
                .ToList();

                // ✅ Thống kê theo quý và tính tổng số ca bệnh cho từng loại
                var statisticsByQuarter = dataBases.GroupBy(data => new
                {
                    Quarter = (data.data.DateSession.Month - 1) / 3 + 1
                })
                .Select(group =>
                {
                    var disease1Stats = GetDiseaseStats(group.SelectMany(g => g.detail), "Disease1");
                    var disease2Stats = GetDiseaseStats(group.SelectMany(g => g.detail), "Disease2");
                    var disease3Stats = GetDiseaseStats(group.SelectMany(g => g.detail), "Disease3");

                    // ✅ Tính tổng số ca bệnh cho từng loại
                    var totalDisease1 = disease1Stats.Sum(d => (int)d.Count);
                    var totalDisease2 = disease2Stats.Sum(d => (int)d.Count);
                    var totalDisease3 = disease3Stats.Sum(d => (int)d.Count);

                    return new
                    {
                        Quarter = $"Q{group.Key.Quarter}",
                        Disease1Stats = disease1Stats,
                        Disease2Stats = disease2Stats,
                        Disease3Stats = disease3Stats,
                        TotalDisease1 = totalDisease1, // ✅ Tổng của Disease1
                        TotalDisease2 = totalDisease2, // ✅ Tổng của Disease2
                        TotalDisease3 = totalDisease3  // ✅ Tổng của Disease3
                    };
                }).ToList();

                var dt31 = dt308Diseases.Where(r => r.DiseaseType == 1).Select((r, index) => new
                {
                    No = index + 1,
                    NameVN = r.DisplayNameVN,
                    NameTW = r.DisplayNameTW,
                    Q1 = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q1")?.Disease1Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-",
                    Q2 = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q2")?.Disease1Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-",
                    Q3 = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q3")?.Disease1Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-",
                    Q4 = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q4")?.Disease1Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-"
                }).ToList();

                var dt32 = dt308Diseases.Where(r => r.DiseaseType == 3).Select((r, index) => new
                {
                    No = index + 1,
                    NameVN = r.DisplayNameVN,
                    NameTW = r.DisplayNameTW,
                    Q1 = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q1")?.Disease2Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-",
                    Q2 = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q2")?.Disease2Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-",
                    Q3 = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q3")?.Disease2Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-",
                    Q4 = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q4")?.Disease2Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-"
                }).ToList();

                // Biểu mẫu 4: TÌNH HÌNH NGHỈ DO ỐM, TAI NẠN LAO ĐỘNG VÀ BỆNH NGHỀ NGHIỆP
                List<SickData> sickDatas = new List<SickData>();
                Dictionary<string, int> userByMonth = Enumerable.Range(1, 12).ToDictionary(m => m.ToString("00"), m => 0);


                if (sickFiles.Count != 0)
                {
                    foreach (var sickFile in sickFiles)
                        using (PdfReader reader = new PdfReader(sickFile))
                        {
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                string text = PdfTextExtractor.GetTextFromPage(reader, i);

                                var matches = new
                                {
                                    DataList = Regex.Matches(text, @"\d{8}\s+0[1|2]\s+\d{4}\s+\d{4}\s+[\d.]+").Cast<Match>().Select(m => m.Value).ToList(),
                                    Id = Regex.Match(text, @"VNW\d{7}").Value,
                                    Month = Regex.Match(text, @"\d{7}-\d{7}").Value
                                };

                                string month = matches.Month.Split('-')[1].Substring(3, 2);

                                if (userByMonth.ContainsKey(month))
                                {
                                    userByMonth[month] += 1;
                                }

                                if (!string.IsNullOrEmpty(matches.Month) && !string.IsNullOrEmpty(matches.Id) && matches.DataList.Count > 0)
                                {
                                    sickDatas.Add(new SickData
                                    {
                                        Id = matches.Id,
                                        Data = matches.DataList,
                                        TotalTime = matches.DataList.Sum(s => double.TryParse(s.Split().Last(), out double val) ? val : 0),
                                        Count = matches.DataList.Count,
                                        Month = matches.Month.Split('-')[1].Substring(3, 2)
                                    });
                                }
                            }
                        }
                }

                var months = Enumerable.Range(1, 12).Select(m => m.ToString("00"));

                var grouped = sickDatas.GroupBy(a => a.Month)
                    .Select(g => new SickData
                    {
                        Month = g.Key,
                        TotalTime = g.Sum(r => r.TotalTime),
                        Count = g.Sum(r => r.Count),
                        CountUser = g.Select(r => r.Id).Distinct().Count()
                    });

                var dt4 = userByMonth.GroupJoin(
                    grouped,
                    m => m.Key,
                    g => g.Month,
                    (m, g) =>
                    {
                        var data = g.FirstOrDefault();

                        return new SickData
                        {
                            Month = m.Key,
                            TotalTime = data?.TotalTime ?? 0,
                            Count = data?.Count ?? 0,
                            CountUser = data?.CountUser ?? 0,
                            Percent = m.Value == 0 ? 0 : (double)(data?.CountUser ?? 0) / m.Value * 100
                        };
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                // Biểu mẫu 5: QUẢN LÝ BỆNH MÃN TÍNH
                int CalculateAge(DateTime? dob) => dob.HasValue ? DateTime.Now.Year - dob.Value.Year - (DateTime.Now < dob.Value.AddYears(DateTime.Now.Year - dob.Value.Year) ? 1 : 0) : 0;

                var dt5 = sessionFilter
                    .Where(r => r.CheckType != f308_CheckSession_Info.KSK_TruocViecLam)
                    .SelectMany(data => dt308CheckDetail
                        .Where(detail => detail.SessionId == data.Id && !string.IsNullOrEmpty(detail.Disease2))
                        .SelectMany(detail => detail.Disease2.Split(',')
                            .Select(diseaseId => new { detail, diseaseId })))
                    .Join(users, x => x.detail.EmpId, user => user.Id, (x, user) => new { x.detail, x.diseaseId, user })
                    .Join(dt308Diseases, x => x.diseaseId.Trim(), disease => disease.Id.ToString(), (x, disease) => new
                    {
                        UserIdDept = x.user.IdDepartment,
                        UserNameVN = x.user.DisplayNameVN,
                        DiseaseNameVN = disease.DisplayNameVN,
                        UserNameTW = x.user.DisplayName,
                        DiseaseNameTW = disease.DisplayNameTW,
                        Sex = x.user.Sex,
                        Age = CalculateAge(x.user.DOB),
                        JobAge = CalculateAge(x.user.DateCreate)
                    })
                    .OrderBy(r => r.UserNameVN)
                    .ToList();

                // Biểu mẫu 6: QUẢN LÝ BỆNH MÃN TÍNH THEO TỪNG BỆNH
                var dt6 = dt5.GroupBy(r => new { r.DiseaseNameVN, r.DiseaseNameTW }).Select(g => new
                {
                    DiseaseNameVN = g.Key.DiseaseNameVN,
                    DiseaseNameTW = g.Key.DiseaseNameTW,
                    Users = g.ToList()
                }).ToList();


                // Biểu mẫu 7:THEO DÕI BỆNH NGHỀ NGHIỆP
                var dt7 = (from data in sessionFilter
                           where data.CheckType != f308_CheckSession_Info.KSK_TruocViecLam
                           from detail in dt308CheckDetail
                           join user in users on detail.EmpId equals user.Id
                           where detail.SessionId == data.Id && !string.IsNullOrEmpty(detail.Disease3)
                           from diseaseId in detail.Disease3.Split(',').Select(d => d.Trim()) // Tách và loại bỏ khoảng trắng
                           join disease in dt308Diseases on diseaseId equals disease.Id.ToString()
                           select new
                           {
                               ExamDate = data.DateSession,
                               DiseaseId = disease.Id,
                               DiseaseDisplayNameVN = disease.DisplayNameVN,
                               DiseaseDisplayNameTW = disease.DisplayNameTW,
                               UserSex = user.Sex
                           })
                            .GroupBy(r => new { r.DiseaseId, r.DiseaseDisplayNameVN, r.DiseaseDisplayNameTW, r.ExamDate })
                            .Select(g => new
                            {
                                ExamDate = g.Key.ExamDate,
                                DiseaseDisplayNameVN = g.Key.DiseaseDisplayNameVN,
                                DiseaseDisplayNameTW = g.Key.DiseaseDisplayNameTW,
                                TotalCount = g.Count(),
                                CountMale = g.Count(x => x.UserSex == true),
                                CountFemale = g.Count(x => x.UserSex != true)
                            })
                            .OrderBy(r => r.ExamDate)
                            .ThenBy(r => r.DiseaseDisplayNameVN)
                            .ToList();

                // BIỂU TỔNG HỢP KIỂM TRA SỨC KHỎE

                var kskThongThuong = dataBases.Where(r => r.data.CheckType == f308_CheckSession_Info.KSK_ThongThuong).SelectMany(r => r.detail).ToList();
                var kskDacBiet = dataBases.Where(r => r.data.CheckType == f308_CheckSession_Info.KSK_DacBiet).SelectMany(r => r.detail).ToList();
                var kskXinGiayPhepLD = dataBases.Where(r => r.data.CheckType == f308_CheckSession_Info.KSK_XinGiayPhepLD).SelectMany(r => r.detail).ToList();
                var kskNhanVienMoi = dataBases.Where(r => r.data.CheckType == f308_CheckSession_Info.KSK_NhanVienMoi).SelectMany(r => r.detail).ToList();
                var kskTruocViecLam = dataBases.Where(r => r.data.CheckType == f308_CheckSession_Info.KSK_TruocViecLam).SelectMany(r => r.detail).ToList();
                int countKskRoiNghiViec = dataBases.SelectMany(r => r.detail).Where(r => r.detail.HealthRating != -1)
                    .Join(users, data => data.detail.EmpId, usr => usr.Id, (data, usr) => new { data, usr })
                    .Count(x => x.usr.Status == 1);

                int chuakhammanghiviec = dataBases.SelectMany(r => r.detail).Where(r => r.detail.HealthRating == -1)
                    .Join(users, data => data.detail.EmpId, usr => usr.Id, (data, usr) => new { data, usr })
                    .Count(x => x.usr.Status == 1);
                var depts = dm_DeptBUS.Instance.GetList();
                var dtTongHop = new
                {
                    tongnhanviec = users.Count(r => r.IdDepartment.StartsWith(idDept2word) && r.Status != 1),
                    luuchuc = users.Count(r => r.IdDepartment.StartsWith(idDept2word) && r.Status == 2),
                    nghiviec = users.Count(r => r.IdDepartment.StartsWith(idDept2word) && r.Status == 1),
                    kskthuongnien = kskThongThuong.Count(r => r.detail.HealthRating != -1),
                    kskdacbiet = kskDacBiet.Count(r => r.detail.HealthRating != -1),
                    kskxingiayphepld = kskXinGiayPhepLD.Count(r => r.detail.HealthRating != -1),
                    ksknhanvienmoi = kskNhanVienMoi.Count(r => r.detail.HealthRating != -1),
                    ksktruocvietlam = kskTruocViecLam.Count(r => r.detail.HealthRating != -1),
                    kskroinghiviec = countKskRoiNghiViec,
                    chuakhamdanghiviec = chuakhammanghiviec,
                    bophanvn = depts.FirstOrDefault(r => r.Id == idDept2word)?.DisplayNameVN,
                    bophantw = depts.FirstOrDefault(r => r.Id == idDept2word)?.DisplayName,
                };

                File.Copy(PATH_TEMPLATE, PATH_EXPORT, true);

                FileInfo newFile = new FileInfo(PATH_EXPORT);
                using (ExcelPackage pck = new ExcelPackage(newFile))
                {
                    var wsPhuLuc2 = pck.Workbook.Worksheets["Phụ lục 2"];
                    var wsBieuMau1 = pck.Workbook.Worksheets["Biểu mẫu 1"];
                    var wsBieuMau2 = pck.Workbook.Worksheets["Biểu mẫu 2"];
                    var wsBieuMau3 = pck.Workbook.Worksheets["Biểu mẫu 3"];
                    var wsBieuMau4 = pck.Workbook.Worksheets["Biểu mẫu 4"];
                    var wsBieuMau5 = pck.Workbook.Worksheets["Biểu mẫu 5"];
                    var wsBieuMau6 = pck.Workbook.Worksheets["Biểu mẫu 6"];
                    var wsBieuMau7 = pck.Workbook.Worksheets["Biểu mẫu 7"];
                    var wsTongHop = pck.Workbook.Worksheets["Tổng hợp"];

                    wsBieuMau6.DefaultRowHeight = 20;

                    wsPhuLuc2.Cells["A2"].Value = $"(Năm {yearStatistic})";
                    wsPhuLuc2.Cells["A10"].Value = $"Năm : {yearStatistic}";

                    wsPhuLuc2.Cells["A16"].Value = $"({yearStatistic}年)";
                    wsPhuLuc2.Cells["A24"].Value = $"年：{yearStatistic}";

                    // Biểu mẫu 1: QUẢN LÝ SỨC KHỎE TRƯỚC KHI BỐ TRÍ VIỆC LÀM
                    var dt1 = dt12.Where(r => r.checktype == f308_CheckSession_Info.KSK_TruocViecLam);
                    int dt1RowIns = dt1.Count() - 1;

                    void InsertDataWS1(int startRow, bool IsVietnamese = true)
                    {
                        int index1 = 0;
                        if (dt1RowIns > 0) wsBieuMau1.InsertRow(startRow, dt1RowIns);
                        string formatDate = IsVietnamese ? "dd/MM/yyyy" : "yyyy/MM/dd";

                        foreach (var item in dt1)
                        {
                            if (item != dt1.Last())
                            {
                                var destinationRange = wsBieuMau1.Cells[$"A{startRow + index1}:Z{startRow + index1}"];
                                wsBieuMau1.Cells[$"A{startRow - 1}:Z{startRow - 1}"].Copy(destinationRange);
                                wsBieuMau1.Rows[startRow + index1].Height = wsBieuMau1.Rows[startRow - 1].Height;
                            }

                            wsBieuMau1.Cells[$"A{startRow - 1 + index1}"].Value = $"{item.datetime?.ToString(formatDate)}";
                            wsBieuMau1.Cells[$"G{startRow - 1 + index1}"].Value = IsVietnamese ? item.male_female_vn : item.male_female_tw;
                            wsBieuMau1.Cells[$"L{startRow - 1 + index1}"].Value = item.total;
                            wsBieuMau1.Cells[$"Q{startRow - 1 + index1}"].Value = item.h1;
                            wsBieuMau1.Cells[$"S{startRow - 1 + index1}"].Value = item.h2;
                            wsBieuMau1.Cells[$"U{startRow - 1 + index1}"].Value = item.h3;
                            wsBieuMau1.Cells[$"W{startRow - 1 + index1}"].Value = item.h4;
                            wsBieuMau1.Cells[$"Y{startRow - 1 + index1}"].Value = item.h5;

                            index1++;
                        }
                    }

                    if (dt1RowIns >= 0)
                    {
                        InsertDataWS1(12, false);
                        InsertDataWS1(6, true);
                    }

                    // Biểu mẫu 2: QUẢN LÝ SỨC KHỎE NGƯỜI LAO ĐỘNG THÔNG QUA KHÁM SỨC KHỎE ĐỊNH KỲ
                    var dt2 = dt12.Where(r => r.checktype != f308_CheckSession_Info.KSK_TruocViecLam).ToList();
                    int dt2RowIns = dt2.Count() - 1;

                    void InsertDataWS2(int startRow, bool IsVietnamese = true)
                    {
                        int index2 = 0;
                        if (dt2RowIns > 0) wsBieuMau2.InsertRow(startRow, dt2RowIns);
                        string checkNameProp = IsVietnamese ? "checkName_vn" : "checkName_tw";
                        string formatDate = IsVietnamese ? "dd/MM/yyyy" : "yyyy/MM/dd";

                        foreach (var item in dt2)
                        {
                            if (item != dt2.Last())
                            {
                                var destinationRange = wsBieuMau2.Cells[$"A{startRow + index2}:Z{startRow + index2}"];
                                wsBieuMau2.Cells[$"A{startRow - 1}:Z{startRow - 1}"].Copy(destinationRange);
                                wsBieuMau2.Rows[startRow + index2].Height = wsBieuMau2.Rows[startRow - 1].Height;
                            }

                            wsBieuMau2.Cells[$"A{startRow - 1 + index2}"].Value = $"{item.datetime?.ToString(formatDate)}\r\n{item.GetType().GetProperty(checkNameProp).GetValue(item)}";
                            wsBieuMau2.Cells[$"G{startRow - 1 + index2}"].Value = IsVietnamese ? item.male_female_vn : item.male_female_tw;
                            wsBieuMau2.Cells[$"L{startRow - 1 + index2}"].Value = item.total;
                            wsBieuMau2.Cells[$"Q{startRow - 1 + index2}"].Value = item.h1;
                            wsBieuMau2.Cells[$"S{startRow - 1 + index2}"].Value = item.h2;
                            wsBieuMau2.Cells[$"U{startRow - 1 + index2}"].Value = item.h3;
                            wsBieuMau2.Cells[$"W{startRow - 1 + index2}"].Value = item.h4;
                            wsBieuMau2.Cells[$"Y{startRow - 1 + index2}"].Value = item.h5;

                            index2++;
                        }
                    }

                    if (dt2RowIns >= 0)
                    {
                        InsertDataWS2(12, false);
                        InsertDataWS2(6, true);
                    }

                    // Biểu mẫu 3: TÌNH HÌNH BỆNH TẬT TRONG THỜI GIAN BÁO CÁO
                    void InsertDataWS3(int startRow, IEnumerable<dynamic> data, bool IsVietnamese = true)
                    {
                        int rowCount = data.Count() - 1;
                        if (rowCount > 0) wsBieuMau3.InsertRow(startRow, rowCount);

                        int i = 0;
                        foreach (var item in data)
                        {
                            if (i != rowCount)
                            {
                                var destinationRange = wsBieuMau3.Cells[$"A{startRow + i}:AD{startRow + i}"];
                                wsBieuMau3.Cells[$"A{startRow + rowCount}:AD{startRow + rowCount}"].Copy(destinationRange);
                            }

                            wsBieuMau3.Cells[$"A{startRow + i}"].Value = item.No;
                            wsBieuMau3.Cells[$"D{startRow + i}"].Value = IsVietnamese ? item.NameVN : item.NameTW;
                            wsBieuMau3.Cells[$"O{startRow + i}"].Value = item.Q1;
                            wsBieuMau3.Cells[$"S{startRow + i}"].Value = item.Q2;
                            wsBieuMau3.Cells[$"W{startRow + i}"].Value = item.Q3;
                            wsBieuMau3.Cells[$"AA{startRow + i}"].Value = item.Q4;
                            i++;
                        }
                    }

                    // Gọi hàm cho từng trường hợp
                    InsertDataWS3(21, dt32, false);
                    InsertDataWS3(18, dt31, false);
                    InsertDataWS3(8, dt32);
                    InsertDataWS3(5, dt31);

                    // Biểu mẫu 4: TÌNH HÌNH NGHỈ DO ỐM, TAI NẠN LAO ĐỘNG VÀ BỆNH NGHỀ NGHIỆP
                    int flag = 8;
                    for (int i = 0; i < dt4.Count; i++)
                    {
                        wsBieuMau4.Cells[$"E{flag + i}"].Value = dt4[i].Count;
                        wsBieuMau4.Cells[$"I{flag + i}"].Value = dt4[i].TotalTime / 8;
                        wsBieuMau4.Cells[$"G{flag + i}"].Value = dt4[i].Percent;
                    }

                    // Biểu mẫu 5: QUẢN LÝ BỆNH MÃN TÍNH
                    int dt5RowIns = dt5.Count() - 1;

                    void InsertDataWS5(int startRow, bool IsVietnamese = true)
                    {
                        int index5 = 0;
                        if (dt5RowIns > 0) wsBieuMau5.InsertRow(startRow, dt5RowIns);
                        int indexMergeStart = startRow - 1;
                        int indexMergeStop = startRow - 1;
                        string valueCell5 = IsVietnamese ? dt5.FirstOrDefault()?.UserNameVN : dt5.FirstOrDefault()?.UserNameTW;

                        foreach (var item in dt5)
                        {
                            if (item != dt5.Last())
                            {
                                var destinationRange = wsBieuMau5.Cells[$"A{startRow + index5}:AD{startRow + index5}"];
                                wsBieuMau5.Cells[$"A{startRow - 1}:AD{startRow - 1}"].Copy(destinationRange);
                                wsBieuMau5.Rows[startRow + index5].Height = wsBieuMau5.Rows[startRow - 1].Height;
                            }

                            wsBieuMau5.Cells[$"A{startRow - 1 + index5}"].Value = item.UserIdDept;
                            wsBieuMau5.Cells[$"E{startRow - 1 + index5}"].Value = IsVietnamese ? item.UserNameVN : item.UserNameTW;
                            wsBieuMau5.Cells[$"J{startRow - 1 + index5}"].Value = IsVietnamese ? item.DiseaseNameVN : item.DiseaseNameTW;
                            wsBieuMau5.Cells[$"O{startRow - 1 + index5}"].Value = item.Sex == true ? item.Age.ToString() : "";
                            wsBieuMau5.Cells[$"Q{startRow - 1 + index5}"].Value = item.Sex != true ? item.Age.ToString() : "";
                            wsBieuMau5.Cells[$"S{startRow - 1 + index5}"].Value = item.JobAge;

                            // Kiểm tra nếu giá trị UserName thay đổi -> merge ô trước đó
                            string currentUserName = IsVietnamese ? item.UserNameVN : item.UserNameTW;
                            if (currentUserName != valueCell5)
                            {
                                // Merge các ô từ indexMergeStart đến indexMergeStop
                                if (indexMergeStop > indexMergeStart)
                                {
                                    wsBieuMau5.Cells[$"A{indexMergeStart}:D{indexMergeStop}"].Merge = true;
                                    wsBieuMau5.Cells[$"E{indexMergeStart}:I{indexMergeStop}"].Merge = true;
                                    wsBieuMau5.Cells[$"O{indexMergeStart}:P{indexMergeStop}"].Merge = true;
                                    wsBieuMau5.Cells[$"Q{indexMergeStart}:R{indexMergeStop}"].Merge = true;
                                    wsBieuMau5.Cells[$"S{indexMergeStart}:T{indexMergeStop}"].Merge = true;
                                }
                                else
                                {
                                    wsBieuMau5.Cells[$"A{indexMergeStart}:D{indexMergeStop}"].Merge = true;
                                    wsBieuMau5.Cells[$"E{indexMergeStart}:I{indexMergeStop}"].Merge = true;
                                    wsBieuMau5.Cells[$"O{indexMergeStart}:P{indexMergeStop}"].Merge = true;
                                    wsBieuMau5.Cells[$"Q{indexMergeStart}:R{indexMergeStop}"].Merge = true;
                                    wsBieuMau5.Cells[$"S{indexMergeStart}:T{indexMergeStop}"].Merge = true;
                                }

                                wsBieuMau5.Cells[$"J{indexMergeStart}:N{indexMergeStop}"].Merge = true;
                                wsBieuMau5.Cells[$"U{indexMergeStart}:W{indexMergeStop}"].Merge = true;
                                wsBieuMau5.Cells[$"X{indexMergeStart}:Z{indexMergeStop}"].Merge = true;
                                wsBieuMau5.Cells[$"AA{indexMergeStart}:AD{indexMergeStop}"].Merge = true;

                                // Cập nhật giá trị mới cho UserName
                                valueCell5 = currentUserName;
                                indexMergeStart = startRow - 1 + index5;
                            }

                            indexMergeStop = startRow - 1 + index5;
                            index5++;
                        }

                        // Merge lần cuối cùng nếu cần
                        if (indexMergeStop >= indexMergeStart)
                        {
                            wsBieuMau5.Cells[$"A{indexMergeStart}:D{indexMergeStop}"].Merge = true;
                            wsBieuMau5.Cells[$"E{indexMergeStart}:I{indexMergeStop}"].Merge = true;
                            wsBieuMau5.Cells[$"O{indexMergeStart}:P{indexMergeStop}"].Merge = true;
                            wsBieuMau5.Cells[$"Q{indexMergeStart}:R{indexMergeStop}"].Merge = true;
                            wsBieuMau5.Cells[$"S{indexMergeStart}:T{indexMergeStop}"].Merge = true;

                            wsBieuMau5.Cells[$"J{indexMergeStart}:N{indexMergeStop}"].Merge = true;
                            wsBieuMau5.Cells[$"U{indexMergeStart}:W{indexMergeStop}"].Merge = true;
                            wsBieuMau5.Cells[$"X{indexMergeStart}:Z{indexMergeStop}"].Merge = true;
                            wsBieuMau5.Cells[$"AA{indexMergeStart}:AD{indexMergeStop}"].Merge = true;
                        }
                    }

                    if (dt5RowIns >= 0)
                    {
                        InsertDataWS5(13, false);
                        InsertDataWS5(6, true);
                    }

                    // Biểu mẫu 6: QUẢN LÝ BỆNH MÃN TÍNH THEO TỪNG BỆNH
                    int dt6RowIns = dt6.Count();

                    void InsertDataWS6(int startRow, bool IsVietnamese = true)
                    {
                        int step = 5;

                        for (int i = 0; i < dt6RowIns; i++)
                        {
                            int index6 = 0;

                            if (i > 0)
                            {
                                wsBieuMau6.InsertRow(startRow, step);
                                var destinationRange = wsBieuMau6.Cells[$"A{startRow}:Z{startRow + step - 1}"];
                                wsBieuMau6.Cells[$"A{startRow + step}:Z{startRow - 1 + step * 2}"].Copy(destinationRange);
                            }

                            if (dt6[i].Users.Count > 1)
                            {
                                wsBieuMau6.InsertRow(startRow + step, dt6[i].Users.Count - 1);
                            }

                            foreach (var item in dt6[i].Users)
                            {
                                var destinationRange = wsBieuMau6.Cells[$"A{startRow + step - 1 + index6}:Z{startRow + step - 1 + index6}"];
                                wsBieuMau6.Cells[$"A{startRow + 4}:Z{startRow + 4}"].Copy(destinationRange);

                                wsBieuMau6.Cells[$"A{startRow + 1}"].Value = IsVietnamese
                                    ? $"Tên bệnh*: {item.DiseaseNameVN}"
                                    : $"慢性病名稱*：{item.DiseaseNameTW}";

                                wsBieuMau6.Cells[$"A{startRow + 4 + index6}"].Value = index6 + 1;
                                wsBieuMau6.Cells[$"B{startRow + 4 + index6}"].Value = item.UserIdDept;
                                wsBieuMau6.Cells[$"F{startRow + 4 + index6}"].Value = IsVietnamese ? item.UserNameVN : item.UserNameTW;
                                wsBieuMau6.Cells[$"K{startRow + 4 + index6}"].Value = item.Sex == true ? item.Age.ToString() : "";
                                wsBieuMau6.Cells[$"M{startRow + 4 + index6}"].Value = item.Sex != true ? item.Age.ToString() : "";
                                wsBieuMau6.Cells[$"O{startRow + 4 + index6}"].Value = item.JobAge;

                                index6++;
                            }
                        }
                    }

                    // Gọi hàm
                    if (dt6RowIns >= 0)
                    {
                        InsertDataWS6(9, false);
                        InsertDataWS6(2, true);
                    }

                    // Biểu mẫu 7:THEO DÕI BỆNH NGHỀ NGHIỆP
                    int dt7RowIns = dt7.Count() - 1;
                    void InsertDataWS7(int startRow, bool IsVietnamese = true)
                    {
                        int index7 = 0;
                        if (dt7RowIns > 0) wsBieuMau7.InsertRow(startRow, dt7RowIns);

                        foreach (var item in dt7)
                        {
                            if (item != dt7.Last())
                            {
                                var destinationRange = wsBieuMau7.Cells[$"A{startRow + index7}:AD{startRow + index7}"];
                                wsBieuMau7.Cells[$"A{startRow + dt7RowIns}:AD{startRow + dt7RowIns}"].Copy(destinationRange);
                                wsBieuMau7.Rows[startRow + dt7RowIns].Height = wsBieuMau7.Rows[startRow].Height;
                            }

                            wsBieuMau7.Cells[$"A{startRow + index7}"].Value = item.ExamDate.ToString(IsVietnamese ? "dd/MM/yyyy" : "yyyy/MM/dd");
                            wsBieuMau7.Cells[$"C{startRow + index7}"].Value = IsVietnamese ? item.DiseaseDisplayNameVN : item.DiseaseDisplayNameTW;
                            wsBieuMau7.Cells[$"G{startRow + index7}"].Value = item.TotalCount;
                            wsBieuMau7.Cells[$"J{startRow + index7}"].Value = item.CountMale;

                            index7++;
                        }
                    }

                    if (dt7RowIns >= 0)
                    {
                        InsertDataWS7(12, false);
                        InsertDataWS7(5, true);
                    }

                    // BIỂU TỔNG HỢP KIỂM TRA SỨC KHỎE
                    wsTongHop.Cells["M4"].Value = dtTongHop.tongnhanviec;
                    wsTongHop.Cells["M5"].Value = dtTongHop.kskthuongnien;
                    wsTongHop.Cells["M6"].Value = dtTongHop.kskdacbiet;
                    wsTongHop.Cells["M7"].Value = dtTongHop.kskxingiayphepld;
                    wsTongHop.Cells["M9"].Value = dtTongHop.ksknhanvienmoi;
                    wsTongHop.Cells["M12"].Value = dtTongHop.ksktruocvietlam;
                    wsTongHop.Cells["M10"].Value = dtTongHop.luuchuc;
                    wsTongHop.Cells["M11"].Value = dtTongHop.kskroinghiviec;
                    wsTongHop.Cells["AC4"].Value = dtTongHop.chuakhamdanghiviec;
                    wsTongHop.Cells["A3"].Value = $"Bộ phận: {dtTongHop.bophanvn}";
                    wsTongHop.Cells["A17"].Value = $"單位：{dtTongHop.bophantw}";

                    // Chỉ lấy giá trị
                    foreach (var ws in pck.Workbook.Worksheets.Where(r => r.Name != "Tổng hợp"))
                    {
                        ws.Calculate();

                        int rowCount = ws.Dimension?.Rows ?? 0;  // Lấy số hàng có dữ liệu
                        int colCount = ws.Dimension?.Columns ?? 0; // Lấy số cột có dữ liệu

                        if (rowCount > 0 && colCount > 0)
                        {
                            object[,] values = new object[rowCount, colCount];

                            // Lấy toàn bộ dữ liệu và loại bỏ công thức
                            for (int row = 1; row <= rowCount; row++)
                            {
                                for (int col = 1; col <= colCount; col++)
                                {
                                    values[row - 1, col - 1] = ws.Cells[row, col].Value; // Chỉ lấy giá trị
                                }
                            }

                            // Ghi đè dữ liệu lên vùng cũ (bỏ công thức)
                            ws.Cells[1, 1, rowCount, colCount].Value = values;

                            // Xóa tất cả ràng buộc kiểm tra dữ liệu (Data Validation)
                            ws.DataValidations.Clear();
                        }
                    }

                    // Lưu và chỉ hiện Sheet BB
                    pck.Save();
                }

                Process.Start(PATH_EXPORT);
            }
        }
    }
}
