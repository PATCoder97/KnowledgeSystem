using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class uc402_KPIWeb : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc402_KPIWeb()
        {
            InitializeComponent();
            InitializeIcon();

            helper = new RefreshHelper(gvData, "Id");
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        BindingSource sourceKPIs = new BindingSource();
        List<string> yearMonths = new List<string>();

        private class CookieAwareWebClient : WebClient
        {
            public CookieContainer CookieContainer { get; set; }

            public CookieAwareWebClient()
            {
                CookieContainer = new CookieContainer();
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                if (request is HttpWebRequest webRequest)
                {
                    webRequest.CookieContainer = CookieContainer;
                }
                return request;
            }
        }

        private void InitializeIcon()
        {
            btnUpload.ImageOptions.SvgImage = TPSvgimages.UploadFile;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnSalary.ImageOptions.SvgImage = TPSvgimages.Money;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();

            List<dt402_KPIWeb> dataKpis = dt402_KPIWebBUS.Instance.GetList();
            var usrs = dm_UserBUS.Instance.GetList();
            yearMonths = dataKpis.GroupBy(r => r.YearMonth).Select(r => r.Key).ToList();


            var signInfos = (from data in dataKpis
                             join usr in usrs on data.IdUsr equals usr.Id
                             select new
                             {
                                 data,
                                 usr
                             }).ToList();

            sourceKPIs.DataSource = signInfos;

            helper.LoadViewInfo();
            gvData.BestFitColumns();
            gcData.RefreshDataSource();
        }

        private string ConvertToDateString(string input)
        {
            if (input.Count() <= 2)
            {
                return $"20{input}END";
            }
            else
            {
                int year = Convert.ToInt16(input.Substring(0, 2));
                int decimalValue = Convert.ToInt16(input.Substring(2), 16); // Chuyển đổi phần sau "23"
                return $"20{year}/{decimalValue:D2}";
            }
        }

        private void uc402_KPIWeb_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceKPIs;

            LoadData();
        }

        private void btnUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "YearMonth: 2024/01 or 2024END",
                DefaultButtonIndex = 0,
                Editor = new TextEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;
            string yearMonth = result?.ToString().Trim() ?? "";
            if (yearMonth.Length < 4) return;

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                int year = Convert.ToInt16(yearMonth.Substring(0, 4));
                int month = yearMonth.Contains("/") ? Convert.ToInt16(yearMonth.Split('/')[1]) : 0;
                string filePath = "";

                using (var client = new CookieAwareWebClient())
                {
                    // Thêm các cookie từ JSON mà bạn cung cấp
                    client.CookieContainer.Add(new Cookie("Admin", "FHSADMIN", "/", "10.198.170.92"));
                    client.CookieContainer.Add(new Cookie("SectionID", "G102", "/", "10.198.170.92"));
                    client.CookieContainer.Add(new Cookie("User", "VNW0000011", "/", "10.198.170.92"));
                    client.CookieContainer.Add(new Cookie("UserName", "%E6%97%A5%E6%9C%88%E6%98%8E", "/", "10.198.170.92"));

                    // Tải file Excel từ API
                    string url = $"http://10.198.170.92:7003/api/GenExcel/GetExcelFile?excelYM={year.ToString().Substring(2, 2)}{month.ToString("X")}&isYear={month == 0}";
                    filePath = Path.Combine(TPConfigs.DocumentPath(), $"KPI - {yearMonth}{DateTime.Now.ToString("yyyMMddHHmmss")}.xlsx");

                    try
                    {
                        client.DownloadFile(url, filePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Đã xảy ra lỗi khi tải tệp: " + ex.Message);
                        return;
                    }
                }
                DataSet ds;

                string extension = Path.GetExtension(filePath);
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    reader.Close();
                }

                DataTable dt = ds.Tables[0];

                var filteredRows = dt.AsEnumerable().Where(r => r.Field<string>("部門代碼").StartsWith("78"));

                List<dt402_KPIWeb> dataKPIs = new List<dt402_KPIWeb>();
                foreach (var item in filteredRows)
                {
                    var dateStr = ConvertToDateString(item["評核年月"].ToString());

                    var data = new dt402_KPIWeb()
                    {
                        YearMonth = dateStr,
                        IdUsr = item["工號"].ToString(),
                        DeptScore = item["核定成績"].ToString(),
                        DeptComments = item["核定評語"].ToString(),
                        MgrScore = item["經理室核定成績"].ToString(),
                        MgrComments = item["經理室核定評語"].ToString()
                    };

                    if (month != 0)
                    {
                        string url = $"https://www.fhs.com.tw/ads/api/Furnace/rest/json/hr/s16/{data.IdUsr}vkokv{year}-{month.ToString("00")}";
                        using (WebClient client = new WebClient())
                        {
                            try
                            {
                                string response = client.DownloadString(url);
                                if (!string.IsNullOrEmpty(response))
                                {
                                    var res = response.Replace("o|o", "").Split('|');

                                    data.TotalSalary = res[32];
                                    data.ActualSalary = res[43];
                                }
                            }
                            catch { }
                        }
                    }

                    dataKPIs.Add(data);
                }

                if (dataKPIs.Count == 0) return;

                if (dt402_KPIWebBUS.Instance.GetList().Any(r => r.YearMonth == dataKPIs.First().YearMonth))
                {
                    dt402_KPIWebBUS.Instance.RemoveByYearMonth(dataKPIs.First().YearMonth);
                }

                var rowIns = dt402_KPIWebBUS.Instance.AddRange(dataKPIs);
            }

            LoadData();
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"KPI - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void btnSalary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = "薪水";
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請選年月</font>";
            args.DefaultButtonIndex = 0;
            ComboBoxEdit editor = new ComboBoxEdit();
            editor.Properties.Items.AddRange(yearMonths);
            editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            editor.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            editor.Properties.AppearanceDropDown.Font = editor.Font;
            editor.ForeColor = System.Drawing.Color.Black;
            args.Editor = editor;

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string yearMonthSelect = result?.ToString() ?? "";

            string year = yearMonthSelect.Substring(0, 4);
            string month = yearMonthSelect.Substring(5, 2);

            if (!yearMonthSelect.Contains("/"))
                return;

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                var dataUpdates = dt402_KPIWebBUS.Instance.GetList().Where(r => r.YearMonth == yearMonthSelect).ToList();
                foreach (var item in dataUpdates)
                {
                    string url = $"https://www.fhs.com.tw/ads/api/Furnace/rest/json/hr/s16/{item.IdUsr}vkokv{year}-{month}";
                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            string response = client.DownloadString(url);

                            if (!string.IsNullOrEmpty(response))
                            {
                                var data = response.Replace("o|o", "").Split('|');

                                item.TotalSalary = data[32];
                                item.ActualSalary = data[43];

                                dt402_KPIWebBUS.Instance.AddOrUpdate(item);
                            }
                        }
                        catch (WebException ex)
                        {
                            Console.WriteLine($"Failed to fetch data: {ex.Message}");
                        }
                    }
                }

                LoadData();
            }
        }
    }
}
