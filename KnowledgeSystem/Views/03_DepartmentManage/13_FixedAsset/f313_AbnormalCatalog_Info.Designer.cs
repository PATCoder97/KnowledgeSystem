namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    partial class f313_AbnormalCatalog_Info
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
            this.memoRemarks = new DevExpress.XtraEditors.MemoEdit();
            this.chkActive = new DevExpress.XtraEditors.CheckEdit();
            this.spinSort = new DevExpress.XtraEditors.SpinEdit();
            this.txbName = new DevExpress.XtraEditors.TextEdit();
            this.txbCode = new DevExpress.XtraEditors.TextEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSort = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcActive = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcRemarks = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoRemarks.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRemarks)).BeginInit();
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
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 231);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(520, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 182);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(520, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 182);
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.memoRemarks);
            this.layoutControl1.Controls.Add(this.chkActive);
            this.layoutControl1.Controls.Add(this.spinSort);
            this.layoutControl1.Controls.Add(this.txbName);
            this.layoutControl1.Controls.Add(this.txbCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(520, 182);
            this.layoutControl1.TabIndex = 9;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // memoRemarks
            // 
            this.memoRemarks.Location = new System.Drawing.Point(112, 120);
            this.memoRemarks.Name = "memoRemarks";
            this.memoRemarks.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.memoRemarks.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.memoRemarks.Properties.Appearance.Options.UseFont = true;
            this.memoRemarks.Properties.Appearance.Options.UseForeColor = true;
            this.memoRemarks.Size = new System.Drawing.Size(396, 50);
            this.memoRemarks.StyleController = this.layoutControl1;
            this.memoRemarks.TabIndex = 8;
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
            this.chkActive.Size = new System.Drawing.Size(496, 30);
            this.chkActive.StyleController = this.layoutControl1;
            this.chkActive.TabIndex = 7;
            // 
            // spinSort
            // 
            this.spinSort.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinSort.Location = new System.Drawing.Point(112, 48);
            this.spinSort.Name = "spinSort";
            this.spinSort.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.spinSort.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.spinSort.Properties.Appearance.Options.UseFont = true;
            this.spinSort.Properties.Appearance.Options.UseForeColor = true;
            this.spinSort.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinSort.Properties.IsFloatValue = false;
            this.spinSort.Properties.MaskSettings.Set("mask", "N00");
            this.spinSort.Size = new System.Drawing.Size(396, 32);
            this.spinSort.StyleController = this.layoutControl1;
            this.spinSort.TabIndex = 6;
            // 
            // txbName
            // 
            this.txbName.Location = new System.Drawing.Point(112, 48);
            this.txbName.Name = "txbName";
            this.txbName.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbName.Properties.Appearance.Options.UseFont = true;
            this.txbName.Properties.Appearance.Options.UseForeColor = true;
            this.txbName.Size = new System.Drawing.Size(396, 32);
            this.txbName.StyleController = this.layoutControl1;
            this.txbName.TabIndex = 5;
            // 
            // txbCode
            // 
            this.txbCode.Location = new System.Drawing.Point(112, 12);
            this.txbCode.Name = "txbCode";
            this.txbCode.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbCode.Properties.Appearance.Options.UseFont = true;
            this.txbCode.Properties.Appearance.Options.UseForeColor = true;
            this.txbCode.Size = new System.Drawing.Size(396, 32);
            this.txbCode.StyleController = this.layoutControl1;
            this.txbCode.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcCode,
            this.lcName,
            this.lcSort,
            this.lcActive,
            this.lcRemarks});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(520, 202);
            this.Root.TextVisible = false;
            // 
            // lcCode
            // 
            this.lcCode.AllowHtmlStringInCaption = true;
            this.lcCode.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcCode.AppearanceItemCaption.Options.UseFont = true;
            this.lcCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcCode.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcCode.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcCode.Control = this.txbCode;
            this.lcCode.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcCode.Location = new System.Drawing.Point(0, 0);
            this.lcCode.Name = "lcCode";
            this.lcCode.Size = new System.Drawing.Size(500, 36);
            this.lcCode.Text = "\u4ee3\u78bc";
            this.lcCode.TextSize = new System.Drawing.Size(88, 24);
            // 
            // lcName
            // 
            this.lcName.AllowHtmlStringInCaption = true;
            this.lcName.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcName.AppearanceItemCaption.Options.UseFont = true;
            this.lcName.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcName.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcName.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcName.Control = this.txbName;
            this.lcName.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcName.Location = new System.Drawing.Point(0, 36);
            this.lcName.Name = "lcName";
            this.lcName.Size = new System.Drawing.Size(500, 36);
            this.lcName.Text = "\u540d\u7a31";
            this.lcName.TextSize = new System.Drawing.Size(88, 24);
            // 
            // lcSort
            // 
            this.lcSort.AllowHtmlStringInCaption = true;
            this.lcSort.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcSort.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcSort.AppearanceItemCaption.Options.UseFont = true;
            this.lcSort.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcSort.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcSort.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcSort.Control = this.spinSort;
            this.lcSort.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcSort.Location = new System.Drawing.Point(0, 72);
            this.lcSort.Name = "lcSort";
            this.lcSort.Size = new System.Drawing.Size(500, 36);
            this.lcSort.Text = "\u6392\u5e8f";
            this.lcSort.TextSize = new System.Drawing.Size(88, 24);
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
            this.lcActive.Location = new System.Drawing.Point(0, 108);
            this.lcActive.Name = "lcActive";
            this.lcActive.Size = new System.Drawing.Size(500, 36);
            this.lcActive.TextSize = new System.Drawing.Size(0, 0);
            this.lcActive.TextVisible = false;
            // 
            // lcRemarks
            // 
            this.lcRemarks.AllowHtmlStringInCaption = true;
            this.lcRemarks.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcRemarks.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcRemarks.AppearanceItemCaption.Options.UseFont = true;
            this.lcRemarks.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcRemarks.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcRemarks.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcRemarks.Control = this.memoRemarks;
            this.lcRemarks.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcRemarks.Location = new System.Drawing.Point(0, 144);
            this.lcRemarks.Name = "lcRemarks";
            this.lcRemarks.Size = new System.Drawing.Size(500, 38);
            this.lcRemarks.Text = "\u5099\u8a3b";
            this.lcRemarks.TextSize = new System.Drawing.Size(88, 24);
            // 
            // f313_AbnormalCatalog_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 231);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f313_AbnormalCatalog_Info";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "f313_AbnormalCatalog_Info";
            this.Load += new System.EventHandler(this.f313_AbnormalCatalog_Info_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoRemarks.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRemarks)).EndInit();
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
        private DevExpress.XtraEditors.TextEdit txbCode;
        private DevExpress.XtraEditors.TextEdit txbName;
        private DevExpress.XtraEditors.SpinEdit spinSort;
        private DevExpress.XtraEditors.CheckEdit chkActive;
        private DevExpress.XtraEditors.MemoEdit memoRemarks;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcCode;
        private DevExpress.XtraLayout.LayoutControlItem lcName;
        private DevExpress.XtraLayout.LayoutControlItem lcSort;
        private DevExpress.XtraLayout.LayoutControlItem lcActive;
        private DevExpress.XtraLayout.LayoutControlItem lcRemarks;
    }
}
