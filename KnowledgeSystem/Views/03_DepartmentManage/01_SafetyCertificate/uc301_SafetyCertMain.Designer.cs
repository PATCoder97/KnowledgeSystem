﻿namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    partial class uc301_SafetyCertMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc301_SafetyCertMain));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.btnFilter = new DevExpress.XtraBars.BarSubItem();
            this.btnValidCert = new DevExpress.XtraBars.BarButtonItem();
            this.btnBackCert = new DevExpress.XtraBars.BarButtonItem();
            this.btnInvalidCert = new DevExpress.XtraBars.BarButtonItem();
            this.btnWaitCert = new DevExpress.XtraBars.BarButtonItem();
            this.subBtnExp = new DevExpress.XtraBars.BarSubItem();
            this.btnExpQuater = new DevExpress.XtraBars.BarButtonItem();
            this.btnExpHaflYear = new DevExpress.XtraBars.BarButtonItem();
            this.btnExpYear = new DevExpress.XtraBars.BarButtonItem();
            this.btnClearFilter = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.btnSpecialFunctions = new DevExpress.XtraBars.BarSubItem();
            this.btnInvalidateExpCert = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(979, 418);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gcData
            // 
            this.gcData.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Remove.Visible = false;
            this.gcData.Location = new System.Drawing.Point(12, 12);
            this.gcData.MainView = this.gvData;
            this.gcData.Name = "gcData";
            this.gcData.Size = new System.Drawing.Size(955, 394);
            this.gcData.TabIndex = 4;
            this.gcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvData});
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
            this.gridColumn11,
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10});
            this.gvData.GridControl = this.gcData;
            this.gvData.Name = "gvData";
            this.gvData.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvData.OptionsView.ColumnAutoWidth = false;
            this.gvData.OptionsView.EnableAppearanceOddRow = true;
            this.gvData.OptionsView.ShowAutoFilterRow = true;
            this.gvData.OptionsView.ShowFooter = true;
            this.gvData.OptionsView.ShowGroupPanel = false;
            this.gvData.DoubleClick += new System.EventHandler(this.gvData_DoubleClick);
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "ID";
            this.gridColumn11.FieldName = "Id";
            this.gridColumn11.Name = "gridColumn11";
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "人員代號";
            this.gridColumn1.FieldName = "IdUser";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "人員名稱";
            this.gridColumn2.FieldName = "UserName";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "職務名稱";
            this.gridColumn3.FieldName = "JobName";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "課程名稱";
            this.gridColumn4.FieldName = "CourseName";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn5.Caption = "上課日期";
            this.gridColumn5.FieldName = "DateReceipt";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "有效期限";
            this.gridColumn6.FieldName = "ExpDate";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "ExpDate", "數量={0}")});
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "應取證照";
            this.gridColumn7.FieldName = "ValidLicense";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom, "ValidLicense", "應取={0}")});
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "備援證照";
            this.gridColumn8.FieldName = "BackupLicense";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom, "BackupLicense", "備援={0}")});
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "無效證照";
            this.gridColumn9.FieldName = "InvalidLicense";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom, "InvalidLicense", "無效={0}")});
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 8;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "備註";
            this.gridColumn10.FieldName = "Describe";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom, "Describe", "共計={0}")});
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 9;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(979, 418);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(959, 398);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
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
            this.btnExpYear,
            this.btnClearFilter,
            this.btnSpecialFunctions,
            this.btnInvalidateExpCert,
            this.btnExpQuater,
            this.btnExpHaflYear,
            this.subBtnExp});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 19;
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
            new DevExpress.XtraBars.LinkPersistInfo(this.subBtnExp),
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
            this.btnValidCert.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SetFilter);
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
            this.btnBackCert.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SetFilter);
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
            this.btnInvalidCert.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SetFilter);
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
            this.btnWaitCert.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SetFilter);
            // 
            // subBtnExp
            // 
            this.subBtnExp.Caption = "過期證照";
            this.subBtnExp.Id = 18;
            this.subBtnExp.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.subBtnExp.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.subBtnExp.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.subBtnExp.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.subBtnExp.ItemAppearance.Normal.Options.UseFont = true;
            this.subBtnExp.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnExpQuater),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnExpHaflYear),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnExpYear)});
            this.subBtnExp.Name = "subBtnExp";
            // 
            // btnExpQuater
            // 
            this.btnExpQuater.Caption = "季";
            this.btnExpQuater.Id = 15;
            this.btnExpQuater.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExpQuater.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExpQuater.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExpQuater.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnExpQuater.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExpQuater.Name = "btnExpQuater";
            this.btnExpQuater.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SetFilter);
            // 
            // btnExpHaflYear
            // 
            this.btnExpHaflYear.Caption = "半年";
            this.btnExpHaflYear.Id = 16;
            this.btnExpHaflYear.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExpHaflYear.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExpHaflYear.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExpHaflYear.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnExpHaflYear.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExpHaflYear.Name = "btnExpHaflYear";
            this.btnExpHaflYear.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SetFilter);
            // 
            // btnExpYear
            // 
            this.btnExpYear.Caption = "年";
            this.btnExpYear.Id = 9;
            this.btnExpYear.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExpYear.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExpYear.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExpYear.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnExpYear.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnExpYear.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExpYear.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnExpYear.Name = "btnExpYear";
            this.btnExpYear.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SetFilter);
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
            this.btnClearFilter.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SetFilter);
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
            this.btnExportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportExcel_ItemClick);
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
            this.btnInvalidateExpCert.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnInvalidateExpCert_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(979, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 467);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(979, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 418);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(979, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 418);
            // 
            // uc301_SafetyCertMain
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "uc301_SafetyCertMain";
            this.Size = new System.Drawing.Size(979, 467);
            this.Load += new System.EventHandler(this.uc301_SafetyCertMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnAdd;
        private DevExpress.XtraBars.BarButtonItem btnReload;
        private DevExpress.XtraBars.BarButtonItem btnExportExcel;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraBars.BarSubItem btnFilter;
        private DevExpress.XtraBars.BarButtonItem btnValidCert;
        private DevExpress.XtraBars.BarButtonItem btnBackCert;
        private DevExpress.XtraBars.BarButtonItem btnInvalidCert;
        private DevExpress.XtraBars.BarButtonItem btnWaitCert;
        private DevExpress.XtraBars.BarButtonItem btnExpYear;
        private DevExpress.XtraBars.BarButtonItem btnClearFilter;
        private DevExpress.XtraBars.BarSubItem btnSpecialFunctions;
        private DevExpress.XtraBars.BarButtonItem btnInvalidateExpCert;
        private DevExpress.XtraBars.BarSubItem subBtnExp;
        private DevExpress.XtraBars.BarButtonItem btnExpQuater;
        private DevExpress.XtraBars.BarButtonItem btnExpHaflYear;
    }
}
