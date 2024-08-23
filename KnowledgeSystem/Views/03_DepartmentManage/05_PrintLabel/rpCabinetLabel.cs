using DevExpress.XtraReports.UI;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel
{
    public partial class rpCabinetLabel : DevExpress.XtraReports.UI.XtraReport
    {
        public rpCabinetLabel()
        {
            InitializeComponent();
            BindData();
        }

        public void BindData()
        {
            lbDept.DataBindings.Add("Text", DataSource, "Dept");
            lbManager.DataBindings.Add("Text", DataSource, "Manager");
            lbAgent.DataBindings.Add("Text", DataSource, "Agent");
            lbBoss.DataBindings.Add("Text", DataSource, "Boss");
        }
    }
}
