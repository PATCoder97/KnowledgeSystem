using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    public partial class uc313_AbnormalTrack : XtraUserControl
    {
        private readonly FixedAsset313Context module = new FixedAsset313Context();
        private readonly BindingSource sourceBases = new BindingSource();
        private readonly RefreshHelper helper;
        private DXMenuItem itemViewInfo;

        public uc313_AbnormalTrack()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
            FixedAsset313UIHelper.ApplyUserControlStyle(this, barManagerTP, bar2);
            helper = new RefreshHelper(gvData, nameof(AbnormalGridRow.AssetCode));
            Load += uc313_AbnormalTrack_Load;
        }

        private void InitializeIcon()
        {
            btnHandle.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = FixedAsset313UIHelper.CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
        }

        private void uc313_AbnormalTrack_Load(object sender, EventArgs e)
        {
            FixedAsset313GridHelper.ConfigureReadOnlyView(gvData);
            gvData.DoubleClick += gvData_DoubleClick;
            gvData.PopupMenuShowing += gvData_PopupMenuShowing;
            LoadData();
            btnHandle.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();
                module.Initialize();
                sourceBases.DataSource = module.BuildAbnormalRows();
                gcData.DataSource = sourceBases;
                gvData.PopulateColumns();
                ConfigureColumns();
                helper.LoadViewInfo();
            }
        }

        private void ConfigureColumns()
        {
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AbnormalGridRow.Entity));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AbnormalGridRow.Batch));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AbnormalGridRow.Asset));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(AbnormalGridRow.CorrectionStatus));

            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.BatchName), "批次名稱", 220);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.BatchTypeDisplay), "批次類型", 110);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.PeriodKey), "期間", 90);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.AssetCode), "資產編號", 120);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.AssetNameTW), "資產名稱", 180);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.DeptName), "部門", 140);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.ManagerName), "經辦", 140);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.AbnormalName), "異常項目", 140);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.AbnormalNote), "異常說明", 180);
            FixedAsset313GridHelper.SetDateColumn(gvData, nameof(AbnormalGridRow.CheckedDate), "檢查時間", 130, "yyyy-MM-dd HH:mm");
            FixedAsset313GridHelper.SetDateColumn(gvData, nameof(AbnormalGridRow.CorrectionDueDate), "改善期限", 100);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.CorrectionStatusDisplay), "改善狀態", 100);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(AbnormalGridRow.CorrectionNote), "改善說明", 180);
            gvData.BestFitColumns();
        }

        private AbnormalGridRow GetFocusedRow()
        {
            return gvData.GetFocusedRow() as AbnormalGridRow;
        }

        private void HandleFocusedRow()
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            var detailRow = new BatchDetailGridRow
            {
                Entity = row.Entity,
                Batch = row.Batch,
                Asset = row.Asset
            };

            bool allowResultEdit = row.Batch.Status != "Closed" && module.CanEditBatchResult(detailRow);
            bool allowCorrectionEdit = module.CanUpdateCorrection(row);
            if (!allowResultEdit && !allowCorrectionEdit)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new f313_InspectionResult_Info(module, detailRow, allowResultEdit, allowCorrectionEdit))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnHandle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            HandleFocusedRow();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            module.ExportGrid(gcData, "FixedAssetAbnormal");
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            HandleFocusedRow();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && e.Menu != null)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            HandleFocusedRow();
        }
    }
}
