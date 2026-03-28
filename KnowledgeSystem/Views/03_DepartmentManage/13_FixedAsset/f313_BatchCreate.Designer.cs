namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    partial class f313_BatchCreate
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.spinRate = new DevExpress.XtraEditors.SpinEdit();
            this.datePeriod = new DevExpress.XtraEditors.DateEdit();
            this.cbbTarget = new DevExpress.XtraEditors.ComboBoxEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcTarget = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPeriod = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSampleRate = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinRate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datePeriod.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datePeriod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbTarget.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSampleRate)).BeginInit();
            this.SuspendLayout();
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
            this.barManagerTP.MaxItemId = 1;
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Caption = "\u78ba\u8a8d";
            this.btnConfirm.Id = 0;
            this.btnConfirm.ImageOptions.SvgImage = KnowledgeSystem.Helpers.TPSvgimages.Confirm;
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
            this.barDockControlTop.Size = new System.Drawing.Size(520, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 157);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(520, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 108);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(520, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 108);
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.spinRate);
            this.layoutControl1.Controls.Add(this.datePeriod);
            this.layoutControl1.Controls.Add(this.cbbTarget);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(520, 108);
            this.layoutControl1.TabIndex = 9;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // spinRate
            // 
            this.spinRate.EditValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.spinRate.Location = new System.Drawing.Point(112, 84);
            this.spinRate.Name = "spinRate";
            this.spinRate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.spinRate.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.spinRate.Properties.Appearance.Options.UseFont = true;
            this.spinRate.Properties.Appearance.Options.UseForeColor = true;
            this.spinRate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinRate.Properties.IsFloatValue = false;
            this.spinRate.Properties.MaskSettings.Set("mask", "N00");
            this.spinRate.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinRate.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinRate.Size = new System.Drawing.Size(396, 32);
            this.spinRate.StyleController = this.layoutControl1;
            this.spinRate.TabIndex = 6;
            // 
            // datePeriod
            // 
            this.datePeriod.EditValue = null;
            this.datePeriod.Location = new System.Drawing.Point(112, 48);
            this.datePeriod.Name = "datePeriod";
            this.datePeriod.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.datePeriod.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.datePeriod.Properties.Appearance.Options.UseFont = true;
            this.datePeriod.Properties.Appearance.Options.UseForeColor = true;
            this.datePeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.datePeriod.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.datePeriod.Size = new System.Drawing.Size(396, 32);
            this.datePeriod.StyleController = this.layoutControl1;
            this.datePeriod.TabIndex = 5;
            // 
            // cbbTarget
            // 
            this.cbbTarget.Location = new System.Drawing.Point(112, 12);
            this.cbbTarget.Name = "cbbTarget";
            this.cbbTarget.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbTarget.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbTarget.Properties.Appearance.Options.UseFont = true;
            this.cbbTarget.Properties.Appearance.Options.UseForeColor = true;
            this.cbbTarget.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbTarget.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbTarget.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbTarget.Size = new System.Drawing.Size(396, 32);
            this.cbbTarget.StyleController = this.layoutControl1;
            this.cbbTarget.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcTarget,
            this.lcPeriod,
            this.lcSampleRate});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(520, 128);
            this.Root.TextVisible = false;
            // 
            // lcTarget
            // 
            this.lcTarget.AllowHtmlStringInCaption = true;
            this.lcTarget.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcTarget.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcTarget.AppearanceItemCaption.Options.UseFont = true;
            this.lcTarget.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcTarget.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcTarget.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcTarget.Control = this.cbbTarget;
            this.lcTarget.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcTarget.Location = new System.Drawing.Point(0, 0);
            this.lcTarget.Name = "lcTarget";
            this.lcTarget.Size = new System.Drawing.Size(500, 36);
            this.lcTarget.Text = "\u76ee\u6a19";
            this.lcTarget.TextSize = new System.Drawing.Size(88, 24);
            // 
            // lcPeriod
            // 
            this.lcPeriod.AllowHtmlStringInCaption = true;
            this.lcPeriod.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcPeriod.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcPeriod.AppearanceItemCaption.Options.UseFont = true;
            this.lcPeriod.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcPeriod.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcPeriod.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcPeriod.Control = this.datePeriod;
            this.lcPeriod.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcPeriod.Location = new System.Drawing.Point(0, 36);
            this.lcPeriod.Name = "lcPeriod";
            this.lcPeriod.Size = new System.Drawing.Size(500, 36);
            this.lcPeriod.Text = "\u76e4\u9ede\u65e5\u671f";
            this.lcPeriod.TextSize = new System.Drawing.Size(88, 24);
            // 
            // lcSampleRate
            // 
            this.lcSampleRate.AllowHtmlStringInCaption = true;
            this.lcSampleRate.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcSampleRate.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcSampleRate.AppearanceItemCaption.Options.UseFont = true;
            this.lcSampleRate.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcSampleRate.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcSampleRate.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcSampleRate.Control = this.spinRate;
            this.lcSampleRate.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcSampleRate.Location = new System.Drawing.Point(0, 72);
            this.lcSampleRate.Name = "lcSampleRate";
            this.lcSampleRate.Size = new System.Drawing.Size(500, 36);
            this.lcSampleRate.Text = "\u62bd\u6a23\u7387%";
            this.lcSampleRate.TextSize = new System.Drawing.Size(88, 24);
            // 
            // f313_BatchCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 157);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f313_BatchCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "f313_BatchCreate";
            this.Load += new System.EventHandler(this.f313_BatchCreate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinRate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datePeriod.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datePeriod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbTarget.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSampleRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.ComboBoxEdit cbbTarget;
        private DevExpress.XtraEditors.DateEdit datePeriod;
        private DevExpress.XtraEditors.SpinEdit spinRate;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcTarget;
        private DevExpress.XtraLayout.LayoutControlItem lcPeriod;
        private DevExpress.XtraLayout.LayoutControlItem lcSampleRate;
    }
}
