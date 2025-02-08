namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    partial class uc308_HealthCheckMain
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc308_HealthCheckMain));
            this.gvSession = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColIdDetail = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColIdSession = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.gvDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.btnFilter = new DevExpress.XtraBars.BarSubItem();
            this.btnValidCert = new DevExpress.XtraBars.BarButtonItem();
            this.btnBackCert = new DevExpress.XtraBars.BarButtonItem();
            this.btnInvalidCert = new DevExpress.XtraBars.BarButtonItem();
            this.btnWaitCert = new DevExpress.XtraBars.BarButtonItem();
            this.btnExpCert = new DevExpress.XtraBars.BarButtonItem();
            this.btnClearFilter = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.btnSpecialFunctions = new DevExpress.XtraBars.BarSubItem();
            this.btnInvalidateExpCert = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.gvSession)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // gvSession
            // 
            this.gvSession.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvSession.Appearance.HeaderPanel.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question;
            this.gvSession.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvSession.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvSession.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvSession.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvSession.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvSession.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvSession.Appearance.Row.Options.UseFont = true;
            this.gvSession.Appearance.Row.Options.UseForeColor = true;
            this.gvSession.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColIdDetail,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9});
            this.gvSession.GridControl = this.gcData;
            this.gvSession.Name = "gvSession";
            this.gvSession.OptionsDetail.ShowDetailTabs = false;
            this.gvSession.OptionsView.ColumnAutoWidth = false;
            this.gvSession.OptionsView.EnableAppearanceOddRow = true;
            this.gvSession.OptionsView.ShowAutoFilterRow = true;
            this.gvSession.OptionsView.ShowGroupPanel = false;
            this.gvSession.MasterRowEmpty += new DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventHandler(this.gvSession_MasterRowEmpty);
            this.gvSession.MasterRowExpanded += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gv_MasterRowExpanded);
            this.gvSession.MasterRowGetChildList += new DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventHandler(this.gvSession_MasterRowGetChildList);
            this.gvSession.MasterRowGetRelationName += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventHandler(this.gvSession_MasterRowGetRelationName);
            this.gvSession.MasterRowGetRelationCount += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventHandler(this.gvSession_MasterRowGetRelationCount);
            this.gvSession.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvSession_PopupMenuShowing);
            // 
            // gColIdDetail
            // 
            this.gColIdDetail.Caption = "Id";
            this.gColIdDetail.FieldName = "Id";
            this.gColIdDetail.Name = "gColIdDetail";
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "人員姓名";
            this.gridColumn5.FieldName = "EmpName";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 0;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "健康評級";
            this.gridColumn6.FieldName = "HealthRating";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 1;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "一般疾病";
            this.gridColumn7.FieldName = "Disease1";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 2;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "慢性病";
            this.gridColumn8.FieldName = "Disease2";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 3;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "得職業病";
            this.gridColumn9.FieldName = "Disease3";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 4;
            // 
            // gcData
            // 
            this.gcData.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Remove.Visible = false;
            gridLevelNode1.LevelTemplate = this.gvSession;
            gridLevelNode2.LevelTemplate = this.gvDetail;
            gridLevelNode2.RelationName = "CheckDetail";
            gridLevelNode1.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode2});
            gridLevelNode1.RelationName = "CheckSession";
            this.gcData.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.gcData.Location = new System.Drawing.Point(12, 12);
            this.gcData.MainView = this.gvData;
            this.gcData.Name = "gcData";
            this.gcData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEdit1});
            this.gcData.Size = new System.Drawing.Size(815, 500);
            this.gcData.TabIndex = 4;
            this.gcData.UseEmbeddedNavigator = true;
            this.gcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvData,
            this.gvDetail,
            this.gvSession});
            this.gcData.DoubleClick += new System.EventHandler(this.gcData_DoubleClick);
            // 
            // gvData
            // 
            this.gvData.Appearance.FooterPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvData.Appearance.FooterPanel.Options.UseFont = true;
            this.gvData.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gvData.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvData.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvData.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvData.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvData.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvData.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvData.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvData.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvData.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvData.Appearance.Row.Options.UseFont = true;
            this.gvData.Appearance.Row.Options.UseForeColor = true;
            this.gvData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColIdSession,
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4});
            this.gvData.GridControl = this.gcData;
            this.gvData.Name = "gvData";
            this.gvData.OptionsDetail.ShowDetailTabs = false;
            this.gvData.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvData.OptionsView.ColumnAutoWidth = false;
            this.gvData.OptionsView.EnableAppearanceOddRow = true;
            this.gvData.OptionsView.ShowAutoFilterRow = true;
            this.gvData.OptionsView.ShowGroupPanel = false;
            this.gvData.MasterRowEmpty += new DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventHandler(this.gvData_MasterRowEmpty);
            this.gvData.MasterRowExpanded += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gv_MasterRowExpanded);
            this.gvData.MasterRowGetChildList += new DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventHandler(this.gvData_MasterRowGetChildList);
            this.gvData.MasterRowGetRelationName += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventHandler(this.gvData_MasterRowGetRelationName);
            this.gvData.MasterRowGetRelationCount += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventHandler(this.gvData_MasterRowGetRelationCount);
            this.gvData.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvData_PopupMenuShowing);
            // 
            // gColIdSession
            // 
            this.gColIdSession.Caption = "ID";
            this.gColIdSession.FieldName = "Id";
            this.gColIdSession.Name = "gColIdSession";
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "越文名稱";
            this.gridColumn1.FieldName = "DisplayNameVN";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "中文名稱";
            this.gridColumn2.FieldName = "DisplayNameTW";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "創建日";
            this.gridColumn3.FieldName = "DateSession";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "檢查類別";
            this.gridColumn4.FieldName = "CheckType";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            // 
            // gvDetail
            // 
            this.gvDetail.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvDetail.Appearance.HeaderPanel.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Information;
            this.gvDetail.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvDetail.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvDetail.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvDetail.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvDetail.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvDetail.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvDetail.Appearance.Row.Options.UseFont = true;
            this.gvDetail.Appearance.Row.Options.UseForeColor = true;
            this.gvDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12});
            this.gvDetail.GridControl = this.gcData;
            this.gvDetail.Name = "gvDetail";
            this.gvDetail.OptionsCustomization.AllowGroup = false;
            this.gvDetail.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvDetail.OptionsCustomization.AllowSort = false;
            this.gvDetail.OptionsView.AllowCellMerge = true;
            this.gvDetail.OptionsView.ColumnAutoWidth = false;
            this.gvDetail.OptionsView.EnableAppearanceOddRow = true;
            this.gvDetail.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn10
            // 
            this.gridColumn10.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn10.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumn10.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
            this.gridColumn10.Caption = "疾病";
            this.gridColumn10.ColumnEdit = this.repositoryItemMemoEdit1;
            this.gridColumn10.FieldName = "DiseaseTypeName";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 0;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "越文名稱";
            this.gridColumn11.FieldName = "DisplayNameVN";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 1;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "中文名稱";
            this.gridColumn12.FieldName = "DisplayNameTW";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 2;
            // 
            // barManagerTP
            // 
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnAdd,
            this.btnReload,
            this.btnExportExcel,
            this.btnFilter,
            this.btnValidCert,
            this.btnBackCert,
            this.btnInvalidCert,
            this.btnWaitCert,
            this.btnExpCert,
            this.btnClearFilter,
            this.btnSpecialFunctions,
            this.btnInvalidateExpCert});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 14;
            // 
            // bar2
            // 
            this.bar2.BarAppearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.bar2.BarAppearance.Disabled.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Hovered.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Options.UseForeColor = true;
            this.bar2.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Normal.Options.UseFont = true;
            this.bar2.BarAppearance.Normal.Options.UseForeColor = true;
            this.bar2.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Pressed.Options.UseFont = true;
            this.bar2.BarAppearance.Pressed.Options.UseForeColor = true;
            this.bar2.BarName = "Main menu";
            this.bar2.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnAdd, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnReload, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnFilter, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnExportExcel, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnSpecialFunctions, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnAdd
            // 
            this.btnAdd.Caption = "新增";
            this.btnAdd.Id = 0;
            this.btnAdd.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnAdd.ImageOptions.SvgImage")));
            this.btnAdd.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnAdd.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnAdd.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnAdd.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnAdd.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnAdd.ItemAppearance.Normal.Options.UseFont = true;
            this.btnAdd.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAdd_ItemClick);
            // 
            // btnReload
            // 
            this.btnReload.Caption = "刷新";
            this.btnReload.Id = 1;
            this.btnReload.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnReload.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnReload.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnReload.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnReload.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnReload.ItemAppearance.Normal.Options.UseFont = true;
            this.btnReload.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnReload.Name = "btnReload";
            this.btnReload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnReload_ItemClick);
            // 
            // btnFilter
            // 
            this.btnFilter.Caption = "篩選";
            this.btnFilter.Id = 4;
            this.btnFilter.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnFilter.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnFilter.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnFilter.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnFilter.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnFilter.ItemAppearance.Normal.Options.UseFont = true;
            this.btnFilter.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnFilter.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnValidCert),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnBackCert),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnInvalidCert),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnWaitCert),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnExpCert),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnClearFilter)});
            this.btnFilter.Name = "btnFilter";
            // 
            // btnValidCert
            // 
            this.btnValidCert.Caption = "應取證照";
            this.btnValidCert.Id = 5;
            this.btnValidCert.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnValidCert.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnValidCert.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnValidCert.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnValidCert.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnValidCert.ItemAppearance.Normal.Options.UseFont = true;
            this.btnValidCert.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnValidCert.Name = "btnValidCert";
            // 
            // btnBackCert
            // 
            this.btnBackCert.Caption = "備援證照";
            this.btnBackCert.Id = 6;
            this.btnBackCert.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnBackCert.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnBackCert.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnBackCert.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnBackCert.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnBackCert.ItemAppearance.Normal.Options.UseFont = true;
            this.btnBackCert.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnBackCert.Name = "btnBackCert";
            // 
            // btnInvalidCert
            // 
            this.btnInvalidCert.Caption = "無效證照";
            this.btnInvalidCert.Id = 7;
            this.btnInvalidCert.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnInvalidCert.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnInvalidCert.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnInvalidCert.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnInvalidCert.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnInvalidCert.ItemAppearance.Normal.Options.UseFont = true;
            this.btnInvalidCert.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnInvalidCert.Name = "btnInvalidCert";
            // 
            // btnWaitCert
            // 
            this.btnWaitCert.Caption = "在等證照";
            this.btnWaitCert.Id = 8;
            this.btnWaitCert.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnWaitCert.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnWaitCert.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnWaitCert.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnWaitCert.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnWaitCert.ItemAppearance.Normal.Options.UseFont = true;
            this.btnWaitCert.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnWaitCert.Name = "btnWaitCert";
            // 
            // btnExpCert
            // 
            this.btnExpCert.Caption = "過期證照";
            this.btnExpCert.Id = 9;
            this.btnExpCert.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExpCert.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExpCert.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExpCert.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnExpCert.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnExpCert.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExpCert.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnExpCert.Name = "btnExpCert";
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.Caption = "清除篩選";
            this.btnClearFilter.Id = 10;
            this.btnClearFilter.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnClearFilter.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnClearFilter.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnClearFilter.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnClearFilter.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnClearFilter.ItemAppearance.Normal.Options.UseFont = true;
            this.btnClearFilter.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnClearFilter.Name = "btnClearFilter";
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Caption = "出表";
            this.btnExportExcel.Id = 2;
            this.btnExportExcel.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExportExcel.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExportExcel.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExportExcel.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnExportExcel.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnExportExcel.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExportExcel.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnExportExcel.Name = "btnExportExcel";
            // 
            // btnSpecialFunctions
            // 
            this.btnSpecialFunctions.Caption = "特殊功能";
            this.btnSpecialFunctions.Id = 12;
            this.btnSpecialFunctions.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnSpecialFunctions.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnSpecialFunctions.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnSpecialFunctions.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnSpecialFunctions.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnSpecialFunctions.ItemAppearance.Normal.Options.UseFont = true;
            this.btnSpecialFunctions.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnSpecialFunctions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnInvalidateExpCert)});
            this.btnSpecialFunctions.Name = "btnSpecialFunctions";
            // 
            // btnInvalidateExpCert
            // 
            this.btnInvalidateExpCert.Caption = "作廢過期證照";
            this.btnInvalidateExpCert.Id = 13;
            this.btnInvalidateExpCert.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnInvalidateExpCert.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnInvalidateExpCert.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnInvalidateExpCert.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnInvalidateExpCert.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnInvalidateExpCert.ItemAppearance.Normal.Options.UseFont = true;
            this.btnInvalidateExpCert.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnInvalidateExpCert.Name = "btnInvalidateExpCert";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(839, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 573);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(839, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 524);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(839, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 524);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(839, 524);
            this.layoutControl1.TabIndex = 5;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(839, 524);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(819, 504);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // uc308_HealthCheckMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "uc308_HealthCheckMain";
            this.Size = new System.Drawing.Size(839, 573);
            this.Load += new System.EventHandler(this.uc308_HealthCheckMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvSession)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnAdd;
        private DevExpress.XtraBars.BarButtonItem btnReload;
        private DevExpress.XtraBars.BarSubItem btnFilter;
        private DevExpress.XtraBars.BarButtonItem btnValidCert;
        private DevExpress.XtraBars.BarButtonItem btnBackCert;
        private DevExpress.XtraBars.BarButtonItem btnInvalidCert;
        private DevExpress.XtraBars.BarButtonItem btnWaitCert;
        private DevExpress.XtraBars.BarButtonItem btnExpCert;
        private DevExpress.XtraBars.BarButtonItem btnClearFilter;
        private DevExpress.XtraBars.BarButtonItem btnExportExcel;
        private DevExpress.XtraBars.BarSubItem btnSpecialFunctions;
        private DevExpress.XtraBars.BarButtonItem btnInvalidateExpCert;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData;
        private DevExpress.XtraGrid.Columns.GridColumn gColIdSession;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Views.Grid.GridView gvSession;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDetail;
        private DevExpress.XtraGrid.Columns.GridColumn gColIdDetail;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
    }
}
