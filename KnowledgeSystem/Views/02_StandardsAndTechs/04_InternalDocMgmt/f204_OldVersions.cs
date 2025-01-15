using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using GridView = DevExpress.XtraGrid.Views.Grid.GridView;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt
{
    public partial class f204_OldVersions : DevExpress.XtraEditors.XtraForm
    {
        public f204_OldVersions()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public int idBase = -1;
        BindingSource sourceBases = new BindingSource();

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void f204_OldVersions_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();

            gcData.DataSource = sourceBases;
            gvData.BestFitColumns();
        }

        private void LoadData()
        {
            List<dt204_OldVersion> dataOldVers = dt204_OldVersionBUS.Instance.GetListIdBase(idBase);
            sourceBases.DataSource = dataOldVers;

            gvData.OptionsBehavior.AutoExpandAllGroups = false;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"版本歷史 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            var pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo hitInfo = view.CalcHitInfo(pt);
            if (!hitInfo.InRowCell) return;

            int idAtt = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdAtt));
            var att = dm_AttachmentBUS.Instance.GetItemById(idAtt);

            string filePath = att.EncryptionName;
            string fileName = att.ActualName;

            string sourcePath = Path.Combine(TPConfigs.Folder204, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"{Regex.Replace(fileName, @"[\\/:*?""<>|]", "")}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_VIewFile fView = new f00_VIewFile(destPath);
            fView.ShowDialog();
        }
    }
}
