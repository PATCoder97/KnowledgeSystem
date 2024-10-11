using BusinessLayer;
using DataAccessLayer;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Wizards;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension
{
    public partial class uc403_SoftwareManual : DevExpress.XtraEditors.XtraUserControl
    {
        public uc403_SoftwareManual()
        {
            InitializeComponent();
            InitializeIcon();

            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();

        List<dt403_SoftwareManual> bases = new List<dt403_SoftwareManual>();

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                bases = dt403_SoftwareManualBUS.Instance.GetList();

                var lsBasesDisplay = bases.Select(r => r.SoftName).Distinct().Select(r => new { SoftName = r }).ToList();

                sourceBases.DataSource = lsBasesDisplay;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
                gvData.ExpandMasterRow(gvData.FocusedRowHandle);
            }
        }

        private void uc403_SoftwareManual_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvAttachment.ReadOnlyGridView();
            gvAttachment.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvAttachment.ReadOnlyGridView();

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            string softName = view.GetRowCellValue(e.RowHandle, gColSoftName).ToString();
            var haveAtt = bases.Any(r => r.SoftName == softName);

            e.IsEmpty = !haveAtt;
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            string softName = view.GetRowCellValue(e.RowHandle, gColSoftName).ToString();
            e.ChildList = bases.Where(r => r.SoftName == softName).ToList();
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "表單";
        }
    }
}
