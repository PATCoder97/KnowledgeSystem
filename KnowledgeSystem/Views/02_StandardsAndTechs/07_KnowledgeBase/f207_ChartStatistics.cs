using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_ChartStatistics : DevExpress.XtraEditors.XtraForm
    {
        public f207_ChartStatistics(List<ChartDataSource> source_)
        {
            InitializeComponent();
            sourceChart = source_;
        }

        List<ChartDataSource> sourceChart = new List<ChartDataSource>();

        private void f207_ChartStatistics_Load(object sender, EventArgs e)
        {
            Series series = new Series("已上傳", ViewType.Bar);
            Series series1 = new Series("應上傳", ViewType.Line);
            chartStatistics.Series.Add(series);
            chartStatistics.Series.Add(series1);

            chartStatistics.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

            // Generate a data table and bind the series to it.
            series.DataSource = sourceChart.Where(r => r.SeriesName == "Actual");
            series.ArgumentDataMember = "XAxis";
            series.ValueScaleType = ScaleType.Numerical;
            series.ValueDataMembers.AddRange(new string[] { "YAxis" });
            series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;

            // Generate a data table and bind the series to it.
            series1.DataSource = sourceChart.Where(r => r.SeriesName == "Targets");
            series1.ArgumentDataMember = "XAxis";
            series1.ValueScaleType = ScaleType.Numerical;
            series1.ValueDataMembers.AddRange(new string[] { "YAxis" });
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            ((SideBySideBarSeriesLabel)chartStatistics.Series[0].Label).Position = BarSeriesLabelPosition.Top;
            ((SideBySideBarSeriesLabel)chartStatistics.Series[0].Label).Font = new Font("Times New Roman", 12);

            ((LineSeriesView)chartStatistics.Series[1].View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
            ((LineSeriesView)chartStatistics.Series[1].View).LineMarkerOptions.Kind = MarkerKind.Circle;
            ((PointSeriesLabel)chartStatistics.Series[1].Label).Font = new Font("Times New Roman", 12);

            // 
            AxisLabel axisXLabel = ((XYDiagram)chartStatistics.Diagram).AxisX.Label;
            AxisLabel axisYLabel = ((XYDiagram)chartStatistics.Diagram).AxisY.Label;

            axisYLabel.Font = new Font("Times New Roman", 12);
            axisXLabel.Font = new Font("DFKai-SB", 12);
        }
    }
}