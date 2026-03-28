namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    partial class f313_DepartmentSetting_Info
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
            this.chkActive = new DevExpress.XtraEditors.CheckEdit();
            this.spinRate = new DevExpress.XtraEditors.SpinEdit();
            this.cbbDept = new DevExpress.XtraEditors.ComboBoxEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcDept = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcRate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcActive = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinRate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcActive)).BeginInit();
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
            this.barDockControlTop.Size = new System.Drawing.Size(440, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 165);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(440, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 116);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(440, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 116);
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.chkActive);
            this.layoutControl1.Controls.Add(this.spinRate);
            this.layoutControl1.Controls.Add(this.cbbDept);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(440, 116);
            this.layoutControl1.TabIndex = 9;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // chkActive
            // 
            this.chkActive.Location = new System.Drawing.Point(12, 84);
            this.chkActive.Name = "chkActive";
            this.chkActive.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.chkActive.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.chkActive.Properties.Appearance.Options.UseFont = true;
            this.chkActive.Properties.Appearance.Options.UseForeColor = true;
            this.chkActive.Properties.Caption = "\u555f\u7528";
            this.chkActive.Size = new System.Drawing.Size(416, 30);
            this.chkActive.StyleController = this.layoutControl1;
            this.chkActive.TabIndex = 6;
            // 
            // spinRate
            // 
            this.spinRate.EditValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.spinRate.Location = new System.Drawing.Point(112, 48);
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
            this.spinRate.Size = new System.Drawing.Size(316, 32);
            this.spinRate.StyleController = this.layoutControl1;
            this.spinRate.TabIndex = 5;
            // 
            // cbbDept
            // 
            this.cbbDept.Location = new System.Drawing.Point(112, 12);
            this.cbbDept.Name = "cbbDept";
            this.cbbDept.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbDept.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbDept.Properties.Appearance.Options.UseFont = true;
            this.cbbDept.Properties.Appearance.Options.UseForeColor = true;
            this.cbbDept.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbDept.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbDept.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbDept.Size = new System.Drawing.Size(316, 32);
            this.cbbDept.StyleController = this.layoutControl1;
            this.cbbDept.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcDept,
            this.lcRate,
            this.lcActive});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(440, 136);
            this.Root.TextVisible = false;
            // 
            // lcDept
            // 
            this.lcDept.AllowHtmlStringInCaption = true;
            this.lcDept.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcDept.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcDept.AppearanceItemCaption.Options.UseFont = true;
            this.lcDept.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcDept.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcDept.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcDept.Control = this.cbbDept;
            this.lcDept.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcDept.Location = new System.Drawing.Point(0, 0);
            this.lcDept.Name = "lcDept";
            this.lcDept.Size = new System.Drawing.Size(420, 36);
            this.lcDept.Text = "\u90e8\u9580";
            this.lcDept.TextSize = new System.Drawing.Size(88, 24);
            // 
            // lcRate
            // 
            this.lcRate.AllowHtmlStringInCaption = true;
            this.lcRate.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcRate.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcRate.AppearanceItemCaption.Options.UseFont = true;
            this.lcRate.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcRate.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcRate.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcRate.Control = this.spinRate;
            this.lcRate.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcRate.Location = new System.Drawing.Point(0, 36);
            this.lcRate.Name = "lcRate";
            this.lcRate.Size = new System.Drawing.Size(420, 36);
            this.lcRate.Text = "\u62bd\u6a23\u7387%";
            this.lcRate.TextSize = new System.Drawing.Size(88, 24);
            // 
            // lcActive
            // 
            this.lcActive.AllowHtmlStringInCaption = true;
            this.lcActive.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcActive.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcActive.AppearanceItemCaption.Options.UseFont = true;
            this.lcActive.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcActive.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcActive.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcActive.Control = this.chkActive;
            this.lcActive.Location = new System.Drawing.Point(0, 72);
            this.lcActive.Name = "lcActive";
            this.lcActive.Size = new System.Drawing.Size(420, 44);
            this.lcActive.TextSize = new System.Drawing.Size(0, 0);
            this.lcActive.TextVisible = false;
            // 
            // f313_DepartmentSetting_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 165);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f313_DepartmentSetting_Info";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "f313_DepartmentSetting_Info";
            this.Load += new System.EventHandler(this.f313_DepartmentSetting_Info_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinRate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcActive)).EndInit();
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
        private DevExpress.XtraEditors.ComboBoxEdit cbbDept;
        private DevExpress.XtraEditors.SpinEdit spinRate;
        private DevExpress.XtraEditors.CheckEdit chkActive;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcDept;
        private DevExpress.XtraLayout.LayoutControlItem lcRate;
        private DevExpress.XtraLayout.LayoutControlItem lcActive;
    }
}
