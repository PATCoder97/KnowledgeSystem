using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel
{
    public partial class rpWasteLabel : DevExpress.XtraReports.UI.XtraReport
    {
        public rpWasteLabel()
        {
            InitializeComponent();
            BindData();
        }

        public void BindData()
        {
            lbDept.DataBindings.Add("Text", DataSource, "Dept");
            lbNameVN.DataBindings.Add("Text", DataSource, "NameVN");
            lbNameTW.DataBindings.Add("Text", DataSource, "NameTW");
        }
    }
}
