namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    partial class f309_MaterialReplacement_Info
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
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.deReplacementDate = new DevExpress.XtraEditors.DateEdit();
            this.sleReplacement = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gvReplacement = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtSourceName = new DevExpress.XtraEditors.MemoEdit();
            this.txtSourceCode = new DevExpress.XtraEditors.TextEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcSourceCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSourceName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcReplacement = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcReplacementDate = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.deReplacementDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deReplacementDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleReplacement.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvReplacement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSourceName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSourceCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSourceCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSourceName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReplacement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReplacementDate)).BeginInit();
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
            this.btnConfirm,
            this.btnCancel});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 2;
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnCancel, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
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
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Caption = "\u53d6\u6d88";
            this.btnCancel.Id = 1;
            this.btnCancel.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCancel_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(700, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 264);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(700, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 215);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(700, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 215);
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.deReplacementDate);
            this.layoutControl1.Controls.Add(this.sleReplacement);
            this.layoutControl1.Controls.Add(this.txtSourceName);
            this.layoutControl1.Controls.Add(this.txtSourceCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(700, 215);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // deReplacementDate
            // 
            this.deReplacementDate.EditValue = null;
            this.deReplacementDate.Location = new System.Drawing.Point(117, 156);
            this.deReplacementDate.MenuManager = this.barManagerTP;
            this.deReplacementDate.Name = "deReplacementDate";
            this.deReplacementDate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.deReplacementDate.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.deReplacementDate.Properties.Appearance.Options.UseFont = true;
            this.deReplacementDate.Properties.Appearance.Options.UseForeColor = true;
            this.deReplacementDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.deReplacementDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.deReplacementDate.Properties.MaskSettings.Set("mask", "yyyy/MM/dd");
            this.deReplacementDate.Properties.UseMaskAsDisplayFormat = true;
            this.deReplacementDate.Size = new System.Drawing.Size(571, 32);
            this.deReplacementDate.StyleController = this.layoutControl1;
            this.deReplacementDate.TabIndex = 7;
            // 
            // sleReplacement
            // 
            this.sleReplacement.Location = new System.Drawing.Point(117, 120);
            this.sleReplacement.MenuManager = this.barManagerTP;
            this.sleReplacement.Name = "sleReplacement";
            this.sleReplacement.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.sleReplacement.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.sleReplacement.Properties.Appearance.Options.UseFont = true;
            this.sleReplacement.Properties.Appearance.Options.UseForeColor = true;
            this.sleReplacement.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.sleReplacement.Properties.DisplayMember = "Code";
            this.sleReplacement.Properties.NullText = "";
            this.sleReplacement.Properties.PopupView = this.gvReplacement;
            this.sleReplacement.Properties.ValueMember = "Id";
            this.sleReplacement.Size = new System.Drawing.Size(571, 32);
            this.sleReplacement.StyleController = this.layoutControl1;
            this.sleReplacement.TabIndex = 6;
            // 
            // gvReplacement
            // 
            this.gvReplacement.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvReplacement.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvReplacement.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvReplacement.Appearance.Row.Options.UseFont = true;
            this.gvReplacement.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gvReplacement.Name = "gvReplacement";
            this.gvReplacement.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvReplacement.OptionsView.ShowGroupPanel = false;
            // 
            // txtSourceName
            // 
            this.txtSourceName.Location = new System.Drawing.Point(117, 48);
            this.txtSourceName.MenuManager = this.barManagerTP;
            this.txtSourceName.Name = "txtSourceName";
            this.txtSourceName.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txtSourceName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtSourceName.Properties.Appearance.Options.UseFont = true;
            this.txtSourceName.Properties.Appearance.Options.UseForeColor = true;
            this.txtSourceName.Properties.AppearanceReadOnly.ForeColor = System.Drawing.Color.Black;
            this.txtSourceName.Properties.AppearanceReadOnly.Options.UseForeColor = true;
            this.txtSourceName.Properties.ReadOnly = true;
            this.txtSourceName.Size = new System.Drawing.Size(571, 68);
            this.txtSourceName.StyleController = this.layoutControl1;
            this.txtSourceName.TabIndex = 5;
            // 
            // txtSourceCode
            // 
            this.txtSourceCode.Location = new System.Drawing.Point(117, 12);
            this.txtSourceCode.MenuManager = this.barManagerTP;
            this.txtSourceCode.Name = "txtSourceCode";
            this.txtSourceCode.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txtSourceCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtSourceCode.Properties.Appearance.Options.UseFont = true;
            this.txtSourceCode.Properties.Appearance.Options.UseForeColor = true;
            this.txtSourceCode.Properties.AppearanceReadOnly.ForeColor = System.Drawing.Color.Black;
            this.txtSourceCode.Properties.AppearanceReadOnly.Options.UseForeColor = true;
            this.txtSourceCode.Properties.ReadOnly = true;
            this.txtSourceCode.Size = new System.Drawing.Size(571, 32);
            this.txtSourceCode.StyleController = this.layoutControl1;
            this.txtSourceCode.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcSourceCode,
            this.lcSourceName,
            this.lcReplacement,
            this.lcReplacementDate});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(700, 215);
            this.Root.TextVisible = false;
            // 
            // lcSourceCode
            // 
            this.lcSourceCode.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcSourceCode.AppearanceItemCaption.Options.UseFont = true;
            this.lcSourceCode.Control = this.txtSourceCode;
            this.lcSourceCode.Location = new System.Drawing.Point(0, 0);
            this.lcSourceCode.Name = "lcSourceCode";
            this.lcSourceCode.Size = new System.Drawing.Size(680, 36);
            this.lcSourceCode.Text = "\u7269\u6599\u7de8\u865f";
            this.lcSourceCode.TextSize = new System.Drawing.Size(93, 24);
            // 
            // lcSourceName
            // 
            this.lcSourceName.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcSourceName.AppearanceItemCaption.Options.UseFont = true;
            this.lcSourceName.Control = this.txtSourceName;
            this.lcSourceName.Location = new System.Drawing.Point(0, 36);
            this.lcSourceName.Name = "lcSourceName";
            this.lcSourceName.Size = new System.Drawing.Size(680, 72);
            this.lcSourceName.Text = "\u54c1\u540d\u898f\u683c";
            this.lcSourceName.TextSize = new System.Drawing.Size(93, 24);
            // 
            // lcReplacement
            // 
            this.lcReplacement.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcReplacement.AppearanceItemCaption.Options.UseFont = true;
            this.lcReplacement.Control = this.sleReplacement;
            this.lcReplacement.Location = new System.Drawing.Point(0, 108);
            this.lcReplacement.Name = "lcReplacement";
            this.lcReplacement.Size = new System.Drawing.Size(680, 36);
            this.lcReplacement.Text = "\u66ff\u4ee3\u6599\u865f";
            this.lcReplacement.TextSize = new System.Drawing.Size(93, 24);
            // 
            // lcReplacementDate
            // 
            this.lcReplacementDate.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcReplacementDate.AppearanceItemCaption.Options.UseFont = true;
            this.lcReplacementDate.Control = this.deReplacementDate;
            this.lcReplacementDate.Location = new System.Drawing.Point(0, 144);
            this.lcReplacementDate.Name = "lcReplacementDate";
            this.lcReplacementDate.Size = new System.Drawing.Size(680, 51);
            this.lcReplacementDate.Text = "\u66ff\u4ee3\u65e5\u671f";
            this.lcReplacementDate.TextSize = new System.Drawing.Size(93, 24);
            // 
            // f309_MaterialReplacement_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 264);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "f309_MaterialReplacement_Info";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "\u8a2d\u5b9a\u66ff\u4ee3\u6599";
            this.Load += new System.EventHandler(this.f309_MaterialReplacement_Info_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.deReplacementDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deReplacementDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleReplacement.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvReplacement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSourceName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSourceCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSourceCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSourceName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReplacement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReplacementDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarButtonItem btnCancel;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txtSourceCode;
        private DevExpress.XtraEditors.MemoEdit txtSourceName;
        private DevExpress.XtraEditors.SearchLookUpEdit sleReplacement;
        private DevExpress.XtraGrid.Views.Grid.GridView gvReplacement;
        private DevExpress.XtraEditors.DateEdit deReplacementDate;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcSourceCode;
        private DevExpress.XtraLayout.LayoutControlItem lcSourceName;
        private DevExpress.XtraLayout.LayoutControlItem lcReplacement;
        private DevExpress.XtraLayout.LayoutControlItem lcReplacementDate;
    }
}
