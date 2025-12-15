namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._05_CalipStandardMgmt
{
    partial class uc403_05_CalipStandardMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc403_05_CalipStandardMain));
            this.gvForm = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColIdAttForm = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolUploadDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolConfirmDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolFinishDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcolActualName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColIdAtt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColEnterDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColDeptId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barCbbDepts = new DevExpress.XtraBars.BarEditItem();
            this.cbbDepts = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.gvForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDepts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // gvForm
            // 
            this.gvForm.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvForm.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Green;
            this.gvForm.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvForm.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvForm.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvForm.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvForm.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvForm.Appearance.Row.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Hyperlink;
            this.gvForm.Appearance.Row.Options.UseFont = true;
            this.gvForm.Appearance.Row.Options.UseForeColor = true;
            this.gvForm.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gColIdAttForm,
            this.gcolUploadDate,
            this.gcolConfirmDate,
            this.gcolFinishDate,
            this.gcolActualName});
            this.gvForm.GridControl = this.gcData;
            this.gvForm.Name = "gvForm";
            this.gvForm.OptionsCustomization.AllowFilter = false;
            this.gvForm.OptionsCustomization.AllowGroup = false;
            this.gvForm.OptionsCustomization.AllowSort = false;
            this.gvForm.OptionsView.ColumnAutoWidth = false;
            this.gvForm.OptionsView.ShowGroupPanel = false;
            this.gvForm.OptionsView.ShowIndicator = false;
            this.gvForm.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn1, DevExpress.Data.ColumnSortOrder.Descending)});
            this.gvForm.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvForm_PopupMenuShowing);
            this.gvForm.DoubleClick += new System.EventHandler(this.gvForm_DoubleClick);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumn1";
            this.gridColumn1.FieldName = "Id";
            this.gridColumn1.Name = "gridColumn1";
            // 
            // gColIdAttForm
            // 
            this.gColIdAttForm.Caption = "IdAtt";
            this.gColIdAttForm.FieldName = "AttId";
            this.gColIdAttForm.Name = "gColIdAttForm";
            // 
            // gcolUploadDate
            // 
            this.gcolUploadDate.Caption = "上傳日期";
            this.gcolUploadDate.FieldName = "UploadDate";
            this.gcolUploadDate.Name = "gcolUploadDate";
            this.gcolUploadDate.Visible = true;
            this.gcolUploadDate.VisibleIndex = 0;
            // 
            // gcolConfirmDate
            // 
            this.gcolConfirmDate.Caption = "確認日期";
            this.gcolConfirmDate.FieldName = "ConfirmDate";
            this.gcolConfirmDate.Name = "gcolConfirmDate";
            this.gcolConfirmDate.Visible = true;
            this.gcolConfirmDate.VisibleIndex = 1;
            // 
            // gcolFinishDate
            // 
            this.gcolFinishDate.Caption = "完成日期";
            this.gcolFinishDate.FieldName = "FinishDate";
            this.gcolFinishDate.Name = "gcolFinishDate";
            this.gcolFinishDate.Visible = true;
            this.gcolFinishDate.VisibleIndex = 2;
            // 
            // gcolActualName
            // 
            this.gcolActualName.Caption = "檔案名稱";
            this.gcolActualName.FieldName = "Name";
            this.gcolActualName.Name = "gcolActualName";
            this.gcolActualName.Visible = true;
            this.gcolActualName.VisibleIndex = 3;
            // 
            // gcData
            // 
            this.gcData.Cursor = System.Windows.Forms.Cursors.Default;
            this.gcData.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Remove.Visible = false;
            gridLevelNode1.LevelTemplate = this.gvForm;
            gridLevelNode1.RelationName = "表單";
            this.gcData.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.gcData.Location = new System.Drawing.Point(12, 12);
            this.gcData.MainView = this.gvData;
            this.gcData.Name = "gcData";
            this.gcData.Size = new System.Drawing.Size(1010, 567);
            this.gcData.TabIndex = 5;
            this.gcData.UseEmbeddedNavigator = true;
            this.gcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvData,
            this.gvForm});
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
            this.gColId,
            this.gColIdAtt,
            this.gridColumn14,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn16,
            this.gridColumn9,
            this.gColEnterDate,
            this.gridColumn2,
            this.gridColumn11,
            this.gridColumn10,
            this.gColDeptId});
            this.gvData.GridControl = this.gcData;
            this.gvData.Name = "gvData";
            this.gvData.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Embedded;
            this.gvData.OptionsDetail.ShowDetailTabs = false;
            this.gvData.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvData.OptionsView.ColumnAutoWidth = false;
            this.gvData.OptionsView.EnableAppearanceOddRow = true;
            this.gvData.OptionsView.ShowAutoFilterRow = true;
            this.gvData.OptionsView.ShowGroupPanel = false;
            this.gvData.MasterRowEmpty += new DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventHandler(this.gvData_MasterRowEmpty);
            this.gvData.MasterRowExpanded += new DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventHandler(this.gvData_MasterRowExpanded);
            this.gvData.MasterRowGetChildList += new DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventHandler(this.gvData_MasterRowGetChildList);
            this.gvData.MasterRowGetRelationName += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventHandler(this.gvData_MasterRowGetRelationName);
            this.gvData.MasterRowGetRelationCount += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventHandler(this.gvData_MasterRowGetRelationCount);
            this.gvData.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvData_PopupMenuShowing);
            // 
            // gColId
            // 
            this.gColId.AppearanceCell.Options.UseTextOptions = true;
            this.gColId.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gColId.Caption = "Std.Id";
            this.gColId.FieldName = "Id";
            this.gColId.Name = "gColId";
            this.gColId.Width = 91;
            // 
            // gColIdAtt
            // 
            this.gColIdAtt.Caption = "IdAtt";
            this.gColIdAtt.FieldName = "data.IdAttachment";
            this.gColIdAtt.Name = "gColIdAtt";
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "中文名稱";
            this.gridColumn14.FieldName = "DisplayNameTW";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 1;
            this.gridColumn14.Width = 99;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "越文名稱";
            this.gridColumn3.FieldName = "DisplayNameVN";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 116;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "英文名稱";
            this.gridColumn4.FieldName = "DisplayNameTA";
            this.gridColumn4.MaxWidth = 300;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Width = 91;
            // 
            // gridColumn16
            // 
            this.gridColumn16.Caption = "出廠編號";
            this.gridColumn16.FieldName = "SN";
            this.gridColumn16.MaxWidth = 300;
            this.gridColumn16.Name = "gridColumn16";
            this.gridColumn16.OptionsFilter.AllowFilter = false;
            this.gridColumn16.Visible = true;
            this.gridColumn16.VisibleIndex = 0;
            this.gridColumn16.Width = 102;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "英文名稱";
            this.gridColumn9.FieldName = "data.DisplayNameEN";
            this.gridColumn9.Name = "gridColumn9";
            // 
            // gColEnterDate
            // 
            this.gColEnterDate.AppearanceCell.Options.UseTextOptions = true;
            this.gColEnterDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gColEnterDate.Caption = "版本";
            this.gColEnterDate.FieldName = "data.VersionNo";
            this.gColEnterDate.Name = "gColEnterDate";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "上傳人員";
            this.gridColumn2.FieldName = "UserUpload";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Width = 91;
            // 
            // gridColumn11
            // 
            this.gridColumn11.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn11.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn11.Caption = "上傳日期";
            this.gridColumn11.FieldName = "data.CreateAt";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Width = 91;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "版權";
            this.gridColumn10.FieldName = "data.HasCopyright";
            this.gridColumn10.Name = "gridColumn10";
            // 
            // gColDeptId
            // 
            this.gColDeptId.Caption = "實驗室";
            this.gColDeptId.FieldName = "data.IdDept";
            this.gColDeptId.Name = "gColDeptId";
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
            this.barCbbDepts});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 12;
            this.barManagerTP.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.cbbDepts});
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnExportExcel, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
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
            this.btnAdd.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ItemAppearance.Normal.Options.UseFont = true;
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
            this.btnReload.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReload.ItemAppearance.Normal.Options.UseFont = true;
            this.btnReload.Name = "btnReload";
            this.btnReload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnReload_ItemClick);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Caption = "出表";
            this.btnExportExcel.Id = 2;
            this.btnExportExcel.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExportExcel.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExportExcel.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExportExcel.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportExcel.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportExcel_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(1034, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 640);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(1034, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 591);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1034, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 591);
            // 
            // barCbbDepts
            // 
            this.barCbbDepts.Caption = "實驗室";
            this.barCbbDepts.Edit = this.cbbDepts;
            this.barCbbDepts.EditWidth = 150;
            this.barCbbDepts.Id = 11;
            this.barCbbDepts.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.barCbbDepts.ItemAppearance.Normal.Options.UseFont = true;
            this.barCbbDepts.Name = "barCbbDepts";
            this.barCbbDepts.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // cbbDepts
            // 
            this.cbbDepts.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbDepts.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbDepts.Appearance.Options.UseFont = true;
            this.cbbDepts.Appearance.Options.UseForeColor = true;
            this.cbbDepts.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbDepts.AppearanceDropDown.Options.UseFont = true;
            this.cbbDepts.AutoHeight = false;
            this.cbbDepts.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbDepts.Name = "cbbDepts";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1034, 591);
            this.layoutControl1.TabIndex = 8;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1034, 591);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1014, 571);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // uc403_05_CalipStandardMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "uc403_05_CalipStandardMain";
            this.Size = new System.Drawing.Size(1034, 640);
            this.Load += new System.EventHandler(this.uc403_05_CalipStandardMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDepts)).EndInit();
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
        private DevExpress.XtraBars.BarButtonItem btnExportExcel;
        private DevExpress.XtraBars.BarEditItem barCbbDepts;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit cbbDepts;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvForm;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gColIdAttForm;
        private DevExpress.XtraGrid.Columns.GridColumn gcolUploadDate;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData;
        private DevExpress.XtraGrid.Columns.GridColumn gColId;
        private DevExpress.XtraGrid.Columns.GridColumn gColIdAtt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gColEnterDate;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gColDeptId;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gcolConfirmDate;
        private DevExpress.XtraGrid.Columns.GridColumn gcolFinishDate;
        private DevExpress.XtraGrid.Columns.GridColumn gcolActualName;
    }
}
