using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class uc309_ChooseDate : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_ChooseDate()
        {
            InitializeComponent();

            // Gán giá trị trực tiếp vào thuộc tính trước khi binding
            DateForm = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1).AddDays(20);
            DateTo = DateTime.Now;

            // Thiết lập DataBindings
            dateFrom.DataBindings.Add("DateTime", this, nameof(DateForm), false, DataSourceUpdateMode.OnPropertyChanged);
            dateTo.DataBindings.Add("DateTime", this, nameof(DateTo), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public DateTime DateForm { get; set; }
        public DateTime DateTo { get; set; }
    }
}
