using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraSpreadsheet.Model;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml;
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
using BusinessLayer;

namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    public partial class uc301_SelectOutputFile : DevExpress.XtraEditors.XtraUserControl
    {
        public uc301_SelectOutputFile()
        {
            InitializeComponent();

            // Gán nguồn dữ liệu cho control
            cbbYear.DataBindings.Add("EditValue", this, "Year", true, DataSourceUpdateMode.OnPropertyChanged);
            cbbQuarter.DataBindings.Add("EditValue", this, "Quarter", true, DataSourceUpdateMode.OnPropertyChanged);
            cbbHalfYear.DataBindings.Add("EditValue", this, "HalfYear", true, DataSourceUpdateMode.OnPropertyChanged);
            cbbTypeOf.DataBindings.Add("EditValue", this, "TypeOf", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        public int year { get; set; }
        public int quarter { get; set; }
        public string HalfYear { get; set; }
        public string TypeOf { get; set; }
        public bool IsWrongData { get; set; } = false;

        string sheetName = "DataFile51";

        private void DownloadTempExcel(string savePath)
        {
            SplashScreenManager.ShowDefaultWaitForm();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = "VNW0014732";
                pck.Workbook.Properties.Company = "FHS";
                pck.Workbook.Properties.Title = "Exported by Phan Anh Tuan";
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);

                // Định dạng toàn Sheet
                ws.Cells.Style.Font.Name = "Times New Roman";
                ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells.Style.Font.Size = 14;
                ws.Cells.Style.WrapText = true;

                // Thêm dữ liệu từ Grid vào Excel
                ws.Cells["A1"].Value = "Mã nhân viên\n人員代號";
                ws.Cells["B1"].Value = "Mã chứng chỉ\n課程代號";
                ws.Cells["C1"].Value = "Loại đào tạo mới\nNếu là 異動 thì tick Y";

                int sumColumn = ws.Dimension.Columns;
                int sumRow = ws.Dimension.Rows;

                ws.Columns[1].Width = 20;
                ws.Columns[2].Width = 20;
                ws.Columns[3].Width = 20;

                // Define the data range on the source sheet
                var dataRange = ws.Cells[ws.Dimension.Address];

                //Ading a table to a Range
                ExcelTable tab = ws.Tables.Add(dataRange, "Table1");

                //Formating the table style
                tab.TableStyle = TableStyles.Medium2;

                FileInfo excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);
            }
            SplashScreenManager.CloseDefaultWaitForm();
        }

        private void uc301_SelectOutputFile_Load(object sender, EventArgs e)
        {
            btnDownTemp51.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnUploadFile51.ImageOptions.SvgImage = TPSvgimages.UploadFile;

            List<int> lsYears = new List<int>();
            for (int i = 0; i < 5; i++)
                lsYears.Add(DateTime.Today.Year - i);
            cbbYear.Properties.Items.AddRange(lsYears);
            cbbYear.SelectedItem = DateTime.Today.Year;

            List<int> lsQuarters = new List<int>() { 1, 2, 3, 4 };
            cbbQuarter.Properties.Items.AddRange(lsQuarters);

            List<string> lsHalfYear = new List<string>() { "上半年", "下半年" };
            cbbHalfYear.Properties.Items.AddRange(lsHalfYear);

            cbbTypeOf.Properties.Items.AddRange(TPConfigs.typeOf301.Select(r => r.Value).ToArray());
            cbbTypeOf.SelectedIndex = 0;

            DateTime today = DateTime.Today;
            int currentQuarter = (today.Month - 1) / 3 + 1;
            cbbQuarter.SelectedItem = currentQuarter;

            cbbHalfYear.SelectedIndex = today.Month < 7 ? 0 : 1;
        }

        internal List<dt301_Base> lsData51 = new List<dt301_Base>();

        private void btnDownTemp51_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveFileDialog.DefaultExt = "xlsx";
                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = "ExcelTemp5.1 " + DateTime.Now.ToString("yyyyMMddHHmmss");

                DialogResult result = saveFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    DownloadTempExcel(filePath);
                    Process.Start(filePath);
                }
            }
        }

        private void btnUploadFile51_Click(object sender, EventArgs e)
        {
            string filePath = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";

                DialogResult result = openFileDialog.ShowDialog();

                if (result != DialogResult.OK) return;

                filePath = openFileDialog.FileName;
            }

            DataSet ds;
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

            if (ds.Tables[0].TableName != sheetName)
            {
                XtraMessageBox.Show("Dữ liệu đầu vào không đúng", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SplashScreenManager.ShowDefaultWaitForm();
            lsData51 = new List<dt301_Base>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string idUser = row[0].ToString().Trim();
                string idCourse = row[1].ToString().Trim();
                bool newTrain = row[2].ToString().Trim().ToUpper() == "Y";

                // Dùng tạm  ValidLicense để lưu là nhân viên mới hay là thay đổi chức vụ
                dt301_Base base51 = new dt301_Base() { IdUser = idUser, IdCourse = idCourse, ValidLicense = newTrain };
                lsData51.Add(base51);
            }

            var lsUsers = dm_UserBUS.Instance.GetList();
            var lsCourses = dt301_CourseBUS.Instance.GetList();
            var lsDisplays = (from data in lsData51
                              join usr in lsUsers on data.IdUser equals usr.Id into dtUsr
                              from dtu in dtUsr.DefaultIfEmpty()
                              join course in lsCourses on data.IdCourse equals course.Id into dtCour
                              from dtc in dtCour.DefaultIfEmpty()
                              select new
                              {
                                  人員代號 = data.IdUser,
                                  課程代號 = data.IdCourse,
                                  人員名稱 = dtu != null ? dtu.DisplayName : "",
                                  課程名稱 = dtc != null ? dtc.DisplayName : "",
                                  異動 = data.ValidLicense
                              }).ToList();

            // Nếu có 1 hàng nào trống thì dữ liệu đó chưa đạt
            IsWrongData = lsDisplays.Any(r => string.IsNullOrEmpty(r.人員名稱) || string.IsNullOrEmpty(r.課程名稱));

            SplashScreenManager.CloseDefaultWaitForm();
            gcData51.DataSource = lsDisplays;
            gvData51.BestFitColumns();
        }

        private void cbbTypeOf_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbbTypeOf.SelectedIndex)
            {
                case 1:
                    cbbHalfYear.Enabled = false;
                    cbbQuarter.Enabled = false;
                    break;
                case 2:
                    cbbHalfYear.Enabled = true;
                    cbbQuarter.Enabled = false;
                    break;
                default:
                    cbbHalfYear.Enabled = false;
                    cbbQuarter.Enabled = true;
                    break;
            }
        }
    }
}
