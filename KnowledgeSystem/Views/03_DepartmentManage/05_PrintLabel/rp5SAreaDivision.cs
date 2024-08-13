using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel
{
    public partial class rp5SAreaDivision : DevExpress.XtraReports.UI.XtraReport
    {
        public rp5SAreaDivision()
        {
            InitializeComponent();
            BindData();
        }

        public void BindData()
        {
            lbCode.DataBindings.Add("Text", DataSource, "Code");
            lbDeptName.DataBindings.Add("Text", DataSource, "DeptName");
            lbManager.DataBindings.Add("Text", DataSource, "Manager");
            lbAgent.DataBindings.Add("Text", DataSource, "Agent");
            lbBoss.DataBindings.Add("Text", DataSource, "Boss");
            lbPlace.DataBindings.Add("Text", DataSource, "Place");
        }
    }
}
