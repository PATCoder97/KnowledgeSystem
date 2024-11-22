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
    public partial class uc304_EditInfo : DevExpress.XtraEditors.XtraUserControl
    {
        public uc304_EditInfo()
        {
            InitializeComponent();
            txbStartKm.DataBindings.Add("EditValue", this, "StartKm");
            txbEndKm.DataBindings.Add("EditValue", this, "EndKm");
        }

        public string StartKm { get; set; }
        public string EndKm { get; set; }
    }
}
