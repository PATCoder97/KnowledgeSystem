using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    public partial class uc313_FixedAssetMain : XtraUserControl
    {
        private readonly FixedAsset313Context module = new FixedAsset313Context();
        private readonly BindingSource sourceBases = new BindingSource();
        private readonly RefreshHelper helper;

        public uc313_FixedAssetMain()
        {
            InitializeComponent();
            InitializeIcon();
            helper = new RefreshHelper(gvData, nameof(AssetGridRow.AssetCode));
            Load += uc313_FixedAssetMain_Load;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnPhotos.ImageOptions.SvgImage = TPSvgimages.View;
            btnImportExcel.ImageOptions.SvgImage = TPSvgimages.UploadFile;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void uc313_FixedAssetMain_Load(object sender, EventArgs e)
        {
            FixedAsset313GridHelper.ConfigureReadOnlyView(gvData);
            gvData.DoubleClick += gvData_DoubleClick;
            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();
                module.Initialize();

                sourceBases.DataSource = module.BuildAssetRows();
                gcData.DataSource = sourceBases;

                gvData.PopulateColumns();
                ConfigureColumns();
                ApplyPermissions();
                helper.LoadViewInfo();
            }
        }

        private void ApplyPermissions()
        {
            btnAdd.Visibility = module.IsManager313 ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Visibility = module.IsManager313 ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnImportExcel.Visibility = module.IsManager313 ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private void ConfigureColumns()
        {
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AssetGridRow.Entity));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AssetGridRow.IdDept));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AssetGridRow.IdManager));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AssetGridRow.AssetCategory));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AssetGridRow.HasCloseUp));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AssetGridRow.HasOverview));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AssetGridRow.HasInUse));

            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.AssetCode), "資產編號", 120);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.AssetNameTW), "資產中文名稱", 220);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.AssetNameVN), "資產越文名稱", 220);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.DeptName), "部門", 160);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.ManagerName), "經辦", 150);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.AssetCategoryDisplay), "分類", 120);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.TypeName), "類別", 120);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.Location), "位置", 140);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.BrandSpec), "廠牌規格", 150);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.Origin), "產地", 120);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.Status), "狀態", 100);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.PhotoCompletion), "照片完整度", 100);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AssetGridRow.Remarks), "備註", 220);
            FixedAsset313GridHelper.SetDateColumn(gvData, nameof(AssetGridRow.AcquireDate), "取得日期", 110);
            FixedAsset313GridHelper.SetDateColumn(gvData, nameof(AssetGridRow.LastMonthlyCheckDate), "上次月檢", 110);
            FixedAsset313GridHelper.SetDateColumn(gvData, nameof(AssetGridRow.LastQuarterlyAuditDate), "上次季檢", 110);

            gvData.BestFitColumns();
        }

        private AssetGridRow GetFocusedRow()
        {
            return gvData.GetFocusedRow() as AssetGridRow;
        }

        private void OpenAssetInfo(EventFormInfo eventInfo)
        {
            dt313_FixedAsset source = null;
            var row = GetFocusedRow();

            if (eventInfo != EventFormInfo.Create)
            {
                if (row == null)
                {
                    return;
                }

                if (!module.CanEditAsset(row.Entity))
                {
                    MsgTP.MsgNoPermission();
                    return;
                }

                source = module.CloneAsset(row.Entity);
            }

            using (var form = new f313_FixedAsset_Info(module, source))
            {
                form.eventInfo = eventInfo;
                form.formName = "固定資產";
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenAssetInfo(EventFormInfo.Create);
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenAssetInfo(EventFormInfo.View);
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenAssetInfo(EventFormInfo.Delete);
        }

        private void btnPhotos_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            if (!module.CanEditAsset(row.Entity))
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new f313_AssetPhoto_Info(module, row.Entity))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnImportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!module.IsManager313)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            if (module.ImportAssetsFromExcel())
            {
                LoadData();
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            module.ExportGrid(gcData, "FixedAssetList");
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || view.FocusedRowHandle < 0)
            {
                return;
            }

            OpenAssetInfo(EventFormInfo.View);
        }
    }
}
