namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    partial class uc308_ExportReport
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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc308_ExportReport));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txbFilePath = new DevExpress.XtraEditors.ButtonEdit();
            this.cbbYear = new DevExpress.XtraEditors.ComboBoxEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcFilePath = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDocLevel = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbFilePath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbYear.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcFilePath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDocLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txbFilePath);
            this.layoutControl1.Controls.Add(this.cbbYear);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(622, 40);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txbFilePath
            // 
            this.txbFilePath.Location = new System.Drawing.Point(264, 4);
            this.txbFilePath.Name = "txbFilePath";
            this.txbFilePath.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbFilePath.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbFilePath.Properties.Appearance.Options.UseFont = true;
            this.txbFilePath.Properties.Appearance.Options.UseForeColor = true;
            editorButtonImageOptions1.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("editorButtonImageOptions1.SvgImage")));
            editorButtonImageOptions1.SvgImageSize = new System.Drawing.Size(16, 16);
            this.txbFilePath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Paste", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)});
            this.txbFilePath.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.txbFilePath.Size = new System.Drawing.Size(354, 32);
            this.txbFilePath.StyleController = this.layoutControl1;
            this.txbFilePath.TabIndex = 17;
            this.txbFilePath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbFilePath_ButtonClick);
            // 
            // cbbYear
            // 
            this.cbbYear.Location = new System.Drawing.Point(54, 4);
            this.cbbYear.Name = "cbbYear";
            this.cbbYear.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbYear.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbYear.Properties.Appearance.Options.UseFont = true;
            this.cbbYear.Properties.Appearance.Options.UseForeColor = true;
            this.cbbYear.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbYear.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbYear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbYear.Properties.DropDownRows = 5;
            this.cbbYear.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbbYear.Size = new System.Drawing.Size(118, 32);
            this.cbbYear.StyleController = this.layoutControl1;
            this.cbbYear.TabIndex = 18;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcFilePath,
            this.lcDocLevel});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.Root.Size = new System.Drawing.Size(622, 40);
            this.Root.TextVisible = false;
            // 
            // lcFilePath
            // 
            this.lcFilePath.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcFilePath.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcFilePath.AppearanceItemCaption.Options.UseFont = true;
            this.lcFilePath.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcFilePath.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcFilePath.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcFilePath.Control = this.txbFilePath;
            this.lcFilePath.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcFilePath.CustomizationFormText = "英越文名稱";
            this.lcFilePath.Location = new System.Drawing.Point(172, 0);
            this.lcFilePath.Name = "lcFilePath";
            this.lcFilePath.Size = new System.Drawing.Size(446, 36);
            this.lcFilePath.Text = "請假檔案";
            this.lcFilePath.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcDocLevel
            // 
            this.lcDocLevel.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcDocLevel.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcDocLevel.AppearanceItemCaption.Options.UseFont = true;
            this.lcDocLevel.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcDocLevel.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcDocLevel.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcDocLevel.Control = this.cbbYear;
            this.lcDocLevel.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcDocLevel.CustomizationFormText = "類別";
            this.lcDocLevel.Location = new System.Drawing.Point(0, 0);
            this.lcDocLevel.Name = "lcDocLevel";
            this.lcDocLevel.Size = new System.Drawing.Size(172, 36);
            this.lcDocLevel.Text = "年份";
            this.lcDocLevel.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lcDocLevel.TextSize = new System.Drawing.Size(38, 24);
            this.lcDocLevel.TextToControlDistance = 12;
            // 
            // uc308_ExportReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc308_ExportReport";
            this.Size = new System.Drawing.Size(622, 40);
            this.Load += new System.EventHandler(this.uc308_ExportReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbFilePath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbYear.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcFilePath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDocLevel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.ButtonEdit txbFilePath;
        private DevExpress.XtraLayout.LayoutControlItem lcFilePath;
        private DevExpress.XtraEditors.ComboBoxEdit cbbYear;
        private DevExpress.XtraLayout.LayoutControlItem lcDocLevel;
    }
}
