using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data.Browsing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using ExcelDataReader;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class uc401_JobTitle : DevExpress.XtraEditors.XtraUserControl
    {
        public uc401_JobTitle()
        {
            InitializeComponent();
            InitializeIcon();
            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        List<dm_JobTitle> lsJobTitles = new List<dm_JobTitle>();
        BindingSource sourceJobTitles = new BindingSource();

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();
            lsJobTitles = dm_JobTitleBUS.Instance.GetList();
            sourceJobTitles.DataSource = lsJobTitles;
            helper.LoadViewInfo();

            gvData.BestFitColumns();
        }

        private void uc401_JobTitle_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            LoadData();

            gcData.DataSource = sourceJobTitles;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f401_JobTitleInfo fInfo = new f401_JobTitleInfo();
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo._formName = "職務";
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            dm_JobTitle _jobSelect = view.GetRow(view.FocusedRowHandle) as dm_JobTitle;

            f401_JobTitleInfo fInfo = new f401_JobTitleInfo();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._formName = "職務";
            fInfo._jobTitle = _jobSelect;
            fInfo.ShowDialog();

            LoadData();
        }
    }
}
