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
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.txbTitle = new DevExpress.XtraEditors.TextEdit();
            this.txbType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txbCode = new DevExpress.XtraEditors.TextEdit();
            this.lcTitle = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcType = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbTitle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcType)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txbTitle);
            this.layoutControl1.Controls.Add(this.txbCode);
            this.layoutControl1.Controls.Add(this.txbType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(518, 92);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcTitle,
            this.layoutControlItem8,
            this.lcType});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(518, 92);
            this.Root.TextVisible = false;
            // 
            // txbTitle
            // 
            this.txbTitle.Location = new System.Drawing.Point(71, 48);
            this.txbTitle.Name = "txbTitle";
            this.txbTitle.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbTitle.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbTitle.Properties.Appearance.Options.UseFont = true;
            this.txbTitle.Properties.Appearance.Options.UseForeColor = true;
            this.txbTitle.Size = new System.Drawing.Size(435, 32);
            this.txbTitle.StyleController = this.layoutControl1;
            this.txbTitle.TabIndex = 11;
            // 
            // txbType
            // 
            this.txbType.Location = new System.Drawing.Point(320, 12);
            this.txbType.Name = "txbType";
            this.txbType.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbType.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbType.Properties.Appearance.Options.UseFont = true;
            this.txbType.Properties.Appearance.Options.UseForeColor = true;
            this.txbType.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbType.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbType.Properties.NullText = "";
            this.txbType.Properties.PopupView = this.gridLookUpEdit1View;
            this.txbType.Size = new System.Drawing.Size(186, 32);
            this.txbType.StyleController = this.layoutControl1;
            this.txbType.TabIndex = 23;
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
            // txbCode
            // 
            this.txbCode.Location = new System.Drawing.Point(71, 12);
            this.txbCode.Name = "txbCode";
            this.txbCode.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbCode.Properties.Appearance.Options.UseFont = true;
            this.txbCode.Properties.Appearance.Options.UseForeColor = true;
            this.txbCode.Size = new System.Drawing.Size(186, 32);
            this.txbCode.StyleController = this.layoutControl1;
            this.txbCode.TabIndex = 11;
            // 
            // lcTitle
            // 
            this.lcTitle.AllowHtmlStringInCaption = true;
            this.lcTitle.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcTitle.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.lcTitle.AppearanceItemCaption.Options.UseFont = true;
            this.lcTitle.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcTitle.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.White;
            this.lcTitle.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcTitle.Control = this.txbTitle;
            this.lcTitle.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcTitle.CustomizationFormText = "編碼";
            this.lcTitle.Location = new System.Drawing.Point(0, 36);
            this.lcTitle.Name = "lcTitle";
            this.lcTitle.OptionsToolTip.AllowHtmlString = DevExpress.Utils.DefaultBoolean.True;
            this.lcTitle.Size = new System.Drawing.Size(498, 36);
            this.lcTitle.Text = "名稱<color=red>*</color>";
            this.lcTitle.TextSize = new System.Drawing.Size(47, 24);
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutControlItem8.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.layoutControlItem8.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem8.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem8.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.White;
            this.layoutControlItem8.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.layoutControlItem8.Control = this.txbCode;
            this.layoutControlItem8.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.layoutControlItem8.CustomizationFormText = "編碼";
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(249, 36);
            this.layoutControlItem8.Text = "編號";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(47, 24);
            // 
            // lcType
            // 
            this.lcType.AllowHtmlStringInCaption = true;
            this.lcType.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcType.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.lcType.AppearanceItemCaption.Options.UseFont = true;
            this.lcType.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcType.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.White;
            this.lcType.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcType.Control = this.txbType;
            this.lcType.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcType.CustomizationFormText = "類別<color=red>*</color>";
            this.lcType.Location = new System.Drawing.Point(249, 0);
            this.lcType.Name = "lcType";
            this.lcType.Size = new System.Drawing.Size(249, 36);
            this.lcType.Text = "類別<color=red>*</color>";
            this.lcType.TextSize = new System.Drawing.Size(47, 24);
            // 
            // uc306_EditSignDoc
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc306_EditSignDoc";
            this.Size = new System.Drawing.Size(518, 92);
            this.Load += new System.EventHandler(this.uc306_EditSignDoc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbTitle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txbTitle;
        private DevExpress.XtraEditors.TextEdit txbCode;
        private DevExpress.XtraEditors.GridLookUpEdit txbType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraLayout.LayoutControlItem lcTitle;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem lcType;
    }
}
