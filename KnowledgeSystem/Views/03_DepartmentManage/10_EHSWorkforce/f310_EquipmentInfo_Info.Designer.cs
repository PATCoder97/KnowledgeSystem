namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    partial class f310_EquipmentInfo_Info
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnEdit;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txbCode;
        private DevExpress.XtraEditors.TextEdit txbNameVN;
        private DevExpress.XtraEditors.TextEdit txbNameTW;
        private DevExpress.XtraEditors.SearchLookUpEdit cbbDept;
        private DevExpress.XtraGrid.Views.Grid.GridView searchDeptView;
        private DevExpress.XtraGrid.Columns.GridColumn deptColId;
        private DevExpress.XtraGrid.Columns.GridColumn deptColDisplay;
        private DevExpress.XtraEditors.SearchLookUpEdit cbbManager;
        private DevExpress.XtraGrid.Views.Grid.GridView searchUserView;
        private DevExpress.XtraGrid.Columns.GridColumn userColId;
        private DevExpress.XtraGrid.Columns.GridColumn userColDisplay;
        private DevExpress.XtraEditors.MemoEdit txbNote;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcCode;
        private DevExpress.XtraLayout.LayoutControlItem lcNameVN;
        private DevExpress.XtraLayout.LayoutControlItem lcNameTW;
        private DevExpress.XtraLayout.LayoutControlItem lcDept;
        private DevExpress.XtraLayout.LayoutControlItem lcManager;
        private DevExpress.XtraLayout.LayoutControlItem lcNote;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txbNote = new DevExpress.XtraEditors.MemoEdit();
            this.cbbManager = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchUserView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.userColId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.userColDisplay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cbbDept = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchDeptView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.deptColId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.deptColDisplay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txbNameTW = new DevExpress.XtraEditors.TextEdit();
            this.txbNameVN = new DevExpress.XtraEditors.TextEdit();
            this.txbCode = new DevExpress.XtraEditors.TextEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcNameVN = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcNameTW = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDept = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcManager = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcNote = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbNote.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbManager.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchUserView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchDeptView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNameTW.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNameVN.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNameVN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNameTW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNote)).BeginInit();

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
            this.btnEdit,
            this.btnDelete,
            this.btnConfirm});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 3;
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnEdit, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnDelete, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnEdit
            // 
            this.btnEdit.Caption = "編輯";
            this.btnEdit.Id = 0;
            this.btnEdit.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnEdit_ItemClick);
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "刪除";
            this.btnDelete.Id = 1;
            this.btnDelete.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_ItemClick);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Caption = "確認";
            this.btnConfirm.Id = 2;
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
            this.barDockControlTop.Size = new System.Drawing.Size(620, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 317);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(620, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 268);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(620, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 268);
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.txbNote);
            this.layoutControl1.Controls.Add(this.cbbManager);
            this.layoutControl1.Controls.Add(this.cbbDept);
            this.layoutControl1.Controls.Add(this.txbNameTW);
            this.layoutControl1.Controls.Add(this.txbNameVN);
            this.layoutControl1.Controls.Add(this.txbCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(620, 268);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txbNote
            // 
            this.txbNote.Location = new System.Drawing.Point(112, 192);
            this.txbNote.MenuManager = this.barManagerTP;
            this.txbNote.Name = "txbNote";
            this.txbNote.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbNote.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbNote.Properties.Appearance.Options.UseFont = true;
            this.txbNote.Properties.Appearance.Options.UseForeColor = true;
            this.txbNote.Size = new System.Drawing.Size(496, 64);
            this.txbNote.StyleController = this.layoutControl1;
            this.txbNote.TabIndex = 9;
            // 
            // cbbManager
            // 
            this.cbbManager.Location = new System.Drawing.Point(112, 156);
            this.cbbManager.MenuManager = this.barManagerTP;
            this.cbbManager.Name = "cbbManager";
            this.cbbManager.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbManager.Properties.Appearance.Options.UseFont = true;
            this.cbbManager.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.cbbManager.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbManager.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbManager.Properties.NullText = "";
            this.cbbManager.Properties.PopupView = this.searchUserView;
            this.cbbManager.Size = new System.Drawing.Size(496, 32);
            this.cbbManager.StyleController = this.layoutControl1;
            this.cbbManager.TabIndex = 8;
            // 
            // searchUserView
            // 
            this.searchUserView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.searchUserView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.searchUserView.Appearance.HeaderPanel.Options.UseFont = true;
            this.searchUserView.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.searchUserView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.searchUserView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.searchUserView.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.searchUserView.Appearance.Row.Options.UseFont = true;
            this.searchUserView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.userColId,
            this.userColDisplay});
            this.searchUserView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchUserView.Name = "searchUserView";
            this.searchUserView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchUserView.OptionsView.EnableAppearanceOddRow = true;
            this.searchUserView.OptionsView.ShowAutoFilterRow = true;
            this.searchUserView.OptionsView.ShowGroupPanel = false;
            // 
            // userColId
            // 
            this.userColId.Caption = "工號";
            this.userColId.FieldName = "Id";
            this.userColId.MaxWidth = 130;
            this.userColId.MinWidth = 100;
            this.userColId.Name = "userColId";
            this.userColId.Visible = true;
            this.userColId.VisibleIndex = 0;
            this.userColId.Width = 120;
            // 
            // userColDisplay
            // 
            this.userColDisplay.Caption = "姓名";
            this.userColDisplay.FieldName = "DisplayName";
            this.userColDisplay.MinWidth = 200;
            this.userColDisplay.Name = "userColDisplay";
            this.userColDisplay.Visible = true;
            this.userColDisplay.VisibleIndex = 1;
            this.userColDisplay.Width = 320;
            // 
            // cbbDept
            // 
            this.cbbDept.Location = new System.Drawing.Point(112, 120);
            this.cbbDept.MenuManager = this.barManagerTP;
            this.cbbDept.Name = "cbbDept";
            this.cbbDept.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbDept.Properties.Appearance.Options.UseFont = true;
            this.cbbDept.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.cbbDept.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbDept.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbDept.Properties.NullText = "";
            this.cbbDept.Properties.PopupView = this.searchDeptView;
            this.cbbDept.Size = new System.Drawing.Size(496, 32);
            this.cbbDept.StyleController = this.layoutControl1;
            this.cbbDept.TabIndex = 7;
            // 
            // searchDeptView
            // 
            this.searchDeptView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.searchDeptView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.searchDeptView.Appearance.HeaderPanel.Options.UseFont = true;
            this.searchDeptView.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.searchDeptView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.searchDeptView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.searchDeptView.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.searchDeptView.Appearance.Row.Options.UseFont = true;
            this.searchDeptView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.deptColId,
            this.deptColDisplay});
            this.searchDeptView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchDeptView.Name = "searchDeptView";
            this.searchDeptView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchDeptView.OptionsView.EnableAppearanceOddRow = true;
            this.searchDeptView.OptionsView.ShowAutoFilterRow = true;
            this.searchDeptView.OptionsView.ShowGroupPanel = false;
            // 
            // deptColId
            // 
            this.deptColId.Caption = "部門";
            this.deptColId.FieldName = "Id";
            this.deptColId.MaxWidth = 120;
            this.deptColId.MinWidth = 80;
            this.deptColId.Name = "deptColId";
            this.deptColId.Visible = true;
            this.deptColId.VisibleIndex = 0;
            this.deptColId.Width = 100;
            // 
            // deptColDisplay
            // 
            this.deptColDisplay.Caption = "名稱";
            this.deptColDisplay.FieldName = "DisplayName";
            this.deptColDisplay.MinWidth = 200;
            this.deptColDisplay.Name = "deptColDisplay";
            this.deptColDisplay.Visible = true;
            this.deptColDisplay.VisibleIndex = 1;
            this.deptColDisplay.Width = 320;
            // 
            // txbNameTW
            // 
            this.txbNameTW.Location = new System.Drawing.Point(112, 84);
            this.txbNameTW.MenuManager = this.barManagerTP;
            this.txbNameTW.Name = "txbNameTW";
            this.txbNameTW.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbNameTW.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbNameTW.Properties.Appearance.Options.UseFont = true;
            this.txbNameTW.Properties.Appearance.Options.UseForeColor = true;
            this.txbNameTW.Size = new System.Drawing.Size(496, 32);
            this.txbNameTW.StyleController = this.layoutControl1;
            this.txbNameTW.TabIndex = 6;
            // 
            // txbNameVN
            // 
            this.txbNameVN.Location = new System.Drawing.Point(112, 48);
            this.txbNameVN.MenuManager = this.barManagerTP;
            this.txbNameVN.Name = "txbNameVN";
            this.txbNameVN.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbNameVN.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbNameVN.Properties.Appearance.Options.UseFont = true;
            this.txbNameVN.Properties.Appearance.Options.UseForeColor = true;
            this.txbNameVN.Size = new System.Drawing.Size(496, 32);
            this.txbNameVN.StyleController = this.layoutControl1;
            this.txbNameVN.TabIndex = 5;
            // 
            // txbCode
            // 
            this.txbCode.Location = new System.Drawing.Point(112, 12);
            this.txbCode.MenuManager = this.barManagerTP;
            this.txbCode.Name = "txbCode";
            this.txbCode.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbCode.Properties.Appearance.Options.UseFont = true;
            this.txbCode.Properties.Appearance.Options.UseForeColor = true;
            this.txbCode.Size = new System.Drawing.Size(496, 32);
            this.txbCode.StyleController = this.layoutControl1;
            this.txbCode.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcCode,
            this.lcNameVN,
            this.lcNameTW,
            this.lcDept,
            this.lcManager,
            this.lcNote});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(620, 268);
            this.Root.TextVisible = false;
            // 
            // lcCode
            // 
            this.lcCode.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcCode.AppearanceItemCaption.Options.UseFont = true;
            this.lcCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcCode.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcCode.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcCode.Control = this.txbCode;
            this.lcCode.Location = new System.Drawing.Point(0, 0);
            this.lcCode.Name = "lcCode";
            this.lcCode.Size = new System.Drawing.Size(600, 36);
            this.lcCode.Text = "設備編號";
            this.lcCode.TextSize = new System.Drawing.Size(97, 24);
            // 
            // lcNameVN
            // 
            this.lcNameVN.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcNameVN.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcNameVN.AppearanceItemCaption.Options.UseFont = true;
            this.lcNameVN.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcNameVN.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcNameVN.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcNameVN.Control = this.txbNameVN;
            this.lcNameVN.Location = new System.Drawing.Point(0, 36);
            this.lcNameVN.Name = "lcNameVN";
            this.lcNameVN.Size = new System.Drawing.Size(600, 36);
            this.lcNameVN.Text = "設備名稱(VN)";
            this.lcNameVN.TextSize = new System.Drawing.Size(97, 24);
            // 
            // lcNameTW
            // 
            this.lcNameTW.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcNameTW.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcNameTW.AppearanceItemCaption.Options.UseFont = true;
            this.lcNameTW.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcNameTW.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcNameTW.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcNameTW.Control = this.txbNameTW;
            this.lcNameTW.Location = new System.Drawing.Point(0, 72);
            this.lcNameTW.Name = "lcNameTW";
            this.lcNameTW.Size = new System.Drawing.Size(600, 36);
            this.lcNameTW.Text = "設備名稱(TW)";
            this.lcNameTW.TextSize = new System.Drawing.Size(97, 24);
            // 
            // lcDept
            // 
            this.lcDept.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcDept.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcDept.AppearanceItemCaption.Options.UseFont = true;
            this.lcDept.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcDept.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcDept.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcDept.Control = this.cbbDept;
            this.lcDept.Location = new System.Drawing.Point(0, 108);
            this.lcDept.Name = "lcDept";
            this.lcDept.Size = new System.Drawing.Size(600, 36);
            this.lcDept.Text = "管理部門";
            this.lcDept.TextSize = new System.Drawing.Size(97, 24);
            // 
            // lcManager
            // 
            this.lcManager.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcManager.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcManager.AppearanceItemCaption.Options.UseFont = true;
            this.lcManager.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcManager.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcManager.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcManager.Control = this.cbbManager;
            this.lcManager.Location = new System.Drawing.Point(0, 144);
            this.lcManager.Name = "lcManager";
            this.lcManager.Size = new System.Drawing.Size(600, 36);
            this.lcManager.Text = "管理人";
            this.lcManager.TextSize = new System.Drawing.Size(97, 24);
            // 
            // lcNote
            // 
            this.lcNote.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcNote.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcNote.AppearanceItemCaption.Options.UseFont = true;
            this.lcNote.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcNote.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcNote.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcNote.Control = this.txbNote;
            this.lcNote.Location = new System.Drawing.Point(0, 180);
            this.lcNote.Name = "lcNote";
            this.lcNote.Size = new System.Drawing.Size(600, 68);
            this.lcNote.Text = "備註";
            this.lcNote.TextSize = new System.Drawing.Size(97, 24);
            // 
            // f310_EquipmentInfo_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 317);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "f310_EquipmentInfo_Info";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "設備信息";
            this.Load += new System.EventHandler(this.f310_EquipmentInfo_Info_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbNote.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbManager.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchUserView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchDeptView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNameTW.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNameVN.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNameVN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNameTW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcNote)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
