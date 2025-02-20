namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    partial class f307_DoExam
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(f307_DoExam));
            DevExpress.XtraGrid.GridFormatRule gridFormatRule1 = new DevExpress.XtraGrid.GridFormatRule();
            DevExpress.XtraEditors.FormatConditionRuleExpression formatConditionRuleExpression1 = new DevExpress.XtraEditors.FormatConditionRuleExpression();
            this.gColId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.webViewQues = new System.Windows.Forms.WebBrowser();
            this.lbTime = new System.Windows.Forms.Label();
            this.lbQuesNo = new System.Windows.Forms.Label();
            this.txbUserAns = new DevExpress.XtraEditors.TokenEdit();
            this.btnPreviousQues = new DevExpress.XtraEditors.SimpleButton();
            this.btnNextQues = new DevExpress.XtraEditors.SimpleButton();
            this.btnSubmit = new DevExpress.XtraEditors.SimpleButton();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbUserAns.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // gColId
            // 
            this.gColId.AppearanceCell.Options.UseTextOptions = true;
            this.gColId.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gColId.Caption = "題目";
            this.gColId.FieldName = "QuestionIndex";
            this.gColId.MaxWidth = 75;
            this.gColId.MinWidth = 75;
            this.gColId.Name = "gColId";
            this.gColId.OptionsColumn.AllowEdit = false;
            this.gColId.Visible = true;
            this.gColId.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "答案";
            this.gridColumn2.FieldName = "UserAnswer";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.webViewQues);
            this.layoutControl1.Controls.Add(this.lbTime);
            this.layoutControl1.Controls.Add(this.lbQuesNo);
            this.layoutControl1.Controls.Add(this.txbUserAns);
            this.layoutControl1.Controls.Add(this.btnPreviousQues);
            this.layoutControl1.Controls.Add(this.btnNextQues);
            this.layoutControl1.Controls.Add(this.btnSubmit);
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1224, 403, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1201, 666);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // webViewQues
            // 
            this.webViewQues.Location = new System.Drawing.Point(12, 12);
            this.webViewQues.MinimumSize = new System.Drawing.Size(20, 20);
            this.webViewQues.Name = "webViewQues";
            this.webViewQues.Size = new System.Drawing.Size(864, 602);
            this.webViewQues.TabIndex = 13;
            // 
            // lbTime
            // 
            this.lbTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.lbTime.Font = new System.Drawing.Font("Microsoft JhengHei UI", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTime.ForeColor = System.Drawing.Color.Black;
            this.lbTime.Location = new System.Drawing.Point(880, 12);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(309, 49);
            this.lbTime.TabIndex = 12;
            this.lbTime.Text = "剩餘時間 00:00";
            this.lbTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbQuesNo
            // 
            this.lbQuesNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.lbQuesNo.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQuesNo.ForeColor = System.Drawing.Color.Black;
            this.lbQuesNo.Location = new System.Drawing.Point(320, 618);
            this.lbQuesNo.Name = "lbQuesNo";
            this.lbQuesNo.Size = new System.Drawing.Size(133, 36);
            this.lbQuesNo.TabIndex = 11;
            this.lbQuesNo.Text = "題目：1/20";
            this.lbQuesNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txbUserAns
            // 
            this.txbUserAns.Location = new System.Drawing.Point(512, 618);
            this.txbUserAns.MaximumSize = new System.Drawing.Size(0, 36);
            this.txbUserAns.MinimumSize = new System.Drawing.Size(0, 36);
            this.txbUserAns.Name = "txbUserAns";
            this.txbUserAns.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbUserAns.Properties.Appearance.Options.UseFont = true;
            this.txbUserAns.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbUserAns.Properties.AppearanceDropDown.ForeColor = System.Drawing.Color.Black;
            this.txbUserAns.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txbUserAns.Properties.AppearanceDropDown.Options.UseForeColor = true;
            this.txbUserAns.Properties.Separators.AddRange(new string[] {
            ","});
            this.txbUserAns.Size = new System.Drawing.Size(164, 36);
            this.txbUserAns.StyleController = this.layoutControl1;
            this.txbUserAns.TabIndex = 10;
            // 
            // btnPreviousQues
            // 
            this.btnPreviousQues.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreviousQues.Appearance.Options.UseFont = true;
            this.btnPreviousQues.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnPreviousQues.ImageOptions.SvgImage")));
            this.btnPreviousQues.Location = new System.Drawing.Point(680, 618);
            this.btnPreviousQues.Name = "btnPreviousQues";
            this.btnPreviousQues.Size = new System.Drawing.Size(96, 36);
            this.btnPreviousQues.StyleController = this.layoutControl1;
            this.btnPreviousQues.TabIndex = 9;
            this.btnPreviousQues.Text = "上題目";
            this.btnPreviousQues.Click += new System.EventHandler(this.btnPreviousQues_Click);
            // 
            // btnNextQues
            // 
            this.btnNextQues.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNextQues.Appearance.Options.UseFont = true;
            this.btnNextQues.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnNextQues.ImageOptions.SvgImage")));
            this.btnNextQues.Location = new System.Drawing.Point(780, 618);
            this.btnNextQues.Name = "btnNextQues";
            this.btnNextQues.Size = new System.Drawing.Size(96, 36);
            this.btnNextQues.StyleController = this.layoutControl1;
            this.btnNextQues.TabIndex = 8;
            this.btnNextQues.Text = "下題目";
            this.btnNextQues.Click += new System.EventHandler(this.btnNextQues_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Appearance.Options.UseForeColor = true;
            this.btnSubmit.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnSubmit.ImageOptions.SvgImage")));
            this.btnSubmit.Location = new System.Drawing.Point(12, 618);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(96, 36);
            this.btnSubmit.StyleController = this.layoutControl1;
            this.btnSubmit.TabIndex = 7;
            this.btnSubmit.Text = "提交";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // gcData
            // 
            this.gcData.Cursor = System.Windows.Forms.Cursors.Default;
            this.gcData.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Remove.Visible = false;
            this.gcData.Location = new System.Drawing.Point(880, 65);
            this.gcData.MainView = this.gvData;
            this.gcData.Name = "gcData";
            this.gcData.Size = new System.Drawing.Size(309, 589);
            this.gcData.TabIndex = 6;
            this.gcData.UseEmbeddedNavigator = true;
            this.gcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvData});
            this.gcData.DoubleClick += new System.EventHandler(this.gcData_DoubleClick);
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
            this.gColId,
            this.gridColumn2});
            gridFormatRule1.Column = this.gColId;
            gridFormatRule1.ColumnApplyTo = this.gColId;
            gridFormatRule1.Name = "Format0";
            formatConditionRuleExpression1.Appearance.BackColor = System.Drawing.Color.Red;
            formatConditionRuleExpression1.Appearance.BackColor2 = System.Drawing.Color.White;
            formatConditionRuleExpression1.Appearance.Options.UseBackColor = true;
            formatConditionRuleExpression1.Expression = "IsNullOrEmpty([UserAnswer])";
            gridFormatRule1.Rule = formatConditionRuleExpression1;
            this.gvData.FormatRules.Add(gridFormatRule1);
            this.gvData.GridControl = this.gcData;
            this.gvData.Name = "gvData";
            this.gvData.OptionsCustomization.AllowColumnMoving = false;
            this.gvData.OptionsCustomization.AllowFilter = false;
            this.gvData.OptionsCustomization.AllowGroup = false;
            this.gvData.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvData.OptionsCustomization.AllowSort = false;
            this.gvData.OptionsDetail.EnableMasterViewMode = false;
            this.gvData.OptionsDetail.ShowDetailTabs = false;
            this.gvData.OptionsScrollAnnotations.ShowFocusedRow = DevExpress.Utils.DefaultBoolean.False;
            this.gvData.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvData.OptionsView.EnableAppearanceOddRow = true;
            this.gvData.OptionsView.ShowGroupPanel = false;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem5,
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1201, 666);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gcData;
            this.layoutControlItem2.Location = new System.Drawing.Point(868, 53);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(313, 0);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(313, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(313, 593);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSubmit;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 606);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(100, 40);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(100, 40);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(100, 40);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnNextQues;
            this.layoutControlItem4.Location = new System.Drawing.Point(768, 606);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(100, 40);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(100, 40);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(100, 40);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(100, 606);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(208, 40);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem6.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem6.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem6.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem6.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem6.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.layoutControlItem6.Control = this.txbUserAns;
            this.layoutControlItem6.Location = new System.Drawing.Point(445, 606);
            this.layoutControlItem6.MaxSize = new System.Drawing.Size(223, 0);
            this.layoutControlItem6.MinSize = new System.Drawing.Size(223, 36);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(223, 40);
            this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem6.Text = " 答案";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(43, 24);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.lbQuesNo;
            this.layoutControlItem7.Location = new System.Drawing.Point(308, 606);
            this.layoutControlItem7.MaxSize = new System.Drawing.Size(137, 0);
            this.layoutControlItem7.MinSize = new System.Drawing.Size(137, 24);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(137, 40);
            this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.lbTime;
            this.layoutControlItem8.Location = new System.Drawing.Point(868, 0);
            this.layoutControlItem8.MaxSize = new System.Drawing.Size(0, 53);
            this.layoutControlItem8.MinSize = new System.Drawing.Size(24, 53);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(313, 53);
            this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnPreviousQues;
            this.layoutControlItem5.Location = new System.Drawing.Point(668, 606);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(100, 40);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(100, 40);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(100, 40);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.webViewQues;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(868, 606);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // f307_DoExam
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1201, 666);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f307_DoExam";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "執行考試";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.f307_DoExam_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.f307_DoExam_FormClosed);
            this.Load += new System.EventHandler(this.f307_DoExam_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbUserAns.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gColId;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.TokenEdit txbUserAns;
        private DevExpress.XtraEditors.SimpleButton btnPreviousQues;
        private DevExpress.XtraEditors.SimpleButton btnNextQues;
        private DevExpress.XtraEditors.SimpleButton btnSubmit;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private System.Windows.Forms.Label lbQuesNo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private System.Windows.Forms.Label lbTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private System.Windows.Forms.WebBrowser webViewQues;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}