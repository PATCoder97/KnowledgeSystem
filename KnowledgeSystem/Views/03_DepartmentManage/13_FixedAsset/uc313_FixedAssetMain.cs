using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    public partial class uc313_FixedAssetMain : XtraUserControl
    {
        private const string GroupManagerName = "固定資產【管理】";
        private const string GroupHandlerName = "固定資產【經辦】";

        private const string BatchTypeMonthly = "Monthly";
        private const string BatchTypeQuarterly = "Quarterly";

        private const string ResultPending = "Pending";
        private const string ResultNormal = "Normal";
        private const string ResultAbnormal = "Abnormal";

        private const string CorrectionOpen = "Open";
        private const string CorrectionClosed = "Closed";
        private const string CorrectionOverdue = "Overdue";

        private const string PhotoTypeCloseUp = "CloseUp";
        private const string PhotoTypeOverview = "Overview";
        private const string PhotoTypeInUse = "InUse";

        private const string InspectionPhotoPurposeAbnormal = "Abnormal";
        private const string InspectionPhotoPurposeCorrection = "Correction";

        private readonly BindingSource assetSource = new BindingSource();
        private readonly BindingSource batchSource = new BindingSource();
        private readonly BindingSource batchDetailSource = new BindingSource();
        private readonly BindingSource abnormalSource = new BindingSource();
        private readonly BindingSource deptSettingSource = new BindingSource();
        private readonly BindingSource abnormalCatalogSource = new BindingSource();

        private LabelControl lblTitle;
        private LabelControl lblScope;
        private TabControl tabMain;

        private GridControl gcAssets;
        private GridView gvAssets;
        private SimpleButton btnAssetReload;
        private SimpleButton btnAssetAdd;
        private SimpleButton btnAssetEdit;
        private SimpleButton btnAssetDelete;
        private SimpleButton btnAssetImport;
        private SimpleButton btnAssetExport;
        private SimpleButton btnAssetPhotos;

        private GridControl gcBatches;
        private GridView gvBatches;
        private GridControl gcBatchDetails;
        private GridView gvBatchDetails;
        private SimpleButton btnBatchReload;
        private SimpleButton btnCreateMonthlyBatch;
        private SimpleButton btnCreateQuarterlyBatch;
        private SimpleButton btnEditBatchResult;
        private SimpleButton btnCloseBatch;
        private SimpleButton btnBatchExport;

        private GridControl gcAbnormals;
        private GridView gvAbnormals;
        private SimpleButton btnAbnormalReload;
        private SimpleButton btnAbnormalHandle;
        private SimpleButton btnAbnormalExport;

        private GridControl gcDeptSettings;
        private GridView gvDeptSettings;
        private GridControl gcAbnormalCatalogs;
        private GridView gvAbnormalCatalogs;
        private SimpleButton btnSettingReload;
        private SimpleButton btnDeptSettingAdd;
        private SimpleButton btnDeptSettingEdit;
        private SimpleButton btnCatalogAdd;
        private SimpleButton btnCatalogEdit;

        public uc313_FixedAssetMain()
        {
            InitializeComponent();
            BuildLayout();
            Load += uc313_FixedAssetMain_Load;
        }

        private void uc313_FixedAssetMain_Load(object sender, EventArgs e)
        {
            InitializeModule();
        }

        private void BuildLayout()
        {
            SuspendLayout();

            Dock = DockStyle.Fill;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblTitle = new LabelControl
            {
                Text = "313 Fixed Asset",
                Appearance =
                {
                    Font = new Font("Microsoft JhengHei UI", 16F, FontStyle.Bold),
                    ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question
                },
                AutoSizeMode = LabelAutoSizeMode.None,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 6)
            };

            lblScope = new LabelControl
            {
                Text = "",
                Appearance =
                {
                    Font = new Font("Microsoft JhengHei UI", 10.5F),
                    ForeColor = Color.DimGray
                },
                AutoSizeMode = LabelAutoSizeMode.None,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 8)
            };

            tabMain = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft JhengHei UI", 10.5F)
            };

            tabMain.TabPages.Add(CreateAssetPage());
            tabMain.TabPages.Add(CreateBatchPage());
            tabMain.TabPages.Add(CreateAbnormalPage());
            tabMain.TabPages.Add(CreateSettingPage());

            root.Controls.Add(lblTitle, 0, 0);
            root.Controls.Add(lblScope, 0, 1);
            root.Controls.Add(tabMain, 0, 2);

            Controls.Add(root);
            ResumeLayout();
        }

        private TabPage CreateAssetPage()
        {
            var page = new TabPage("資產主檔");
            var layout = CreatePageLayout();

            btnAssetReload = CreateButton("重新整理", TPSvgimages.Reload, BtnAssetReload_Click);
            btnAssetAdd = CreateButton("新增資產", TPSvgimages.Add, BtnAssetAdd_Click);
            btnAssetEdit = CreateButton("編輯資料", TPSvgimages.Edit, BtnAssetEdit_Click);
            btnAssetDelete = CreateButton("刪除資產", TPSvgimages.Remove, BtnAssetDelete_Click);
            btnAssetImport = CreateButton("匯入 Excel", TPSvgimages.UploadFile, BtnAssetImport_Click);
            btnAssetExport = CreateButton("匯出 Excel", TPSvgimages.Excel, BtnAssetExport_Click);
            btnAssetPhotos = CreateButton("照片管理", TPSvgimages.View, BtnAssetPhotos_Click);

            gcAssets = CreateGrid(out gvAssets);
            gcAssets.DataSource = assetSource;
            gvAssets.DoubleClick += GvAssets_DoubleClick;

            layout.Controls.Add(CreateActionPanel(btnAssetReload, btnAssetAdd, btnAssetEdit, btnAssetDelete, btnAssetImport, btnAssetExport, btnAssetPhotos), 0, 0);
            layout.Controls.Add(gcAssets, 0, 1);
            page.Controls.Add(layout);
            return page;
        }

        private TabPage CreateBatchPage()
        {
            var page = new TabPage("檢查批次");
            var layout = CreatePageLayout();

            btnBatchReload = CreateButton("重新整理", TPSvgimages.Reload, BtnBatchReload_Click);
            btnCreateMonthlyBatch = CreateButton("建立月檢批次", TPSvgimages.DateAdd, BtnCreateMonthlyBatch_Click);
            btnCreateQuarterlyBatch = CreateButton("建立季檢批次", TPSvgimages.Schedule, BtnCreateQuarterlyBatch_Click);
            btnEditBatchResult = CreateButton("更新檢查結果", TPSvgimages.Edit, BtnEditBatchResult_Click);
            btnCloseBatch = CreateButton("結案批次", TPSvgimages.Confirm, BtnCloseBatch_Click);
            btnBatchExport = CreateButton("匯出明細", TPSvgimages.Excel, BtnBatchExport_Click);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 260
            };

            gcBatches = CreateGrid(out gvBatches);
            gcBatches.DataSource = batchSource;
            gvBatches.FocusedRowChanged += GvBatches_FocusedRowChanged;
            gvBatches.DoubleClick += GvBatches_DoubleClick;

            gcBatchDetails = CreateGrid(out gvBatchDetails);
            gcBatchDetails.DataSource = batchDetailSource;
            gvBatchDetails.DoubleClick += GvBatchDetails_DoubleClick;

            split.Panel1.Controls.Add(gcBatches);
            split.Panel2.Controls.Add(gcBatchDetails);

            layout.Controls.Add(CreateActionPanel(btnBatchReload, btnCreateMonthlyBatch, btnCreateQuarterlyBatch, btnEditBatchResult, btnCloseBatch, btnBatchExport), 0, 0);
            layout.Controls.Add(split, 0, 1);
            page.Controls.Add(layout);
            return page;
        }

        private TabPage CreateAbnormalPage()
        {
            var page = new TabPage("異常改善");
            var layout = CreatePageLayout();

            btnAbnormalReload = CreateButton("重新整理", TPSvgimages.Reload, BtnAbnormalReload_Click);
            btnAbnormalHandle = CreateButton("更新改善", TPSvgimages.Edit, BtnAbnormalHandle_Click);
            btnAbnormalExport = CreateButton("匯出清單", TPSvgimages.Excel, BtnAbnormalExport_Click);

            gcAbnormals = CreateGrid(out gvAbnormals);
            gcAbnormals.DataSource = abnormalSource;
            gvAbnormals.DoubleClick += GvAbnormals_DoubleClick;

            layout.Controls.Add(CreateActionPanel(btnAbnormalReload, btnAbnormalHandle, btnAbnormalExport), 0, 0);
            layout.Controls.Add(gcAbnormals, 0, 1);
            page.Controls.Add(layout);
            return page;
        }

        private TabPage CreateSettingPage()
        {
            var page = new TabPage("設定");
            page.Name = "Settings";
            var layout = CreatePageLayout();

            btnSettingReload = CreateButton("重新整理", TPSvgimages.Reload, BtnSettingReload_Click);
            btnDeptSettingAdd = CreateButton("新增部門設定", TPSvgimages.Add, BtnDeptSettingAdd_Click);
            btnDeptSettingEdit = CreateButton("編輯部門設定", TPSvgimages.Edit, BtnDeptSettingEdit_Click);
            btnCatalogAdd = CreateButton("新增異常項目", TPSvgimages.Add2, BtnCatalogAdd_Click);
            btnCatalogEdit = CreateButton("編輯異常項目", TPSvgimages.Edit, BtnCatalogEdit_Click);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 220
            };

            gcDeptSettings = CreateGrid(out gvDeptSettings);
            gcDeptSettings.DataSource = deptSettingSource;
            gvDeptSettings.DoubleClick += GvDeptSettings_DoubleClick;

            gcAbnormalCatalogs = CreateGrid(out gvAbnormalCatalogs);
            gcAbnormalCatalogs.DataSource = abnormalCatalogSource;
            gvAbnormalCatalogs.DoubleClick += GvAbnormalCatalogs_DoubleClick;

            split.Panel1.Controls.Add(gcDeptSettings);
            split.Panel2.Controls.Add(gcAbnormalCatalogs);

            layout.Controls.Add(CreateActionPanel(btnSettingReload, btnDeptSettingAdd, btnDeptSettingEdit, btnCatalogAdd, btnCatalogEdit), 0, 0);
            layout.Controls.Add(split, 0, 1);
            page.Controls.Add(layout);
            return page;
        }

        private TableLayoutPanel CreatePageLayout()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(4)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            return layout;
        }

        private SimpleButton CreateButton(string text, SvgImage icon, EventHandler clickEvent)
        {
            var button = new SimpleButton
            {
                Text = text,
                Height = 34,
                MinimumSize = new Size(120, 34),
                Appearance =
                {
                    Font = new Font("Microsoft JhengHei UI", 10.5F)
                },
                Margin = new Padding(0, 0, 8, 8)
            };

            if (icon != null)
            {
                button.ImageOptions.SvgImage = icon;
                button.ImageOptions.SvgImageSize = new Size(18, 18);
            }

            button.Click += clickEvent;
            return button;
        }

        private PanelControl CreateActionPanel(params Control[] controls)
        {
            var panel = new PanelControl
            {
                Dock = DockStyle.Fill,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder,
                Padding = new Padding(0, 0, 0, 6),
                Height = 50
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true
            };

            flow.Controls.AddRange(controls);
            panel.Controls.Add(flow);
            return panel;
        }

        private GridControl CreateGrid(out GridView view)
        {
            var grid = new GridControl
            {
                Dock = DockStyle.Fill
            };

            view = new GridView(grid);
            grid.MainView = view;
            grid.ViewCollection.Add(view);

            view.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Regular);
            view.Appearance.HeaderPanel.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question;
            view.Appearance.HeaderPanel.Options.UseFont = true;
            view.Appearance.HeaderPanel.Options.UseForeColor = true;
            view.Appearance.HeaderPanel.Options.UseTextOptions = true;
            view.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
            view.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 10.5F, FontStyle.Regular);
            view.Appearance.Row.Options.UseFont = true;
            view.OptionsBehavior.Editable = false;
            view.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.True;
            view.OptionsSelection.MultiSelect = false;
            view.OptionsView.ShowAutoFilterRow = true;
            view.OptionsView.EnableAppearanceOddRow = true;
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.ColumnAutoWidth = false;
            view.OptionsCustomization.AllowGroup = false;
            view.OptionsCustomization.AllowColumnMoving = false;
            view.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            view.ReadOnlyGridView();

            return grid;
        }
    }
}
