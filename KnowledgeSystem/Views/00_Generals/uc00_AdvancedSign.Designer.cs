namespace KnowledgeSystem.Views._00_Generals
{
    partial class uc00_AdvancedSign
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
            this.txbMoreInfo = new DevExpress.XtraEditors.MemoEdit();
            this.picSign = new System.Windows.Forms.PictureBox();
            this.txbDate = new DevExpress.XtraEditors.DateEdit();
            this.cbbSign = new DevExpress.XtraEditors.LookUpEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.cbbDateFormat = new DevExpress.XtraEditors.ComboBoxEdit();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbMoreInfo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbSign.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDateFormat.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cbbDateFormat);
            this.layoutControl1.Controls.Add(this.txbMoreInfo);
            this.layoutControl1.Controls.Add(this.picSign);
            this.layoutControl1.Controls.Add(this.txbDate);
            this.layoutControl1.Controls.Add(this.cbbSign);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(427, 360);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txbMoreInfo
            // 
            this.txbMoreInfo.Location = new System.Drawing.Point(62, 265);
            this.txbMoreInfo.Name = "txbMoreInfo";
            this.txbMoreInfo.Size = new System.Drawing.Size(353, 83);
            this.txbMoreInfo.StyleController = this.layoutControl1;
            this.txbMoreInfo.TabIndex = 7;
            this.txbMoreInfo.EditValueChanged += new System.EventHandler(this.txbMoreInfo_EditValueChanged);
            // 
            // picSign
            // 
            this.picSign.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSign.Location = new System.Drawing.Point(62, 84);
            this.picSign.Name = "picSign";
            this.picSign.Size = new System.Drawing.Size(353, 177);
            this.picSign.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picSign.TabIndex = 6;
            this.picSign.TabStop = false;
            // 
            // txbDate
            // 
            this.txbDate.EditValue = null;
            this.txbDate.Location = new System.Drawing.Point(62, 48);
            this.txbDate.Name = "txbDate";
            this.txbDate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbDate.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbDate.Properties.Appearance.Options.UseFont = true;
            this.txbDate.Properties.Appearance.Options.UseForeColor = true;
            this.txbDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbDate.Properties.DisplayFormat.FormatString = "yyyyMMdd";
            this.txbDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txbDate.Properties.EditFormat.FormatString = "yyyyMMdd";
            this.txbDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txbDate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.txbDate.Size = new System.Drawing.Size(149, 32);
            this.txbDate.StyleController = this.layoutControl1;
            this.txbDate.TabIndex = 5;
            this.txbDate.EditValueChanged += new System.EventHandler(this.txbDate_EditValueChanged);
            // 
            // cbbSign
            // 
            this.cbbSign.Location = new System.Drawing.Point(62, 12);
            this.cbbSign.Name = "cbbSign";
            this.cbbSign.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbSign.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbSign.Properties.Appearance.Options.UseFont = true;
            this.cbbSign.Properties.Appearance.Options.UseForeColor = true;
            this.cbbSign.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbSign.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbSign.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbSign.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "DisplayName")});
            this.cbbSign.Properties.NullText = "";
            this.cbbSign.Properties.PopupSizeable = false;
            this.cbbSign.Properties.ShowHeader = false;
            this.cbbSign.Size = new System.Drawing.Size(353, 32);
            this.cbbSign.StyleController = this.layoutControl1;
            this.cbbSign.TabIndex = 4;
            this.cbbSign.EditValueChanged += new System.EventHandler(this.cbbSign_EditValueChanged);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(427, 360);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.Control = this.cbbSign;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(407, 36);
            this.layoutControlItem1.Text = "簽名";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(38, 24);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.Control = this.txbDate;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 36);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(203, 36);
            this.layoutControlItem2.Text = "日期";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(38, 24);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem3.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem3.Control = this.picSign;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(407, 181);
            this.layoutControlItem3.Text = "照片";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(38, 24);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutControlItem4.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem4.Control = this.txbMoreInfo;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 253);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(407, 87);
            this.layoutControlItem4.Text = "資訊";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(38, 24);
            // 
            // cbbDateFormat
            // 
            this.cbbDateFormat.Location = new System.Drawing.Point(265, 48);
            this.cbbDateFormat.Name = "cbbDateFormat";
            this.cbbDateFormat.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbDateFormat.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbDateFormat.Properties.Appearance.Options.UseFont = true;
            this.cbbDateFormat.Properties.Appearance.Options.UseForeColor = true;
            this.cbbDateFormat.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbDateFormat.Properties.AppearanceDropDown.ForeColor = System.Drawing.Color.Black;
            this.cbbDateFormat.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbDateFormat.Properties.AppearanceDropDown.Options.UseForeColor = true;
            this.cbbDateFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbDateFormat.Properties.Items.AddRange(new object[] {
            "yyyyMMdd",
            "yyyy.MM.dd",
            "yyyy/MM/dd",
            "MM/dd"});
            this.cbbDateFormat.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbbDateFormat.Size = new System.Drawing.Size(150, 32);
            this.cbbDateFormat.StyleController = this.layoutControl1;
            this.cbbDateFormat.TabIndex = 8;
            this.cbbDateFormat.SelectedIndexChanged += new System.EventHandler(this.cbbDateFormat_SelectedIndexChanged);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutControlItem5.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem5.Control = this.cbbDateFormat;
            this.layoutControlItem5.Location = new System.Drawing.Point(203, 36);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(204, 36);
            this.layoutControlItem5.Text = "格式";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(38, 24);
            // 
            // uc00_AdvancedSign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc00_AdvancedSign";
            this.Size = new System.Drawing.Size(427, 360);
            this.Load += new System.EventHandler(this.uc00_AdvancedSign_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbMoreInfo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbSign.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDateFormat.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private System.Windows.Forms.PictureBox picSign;
        private DevExpress.XtraEditors.DateEdit txbDate;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.LookUpEdit cbbSign;
        private DevExpress.XtraEditors.MemoEdit txbMoreInfo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.ComboBoxEdit cbbDateFormat;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
    }
}
