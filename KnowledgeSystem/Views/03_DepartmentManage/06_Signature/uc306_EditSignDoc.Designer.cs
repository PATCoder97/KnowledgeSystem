namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    partial class uc306_EditSignDoc
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.ckConfidential = new DevExpress.XtraEditors.CheckEdit();
            this.txbFieldType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txbTitle = new DevExpress.XtraEditors.TextEdit();
            this.txbDocType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcTitle = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcType = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ckConfidential.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbFieldType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbTitle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDocType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.ckConfidential);
            this.layoutControl1.Controls.Add(this.txbFieldType);
            this.layoutControl1.Controls.Add(this.txbTitle);
            this.layoutControl1.Controls.Add(this.txbDocType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(518, 92);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // ckConfidential
            // 
            this.ckConfidential.Location = new System.Drawing.Point(434, 12);
            this.ckConfidential.Name = "ckConfidential";
            this.ckConfidential.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckConfidential.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.ckConfidential.Properties.Appearance.Options.UseFont = true;
            this.ckConfidential.Properties.Appearance.Options.UseForeColor = true;
            this.ckConfidential.Properties.Caption = "機密";
            this.ckConfidential.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgFlag1;
            this.ckConfidential.Properties.CheckBoxOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.ckConfidential.Properties.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.ckConfidential.Size = new System.Drawing.Size(72, 28);
            this.ckConfidential.StyleController = this.layoutControl1;
            this.ckConfidential.TabIndex = 25;
            // 
            // txbFieldType
            // 
            this.txbFieldType.Location = new System.Drawing.Point(128, 12);
            this.txbFieldType.Name = "txbFieldType";
            this.txbFieldType.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbFieldType.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbFieldType.Properties.Appearance.Options.UseFont = true;
            this.txbFieldType.Properties.Appearance.Options.UseForeColor = true;
            this.txbFieldType.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbFieldType.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txbFieldType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbFieldType.Properties.NullText = "";
            this.txbFieldType.Properties.PopupView = this.gridView1;
            this.txbFieldType.Size = new System.Drawing.Size(105, 32);
            this.txbFieldType.StyleController = this.layoutControl1;
            this.txbFieldType.TabIndex = 24;
            this.txbFieldType.EditValueChanged += new System.EventHandler(this.txbFieldType_EditValueChanged);
            // 
            // gridView1
            // 
            this.gridView1.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView1.Appearance.Row.Options.UseFont = true;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1});
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.EnableAppearanceOddRow = true;
            this.gridView1.OptionsView.ShowAutoFilterRow = true;
            this.gridView1.OptionsView.ShowColumnHeaders = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumn5";
            this.gridColumn1.FieldName = "DisplayName";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // txbTitle
            // 
            this.txbTitle.Location = new System.Drawing.Point(128, 48);
            this.txbTitle.Name = "txbTitle";
            this.txbTitle.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbTitle.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbTitle.Properties.Appearance.Options.UseFont = true;
            this.txbTitle.Properties.Appearance.Options.UseForeColor = true;
            this.txbTitle.Size = new System.Drawing.Size(378, 32);
            this.txbTitle.StyleController = this.layoutControl1;
            this.txbTitle.TabIndex = 11;
            // 
            // txbDocType
            // 
            this.txbDocType.Location = new System.Drawing.Point(327, 12);
            this.txbDocType.Name = "txbDocType";
            this.txbDocType.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbDocType.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbDocType.Properties.Appearance.Options.UseFont = true;
            this.txbDocType.Properties.Appearance.Options.UseForeColor = true;
            this.txbDocType.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDocType.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txbDocType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbDocType.Properties.NullText = "";
            this.txbDocType.Properties.PopupView = this.gridLookUpEdit1View;
            this.txbDocType.Size = new System.Drawing.Size(103, 32);
            this.txbDocType.StyleController = this.layoutControl1;
            this.txbDocType.TabIndex = 23;
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridLookUpEdit1View.Appearance.Row.Options.UseFont = true;
            this.gridLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn5});
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.EnableAppearanceOddRow = true;
            this.gridLookUpEdit1View.OptionsView.ShowAutoFilterRow = true;
            this.gridLookUpEdit1View.OptionsView.ShowColumnHeaders = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "gridColumn5";
            this.gridColumn5.FieldName = "DisplayName";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 0;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcTitle,
            this.lcType,
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(518, 92);
            this.Root.TextVisible = false;
            // 
            // lcTitle
            // 
            this.lcTitle.AllowHtmlStringInCaption = true;
            this.lcTitle.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcTitle.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcTitle.AppearanceItemCaption.Options.UseFont = true;
            this.lcTitle.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcTitle.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcTitle.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcTitle.Control = this.txbTitle;
            this.lcTitle.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcTitle.CustomizationFormText = "編碼";
            this.lcTitle.Location = new System.Drawing.Point(0, 36);
            this.lcTitle.Name = "lcTitle";
            this.lcTitle.OptionsToolTip.AllowHtmlString = DevExpress.Utils.DefaultBoolean.True;
            this.lcTitle.Size = new System.Drawing.Size(498, 36);
            this.lcTitle.Text = "名稱<color=red>*</color>";
            this.lcTitle.TextSize = new System.Drawing.Size(104, 24);
            // 
            // lcType
            // 
            this.lcType.AllowHtmlStringInCaption = true;
            this.lcType.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcType.AppearanceItemCaption.Options.UseFont = true;
            this.lcType.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcType.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcType.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcType.Control = this.txbDocType;
            this.lcType.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcType.CustomizationFormText = "類別<color=red>*</color>";
            this.lcType.Location = new System.Drawing.Point(225, 0);
            this.lcType.Name = "lcType";
            this.lcType.Size = new System.Drawing.Size(197, 36);
            this.lcType.Text = "文件種類<color=red>*</color>";
            this.lcType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lcType.TextSize = new System.Drawing.Size(85, 24);
            this.lcType.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AllowHtmlStringInCaption = true;
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem1.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.layoutControlItem1.Control = this.txbFieldType;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(225, 36);
            this.layoutControlItem1.Text = "作業機能類<color=red>*</color>";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(104, 24);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.ckConfidential;
            this.layoutControlItem2.Location = new System.Drawing.Point(422, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(76, 36);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // uc306_EditSignDoc
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc306_EditSignDoc";
            this.Size = new System.Drawing.Size(518, 92);
            this.Load += new System.EventHandler(this.uc306_EditSignDoc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ckConfidential.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbFieldType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbTitle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDocType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txbTitle;
        private DevExpress.XtraEditors.GridLookUpEdit txbDocType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraLayout.LayoutControlItem lcTitle;
        private DevExpress.XtraLayout.LayoutControlItem lcType;
        private DevExpress.XtraEditors.GridLookUpEdit txbFieldType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.CheckEdit ckConfidential;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
