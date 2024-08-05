using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml;
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

        private void InitializeIcon()
        {
            btnUpload.ImageOptions.SvgImage = TPSvgimages.UploadFile;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();

            List<dt402_KPIWeb> dataKpis = dt402_KPIWebBUS.Instance.GetList();
            var usrs = dm_UserBUS.Instance.GetList();

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
            string dataPath = "";
            DataSet ds;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel files (*.xls, *.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                dataPath = openFileDialog.FileName;
            }

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {

                string extension = Path.GetExtension(dataPath);
                using (var stream = File.Open(dataPath, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader reader;
                    if (extension == "*.xls")
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }

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

                    dataKPIs.Add(new dt402_KPIWeb()
                    {
                        YearMonth = dateStr,
                        IdUsr = item["工號"].ToString(),
                        DeptScore = item["核定成績"].ToString(),
                        DeptComments = item["核定評語"].ToString(),
                        MgrScore = item["經理室核定成績"].ToString(),
                        MgrComments = item["經理室核定評語"].ToString()
                    });
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
    }
}
