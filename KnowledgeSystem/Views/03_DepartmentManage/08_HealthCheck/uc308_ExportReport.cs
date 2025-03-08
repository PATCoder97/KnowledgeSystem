using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    public partial class uc308_ExportReport : DevExpress.XtraEditors.XtraUserControl
    {
        public uc308_ExportReport()
        {
            InitializeComponent();

            cbbYear.DataBindings.Add("Text", this, "Year", false, DataSourceUpdateMode.OnPropertyChanged);
            txbFilePath.DataBindings.Add("EditValue", this, "LeaveFile", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public int Year { get; set; }
        public string SickFile { get; set; }

        private void uc308_ExportReport_Load(object sender, EventArgs e)
        {
            int currentYear = DateTime.Now.Year;

            // Thêm năm hiện tại và 5 năm trước
            cbbYear.Properties.Items.Clear();
            cbbYear.Properties.Items.AddRange(Enumerable.Range(currentYear - 5, 6).Reverse().ToArray());

            // Đặt năm mặc định là năm hiện tại
            cbbYear.SelectedItem = currentYear;
        }
    }
}
