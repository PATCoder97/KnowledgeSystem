using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel
{
    public partial class rpDeviceManagement : DevExpress.XtraReports.UI.XtraReport
    {
        public rpDeviceManagement()
        {
            InitializeComponent();
            BindData();
        }

        public void BindData()
        {
            lbNameVN.DataBindings.Add("Text", DataSource, "NameVN");
            lbNameTW.DataBindings.Add("Text", DataSource, "NameTW");
            lbCode.DataBindings.Add("Text", DataSource, "Code");
            lbDept.DataBindings.Add("Text", DataSource, "Dept");
            lbUserVN.DataBindings.Add("Text", DataSource, "UserVN");
            lbUserTW.DataBindings.Add("Text", DataSource, "UserTW");
        }
    }
}
