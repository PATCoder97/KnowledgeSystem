using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel
{
    public partial class rpFixedAssets : DevExpress.XtraReports.UI.XtraReport
    {
        public rpFixedAssets()
        {
            InitializeComponent();
            BindData();
        }

        public void BindData()
        {
            lbDept.DataBindings.Add("Text", DataSource, "Dept");
            lbAssetId.DataBindings.Add("Text", DataSource, "AssetId");
            lbNameTW.DataBindings.Add("Text", DataSource, "NameTW");
            lbNameVN.DataBindings.Add("Text", DataSource, "NameVN");
            lbFormat.DataBindings.Add("Text", DataSource, "Format");
            lbMadeBy.DataBindings.Add("Text", DataSource, "MadeBy");
            lbDateGet.DataBindings.Add("Text", DataSource, "DateGet");
        }
    }
}
