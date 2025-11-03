namespace KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement
{
    partial class f311_AddFuel_Info
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(f311_AddFuel_Info));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txbOdometerReading = new DevExpress.XtraEditors.TextEdit();
            this.cbbFuelFilledBy = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gridView11 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcOdometerReading = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcFuelFilledBy = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcLicensePlate = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.txbLicensePlate = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbOdometerReading.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbFuelFilledBy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcOdometerReading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcFuelFilledBy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcLicensePlate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbLicensePlate.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txbOdometerReading);
            this.layoutControl1.Controls.Add(this.cbbFuelFilledBy);
            this.layoutControl1.Controls.Add(this.txbLicensePlate);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(359, 128);
            this.layoutControl1.TabIndex = 6;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txbOdometerReading
            // 
            this.txbOdometerReading.Location = new System.Drawing.Point(100, 48);
            this.txbOdometerReading.Name = "txbOdometerReading";
            this.txbOdometerReading.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbOdometerReading.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbOdometerReading.Properties.Appearance.Options.UseFont = true;
            this.txbOdometerReading.Properties.Appearance.Options.UseForeColor = true;
            this.txbOdometerReading.Properties.DisplayFormat.FormatString = "#,##0";
            this.txbOdometerReading.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txbOdometerReading.Properties.EditFormat.FormatString = "#,##0";
            this.txbOdometerReading.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txbOdometerReading.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txbOdometerReading.Properties.MaskSettings.Set("MaskManagerSignature", "allowNull=False");
            this.txbOdometerReading.Properties.MaskSettings.Set("mask", "N0");
            this.txbOdometerReading.Properties.UseMaskAsDisplayFormat = true;
            this.txbOdometerReading.Size = new System.Drawing.Size(247, 32);
            this.txbOdometerReading.StyleController = this.layoutControl1;
            this.txbOdometerReading.TabIndex = 11;
            // 
            // cbbFuelFilledBy
            // 
            this.cbbFuelFilledBy.Location = new System.Drawing.Point(100, 84);
            this.cbbFuelFilledBy.Name = "cbbFuelFilledBy";
            this.cbbFuelFilledBy.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbFuelFilledBy.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbFuelFilledBy.Properties.Appearance.Options.UseFont = true;
            this.cbbFuelFilledBy.Properties.Appearance.Options.UseForeColor = true;
            this.cbbFuelFilledBy.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbFuelFilledBy.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbFuelFilledBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbFuelFilledBy.Properties.NullText = "";
            this.cbbFuelFilledBy.Properties.PopupView = this.gridView11;
            this.cbbFuelFilledBy.Size = new System.Drawing.Size(247, 32);
            this.cbbFuelFilledBy.StyleController = this.layoutControl1;
            this.cbbFuelFilledBy.TabIndex = 17;
            // 
            // gridView11
            // 
            this.gridView11.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView11.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gridView11.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView11.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gridView11.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridView11.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridView11.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView11.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gridView11.Appearance.Row.Options.UseFont = true;
            this.gridView11.Appearance.Row.Options.UseForeColor = true;
            this.gridView11.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn7,
            this.gridColumn8});
            this.gridView11.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView11.Name = "gridView11";
            this.gridView11.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView11.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "人員代號";
            this.gridColumn7.FieldName = "Id";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 0;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "人員名稱";
            this.gridColumn8.FieldName = "DisplayName";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 1;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcOdometerReading,
            this.lcFuelFilledBy,
            this.lcLicensePlate});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(359, 128);
            this.Root.TextVisible = false;
            // 
            // lcOdometerReading
            // 
            this.lcOdometerReading.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcOdometerReading.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcOdometerReading.AppearanceItemCaption.Options.UseFont = true;
            this.lcOdometerReading.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcOdometerReading.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcOdometerReading.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcOdometerReading.Control = this.txbOdometerReading;
            this.lcOdometerReading.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcOdometerReading.CustomizationFormText = "編碼";
            this.lcOdometerReading.Location = new System.Drawing.Point(0, 36);
            this.lcOdometerReading.Name = "lcOdometerReading";
            this.lcOdometerReading.Size = new System.Drawing.Size(339, 36);
            this.lcOdometerReading.Text = "文件編碼";
            this.lcOdometerReading.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcFuelFilledBy
            // 
            this.lcFuelFilledBy.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcFuelFilledBy.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcFuelFilledBy.AppearanceItemCaption.Options.UseFont = true;
            this.lcFuelFilledBy.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcFuelFilledBy.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcFuelFilledBy.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcFuelFilledBy.Control = this.cbbFuelFilledBy;
            this.lcFuelFilledBy.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcFuelFilledBy.CustomizationFormText = "二級主管";
            this.lcFuelFilledBy.Location = new System.Drawing.Point(0, 72);
            this.lcFuelFilledBy.Name = "lcFuelFilledBy";
            this.lcFuelFilledBy.Size = new System.Drawing.Size(339, 36);
            this.lcFuelFilledBy.Text = "加油人員";
            this.lcFuelFilledBy.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcLicensePlate
            // 
            this.lcLicensePlate.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcLicensePlate.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcLicensePlate.AppearanceItemCaption.Options.UseFont = true;
            this.lcLicensePlate.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcLicensePlate.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcLicensePlate.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcLicensePlate.Control = this.txbLicensePlate;
            this.lcLicensePlate.Location = new System.Drawing.Point(0, 0);
            this.lcLicensePlate.Name = "lcLicensePlate";
            this.lcLicensePlate.Size = new System.Drawing.Size(339, 36);
            this.lcLicensePlate.Text = "車牌號碼";
            this.lcLicensePlate.TextSize = new System.Drawing.Size(76, 24);
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
            this.btnConfirm});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 4;
            // 
            // bar2
            // 
            this.bar2.BarAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Caption = "確認";
            this.btnConfirm.Id = 2;
            this.btnConfirm.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnConfirm.ImageOptions.SvgImage")));
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(359, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 177);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(359, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 128);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(359, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 128);
            // 
            // txbLicensePlate
            // 
            this.txbLicensePlate.EditValue = "";
            this.txbLicensePlate.Location = new System.Drawing.Point(100, 12);
            this.txbLicensePlate.Name = "txbLicensePlate";
            this.txbLicensePlate.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txbLicensePlate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbLicensePlate.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbLicensePlate.Properties.Appearance.Options.UseFont = true;
            this.txbLicensePlate.Properties.Appearance.Options.UseForeColor = true;
            this.txbLicensePlate.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbLicensePlate.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txbLicensePlate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbLicensePlate.Properties.PopupSizeable = true;
            this.txbLicensePlate.Size = new System.Drawing.Size(247, 32);
            this.txbLicensePlate.StyleController = this.layoutControl1;
            this.txbLicensePlate.TabIndex = 17;
            // 
            // f311_AddFuel_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 177);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f311_AddFuel_Info";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "f311_AddFuel_Info";
            this.Load += new System.EventHandler(this.f311_AddFuel_Info_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbOdometerReading.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbFuelFilledBy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcOdometerReading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcFuelFilledBy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcLicensePlate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbLicensePlate.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txbOdometerReading;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcOdometerReading;
        private DevExpress.XtraLayout.LayoutControlItem lcLicensePlate;
        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SearchLookUpEdit cbbFuelFilledBy;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraLayout.LayoutControlItem lcFuelFilledBy;
        private DevExpress.XtraEditors.ComboBoxEdit txbLicensePlate;
    }
}