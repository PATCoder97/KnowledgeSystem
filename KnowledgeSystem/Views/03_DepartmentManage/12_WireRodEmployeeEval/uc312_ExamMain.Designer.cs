namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class uc312_ExamMain
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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            this.gvEmp = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColEmpId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcExamResult = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.lvData = new DevExpress.XtraGrid.Views.Layout.LayoutView();
            this.gColId = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_gColId = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gColCode = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_layoutViewColumn1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.txbExamName = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.layoutViewField_gridColumn1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_gridColumn2 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_gridColumn4 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gColEnterDate = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField_gColEnterDate = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewCard1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewCard();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.gvEmp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcExamResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gColId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_layoutViewColumn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExamName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gColEnterDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // gvEmp
            // 
            this.gvEmp.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvEmp.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Blue;
            this.gvEmp.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvEmp.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvEmp.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvEmp.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvEmp.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvEmp.Appearance.Row.Options.UseFont = true;
            this.gvEmp.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColEmpId,
            this.gridColumn3,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7});
            this.gvEmp.GridControl = this.gcExamResult;
            this.gvEmp.Name = "gvEmp";
            this.gvEmp.OptionsCustomization.AllowSort = false;
            this.gvEmp.OptionsView.ColumnAutoWidth = false;
            this.gvEmp.OptionsView.EnableAppearanceOddRow = true;
            this.gvEmp.OptionsView.ShowGroupPanel = false;
            this.gvEmp.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn5, DevExpress.Data.ColumnSortOrder.Descending)});
            this.gvEmp.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvEmp_PopupMenuShowing);
            // 
            // gColEmpId
            // 
            this.gColEmpId.Caption = "gColEmpId";
            this.gColEmpId.FieldName = "exam.Id";
            this.gColEmpId.Name = "gColEmpId";
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "考試名稱";
            this.gridColumn3.FieldName = "data.DisplayName";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "提交時期";
            this.gridColumn5.FieldName = "exam.SubmitAt";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 1;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "分數";
            this.gridColumn6.FieldName = "exam.Score";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 2;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "合格";
            this.gridColumn7.FieldName = "exam.IsPass";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 3;
            // 
            // gcExamResult
            // 
            this.gcExamResult.Cursor = System.Windows.Forms.Cursors.Default;
            this.gcExamResult.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcExamResult.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcExamResult.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcExamResult.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcExamResult.EmbeddedNavigator.Buttons.Remove.Visible = false;
            gridLevelNode1.RelationName = "emp";
            this.gcExamResult.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.gcExamResult.Location = new System.Drawing.Point(517, 40);
            this.gcExamResult.MainView = this.gvEmp;
            this.gcExamResult.Name = "gcExamResult";
            this.gcExamResult.Size = new System.Drawing.Size(354, 546);
            this.gcExamResult.TabIndex = 7;
            this.gcExamResult.UseEmbeddedNavigator = true;
            this.gcExamResult.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvEmp,
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
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10});
            this.gvData.GridControl = this.gcExamResult;
            this.gvData.Name = "gvData";
            this.gvData.OptionsDetail.ShowDetailTabs = false;
            this.gvData.OptionsSelection.CheckBoxSelectorColumnWidth = 50;
            this.gvData.OptionsView.ColumnAutoWidth = false;
            this.gvData.OptionsView.EnableAppearanceOddRow = true;
            this.gvData.OptionsView.ShowAutoFilterRow = true;
            this.gvData.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn8
            // 
            this.gridColumn8.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn8.Caption = "ID";
            this.gridColumn8.FieldName = "Id";
            this.gridColumn8.Name = "gridColumn8";
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "考試名稱";
            this.gridColumn9.FieldName = "DisplayName";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 0;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "建立日期";
            this.gridColumn10.FieldName = "CreateAt";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 1;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcExamResult);
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(883, 598);
            this.layoutControl1.TabIndex = 5;
            this.layoutControl1.Text = "layoutControl1";
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
            this.gcData.Size = new System.Drawing.Size(501, 574);
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
            this.layoutViewField_gridColumn2.Location = new System.Drawing.Point(0, 111);
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
            this.layoutViewField_gridColumn4.Location = new System.Drawing.Point(0, 151);
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
            this.layoutViewField_gColEnterDate.Location = new System.Drawing.Point(0, 71);
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
            this.layoutViewField_gColEnterDate});
            this.layoutViewCard1.Name = "layoutViewCard1";
            this.layoutViewCard1.OptionsItemText.TextToControlDistance = 0;
            this.layoutViewCard1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutViewCard1.Text = "TemplateCard";
            this.layoutViewCard1.TextLocation = DevExpress.Utils.Locations.Default;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(883, 598);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(505, 578);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.Control = this.gcExamResult;
            this.layoutControlItem2.Location = new System.Drawing.Point(505, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(358, 578);
            this.layoutControlItem2.Text = "考試結果";
            this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(76, 24);
            // 
            // uc312_ExamMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc312_ExamMain";
            this.Size = new System.Drawing.Size(883, 598);
            this.Load += new System.EventHandler(this.uc312_ExamMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvEmp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcExamResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gColId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_layoutViewColumn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExamName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumn4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gColEnterDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraGrid.Views.Layout.LayoutView lvData;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gColId;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gColId;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gColCode;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_layoutViewColumn1;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit txbExamName;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumn1;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gridColumn2;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumn2;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gridColumn4;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumn4;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gColEnterDate;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gColEnterDate;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewCard layoutViewCard1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl gcExamResult;
        private DevExpress.XtraGrid.Views.Grid.GridView gvEmp;
        private DevExpress.XtraGrid.Columns.GridColumn gColEmpId;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
