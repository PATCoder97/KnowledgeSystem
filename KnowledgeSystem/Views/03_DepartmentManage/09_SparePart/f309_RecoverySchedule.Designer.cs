namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    partial class f309_RecoverySchedule
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
            this.dePlannedDisposeDate = new DevExpress.XtraEditors.DateEdit();
            this.sleAssignedUser = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gvUserLookup = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcAssignedUser = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPlannedDisposeDate = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dePlannedDisposeDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dePlannedDisposeDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleAssignedUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvUserLookup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAssignedUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPlannedDisposeDate)).BeginInit();
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
            this.barDockControlTop.Size = new System.Drawing.Size(560, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 169);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(560, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 120);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(560, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 120);
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.dePlannedDisposeDate);
            this.layoutControl1.Controls.Add(this.sleAssignedUser);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(560, 120);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // dePlannedDisposeDate
            // 
            this.dePlannedDisposeDate.EditValue = null;
            this.dePlannedDisposeDate.Location = new System.Drawing.Point(124, 48);
            this.dePlannedDisposeDate.MenuManager = this.barManagerTP;
            this.dePlannedDisposeDate.Name = "dePlannedDisposeDate";
            this.dePlannedDisposeDate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.dePlannedDisposeDate.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.dePlannedDisposeDate.Properties.Appearance.Options.UseFont = true;
            this.dePlannedDisposeDate.Properties.Appearance.Options.UseForeColor = true;
            this.dePlannedDisposeDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dePlannedDisposeDate.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            this.dePlannedDisposeDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dePlannedDisposeDate.Properties.MaskSettings.Set("mask", "yyyy/MM/dd HH:mm");
            this.dePlannedDisposeDate.Properties.UseMaskAsDisplayFormat = true;
            this.dePlannedDisposeDate.Size = new System.Drawing.Size(424, 32);
            this.dePlannedDisposeDate.StyleController = this.layoutControl1;
            this.dePlannedDisposeDate.TabIndex = 5;
            // 
            // sleAssignedUser
            // 
            this.sleAssignedUser.Location = new System.Drawing.Point(124, 12);
            this.sleAssignedUser.MenuManager = this.barManagerTP;
            this.sleAssignedUser.Name = "sleAssignedUser";
            this.sleAssignedUser.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.sleAssignedUser.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.sleAssignedUser.Properties.Appearance.Options.UseFont = true;
            this.sleAssignedUser.Properties.Appearance.Options.UseForeColor = true;
            this.sleAssignedUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.sleAssignedUser.Properties.DisplayMember = "DisplayName";
            this.sleAssignedUser.Properties.NullText = "";
            this.sleAssignedUser.Properties.PopupView = this.gvUserLookup;
            this.sleAssignedUser.Properties.ValueMember = "Id";
            this.sleAssignedUser.Size = new System.Drawing.Size(424, 32);
            this.sleAssignedUser.StyleController = this.layoutControl1;
            this.sleAssignedUser.TabIndex = 4;
            // 
            // gvUserLookup
            // 
            this.gvUserLookup.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvUserLookup.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvUserLookup.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvUserLookup.Appearance.Row.Options.UseFont = true;
            this.gvUserLookup.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gvUserLookup.Name = "gvUserLookup";
            this.gvUserLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvUserLookup.OptionsView.ShowGroupPanel = false;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcAssignedUser,
            this.lcPlannedDisposeDate});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(560, 120);
            this.Root.TextVisible = false;
            // 
            // lcAssignedUser
            // 
            this.lcAssignedUser.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcAssignedUser.AppearanceItemCaption.Options.UseFont = true;
            this.lcAssignedUser.Control = this.sleAssignedUser;
            this.lcAssignedUser.Location = new System.Drawing.Point(0, 0);
            this.lcAssignedUser.Name = "lcAssignedUser";
            this.lcAssignedUser.Size = new System.Drawing.Size(540, 36);
            this.lcAssignedUser.Text = "\u5831\u5ee2\u7d93\u8fa6";
            this.lcAssignedUser.TextSize = new System.Drawing.Size(109, 24);
            // 
            // lcPlannedDisposeDate
            // 
            this.lcPlannedDisposeDate.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcPlannedDisposeDate.AppearanceItemCaption.Options.UseFont = true;
            this.lcPlannedDisposeDate.Control = this.dePlannedDisposeDate;
            this.lcPlannedDisposeDate.Location = new System.Drawing.Point(0, 36);
            this.lcPlannedDisposeDate.Name = "lcPlannedDisposeDate";
            this.lcPlannedDisposeDate.Size = new System.Drawing.Size(540, 64);
            this.lcPlannedDisposeDate.Text = "\u9810\u8a08\u6642\u9593";
            this.lcPlannedDisposeDate.TextSize = new System.Drawing.Size(109, 24);
            // 
            // f309_RecoverySchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 169);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "f309_RecoverySchedule";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "\u66f4\u65b0\u5831\u5ee2\u5b89\u6392";
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dePlannedDisposeDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dePlannedDisposeDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleAssignedUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvUserLookup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAssignedUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPlannedDisposeDate)).EndInit();
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
        private DevExpress.XtraEditors.SearchLookUpEdit sleAssignedUser;
        private DevExpress.XtraGrid.Views.Grid.GridView gvUserLookup;
        private DevExpress.XtraEditors.DateEdit dePlannedDisposeDate;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcAssignedUser;
        private DevExpress.XtraLayout.LayoutControlItem lcPlannedDisposeDate;
    }
}
