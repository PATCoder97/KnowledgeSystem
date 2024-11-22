using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._04_BorrVehicle
{
    public partial class uc304_BackVehicle : DevExpress.XtraEditors.XtraUserControl
    {
        public uc304_BackVehicle()
        {
            InitializeComponent();

            timeBackTime.DataBindings.Add("DateTimeOffset", this, "BackTime");
            txbEndKm.DataBindings.Add("EditValue", this, "EndKm");
        }

        public DateTimeOffset BackTime { get; set; }
        public string EndKm { get; set; }

        private void uc304_BackVehicle_Load(object sender, EventArgs e)
        {
            BackTime =  DateTimeOffset.Now;
        }
    }
}
