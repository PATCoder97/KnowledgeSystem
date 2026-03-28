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
    public partial class uc313_InspectionBatch : XtraUserControl
    {
        private readonly FixedAsset313Context module = new FixedAsset313Context();
        private readonly BindingSource sourceBases = new BindingSource();
        private readonly RefreshHelper helper;
        private DXMenuItem itemViewInfo;

        public uc313_InspectionBatch()
        {
            InitializeComponent();
            InitializeMenuItems();
            helper = new RefreshHelper(gvData, nameof(BatchGridRow.BatchName));
            Load += uc313_InspectionBatch_Load;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = FixedAsset313UIHelper.CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
        }

        private void uc313_InspectionBatch_Load(object sender, EventArgs e)
        {
            FixedAsset313GridHelper.ConfigureReadOnlyView(gvData);
            FixedAsset313GridHelper.ConfigureReadOnlyView(gvAsset);
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.DoubleClick += gvData_DoubleClick;
            gvAsset.DoubleClick += gvAsset_DoubleClick;
            gvAsset.PopupMenuShowing += gvAsset_PopupMenuShowing;
            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();
                module.Initialize();

                sourceBases.DataSource = module.BuildBatchRows();
                gcData.DataSource = sourceBases;

                gvData.PopulateColumns();
                gvAsset.PopulateColumns();
                ConfigureMainColumns();
                ConfigureDetailColumns();
                ApplyPermissions();
                helper.LoadViewInfo();
            }
        }

        private void ApplyPermissions()
        {
            var visible = module.IsManager313 ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnCreateMonthly.Visibility = visible;
            btnCreateQuarterly.Visibility = visible;
            btnCloseBatch.Visibility = visible;
            btnEditResult.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private void ConfigureMainColumns()
        {
            FixedAsset313GridHelper.HideColumn(gvData, nameof(BatchGridRow.Entity));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(BatchGridRow.BatchType));
            FixedAsset313GridHelper.HideColumn(gvData, nameof(BatchGridRow.Details));

            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.BatchName), "批次名稱", 240);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.BatchTypeDisplay), "類型", 100);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.PeriodKey), "期間", 90);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.DeptName), "部門", 150);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.AssignedUserName), "經辦", 150);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.SampleRate), "抽樣%", 70);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.TargetQty), "目標數", 70);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.CompletedQty), "已檢數", 70);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.AbnormalQty), "異常數", 70);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.ProgressText), "進度", 90);
            FixedAsset313GridHelper.SetColumn(gvData, nameof(BatchGridRow.Status), "狀態", 80);
            FixedAsset313GridHelper.SetDateColumn(gvData, nameof(BatchGridRow.CreatedDate), "建立日期", 130, "yyyy-MM-dd HH:mm");
            FixedAsset313GridHelper.SetDateColumn(gvData, nameof(BatchGridRow.ClosedDate), "結案日期", 130, "yyyy-MM-dd HH:mm");
            gvData.BestFitColumns();
        }

        private void ConfigureDetailColumns()
        {
            FixedAsset313GridHelper.HideColumn(gvAsset, nameof(BatchDetailGridRow.Entity));
            FixedAsset313GridHelper.HideColumn(gvAsset, nameof(BatchDetailGridRow.Batch));
            FixedAsset313GridHelper.HideColumn(gvAsset, nameof(BatchDetailGridRow.Asset));
            FixedAsset313GridHelper.HideColumn(gvAsset, nameof(BatchDetailGridRow.Result));
            FixedAsset313GridHelper.HideColumn(gvAsset, nameof(BatchDetailGridRow.CorrectionStatus));

            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.AssetCode), "資產編號", 120);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.AssetNameTW), "資產名稱", 180);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.DeptName), "部門", 140);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.ManagerName), "經辦", 140);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.ResultDisplay), "結果", 90);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.AbnormalName), "異常項目", 140);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.AbnormalNote), "異常說明", 180);
            FixedAsset313GridHelper.SetDateColumn(gvAsset, nameof(BatchDetailGridRow.CheckedDate), "檢查時間", 130, "yyyy-MM-dd HH:mm");
            FixedAsset313GridHelper.SetDateColumn(gvAsset, nameof(BatchDetailGridRow.CorrectionDueDate), "改善期限", 100);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.CorrectionStatusDisplay), "改善狀態", 100);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.CorrectionNote), "改善說明", 180);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.EvidencePhotoCount), "異常照片", 80);
            FixedAsset313GridHelper.SetColumn(gvAsset, nameof(BatchDetailGridRow.CorrectionPhotoCount), "改善照片", 80);
            gvAsset.BestFitColumns();
        }

        private BatchGridRow GetFocusedBatch()
        {
            return gvData.GetFocusedRow() as BatchGridRow;
        }

        private BatchDetailGridRow GetFocusedDetail()
        {
            return gvAsset.GetFocusedRow() as BatchDetailGridRow;
        }

        private void EditFocusedDetail()
        {
            var row = GetFocusedDetail();
            if (row == null)
            {
                return;
            }

            bool allowResultEdit = module.CanEditBatchResult(row);
            bool allowCorrectionEdit = module.CanUpdateCorrection(row);
            if (!allowResultEdit && !allowCorrectionEdit)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new f313_InspectionResult_Info(module, row, allowResultEdit, allowCorrectionEdit))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnCreateMonthly_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var form = f313_BatchCreate.CreateMonthly(module))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK && module.CreateMonthlyBatch(form.ResultData))
                {
                    LoadData();
                }
            }
        }

        private void btnCreateQuarterly_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var form = f313_BatchCreate.CreateQuarterly(module))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK && module.CreateQuarterlyBatch(form.ResultData))
                {
                    LoadData();
                }
            }
        }

        private void btnEditResult_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EditFocusedDetail();
        }

        private void btnCloseBatch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var row = GetFocusedBatch();
            if (!module.IsManager313)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            if (module.CloseBatch(row))
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
            module.ExportGrid(gcData, "FixedAssetBatch");
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "Details";
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            var row = gvData.GetRow(e.RowHandle) as BatchGridRow;
            e.ChildList = row?.Details;
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;
            detailView?.BestFitColumns();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            int rowHandle = gvData.FocusedRowHandle;
            if (rowHandle >= 0 && gvData.IsMasterRow(rowHandle))
            {
                if (gvData.GetMasterRowExpanded(rowHandle))
                {
                    gvData.CollapseMasterRow(rowHandle);
                }
                else
                {
                    gvData.ExpandMasterRow(rowHandle);
                }
            }
        }

        private void gvAsset_DoubleClick(object sender, EventArgs e)
        {
            EditFocusedDetail();
        }

        private void gvAsset_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && e.Menu != null)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            EditFocusedDetail();
        }
    }
}
