namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension
{
    partial class uc403_SoftwareManual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc403_SoftwareManual));
            this.gvAttachment = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColActualName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColEncryptName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColIdReport = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColSoftName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.gvAttachment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // gvAttachment
            // 
            this.gvAttachment.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvAttachment.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Blue;
            this.gvAttachment.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvAttachment.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvAttachment.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvAttachment.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvAttachment.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvAttachment.Appearance.Row.ForeColor = System.Drawing.Color.Blue;
            this.gvAttachment.Appearance.Row.Options.UseFont = true;
            this.gvAttachment.Appearance.Row.Options.UseForeColor = true;
            this.gvAttachment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColActualName,
            this.gColEncryptName,
            this.gColIdReport});
            this.gvAttachment.GridControl = this.gcData;
            this.gvAttachment.Name = "gvAttachment";
            this.gvAttachment.OptionsCustomization.AllowFilter = false;
            this.gvAttachment.OptionsCustomization.AllowSort = false;
            this.gvAttachment.OptionsDetail.ShowDetailTabs = false;
            this.gvAttachment.OptionsView.ColumnAutoWidth = false;
            this.gvAttachment.OptionsView.ShowColumnHeaders = false;
            this.gvAttachment.OptionsView.ShowGroupPanel = false;
            this.gvAttachment.OptionsView.ShowIndicator = false;
            this.gvAttachment.DoubleClick += new System.EventHandler(this.gvAttachment_DoubleClick);
            // 
            // gColActualName
            // 
            this.gColActualName.Caption = "ActualName";
            this.gColActualName.FieldName = "DisplayName";
            this.gColActualName.Name = "gColActualName";
            this.gColActualName.Visible = true;
            this.gColActualName.VisibleIndex = 0;
            // 
            // gColEncryptName
            // 
            this.gColEncryptName.AppearanceCell.Options.UseTextOptions = true;
            this.gColEncryptName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gColEncryptName.Caption = "EncryptionName";
            this.gColEncryptName.FieldName = "EncryptName";
            this.gColEncryptName.Name = "gColEncryptName";
            // 
            // gColIdReport
            // 
            this.gColIdReport.Caption = "Id";
            this.gColIdReport.FieldName = "Id";
            this.gColIdReport.Name = "gColIdReport";
            // 
            // gcData
            // 
            this.gcData.Cursor = System.Windows.Forms.Cursors.Default;
            this.gcData.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Remove.Visible = false;
            gridLevelNode1.LevelTemplate = this.gvAttachment;
            gridLevelNode1.RelationName = "表單";
            this.gcData.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.gcData.Location = new System.Drawing.Point(12, 12);
            this.gcData.MainView = this.gvData;
            this.gcData.Name = "gcData";
            this.gcData.Size = new System.Drawing.Size(943, 539);
            this.gcData.TabIndex = 5;
            this.gcData.UseEmbeddedNavigator = true;
            this.gcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvData,
            this.gvAttachment});
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
            this.gColSoftName});
            this.gvData.GridControl = this.gcData;
            this.gvData.Name = "gvData";
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
            // 
            // gColId
            // 
            this.gColId.AppearanceCell.Options.UseTextOptions = true;
            this.gColId.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gColId.Caption = "Id";
            this.gColId.FieldName = "Id";
            this.gColId.Name = "gColId";
            // 
            // gColSoftName
            // 
            this.gColSoftName.Caption = "軟體名稱、係統名稱";
            this.gColSoftName.FieldName = "SoftName";
            this.gColSoftName.Name = "gColSoftName";
            this.gColSoftName.Visible = true;
            this.gColSoftName.VisibleIndex = 0;
            this.gColSoftName.Width = 279;
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
            this.btnReload});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 11;
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnReload, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
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
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(967, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 612);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(967, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 563);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(967, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 563);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(967, 563);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(967, 563);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(947, 543);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // uc403_SoftwareManual
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
            this.Name = "uc403_SoftwareManual";
            this.Size = new System.Drawing.Size(967, 612);
            this.Load += new System.EventHandler(this.uc403_SoftwareManual_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvAttachment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
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
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvAttachment;
        private DevExpress.XtraGrid.Columns.GridColumn gColActualName;
        private DevExpress.XtraGrid.Columns.GridColumn gColEncryptName;
        private DevExpress.XtraGrid.Columns.GridColumn gColIdReport;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData;
        private DevExpress.XtraGrid.Columns.GridColumn gColId;
        private DevExpress.XtraGrid.Columns.GridColumn gColSoftName;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
