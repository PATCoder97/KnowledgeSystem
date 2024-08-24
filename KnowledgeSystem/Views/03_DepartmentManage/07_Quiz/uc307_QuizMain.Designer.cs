namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    partial class uc307_QuizMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc307_QuizMain));
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.lvData = new DevExpress.XtraGrid.Views.Layout.LayoutView();
            this.gColId = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_gColId = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gColCode = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_layoutViewColumn1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.txbExamName = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.layoutViewField_gridColumn1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_gridColumn3 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_gridColumn2 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_gridColumn4 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gColEnterDate = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_gColEnterDate = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewCard1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewCard();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.grPractise = new DevExpress.XtraBars.BarSubItem();
            this.btnPractiseMyJob = new DevExpress.XtraBars.BarButtonItem();
            this.btnPractiseOtherJob = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gColId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_layoutViewColumn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExamName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gColEnterDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // gcData
            // 
            this.gcData.Cursor = System.Windows.Forms.Cursors.Default;
            this.gcData.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Remove.Visible = false;
            this.gcData.Location = new System.Drawing.Point(12, 12);
            this.gcData.MainView = this.lvData;
            this.gcData.Name = "gcData";
            this.gcData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.txbExamName});
            this.gcData.Size = new System.Drawing.Size(878, 529);
            this.gcData.TabIndex = 5;
            this.gcData.UseEmbeddedNavigator = true;
            this.gcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.lvData});
            // 
            // lvData
            // 
            this.lvData.Appearance.Card.Options.UseFont = true;
            this.lvData.Appearance.FieldCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lvData.Appearance.FieldCaption.Options.UseFont = true;
            this.lvData.Appearance.FieldValue.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lvData.Appearance.FieldValue.ForeColor = System.Drawing.Color.Blue;
            this.lvData.Appearance.FieldValue.Options.UseFont = true;
            this.lvData.Appearance.FieldValue.Options.UseForeColor = true;
            this.lvData.Appearance.FocusedCardCaption.ForeColor = System.Drawing.Color.Blue;
            this.lvData.Appearance.FocusedCardCaption.Options.UseForeColor = true;
            this.lvData.Appearance.SelectedCardCaption.ForeColor = System.Drawing.Color.Red;
            this.lvData.Appearance.SelectedCardCaption.Options.UseForeColor = true;
            this.lvData.CardMinSize = new System.Drawing.Size(221, 250);
            this.lvData.Columns.AddRange(new DevExpress.XtraGrid.Columns.LayoutViewColumn[] {
            this.gColId,
            this.gColCode,
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn2,
            this.gridColumn4,
            this.gColEnterDate});
            this.lvData.GridControl = this.gcData;
            this.lvData.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutViewField_gColId,
            this.layoutViewField_layoutViewColumn1});
            this.lvData.Name = "lvData";
            this.lvData.OptionsBehavior.Editable = false;
            this.lvData.OptionsCustomization.AllowFilter = false;
            this.lvData.OptionsCustomization.AllowSort = false;
            this.lvData.OptionsItemText.AlignMode = DevExpress.XtraGrid.Views.Layout.FieldTextAlignMode.AutoSize;
            this.lvData.OptionsItemText.TextToControlDistance = 0;
            this.lvData.OptionsView.AllowHotTrackFields = false;
            this.lvData.OptionsView.ShowCardCaption = false;
            this.lvData.OptionsView.ShowCardExpandButton = false;
            this.lvData.OptionsView.ShowCardFieldBorders = true;
            this.lvData.OptionsView.ShowFieldHints = false;
            this.lvData.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            this.lvData.OptionsView.ShowHeaderPanel = false;
            this.lvData.OptionsView.ViewMode = DevExpress.XtraGrid.Views.Layout.LayoutViewMode.Row;
            this.lvData.TemplateCard = this.layoutViewCard1;
            this.lvData.DoubleClick += new System.EventHandler(this.lvData_DoubleClick);
            // 
            // gColId
            // 
            this.gColId.AppearanceCell.Options.UseTextOptions = true;
            this.gColId.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gColId.Caption = "data.Id";
            this.gColId.FieldName = "exam.Id";
            this.gColId.LayoutViewField = this.layoutViewField_gColId;
            this.gColId.Name = "gColId";
            // 
            // layoutViewField_gColId
            // 
            this.layoutViewField_gColId.EditorPreferredWidth = 20;
            this.layoutViewField_gColId.Location = new System.Drawing.Point(0, 0);
            this.layoutViewField_gColId.Name = "layoutViewField_gColId";
            this.layoutViewField_gColId.Size = new System.Drawing.Size(217, 231);
            this.layoutViewField_gColId.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutViewField_gColId.TextSize = new System.Drawing.Size(14, 14);
            // 
            // gColCode
            // 
            this.gColCode.Caption = "layoutViewColumn1";
            this.gColCode.FieldName = "exam.IdJob";
            this.gColCode.LayoutViewField = this.layoutViewField_layoutViewColumn1;
            this.gColCode.Name = "gColCode";
            // 
            // layoutViewField_layoutViewColumn1
            // 
            this.layoutViewField_layoutViewColumn1.EditorPreferredWidth = 10;
            this.layoutViewField_layoutViewColumn1.Location = new System.Drawing.Point(0, 231);
            this.layoutViewField_layoutViewColumn1.Name = "layoutViewField_layoutViewColumn1";
            this.layoutViewField_layoutViewColumn1.Size = new System.Drawing.Size(217, 26);
            this.layoutViewField_layoutViewColumn1.TextSize = new System.Drawing.Size(68, 20);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "考試名稱";
            this.gridColumn1.ColumnEdit = this.txbExamName;
            this.gridColumn1.FieldName = "data.DisplayName";
            this.gridColumn1.LayoutViewField = this.layoutViewField_gridColumn1;
            this.gridColumn1.Name = "gridColumn1";
            // 
            // txbExamName
            // 
            this.txbExamName.LinesCount = 3;
            this.txbExamName.Name = "txbExamName";
            // 
            // layoutViewField_gridColumn1
            // 
            this.layoutViewField_gridColumn1.EditorPreferredWidth = 213;
            this.layoutViewField_gridColumn1.Location = new System.Drawing.Point(0, 0);
            this.layoutViewField_gridColumn1.Name = "layoutViewField_gridColumn1";
            this.layoutViewField_gridColumn1.Size = new System.Drawing.Size(217, 71);
            this.layoutViewField_gridColumn1.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutViewField_gridColumn1.TextSize = new System.Drawing.Size(47, 14);
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "考試編號";
            this.gridColumn3.FieldName = "data.Code";
            this.gridColumn3.LayoutViewField = this.layoutViewField_gridColumn3;
            this.gridColumn3.Name = "gridColumn3";
            // 
            // layoutViewField_gridColumn3
            // 
            this.layoutViewField_gridColumn3.EditorPreferredWidth = 213;
            this.layoutViewField_gridColumn3.Location = new System.Drawing.Point(0, 71);
            this.layoutViewField_gridColumn3.Name = "layoutViewField_gridColumn3";
            this.layoutViewField_gridColumn3.Size = new System.Drawing.Size(217, 40);
            this.layoutViewField_gridColumn3.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutViewField_gridColumn3.TextSize = new System.Drawing.Size(47, 14);
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "及格分數";
            this.gridColumn2.FieldName = "PassingScore";
            this.gridColumn2.LayoutViewField = this.layoutViewField_gridColumn2;
            this.gridColumn2.Name = "gridColumn2";
            // 
            // layoutViewField_gridColumn2
            // 
            this.layoutViewField_gridColumn2.EditorPreferredWidth = 213;
            this.layoutViewField_gridColumn2.Location = new System.Drawing.Point(0, 151);
            this.layoutViewField_gridColumn2.Name = "layoutViewField_gridColumn2";
            this.layoutViewField_gridColumn2.Size = new System.Drawing.Size(217, 40);
            this.layoutViewField_gridColumn2.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutViewField_gridColumn2.TextSize = new System.Drawing.Size(47, 14);
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "考試時期";
            this.gridColumn4.FieldName = "TestDuration";
            this.gridColumn4.LayoutViewField = this.layoutViewField_gridColumn4;
            this.gridColumn4.Name = "gridColumn4";
            // 
            // layoutViewField_gridColumn4
            // 
            this.layoutViewField_gridColumn4.EditorPreferredWidth = 213;
            this.layoutViewField_gridColumn4.Location = new System.Drawing.Point(0, 191);
            this.layoutViewField_gridColumn4.Name = "layoutViewField_gridColumn4";
            this.layoutViewField_gridColumn4.Size = new System.Drawing.Size(217, 40);
            this.layoutViewField_gridColumn4.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutViewField_gridColumn4.TextSize = new System.Drawing.Size(47, 14);
            // 
            // gColEnterDate
            // 
            this.gColEnterDate.Caption = "題目數量";
            this.gColEnterDate.FieldName = "QuesCount";
            this.gColEnterDate.LayoutViewField = this.layoutViewField_gColEnterDate;
            this.gColEnterDate.Name = "gColEnterDate";
            // 
            // layoutViewField_gColEnterDate
            // 
            this.layoutViewField_gColEnterDate.EditorPreferredWidth = 213;
            this.layoutViewField_gColEnterDate.Location = new System.Drawing.Point(0, 111);
            this.layoutViewField_gColEnterDate.Name = "layoutViewField_gColEnterDate";
            this.layoutViewField_gColEnterDate.Size = new System.Drawing.Size(217, 40);
            this.layoutViewField_gColEnterDate.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutViewField_gColEnterDate.TextSize = new System.Drawing.Size(47, 14);
            // 
            // layoutViewCard1
            // 
            this.layoutViewCard1.CustomizationFormText = "TemplateCard";
            this.layoutViewCard1.GroupBordersVisible = false;
            this.layoutViewCard1.HeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.layoutViewCard1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutViewField_gridColumn1,
            this.layoutViewField_gridColumn2,
            this.layoutViewField_gridColumn4,
            this.layoutViewField_gridColumn3,
            this.layoutViewField_gColEnterDate});
            this.layoutViewCard1.Name = "layoutViewCard1";
            this.layoutViewCard1.OptionsItemText.TextToControlDistance = 0;
            this.layoutViewCard1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutViewCard1.Text = "TemplateCard";
            this.layoutViewCard1.TextLocation = DevExpress.Utils.Locations.Default;
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
            this.btnReload,
            this.btnExportExcel,
            this.btnPractiseMyJob,
            this.grPractise,
            this.btnPractiseOtherJob});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 14;
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnReload, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnExportExcel, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.grPractise, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnReload
            // 
            this.btnReload.Caption = "重新整理";
            this.btnReload.Id = 1;
            this.btnReload.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnReload.ImageOptions.SvgImage")));
            this.btnReload.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnReload.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnReload.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnReload.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReload.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnReload.ItemAppearance.Normal.Options.UseFont = true;
            this.btnReload.ItemAppearance.Normal.Options.UseForeColor = true;
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
            this.btnExportExcel.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnExportExcel.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExportExcel.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportExcel_ItemClick);
            // 
            // grPractise
            // 
            this.grPractise.Caption = "練習考試";
            this.grPractise.Id = 12;
            this.grPractise.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.grPractise.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.grPractise.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.grPractise.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grPractise.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.grPractise.ItemAppearance.Normal.Options.UseFont = true;
            this.grPractise.ItemAppearance.Normal.Options.UseForeColor = true;
            this.grPractise.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnPractiseMyJob),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnPractiseOtherJob)});
            this.grPractise.Name = "grPractise";
            // 
            // btnPractiseMyJob
            // 
            this.btnPractiseMyJob.Caption = "當前職務";
            this.btnPractiseMyJob.Id = 11;
            this.btnPractiseMyJob.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnPractiseMyJob.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnPractiseMyJob.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnPractiseMyJob.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPractiseMyJob.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnPractiseMyJob.ItemAppearance.Normal.Options.UseFont = true;
            this.btnPractiseMyJob.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnPractiseMyJob.Name = "btnPractiseMyJob";
            this.btnPractiseMyJob.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnPractiseMyJob_ItemClick);
            // 
            // btnPractiseOtherJob
            // 
            this.btnPractiseOtherJob.Caption = "其他職務";
            this.btnPractiseOtherJob.Id = 13;
            this.btnPractiseOtherJob.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnPractiseOtherJob.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnPractiseOtherJob.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnPractiseOtherJob.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPractiseOtherJob.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnPractiseOtherJob.ItemAppearance.Normal.Options.UseFont = true;
            this.btnPractiseOtherJob.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnPractiseOtherJob.Name = "btnPractiseOtherJob";
            this.btnPractiseOtherJob.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnPractiseOtherJob_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(902, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 602);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(902, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 553);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(902, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 553);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(902, 553);
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
            this.Root.Size = new System.Drawing.Size(902, 553);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(882, 533);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // uc307_QuizMain
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "uc307_QuizMain";
            this.Size = new System.Drawing.Size(902, 602);
            this.Load += new System.EventHandler(this.uc307_QuizMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gColId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_layoutViewColumn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExamName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gColEnterDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard1)).EndInit();
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
        private DevExpress.XtraBars.BarButtonItem btnReload;
        private DevExpress.XtraBars.BarButtonItem btnExportExcel;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem btnPractiseMyJob;
        private DevExpress.XtraGrid.Views.Layout.LayoutView lvData;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gColId;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gColEnterDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit txbExamName;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gColId;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumn1;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumn3;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumn2;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumn4;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gColEnterDate;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewCard layoutViewCard1;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gColCode;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_layoutViewColumn1;
        private DevExpress.XtraBars.BarSubItem grPractise;
        private DevExpress.XtraBars.BarButtonItem btnPractiseOtherJob;
    }
}
