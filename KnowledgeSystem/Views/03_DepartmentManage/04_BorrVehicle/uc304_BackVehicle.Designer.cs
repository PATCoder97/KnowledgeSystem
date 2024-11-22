namespace KnowledgeSystem.Views._03_DepartmentManage._04_BorrVehicle
{
    partial class uc304_BackVehicle
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
            this.timeBackTime = new DevExpress.XtraEditors.DateTimeOffsetEdit();
            this.txbEndKm = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcMajor5 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeBackTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbEndKm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMajor5)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.timeBackTime);
            this.layoutControl1.Controls.Add(this.txbEndKm);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(389, 82);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.lcMajor5});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.Root.Size = new System.Drawing.Size(389, 82);
            this.Root.TextVisible = false;
            // 
            // timeBackTime
            // 
            this.timeBackTime.EditValue = null;
            this.timeBackTime.Location = new System.Drawing.Point(127, 7);
            this.timeBackTime.Name = "timeBackTime";
            this.timeBackTime.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeBackTime.Properties.Appearance.Options.UseFont = true;
            this.timeBackTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.timeBackTime.Properties.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
            this.timeBackTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.timeBackTime.Properties.MaskSettings.Set("mask", "yyyy/MM/dd HH:mm");
            this.timeBackTime.Size = new System.Drawing.Size(255, 32);
            this.timeBackTime.StyleController = this.layoutControl1;
            this.timeBackTime.TabIndex = 19;
            // 
            // txbEndKm
            // 
            this.txbEndKm.Location = new System.Drawing.Point(127, 43);
            this.txbEndKm.Name = "txbEndKm";
            this.txbEndKm.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbEndKm.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbEndKm.Properties.Appearance.Options.UseFont = true;
            this.txbEndKm.Properties.Appearance.Options.UseForeColor = true;
            this.txbEndKm.Size = new System.Drawing.Size(255, 32);
            this.txbEndKm.StyleController = this.layoutControl1;
            this.txbEndKm.TabIndex = 18;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.AppearanceItemCaptionDisabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutControlItem2.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem2.AppearanceItemCaptionDisabled.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.layoutControlItem2.Control = this.timeBackTime;
            this.layoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.layoutControlItem2.CustomizationFormText = "Thời gian";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(379, 36);
            this.layoutControlItem2.Text = "Thời gian";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(108, 24);
            // 
            // lcMajor5
            // 
            this.lcMajor5.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcMajor5.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcMajor5.AppearanceItemCaption.Options.UseFont = true;
            this.lcMajor5.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcMajor5.AppearanceItemCaptionDisabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcMajor5.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcMajor5.AppearanceItemCaptionDisabled.Options.UseFont = true;
            this.lcMajor5.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcMajor5.Control = this.txbEndKm;
            this.lcMajor5.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.lcMajor5.CustomizationFormText = "Xuất phát";
            this.lcMajor5.Location = new System.Drawing.Point(0, 36);
            this.lcMajor5.Name = "lcMajor5";
            this.lcMajor5.Size = new System.Drawing.Size(379, 36);
            this.lcMajor5.Text = "Km kết thúc";
            this.lcMajor5.TextSize = new System.Drawing.Size(108, 24);
            // 
            // uc304_BackVehicle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc304_BackVehicle";
            this.Size = new System.Drawing.Size(389, 82);
            this.Load += new System.EventHandler(this.uc304_BackVehicle_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeBackTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbEndKm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcMajor5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.DateTimeOffsetEdit timeBackTime;
        private DevExpress.XtraEditors.TextEdit txbEndKm;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem lcMajor5;
    }
}
