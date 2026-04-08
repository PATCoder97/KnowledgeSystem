namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    partial class uc309_RecoveryMgmt
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
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barCbbDept = new DevExpress.XtraBars.BarEditItem();
            this.cbbDept = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.barCbbStatus = new DevExpress.XtraBars.BarEditItem();
            this.cbbStatus = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.barCbbUser = new DevExpress.XtraBars.BarEditItem();
            this.cbbUser = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.barDateFrom = new DevExpress.XtraBars.BarEditItem();
            this.riDateFrom = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.barDateTo = new DevExpress.XtraBars.BarEditItem();
            this.riDateTo = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.btnViewIssue = new DevExpress.XtraBars.BarButtonItem();
            this.btnDownloadGuide = new DevExpress.XtraBars.BarButtonItem();
            this.btnManageGuide = new DevExpress.XtraBars.BarButtonItem();
            this.btnUploadEvidence = new DevExpress.XtraBars.BarButtonItem();
            this.btnViewEvidence = new DevExpress.XtraBars.BarButtonItem();
            this.btnUpdateTime = new DevExpress.XtraBars.BarButtonItem();
            this.btnConfirmComplete = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancelTicket = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTicketNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNewMaterialCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNewMaterialDisplayName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOldBaseMaterialCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOldBaseMaterialDisplayName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRecoveryOptionDisplay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQuantity = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSourceStorageName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRestockStorageName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAssignedUserName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPlannedDisposeDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colActualDisposeDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatusDisplay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEvidenceCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riDateFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riDateFrom.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riDateTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.riDateTo.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
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
            this.barCbbDept,
            this.barCbbStatus,
            this.barCbbUser,
            this.barDateFrom,
            this.barDateTo,
            this.btnReload,
            this.btnViewIssue,
            this.btnDownloadGuide,
            this.btnManageGuide,
            this.btnUploadEvidence,
            this.btnViewEvidence,
            this.btnUpdateTime,
            this.btnConfirmComplete,
            this.btnCancelTicket});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 14;
            this.barManagerTP.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.cbbDept,
            this.cbbStatus,
            this.cbbUser,
            this.riDateFrom,
            this.riDateTo});
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.barCbbDept, "", true, true, true, 220),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnReload, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnDownloadGuide, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnManageGuide, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barCbbDept
            // 
            this.barCbbDept.Caption = "單位";
            this.barCbbDept.Edit = this.cbbDept;
            this.barCbbDept.Id = 0;
            this.barCbbDept.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.barCbbDept.Name = "barCbbDept";
            this.barCbbDept.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barCbbDept.EditValueChanged += new System.EventHandler(this.Filter_EditValueChanged);
            // 
            // cbbDept
            // 
            this.cbbDept.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbDept.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbDept.Appearance.Options.UseFont = true;
            this.cbbDept.Appearance.Options.UseForeColor = true;
            this.cbbDept.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbDept.AppearanceDropDown.Options.UseFont = true;
            this.cbbDept.AutoHeight = false;
            this.cbbDept.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbDept.Name = "cbbDept";
            this.cbbDept.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // barCbbStatus
            // 
            this.barCbbStatus.Caption = "狀態";
            this.barCbbStatus.Edit = this.cbbStatus;
            this.barCbbStatus.Id = 1;
            this.barCbbStatus.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.barCbbStatus.Name = "barCbbStatus";
            this.barCbbStatus.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barCbbStatus.EditValueChanged += new System.EventHandler(this.Filter_EditValueChanged);
            // 
            // cbbStatus
            // 
            this.cbbStatus.Appearance.Assign(this.cbbDept.Appearance);
            this.cbbStatus.AppearanceDropDown.Assign(this.cbbDept.AppearanceDropDown);
            this.cbbStatus.AutoHeight = false;
            this.cbbStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbStatus.Name = "cbbStatus";
            this.cbbStatus.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // barCbbUser
            // 
            this.barCbbUser.Caption = "經辦";
            this.barCbbUser.Edit = this.cbbUser;
            this.barCbbUser.Id = 2;
            this.barCbbUser.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.barCbbUser.Name = "barCbbUser";
            this.barCbbUser.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barCbbUser.EditValueChanged += new System.EventHandler(this.Filter_EditValueChanged);
            // 
            // cbbUser
            // 
            this.cbbUser.Appearance.Assign(this.cbbDept.Appearance);
            this.cbbUser.AppearanceDropDown.Assign(this.cbbDept.AppearanceDropDown);
            this.cbbUser.AutoHeight = false;
            this.cbbUser.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbUser.Name = "cbbUser";
            this.cbbUser.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // barDateFrom
            // 
            this.barDateFrom.Caption = "起";
            this.barDateFrom.Edit = this.riDateFrom;
            this.barDateFrom.Id = 3;
            this.barDateFrom.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.barDateFrom.Name = "barDateFrom";
            this.barDateFrom.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barDateFrom.EditValueChanged += new System.EventHandler(this.Filter_EditValueChanged);
            // 
            // riDateFrom
            // 
            this.riDateFrom.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.riDateFrom.Appearance.Options.UseFont = true;
            this.riDateFrom.AutoHeight = false;
            this.riDateFrom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riDateFrom.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riDateFrom.MaskSettings.Set("mask", "yyyy/MM/dd");
            this.riDateFrom.Name = "riDateFrom";
            this.riDateFrom.UseMaskAsDisplayFormat = true;
            // 
            // barDateTo
            // 
            this.barDateTo.Caption = "迄";
            this.barDateTo.Edit = this.riDateTo;
            this.barDateTo.Id = 4;
            this.barDateTo.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.barDateTo.Name = "barDateTo";
            this.barDateTo.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barDateTo.EditValueChanged += new System.EventHandler(this.Filter_EditValueChanged);
            // 
            // riDateTo
            // 
            this.riDateTo.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.riDateTo.Appearance.Options.UseFont = true;
            this.riDateTo.AutoHeight = false;
            this.riDateTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riDateTo.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.riDateTo.MaskSettings.Set("mask", "yyyy/MM/dd");
            this.riDateTo.Name = "riDateTo";
            this.riDateTo.UseMaskAsDisplayFormat = true;
            // 
            // btnReload
            // 
            this.btnReload.Caption = "刷新";
            this.btnReload.Id = 5;
            this.btnReload.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnReload.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnReload.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnReload.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReload.ItemAppearance.Normal.Options.UseFont = true;
            this.btnReload.Name = "btnReload";
            this.btnReload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnReload_ItemClick);
            // 
            // btnViewIssue
            // 
            this.btnViewIssue.Caption = "查看領用";
            this.btnViewIssue.Id = 6;
            this.btnViewIssue.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnViewIssue.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnViewIssue.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnViewIssue.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewIssue.ItemAppearance.Normal.Options.UseFont = true;
            this.btnViewIssue.Name = "btnViewIssue";
            this.btnViewIssue.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnViewIssue_ItemClick);
            // 
            // btnDownloadGuide
            // 
            this.btnDownloadGuide.Caption = "下載指引";
            this.btnDownloadGuide.Id = 7;
            this.btnDownloadGuide.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnDownloadGuide.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnDownloadGuide.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnDownloadGuide.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownloadGuide.ItemAppearance.Normal.Options.UseFont = true;
            this.btnDownloadGuide.Name = "btnDownloadGuide";
            this.btnDownloadGuide.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDownloadGuide_ItemClick);
            // 
            // btnManageGuide
            // 
            this.btnManageGuide.Caption = "管理指引";
            this.btnManageGuide.Id = 8;
            this.btnManageGuide.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnManageGuide.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnManageGuide.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnManageGuide.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManageGuide.ItemAppearance.Normal.Options.UseFont = true;
            this.btnManageGuide.Name = "btnManageGuide";
            this.btnManageGuide.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnManageGuide_ItemClick);
            // 
            // btnUploadEvidence
            // 
            this.btnUploadEvidence.Caption = "上傳證明";
            this.btnUploadEvidence.Id = 9;
            this.btnUploadEvidence.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnUploadEvidence.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnUploadEvidence.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnUploadEvidence.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUploadEvidence.ItemAppearance.Normal.Options.UseFont = true;
            this.btnUploadEvidence.Name = "btnUploadEvidence";
            this.btnUploadEvidence.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnUploadEvidence_ItemClick);
            // 
            // btnViewEvidence
            // 
            this.btnViewEvidence.Caption = "查看證明";
            this.btnViewEvidence.Id = 10;
            this.btnViewEvidence.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnViewEvidence.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnViewEvidence.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnViewEvidence.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewEvidence.ItemAppearance.Normal.Options.UseFont = true;
            this.btnViewEvidence.Name = "btnViewEvidence";
            this.btnViewEvidence.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnViewEvidence_ItemClick);
            // 
            // btnUpdateTime
            // 
            this.btnUpdateTime.Caption = "更新時間";
            this.btnUpdateTime.Id = 11;
            this.btnUpdateTime.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnUpdateTime.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnUpdateTime.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnUpdateTime.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateTime.ItemAppearance.Normal.Options.UseFont = true;
            this.btnUpdateTime.Name = "btnUpdateTime";
            this.btnUpdateTime.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnUpdateTime_ItemClick);
            // 
            // btnConfirmComplete
            // 
            this.btnConfirmComplete.Caption = "確認完成";
            this.btnConfirmComplete.Id = 12;
            this.btnConfirmComplete.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnConfirmComplete.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnConfirmComplete.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnConfirmComplete.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmComplete.ItemAppearance.Normal.Options.UseFont = true;
            this.btnConfirmComplete.Name = "btnConfirmComplete";
            this.btnConfirmComplete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirmComplete_ItemClick);
            // 
            // btnCancelTicket
            // 
            this.btnCancelTicket.Caption = "取消單";
            this.btnCancelTicket.Id = 13;
            this.btnCancelTicket.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnCancelTicket.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnCancelTicket.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnCancelTicket.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelTicket.ItemAppearance.Normal.Options.UseFont = true;
            this.btnCancelTicket.Name = "btnCancelTicket";
            this.btnCancelTicket.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCancelTicket_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(1280, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 720);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(1280, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 671);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1280, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 671);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1280, 671);
            this.layoutControl1.TabIndex = 8;
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
            this.gcData.Size = new System.Drawing.Size(1256, 647);
            this.gcData.TabIndex = 4;
            this.gcData.UseEmbeddedNavigator = true;
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
            this.colTicketNo,
            this.colNewMaterialCode,
            this.colNewMaterialDisplayName,
            this.colOldBaseMaterialCode,
            this.colOldBaseMaterialDisplayName,
            this.colRecoveryOptionDisplay,
            this.colQuantity,
            this.colSourceStorageName,
            this.colRestockStorageName,
            this.colAssignedUserName,
            this.colPlannedDisposeDate,
            this.colActualDisposeDate,
            this.colStatusDisplay,
            this.colEvidenceCount,
            this.colCreatedDate,
            this.colDescription});
            this.gvData.GridControl = this.gcData;
            this.gvData.Name = "gvData";
            this.gvData.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvData.OptionsView.ColumnAutoWidth = false;
            this.gvData.OptionsView.EnableAppearanceOddRow = true;
            this.gvData.OptionsView.ShowAutoFilterRow = true;
            this.gvData.OptionsView.ShowGroupPanel = false;
            this.gvData.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvData_PopupMenuShowing);
            // 
            // colTicketNo
            // 
            this.colTicketNo.Caption = "單號";
            this.colTicketNo.FieldName = "TicketNo";
            this.colTicketNo.Name = "colTicketNo";
            this.colTicketNo.Visible = true;
            this.colTicketNo.VisibleIndex = 0;
            this.colTicketNo.Width = 170;
            // 
            // colNewMaterialCode
            // 
            this.colNewMaterialCode.Caption = "新料號";
            this.colNewMaterialCode.FieldName = "NewMaterialCode";
            this.colNewMaterialCode.Name = "colNewMaterialCode";
            this.colNewMaterialCode.Visible = true;
            this.colNewMaterialCode.VisibleIndex = 1;
            this.colNewMaterialCode.Width = 130;
            // 
            // colNewMaterialDisplayName
            // 
            this.colNewMaterialDisplayName.Caption = "新品名規格";
            this.colNewMaterialDisplayName.FieldName = "NewMaterialDisplayName";
            this.colNewMaterialDisplayName.Name = "colNewMaterialDisplayName";
            this.colNewMaterialDisplayName.Visible = true;
            this.colNewMaterialDisplayName.VisibleIndex = 2;
            this.colNewMaterialDisplayName.Width = 220;
            // 
            // colOldBaseMaterialCode
            // 
            this.colOldBaseMaterialCode.Caption = "舊料號";
            this.colOldBaseMaterialCode.FieldName = "OldBaseMaterialCode";
            this.colOldBaseMaterialCode.Name = "colOldBaseMaterialCode";
            this.colOldBaseMaterialCode.Visible = true;
            this.colOldBaseMaterialCode.VisibleIndex = 3;
            this.colOldBaseMaterialCode.Width = 130;
            // 
            // colOldBaseMaterialDisplayName
            // 
            this.colOldBaseMaterialDisplayName.Caption = "舊品名規格";
            this.colOldBaseMaterialDisplayName.FieldName = "OldBaseMaterialDisplayName";
            this.colOldBaseMaterialDisplayName.Name = "colOldBaseMaterialDisplayName";
            this.colOldBaseMaterialDisplayName.Visible = true;
            this.colOldBaseMaterialDisplayName.VisibleIndex = 4;
            this.colOldBaseMaterialDisplayName.Width = 220;
            // 
            // colRecoveryOptionDisplay
            // 
            this.colRecoveryOptionDisplay.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colRecoveryOptionDisplay.Caption = "處理方式";
            this.colRecoveryOptionDisplay.FieldName = "RecoveryOptionDisplay";
            this.colRecoveryOptionDisplay.Name = "colRecoveryOptionDisplay";
            this.colRecoveryOptionDisplay.Visible = true;
            this.colRecoveryOptionDisplay.VisibleIndex = 5;
            this.colRecoveryOptionDisplay.Width = 110;
            // 
            // colQuantity
            // 
            this.colQuantity.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colQuantity.Caption = "數量";
            this.colQuantity.FieldName = "Quantity";
            this.colQuantity.Name = "colQuantity";
            this.colQuantity.Visible = true;
            this.colQuantity.VisibleIndex = 6;
            this.colQuantity.Width = 80;
            // 
            // colSourceStorageName
            // 
            this.colSourceStorageName.Caption = "來源倉庫";
            this.colSourceStorageName.FieldName = "SourceStorageName";
            this.colSourceStorageName.Name = "colSourceStorageName";
            this.colSourceStorageName.Visible = true;
            this.colSourceStorageName.VisibleIndex = 7;
            this.colSourceStorageName.Width = 120;
            // 
            // colRestockStorageName
            // 
            this.colRestockStorageName.Caption = "回收倉庫";
            this.colRestockStorageName.FieldName = "RestockStorageName";
            this.colRestockStorageName.Name = "colRestockStorageName";
            this.colRestockStorageName.Visible = true;
            this.colRestockStorageName.VisibleIndex = 8;
            this.colRestockStorageName.Width = 120;
            // 
            // colAssignedUserName
            // 
            this.colAssignedUserName.Caption = "經辦";
            this.colAssignedUserName.FieldName = "AssignedUserName";
            this.colAssignedUserName.Name = "colAssignedUserName";
            this.colAssignedUserName.Visible = true;
            this.colAssignedUserName.VisibleIndex = 9;
            this.colAssignedUserName.Width = 130;
            // 
            // colPlannedDisposeDate
            // 
            this.colPlannedDisposeDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colPlannedDisposeDate.Caption = "預計時間";
            this.colPlannedDisposeDate.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
            this.colPlannedDisposeDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colPlannedDisposeDate.FieldName = "PlannedDisposeDate";
            this.colPlannedDisposeDate.Name = "colPlannedDisposeDate";
            this.colPlannedDisposeDate.Visible = true;
            this.colPlannedDisposeDate.VisibleIndex = 10;
            this.colPlannedDisposeDate.Width = 150;
            // 
            // colActualDisposeDate
            // 
            this.colActualDisposeDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colActualDisposeDate.Caption = "實際時間";
            this.colActualDisposeDate.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
            this.colActualDisposeDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colActualDisposeDate.FieldName = "ActualDisposeDate";
            this.colActualDisposeDate.Name = "colActualDisposeDate";
            this.colActualDisposeDate.Visible = true;
            this.colActualDisposeDate.VisibleIndex = 11;
            this.colActualDisposeDate.Width = 150;
            // 
            // colStatusDisplay
            // 
            this.colStatusDisplay.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colStatusDisplay.Caption = "狀態";
            this.colStatusDisplay.FieldName = "StatusDisplay";
            this.colStatusDisplay.Name = "colStatusDisplay";
            this.colStatusDisplay.Visible = true;
            this.colStatusDisplay.VisibleIndex = 12;
            this.colStatusDisplay.Width = 120;
            // 
            // colEvidenceCount
            // 
            this.colEvidenceCount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colEvidenceCount.Caption = "證明數";
            this.colEvidenceCount.FieldName = "EvidenceCount";
            this.colEvidenceCount.Name = "colEvidenceCount";
            this.colEvidenceCount.Visible = true;
            this.colEvidenceCount.VisibleIndex = 13;
            this.colEvidenceCount.Width = 90;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colCreatedDate.Caption = "建立時間";
            this.colCreatedDate.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
            this.colCreatedDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.Visible = true;
            this.colCreatedDate.VisibleIndex = 14;
            this.colCreatedDate.Width = 150;
            // 
            // colDescription
            // 
            this.colDescription.Caption = "備註";
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 15;
            this.colDescription.Width = 240;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1280, 671);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1260, 651);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // uc309_RecoveryMgmt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "uc309_RecoveryMgmt";
            this.Size = new System.Drawing.Size(1280, 720);
            this.Load += new System.EventHandler(this.uc309_RecoveryMgmt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riDateFrom.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riDateFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riDateTo.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.riDateTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarEditItem barCbbDept;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cbbDept;
        private DevExpress.XtraBars.BarEditItem barCbbStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cbbStatus;
        private DevExpress.XtraBars.BarEditItem barCbbUser;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cbbUser;
        private DevExpress.XtraBars.BarEditItem barDateFrom;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit riDateFrom;
        private DevExpress.XtraBars.BarEditItem barDateTo;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit riDateTo;
        private DevExpress.XtraBars.BarButtonItem btnReload;
        private DevExpress.XtraBars.BarButtonItem btnViewIssue;
        private DevExpress.XtraBars.BarButtonItem btnDownloadGuide;
        private DevExpress.XtraBars.BarButtonItem btnManageGuide;
        private DevExpress.XtraBars.BarButtonItem btnUploadEvidence;
        private DevExpress.XtraBars.BarButtonItem btnViewEvidence;
        private DevExpress.XtraBars.BarButtonItem btnUpdateTime;
        private DevExpress.XtraBars.BarButtonItem btnConfirmComplete;
        private DevExpress.XtraBars.BarButtonItem btnCancelTicket;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData;
        private DevExpress.XtraGrid.Columns.GridColumn colTicketNo;
        private DevExpress.XtraGrid.Columns.GridColumn colNewMaterialCode;
        private DevExpress.XtraGrid.Columns.GridColumn colNewMaterialDisplayName;
        private DevExpress.XtraGrid.Columns.GridColumn colOldBaseMaterialCode;
        private DevExpress.XtraGrid.Columns.GridColumn colOldBaseMaterialDisplayName;
        private DevExpress.XtraGrid.Columns.GridColumn colRecoveryOptionDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn colQuantity;
        private DevExpress.XtraGrid.Columns.GridColumn colSourceStorageName;
        private DevExpress.XtraGrid.Columns.GridColumn colRestockStorageName;
        private DevExpress.XtraGrid.Columns.GridColumn colAssignedUserName;
        private DevExpress.XtraGrid.Columns.GridColumn colPlannedDisposeDate;
        private DevExpress.XtraGrid.Columns.GridColumn colActualDisposeDate;
        private DevExpress.XtraGrid.Columns.GridColumn colStatusDisplay;
        private DevExpress.XtraGrid.Columns.GridColumn colEvidenceCount;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
