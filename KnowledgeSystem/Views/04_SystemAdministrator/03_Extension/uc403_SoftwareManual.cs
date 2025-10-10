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
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            // Kiểm tra xem có quyền thêm SOP hay không
            var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var groupEditDeptAndJob = dm_GroupBUS.Instance.GetListByName("SOP【新增】");

            bool roleAddSOP = groupEditDeptAndJob.Any(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id));
            if (!roleAddSOP)
            {
                btnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

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
            string softName = view.GetRowCellValue(e.RowHandle, gColSoftName)?.ToString();
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

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            uc403_SoftManual_Info ucInfo = new uc403_SoftManual_Info();
            if (XtraDialog.Show(ucInfo, "新增操作手冊", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            string softName = ucInfo.SoftName;
            string sopName = ucInfo.SOPName;
            string filePath = ucInfo.FilePath;

            string encrytName = EncryptionHelper.EncryptionFileName(filePath);

            if (Directory.Exists(TPConfigs.Folder403))
                Directory.CreateDirectory(TPConfigs.Folder403);

            File.Copy(filePath, Path.Combine(TPConfigs.Folder403, encrytName));

            dt403_SoftwareManual sopManual = new dt403_SoftwareManual()
            {
                SoftName = softName,
                DisplayName = sopName,
                EncryptName = encrytName,
                DateUpload = DateTime.Now,
            };

            dt403_SoftwareManualBUS.Instance.Add(sopManual);

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gvAttachment_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            string actualName = view.GetRowCellValue(view.FocusedRowHandle, gColActualName).ToString();
            string encryptName = view.GetRowCellValue(view.FocusedRowHandle, gColEncryptName).ToString();

            string source = Path.Combine(TPConfigs.Folder403, encryptName);
            string dest = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyMMddhhmmss} {actualName}");
            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(source, dest, true);

            f00_VIewFile viewFile = new f00_VIewFile(dest);
            viewFile.ShowDialog();
        }
    }
}
