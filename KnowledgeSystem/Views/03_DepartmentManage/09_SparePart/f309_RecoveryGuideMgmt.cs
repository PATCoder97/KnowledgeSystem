using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class f309_RecoveryGuideMgmt : XtraForm
    {
        private readonly BindingSource sourceGuides = new BindingSource();
        private RefreshHelper helper;

        public f309_RecoveryGuideMgmt()
        {
            InitializeComponent();
            InitializeIcon();
            ConfigureGrid();

            helper = new RefreshHelper(gvData, "Id");
            gcData.DataSource = sourceGuides;
        }

        private void f309_RecoveryGuideMgmt_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void InitializeIcon()
        {
            btnUpload.ImageOptions.SvgImage = TPSvgimages.UploadFile;
            btnView.ImageOptions.SvgImage = TPSvgimages.View;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void ConfigureGrid()
        {
            gvData.Columns.Clear();
            gvData.ReadOnlyGridView();
            gvData.OptionsBehavior.Editable = false;
            gvData.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvData.OptionsView.ColumnAutoWidth = false;
            gvData.OptionsView.EnableAppearanceOddRow = true;
            gvData.OptionsView.ShowAutoFilterRow = true;
            gvData.OptionsView.ShowGroupPanel = false;
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gvData.Columns.AddVisible(nameof(dt309_RecoveryGuides.Title), "\u6a19\u984c").Width = 220;
            gvData.Columns.AddVisible(nameof(dt309_RecoveryGuides.ActualName), "\u6a94\u540d").Width = 260;
            gvData.Columns.AddVisible(nameof(dt309_RecoveryGuides.DisplayOrder), "\u6392\u5e8f").Width = 80;
            gvData.Columns.AddVisible(nameof(dt309_RecoveryGuides.UploadedBy), "\u4e0a\u50b3\u8005").Width = 120;

            var colDate = gvData.Columns.AddVisible(nameof(dt309_RecoveryGuides.UploadedDate), "\u4e0a\u50b3\u6642\u9593");
            colDate.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
            colDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colDate.Width = 160;
        }

        private void LoadData()
        {
            sourceGuides.DataSource = dt309_RecoveryBUS.Instance.GetGuideList();
            gvData.BestFitColumns();
        }

        private dt309_RecoveryGuides GetFocusedGuide()
        {
            if (gvData.FocusedRowHandle < 0)
            {
                return null;
            }

            return gvData.GetRow(gvData.FocusedRowHandle) as dt309_RecoveryGuides;
        }

        private void btnUpload_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = TPConfigs.FilterFile;
                dialog.Multiselect = true;
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                foreach (string file in dialog.FileNames)
                {
                    var saved = Material309RecoveryHelper.SaveGuideFile(file);
                    int id = dt309_RecoveryBUS.Instance.AddGuide(new dt309_RecoveryGuides
                    {
                        Title = Path.GetFileNameWithoutExtension(saved.actualName),
                        ActualName = saved.actualName,
                        EncryptionName = saved.encryptionName,
                        FileExt = saved.extension,
                        UploadedBy = TPConfigs.LoginUser.Id,
                        UploadedDate = DateTime.Now
                    });

                    if (id <= 0)
                    {
                        MsgTP.MsgError($"\u4e0a\u50b3\u5931\u6557: {Path.GetFileName(file)}");
                    }
                }
            }

            LoadData();
        }

        private void btnView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var guide = GetFocusedGuide();
            if (guide == null)
            {
                return;
            }

            Material309RecoveryHelper.OpenGuideFiles(new[] { guide });
        }

        private void btnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            var guide = GetFocusedGuide();
            if (guide == null)
            {
                return;
            }

            var result = XtraMessageBox.Show(
                $"\u78ba\u5b9a\u8981\u522a\u9664\u6307\u5f15\uff1a\r\n{guide.Title}",
                TPConfigs.SoftNameTW,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            if (!dt309_RecoveryBUS.Instance.DeactivateGuide(guide.Id))
            {
                MsgTP.MsgErrorDB();
                return;
            }

            LoadData();
        }

        private void btnReload_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
