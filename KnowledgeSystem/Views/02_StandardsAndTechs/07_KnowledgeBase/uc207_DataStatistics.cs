using DevExpress.XtraEditors;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KnowledgeSystem.Configs;
using System.IO;
using DevExpress.XtraEditors.TextEditController.InputHandler;
using DevExpress.Utils.CommonDialogs;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts;
using System.Diagnostics;
using DevExpress.XtraSplashScreen;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_DataStatistics : DevExpress.XtraEditors.XtraUserControl
    {
        public uc207_DataStatistics()
        {
            InitializeComponent();
        }

        #region parameters

        private class DataStatisticsChart
        {
            public string DisplayName { get; set; }
            public int Achieve { get; set; }
            public int Target { get; set; }
        }

        Font fontIndicator = new Font("Times New Roman", 12.0f, FontStyle.Italic);

        const string NAME_GRADE = "處別";
        const string NAME_CLASS = "課別";
        const string NAME_USER = "上傳者";

        const string NAME_UPPER = "超標";
        const string NAME_EQUAL = "達標";
        const string NAME_LOWER = "未達標";

        List<DataStatisticsChart> lsDataStatistic = new List<DataStatisticsChart>();

        BindingSource source = new BindingSource();

        #endregion

        #region methods

        bool cal(Int32 _Width, GridView _View)
        {
            _View.IndicatorWidth = _View.IndicatorWidth < _Width ? _Width : _View.IndicatorWidth;
            return true;
        }

        void IndicatorDraw(RowIndicatorCustomDrawEventArgs e)
        {
            e.Info.Appearance.Font = fontIndicator;
            e.Info.Appearance.ForeColor = Color.FromArgb(16, 110, 190);
        }

        private void LoadData()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsGrade = db.dm_Departments.Where(r => r.IdParent == -1).ToList();

                cbbGrade.Properties.DataSource = lsGrade;
                cbbGrade.Properties.DisplayMember = "DisplayName"; ;
                cbbGrade.Properties.ValueMember = "Id";
            }

            cbbGrade.EditValue = "7";
        }

        private void CreateRuleGV()
        {
            gvData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Blue }, $"[Remark] = \'{NAME_UPPER}\'");
            gvData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Green }, $"[Remark] = \'{NAME_EQUAL}\'");
            gvData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Red }, $"[Remark] = \'{NAME_LOWER}\'");
        }

        private void StatisticsData()
        {
            gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;

            string nameType = gColType.Caption;
            string IdGrade = cbbGrade.EditValue.ToString();

            DateTime fromDate = txbFromDate.DateTime.Date;
            DateTime toDate = txbToDate.DateTime.Date.AddDays(1).AddSeconds(-1);

            if (string.IsNullOrEmpty(txbFromDate.Text) || string.IsNullOrEmpty(txbFromDate.Text) || toDate < fromDate)
            {
                XtraMessageBox.Show("請選擇正確的日期數據！", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsTargets = db.dt207_Targets.ToList();

                // Lấy danh sách văn kiện và kèm theo mã bộ phận (Lấy luôn văn kiện đang trình ký Thêm, Sửa, Xoá)
                var lsDoc = (from data in db.dt207_Base.
                             Where(r => !r.IsDelete && r.UploadDate >= fromDate && r.UploadDate <= toDate)
                             join users in db.Users on data.UserUpload equals users.Id
                             select new
                             {
                                 data.Id,
                                 data.UserUpload,
                                 users.DisplayName,
                                 Class = users.IdDepartment,
                                 Grade = users.IdDepartment.Substring(0, 2)
                             }).ToList().Select(r => new
                             {
                                 r.Id,
                                 UserUploadName = $"{r.UserUpload} {r.DisplayName}",
                                 r.Class,
                                 r.Grade
                             }).ToList();

                // Xử lý các văn kiện đang trong lưu trình trình ký không tính
                var lsDocProcessing = db.dt207_DocProgress.Where(r => !r.IsComplete).Select(r => r.IdKnowledgeBase).Distinct().ToList();

                lsDoc = lsDoc.Where(r => !lsDocProcessing.Contains(r.Id)).ToList();

                lsDataStatistic.Clear();

                if (nameType == NAME_GRADE)
                {
                    var lsGrade = db.dm_Departments.Where(r => r.IdParent == -1 && r.Id != "7").ToList();

                    foreach (var item in lsGrade)
                    {
                        DataStatisticsChart data = new DataStatisticsChart()
                        {
                            DisplayName = item.DisplayName,
                            Achieve = lsDoc.Count(r => r.Grade == item.Id),
                            Target = lsTargets.FirstOrDefault(r => r.IdDept == item.Id).Targets
                        };

                        lsDataStatistic.Add(data);
                    }
                }

                if (nameType == NAME_CLASS)
                {
                    var idChildGrade = db.dm_Departments.First(r => r.Id == IdGrade).IdChild;

                    var lsGrade = db.dm_Departments.Where(r => r.IdParent == idChildGrade).ToList();

                    foreach (var item in lsGrade)
                    {
                        DataStatisticsChart data = new DataStatisticsChart()
                        {
                            DisplayName = item.DisplayName,
                            Achieve = lsDoc.Count(r => r.Class == item.Id),
                            Target = lsTargets.FirstOrDefault(r => r.IdDept == item.Id).Targets
                        };

                        lsDataStatistic.Add(data);
                    }
                }

                if (nameType == NAME_USER)
                {
                    string idClass = cbbClass.EditValue.ToString();
                    var lsDocClass = lsDoc.Where(r => r.Class == idClass).GroupBy(r => r.UserUploadName).Select(r => new { r.Key, Count = r.Count() }).ToList();

                    foreach (var item in lsDocClass)
                    {
                        DataStatisticsChart data = new DataStatisticsChart()
                        {
                            DisplayName = item.Key,
                            Achieve = item.Count
                        };

                        lsDataStatistic.Add(data);
                    }
                }

                gcData.RefreshDataSource();
            }
        }

        #endregion

        private void uc207_DataStatistics_Load(object sender, EventArgs e)
        {
            source.DataSource = lsDataStatistic;
            gcData.DataSource = source;

            CreateRuleGV();
            LoadData();

            btnExcel.Text = "導出\r\nExcel";
            btnStatistics.Text = "資料\r\n統計";
            btnChart.Text = "繪製\r\n圖表";
            btnTarget.Text = "設定\r\n目標";

            gcData.ForceInitialize();

            gvData.CustomUnboundColumnData += gvData_CustomUnboundColumnData;
        }

        private void gvData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Column.FieldName == "Remark" && e.IsGetData)
            {
                int value1 = Convert.ToInt32(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "Achieve"));
                int value2 = Convert.ToInt32(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "Target"));

                int difference = value1 - value2;

                if (difference == 0)
                    e.Value = NAME_EQUAL;
                else if (difference > 0)
                    e.Value = NAME_UPPER;
                else
                    e.Value = NAME_LOWER;
            }
        }

        private void cbbGrade_EditValueChanged(object sender, EventArgs e)
        {
            string idGrade = cbbGrade.EditValue.ToString();

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var idParent = db.dm_Departments.First(r => r.Id == idGrade).IdChild;

                var lsGrade = db.dm_Departments.Where(r => r.IdParent == idParent).ToList();

                cbbClass.Properties.DataSource = lsGrade;
                cbbClass.Properties.DisplayMember = "DisplayName"; ;
                cbbClass.Properties.ValueMember = "Id";
            }

            gColType.Caption = idGrade == "7" ? NAME_GRADE : NAME_CLASS;
            cbbClass.EditValue = null;
        }

        private void cbbClass_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cbbClass.Text))
            {
                gColType.Caption = NAME_USER;
            }
        }

        private void gvData_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (!gvData.IsGroupRow(e.RowHandle))
            {
                if (e.Info.IsRowIndicator)
                {
                    if (e.RowHandle < 0)
                    {
                        e.Info.ImageIndex = 0;
                        e.Info.DisplayText = string.Empty;
                    }
                    else
                    {
                        e.Info.ImageIndex = -1;
                        e.Info.DisplayText = (e.RowHandle + 1).ToString();
                    }

                    IndicatorDraw(e);

                    SizeF size = e.Graphics.MeasureString(e.Info.DisplayText, fontIndicator);
                    int width = (int)size.Width + 20;

                    BeginInvoke(new MethodInvoker(() => cal(width, gvData)));
                }
            }
            else
            {
                e.Info.ImageIndex = -1;
                e.Info.DisplayText = $"[{e.RowHandle * -1}]";

                IndicatorDraw(e);

                SizeF size = e.Graphics.MeasureString(e.Info.DisplayText, fontIndicator);
                int width = (int)size.Width + 20;

                BeginInvoke(new MethodInvoker(() => cal(width, gvData)));
            }
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            StatisticsData();
        }

        private void btnChart_Click(object sender, EventArgs e)
        {
            List<ChartDataSource> sourceChart = new List<ChartDataSource>();

            sourceChart.AddRange(lsDataStatistic.Select(r => new ChartDataSource() { SeriesName = "Actual", XAxis = r.DisplayName, YAxis = r.Achieve }));
            sourceChart.AddRange(lsDataStatistic.Select(r => new ChartDataSource() { SeriesName = "Targets", XAxis = r.DisplayName, YAxis = r.Target }));

            f207_ChartStatistics f207_Chart = new f207_ChartStatistics(sourceChart);
            f207_Chart.ShowDialog();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            string nameFile = "";
            switch (gColType.Caption)
            {
                case NAME_GRADE:
                    nameFile = "部門資料上傳統計表";
                    break;
                case NAME_CLASS:
                    nameFile = "各處資料上傳統計表";
                    break;
                case NAME_USER:
                    nameFile = "各單位同仁資料上傳統計表";
                    break;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "導出資料上傳統計表";
            saveFileDialog1.DefaultExt = "xlsx";
            saveFileDialog1.Filter = "Excel Files|*.xlsx";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = $"{DateTime.Now:yyyyMMddHHmmss}-{nameFile}";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            string newFilePath = Path.Combine(saveFileDialog1.FileName);

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(newFilePath))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("資料上傳統計表");
                    ws.Cells.Style.Font.Name = "DFKai-SB";
                    ws.Cells.Style.Font.Size = 14;
                    ws.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    ws.Column(1).Width = 35;
                    ws.Column(2).Width = 20;
                    ws.Column(3).Width = 20;

                    // Xuất dữ liệu từ list data sang Table
                    ws.Cells["A2"].LoadFromCollection(lsDataStatistic, true, TableStyles.Medium2);

                    ws.Cells["A1"].Value = nameFile;
                    ws.Cells["A1"].Style.Font.Size = 24;
                    ws.Cells["A1:C1"].Merge = true;

                    ws.Cells["A2"].Value = gColType.Caption;
                    ws.Cells["B2"].Value = gColAchieve.Caption;
                    ws.Cells["C2"].Value = gColTarget.Caption;

                    // Vẽ đồ thị
                    int sumRow = lsDataStatistic.Count();

                    ExcelChart chart = ws.Drawings.AddChart("FindingsChart", eChartType.ColumnClustered);
                    chart.Title.Text = nameFile;
                    chart.SetPosition(1, 0, 4, 0);
                    chart.SetSize(1000, 300);
                    var ser1 = (ExcelBarChartSerie)(chart.Series.Add(ws.Cells[$"B2:B{sumRow + 2}"], ws.Cells[$"A2:A{sumRow + 2}"]));
                    var ser2 = (ExcelBarChartSerie)(chart.Series.Add(ws.Cells[$"C2:C{sumRow + 2}"], ws.Cells[$"A2:A{sumRow + 2}"]));
                    ser1.Header = gColAchieve.Caption;
                    ser1.Header = gColTarget.Caption;

                    //Format the labels
                    ser1.DataLabel.ShowValue = true;
                    ser1.DataLabel.Position = eLabelPosition.OutEnd;
                    ser2.DataLabel.ShowValue = true;
                    ser2.DataLabel.Position = eLabelPosition.OutEnd;

                    //Format the legend
                    chart.Legend.Add();
                    chart.Legend.Border.Width = 0;
                    chart.Legend.Font.Size = 10;
                    chart.Legend.Font.Bold = true;
                    chart.Legend.Position = eLegendPosition.Top;

                    chart.StyleManager.SetChartStyle(ePresetChartStyle.StackedColumnChartStyle1);

                    pck.Save();
                }

                Process.Start(newFilePath);
            }
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            f207_SetTarget setTarget = new f207_SetTarget();
            setTarget.ShowDialog();
        }
    }
}
