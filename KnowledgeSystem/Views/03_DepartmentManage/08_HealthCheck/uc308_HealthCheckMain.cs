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
using DevExpress.XtraSplashScreen;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt;
using MiniSoftware;
using OfficeOpenXml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
                int minute = (int)(Convert.ToDateTime(row[0]).Ticks / System.TimeSpan.TicksPerMinute);

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
            var editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) };

            // Thiết lập mask để buộc nhập đúng định dạng
            editor.Properties.Mask.EditMask = "####"; // 4 dấu # cho 4 chữ số
            editor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            editor.Properties.Mask.UseMaskAsDisplayFormat = true; // Hiển thị mask khi không focus

            var resultYear = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "輸入要計算的年份",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = DateTime.Now.ToString("yyyy") // Định dạng mặc định
            });

            if (string.IsNullOrEmpty(resultYear?.ToString())) return;

            SaveFileDialog saveFile = new SaveFileDialog()
            {
                RestoreDirectory = true,
                //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = $"健康檢查報告-{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            };

            if (saveFile.ShowDialog() != DialogResult.OK) return;

            int yearStatistic = Convert.ToInt16(resultYear);

            // Đường dẫn đến temp và file kết quả
            string PATH_TEMPLATE = Path.Combine(TPConfigs.ResourcesPath, "308_Statistic.xlsx");
            string PATH_EXPORT = saveFile.FileName;

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

            var dt31 = dt308Diseases
            .Where(r => r.DiseaseType == 1)
            .Select((r, index) => new Dictionary<string, object>
            {
                { "no", index + 1 }, // Tăng từ 1
                { "name", r.DisplayNameVN },
                { "q1", statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q1")?.Disease1Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-" },
                { "q2", statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q2")?.Disease1Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-" },
                { "q3", statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q3")?.Disease1Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-" },
                { "q4", statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q4")?.Disease1Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-" }
            })
            .ToList();

            var dt32 = dt308Diseases
            .Where(r => r.DiseaseType == 2)
            .Select((r, index) => new Dictionary<string, object>
            {
            { "no", index + 1 }, // Tăng từ 1
            { "name", r.DisplayNameVN },
            { "q1", statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q1")?.Disease3Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-" },
            { "q2", statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q2")?.Disease3Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-" },
            { "q3", statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q3")?.Disease3Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-" },
            { "q4", statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q4")?.Disease3Stats.FirstOrDefault(u => u.Id == r.Id)?.Count ?? "-" }
            })
            .ToList();

            // Biểu mẫu 5: QUẢN LÝ BỆNH MÃN TÍNH
            var result = (from data in sessionFilter.Where(r => r.CheckType != f308_CheckSession_Info.KSK_TruocViecLam)
                          from detail in dt308CheckDetail
                          join user in users on detail.EmpId equals user.Id
                          where detail.SessionId == data.Id && !string.IsNullOrEmpty(detail.Disease2)
                          from diseaseId in detail.Disease2.Split(',') // Tách chuỗi Disease2
                          join disease in dt308Diseases on diseaseId.Trim() equals disease.Id.ToString() // Join với dt308Diseases
                          select new
                          {
                              UserIdDept = user.IdDepartment,
                              UserDisplayName = user.DisplayNameVN,
                              DiseaseDisplayNameVN = disease.DisplayNameVN,
                              UserSex = user.Sex,
                              JobAge = DateTime.Now.Year - user.DateCreate.Year - (DateTime.Now < user.DateCreate.AddYears(DateTime.Now.Year - user.DateCreate.Year) ? 1 : 0)
                          }).OrderBy(r => r.UserDisplayName).ToList();

            var dt5 = result.Select(stat => new Dictionary<string, object>
            {
                { "dept",   $"LG{stat.UserIdDept}" },
                { "name",    stat.UserDisplayName },
                { "disease", stat.DiseaseDisplayNameVN },
                { "male",    stat.UserSex == true  ? "✓" : "" },
                { "female",  stat.UserSex != true  ? "✓" : "" },
                { "jobage",  stat.JobAge }
            }).ToList();

            // Biểu mẫu 7:THEO DÕI BỆNH NGHỀ NGHIỆP
            var statistics7 = (from data in sessionFilter.Where(r => r.CheckType != f308_CheckSession_Info.KSK_TruocViecLam)
                               from detail in dt308CheckDetail
                               join user in users on detail.EmpId equals user.Id
                               where detail.SessionId == data.Id && !string.IsNullOrEmpty(detail.Disease3)
                               from diseaseId in detail.Disease3.Split(',') // Tách chuỗi Disease2
                               join disease in dt308Diseases on diseaseId.Trim() equals disease.Id.ToString() // Join với dt308Diseases
                               select new
                               {
                                   ExamDate = data.DateSession,                      // Ngày khám
                                   DiseaseId = disease.Id,
                                   DiseaseDisplayNameVN = disease.DisplayNameVN, // Tên bệnh
                                   UserSex = user.Sex                            // Giới tính
                               })
              .GroupBy(r => new { r.DiseaseId, r.DiseaseDisplayNameVN, r.ExamDate }) // Nhóm theo bệnh và ngày khám
              .Select(g => new
              {
                  ExamDate = g.Key.ExamDate,
                  DiseaseDisplayNameVN = g.Key.DiseaseDisplayNameVN,
                  TotalCount = g.Count(),                        // Tổng số bệnh nhân
                  CountMale = g.Count(x => x.UserSex == true),  // Đếm bệnh nhân nam
                  CountFemale = g.Count(x => x.UserSex != true) // Đếm bệnh nhân nữ (nếu cần)
              })
              .OrderBy(r => r.ExamDate)
              .ThenBy(r => r.DiseaseDisplayNameVN)
              .ToList();

            var dt7 = statistics7.Select(stat => new Dictionary<string, object>
            {
                { "date",    stat.ExamDate.ToString("dd/MM/yyyy") },
                { "disease", stat.DiseaseDisplayNameVN },
                { "total",   stat.TotalCount },
                { "female",  stat.CountFemale },
            }).ToList();

            File.Copy(PATH_TEMPLATE, PATH_EXPORT, true);

            FileInfo newFile = new FileInfo(PATH_EXPORT);
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                SplashScreenManager.ShowDefaultWaitForm();
                var wsPhuLuc2 = pck.Workbook.Worksheets["Phụ lục 2"];
                var wsBieuMau1 = pck.Workbook.Worksheets["Biểu mẫu 1"];
                var wsBieuMau2 = pck.Workbook.Worksheets["Biểu mẫu 2"];

                wsPhuLuc2.Cells["A2"].Value = $"(Năm {yearStatistic})";
                wsPhuLuc2.Cells["A10"].Value = $"Năm : {yearStatistic}";

                wsPhuLuc2.Cells["A16"].Value = $"({yearStatistic}年)";
                wsPhuLuc2.Cells["A24"].Value = $"年：{yearStatistic}";

                var dt1 = dt12.Where(r => r.checktype == f308_CheckSession_Info.KSK_TruocViecLam);
                int dt1RowIns = dt1.Count() - 1;
                if (dt1RowIns != 0)
                {
                    wsBieuMau1.InsertRow(6, dt1RowIns);
                    wsBieuMau1.InsertRow(12 + dt1RowIns, dt1RowIns);

                    for (int i = 0; i < dt1RowIns; i++)
                    {
                        var destinationRange = wsBieuMau1.Cells[$"A{6 + i}:Z{6 + i}"];
                        wsBieuMau1.Cells["A5:Z5"].Copy(destinationRange);
                        wsBieuMau1.Rows[6 + i].Height = wsBieuMau1.Rows[5 + i].Height;

                        destinationRange = wsBieuMau1.Cells[$"A{12 + dt1RowIns + i}:Z{12 + dt1RowIns + i}"];
                        wsBieuMau1.Cells["A5:Z5"].Copy(destinationRange);
                        wsBieuMau1.Rows[12 + dt1RowIns + i].Height = wsBieuMau1.Rows[11 + dt1RowIns + i].Height;
                    }
                }

                // Biểu mẫu 1: QUẢN LÝ SỨC KHỎE TRƯỚC KHI BỐ TRÍ VIỆC LÀM
                int index = 0;
                foreach (var item in dt1)
                {
                    wsBieuMau1.Cells[$"A{5 + index}"].Value = item.datetime?.ToString("dd/MM/yyyy");
                    wsBieuMau1.Cells[$"G{5 + index}"].Value = item.male_female_vn;
                    wsBieuMau1.Cells[$"L{5 + index}"].Value = item.total;
                    wsBieuMau1.Cells[$"Q{5 + index}"].Value = item.h1;
                    wsBieuMau1.Cells[$"S{5 + index}"].Value = item.h2;
                    wsBieuMau1.Cells[$"U{5 + index}"].Value = item.h3;
                    wsBieuMau1.Cells[$"W{5 + index}"].Value = item.h4;
                    wsBieuMau1.Cells[$"Y{5 + index}"].Value = item.h5;

                    wsBieuMau1.Cells[$"A{11 + dt1RowIns + index}"].Value = item.datetime?.ToString("yyyy/MM/dd");
                    wsBieuMau1.Cells[$"G{11 + dt1RowIns + index}"].Value = item.male_female_tw;
                    wsBieuMau1.Cells[$"L{11 + dt1RowIns + index}"].Value = item.total;
                    wsBieuMau1.Cells[$"Q{11 + dt1RowIns + index}"].Value = item.h1;
                    wsBieuMau1.Cells[$"S{11 + dt1RowIns + index}"].Value = item.h2;
                    wsBieuMau1.Cells[$"U{11 + dt1RowIns + index}"].Value = item.h3;
                    wsBieuMau1.Cells[$"W{11 + dt1RowIns + index}"].Value = item.h4;
                    wsBieuMau1.Cells[$"Y{11 + dt1RowIns + index}"].Value = item.h5;

                    index++;
                }

                var dt2 = dt12.Where(r => r.checktype != f308_CheckSession_Info.KSK_TruocViecLam);
                int dt2RowIns = dt2.Count() - 1;
                if (dt2RowIns != 0)
                {
                    wsBieuMau2.InsertRow(6, dt2RowIns);
                    wsBieuMau2.InsertRow(12 + dt2RowIns, dt2RowIns);

                    for (int i = 0; i < dt2RowIns; i++)
                    {
                        var destinationRange = wsBieuMau2.Cells[$"A{6 + i}:Z{6 + i}"];
                        wsBieuMau2.Cells["A5:Z5"].Copy(destinationRange);
                        wsBieuMau2.Rows[6 + i].Height = wsBieuMau2.Rows[5 + i].Height;

                        destinationRange = wsBieuMau2.Cells[$"A{12 + dt2RowIns + i}:Z{12 + dt2RowIns + i}"];
                        wsBieuMau2.Cells["A5:Z5"].Copy(destinationRange);
                        wsBieuMau2.Rows[12 + dt2RowIns + i].Height = wsBieuMau2.Rows[11 + dt2RowIns + i].Height;
                    }
                }

                // Biểu mẫu 2: QUẢN LÝ SỨC KHỎE NGƯỜI LAO ĐỘNG THÔNG QUA KHÁM SỨC KHỎE ĐỊNH KỲ
                index = 0;
                foreach (var item in dt2)
                {
                    wsBieuMau2.Cells[$"A{5 + index}"].Value = $"{item.datetime:dd/MM/yyyy}\r\n{item.checkName_vn}";
                    wsBieuMau2.Cells[$"G{5 + index}"].Value = item.male_female_vn;
                    wsBieuMau2.Cells[$"L{5 + index}"].Value = item.total;
                    wsBieuMau2.Cells[$"Q{5 + index}"].Value = item.h1;
                    wsBieuMau2.Cells[$"S{5 + index}"].Value = item.h2;
                    wsBieuMau2.Cells[$"U{5 + index}"].Value = item.h3;
                    wsBieuMau2.Cells[$"W{5 + index}"].Value = item.h4;
                    wsBieuMau2.Cells[$"Y{5 + index}"].Value = item.h5;

                    wsBieuMau2.Cells[$"A{11 + dt2RowIns + index}"].Value = $"{item.datetime:yyyy/MM/dd}\r\n{item.checkName_tw}";
                    wsBieuMau2.Cells[$"G{11 + dt2RowIns + index}"].Value = item.male_female_tw;
                    wsBieuMau2.Cells[$"L{11 + dt2RowIns + index}"].Value = item.total;
                    wsBieuMau2.Cells[$"Q{11 + dt2RowIns + index}"].Value = item.h1;
                    wsBieuMau2.Cells[$"S{11 + dt2RowIns + index}"].Value = item.h2;
                    wsBieuMau2.Cells[$"U{11 + dt2RowIns + index}"].Value = item.h3;
                    wsBieuMau2.Cells[$"W{11 + dt2RowIns + index}"].Value = item.h4;
                    wsBieuMau2.Cells[$"Y{11 + dt2RowIns + index}"].Value = item.h5;

                    index++;
                }

                // Biểu mẫu 3: TÌNH HÌNH BỆNH TẬT TRONG THỜI GIAN BÁO CÁO

                // Lưu và chỉ hiện Sheet BB
                //pck.Workbook.View.ActiveTab = 1;
                pck.Save();

                SplashScreenManager.CloseDefaultWaitForm();

                Process.Start(PATH_EXPORT);
            }



            //var value = new Dictionary<string, object>
            //{
            //    ["year"] = yearStatistic,

            //    // Biểu mẫu 1: QUẢN LÝ SỨC KHỎE TRƯỚC KHI BỐ TRÍ VIỆC LÀM
            //    ["dt1"] = dt1,

            //    // Biểu mẫu 2: QUẢN LÝ SỨC KHỎE NGƯỜI LAO ĐỘNG THÔNG QUA KHÁM SỨC KHỎE ĐỊNH KỲ
            //    ["dt2"] = dt2,

            //    // Biểu mẫu 3: TÌNH HÌNH BỆNH TẬT TRONG THỜI GIAN BÁO CÁO
            //    ["dt31"] = dt31,
            //    ["dt32"] = dt32,

            //    ["t1q1"] = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q1")?.TotalDisease1.ToString() ?? "-",
            //    ["t1q2"] = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q2")?.TotalDisease1.ToString() ?? "-",
            //    ["t1q3"] = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q3")?.TotalDisease1.ToString() ?? "-",
            //    ["t1q4"] = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q4")?.TotalDisease1.ToString() ?? "-",

            //    ["t2q1"] = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q1")?.TotalDisease3.ToString() ?? "-",
            //    ["t2q2"] = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q2")?.TotalDisease3.ToString() ?? "-",
            //    ["t2q3"] = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q3")?.TotalDisease3.ToString() ?? "-",
            //    ["t2q4"] = statisticsByQuarter.FirstOrDefault(u => u.Quarter == "Q4")?.TotalDisease3.ToString() ?? "-",

            //    // Biểu mẫu 5: QUẢN LÝ BỆNH MÃN TÍNH 
            //    ["dt5"] = dt5,

            //    // Biểu mẫu 7:THEO DÕI BỆNH NGHỀ NGHIỆP
            //    ["dt7"] = dt7,
            //    ["dt7ttotal"] = statistics7.Sum(r => r.TotalCount),
            //    ["dt7tfemale"] = statistics7.Sum(r => r.CountFemale)
            //};


            //MiniWord.SaveAsByTemplate(PATH_EXPORT, PATH_TEMPLATE, value);

            //Process.Start(PATH_EXPORT);

        }
    }
}
