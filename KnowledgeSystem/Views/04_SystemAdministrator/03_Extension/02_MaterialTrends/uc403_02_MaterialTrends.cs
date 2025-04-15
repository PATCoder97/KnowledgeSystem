using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DevExpress.XtraEditors;
using ExcelDataReader;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._02_MaterialTrends
{
    public partial class uc403_02_MaterialTrends : DevExpress.XtraEditors.XtraUserControl
    {
        public uc403_02_MaterialTrends()
        {
            InitializeComponent();
        }

        List<string> excelFiles = new List<string>();
        List<MaterialInfo> resultList = new List<MaterialInfo>();

        public class MaterialInfo
        {
            public string Month { get; set; }
            public string Keyword { get; set; }
            public double Quantity { get; set; }
            public double Total { get; set; }
        }


        private bool SelectAndGetExcelFiles()
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select folder containing Excel files";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    //string selectedPath = folderDialog.SelectedPath;
                    string selectedPath = "E:\\01. Softwares Programming\\24. Knowledge System\\03. Documents\\5.檢驗量統計";

                    // Lấy tất cả các file Excel có chứa yyyyMM trong tên
                    excelFiles = Directory.GetFiles(selectedPath, "*.*")
                       .Where(file => (file.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)) &&
                                      Regex.IsMatch(Path.GetFileName(file), @"\d{4}(0[1-9]|1[0-2])"))
                       .ToList();

                    if (excelFiles.Count == 0)
                    {
                        XtraMessageBox.Show("Không tìm thấy file Excel nào trong thư mục đã chọn.",
                            "Excel Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }

                    // ✅ Kiểm tra toàn bộ file xem có file nào đang bị mở hay không
                    var fileInUse = excelFiles.FirstOrDefault(file =>
                    {
                        try
                        {
                            using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                            {
                                return false; // File không bị khóa
                            }
                        }
                        catch (IOException)
                        {
                            return true; // File đang bị mở
                        }
                    });

                    if (fileInUse != null)
                    {
                        XtraMessageBox.Show($"File đang được sử dụng: {Path.GetFileName(fileInUse)}\nVui lòng đóng file và thử lại.",
                            "File Đang Mở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    // ✅ Mọi thứ OK
                    return true;
                }
            }

            // Người dùng nhấn Cancel khi chọn thư mục
            return false;
        }


        private DataTable ReadExcelFile(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Automatically detect whether the Excel file is xls or xlsx
                IExcelDataReader reader = filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
                    ? ExcelReaderFactory.CreateOpenXmlReader(stream)
                    : ExcelReaderFactory.CreateBinaryReader(stream);

                // Configure the reader to use headers
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };

                // Read the Excel file into a DataSet
                DataSet dataSet = reader.AsDataSet(conf);
                reader.Close();

                // Return the table with a name similar to "檢化驗彙總表"
                var table = dataSet.Tables.Cast<DataTable>()
                    .FirstOrDefault(t => t.TableName.Contains("檢化驗彙總表"));

                if (table == null)
                {
                    throw new Exception("No table with a name similar to '檢化驗彙總表' was found in the Excel file.");
                }

                return table;
            }
        }

        void ExportChartByKeyword(List<MaterialInfo> data, string outputPath)
        {
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var keywords = data.Select(x => x.Keyword).Distinct();

                foreach (var keyword in keywords)
                {
                    var sheet = package.Workbook.Worksheets.Add(keyword);
                    var filtered = data.Where(x => x.Keyword == keyword).OrderBy(x => x.Month).ToList();

                    // Header
                    sheet.Cells[1, 1].Value = "Month";
                    sheet.Cells[1, 2].Value = "Quantity";

                    // Data
                    for (int i = 0; i < filtered.Count; i++)
                    {
                        sheet.Cells[i + 2, 1].Value = filtered[i].Month;
                        sheet.Cells[i + 2, 2].Value = filtered[i].Quantity;
                    }

                    // Biểu đồ cột
                    var chart = sheet.Drawings.AddChart("chart_" + keyword, eChartType.ColumnClustered) as ExcelBarChart;
                    chart.SetPosition(1, 0, 3, 0);
                    chart.SetSize(800, 400);
                    chart.Legend.Remove();

                    var xRange = sheet.Cells[2, 1, filtered.Count + 1, 1]; // Tháng
                    var yRange = sheet.Cells[2, 2, filtered.Count + 1, 2]; // Quantity

                    var series = chart.Series.Add(yRange, xRange);
                    series.Header = "";
                    // Trục Y
                    chart.YAxis.Title.Text = "分析項目數";
                    chart.YAxis.Title.Rotation = 270;
                    chart.YAxis.Title.Font.SetFromFont("Arial", 12);
                    chart.YAxis.Title.Font.ComplexFont = "Arial";
                    chart.YAxis.Title.Font.LatinFont = "Arial";

                    // Trục X
                    chart.XAxis.Title.Text = "月份";
                    chart.XAxis.Title.Font.SetFromFont("DFKai-SB", 12);

                    chart.YAxis.MajorTickMark = eAxisTickMark.Cross; // Tick mark chính
                    chart.YAxis.MinorTickMark = eAxisTickMark.None;   // Ẩn tick mark phụ
                    chart.XAxis.MajorTickMark = eAxisTickMark.Cross; // Tick mark chính
                    chart.XAxis.MinorTickMark = eAxisTickMark.None;   // Ẩn tick mark phụ
                }

                // Lưu file
                File.WriteAllBytes(outputPath, package.GetAsByteArray());
            }
        }

        private void uc403_02_MaterialTrends_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            var resultFile = SelectAndGetExcelFiles();
            if (!resultFile) return;

            string[] keywords = { "鐵礦石", "固雜料", "鋼渣", "助熔劑", "耐火材", "合金鐵", "煤焦炭", "增碳劑", "化學品", "環境水質", "金屬工程材料", "煤化學", "校正科研", "快速化學分析", "產品檢驗", "非破壞檢驗" };

            foreach (var file in excelFiles)
            {
                var dt = ReadExcelFile(file);


                int indexRowHeader = dt.AsEnumerable()
                    .Select((row, index) => new { Row = row, Index = index })
                    .FirstOrDefault(x => keywords.Any(k => x.Row.ItemArray.Any(field => field.ToString().Contains(k))))
                    ?.Index ?? -1;

                int indexRowData = dt.AsEnumerable().Select((row, index) => new { Row = row, Index = index }).FirstOrDefault(x => x.Row.ItemArray.Any(field => field.ToString().Contains("合計")))?.Index ?? -1;

                string month = Regex.Match(Path.GetFileName(file), @"(20\d{2}(0[1-9]|1[0-2]))").Value;
                // Lấy dòng tiêu đề và dòng dữ liệu tổng
                DataRow headerRow = dt.Rows[indexRowHeader];
                DataRow dataRow = dt.Rows[indexRowData];

                foreach (string keyword in keywords)
                {
                    // Tìm index của cột tương ứng với từ khóa
                    int columnIndex = -1;
                    for (int i = 0; i < headerRow.ItemArray.Length; i++)
                    {
                        if (headerRow[i].ToString().Trim().Contains(keyword))
                        {
                            columnIndex = i;
                            break;
                        }
                    }

                    // Nếu tìm được, lấy Quantity và Total (cột bên phải)
                    if (columnIndex != -1)
                    {
                        double quantity = 0;
                        double total = 0;

                        double.TryParse(dataRow[columnIndex].ToString(), out quantity);
                        if (columnIndex + 1 < dataRow.ItemArray.Length)
                            double.TryParse(dataRow[columnIndex + 1].ToString(), out total);

                        resultList.Add(new MaterialInfo
                        {
                            Keyword = keyword,
                            Quantity = quantity,
                            Total = total,
                            Month = month
                        });
                    }
                }
            }

            gcData.DataSource = resultList;

            gvData.BestFitColumns();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportChartByKeyword(resultList, $"C:\\Users\\Dell Alpha\\Desktop\\RÁC 1\\Test403\\Biểu_đồ_nguyên_vật_liệu_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
    }
}
