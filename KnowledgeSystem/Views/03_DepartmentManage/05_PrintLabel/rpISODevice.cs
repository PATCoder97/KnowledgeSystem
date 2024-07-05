using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel
{
    public partial class rpISODevice : DevExpress.XtraReports.UI.XtraReport
    {
        public rpISODevice()
        {
            InitializeComponent();
            BindData();
        }

        public void BindData()
        {
            lb1.DataBindings.Add("Text", DataSource, "Lb1");
            lb2.DataBindings.Add("Text", DataSource, "Lb2");
        }
    }
}
