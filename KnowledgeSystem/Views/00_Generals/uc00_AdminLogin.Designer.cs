namespace KnowledgeSystem.Views._00_Generals
{
    partial class uc00_AdminLogin
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
            this.txbPass = new DevExpress.XtraEditors.TextEdit();
            this.txbID = new DevExpress.XtraEditors.TextEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbPass.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.txbPass);
            this.layoutControl1.Controls.Add(this.txbID);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(366, 0, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(246, 118);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txbPass
            // 
            this.txbPass.Location = new System.Drawing.Point(8, 61);
            this.txbPass.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txbPass.Name = "txbPass";
            this.txbPass.Properties.AdvancedModeOptions.Label = "Master Key";
            this.txbPass.Properties.AdvancedModeOptions.LabelAppearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbPass.Properties.AdvancedModeOptions.LabelAppearance.ForeColor = System.Drawing.Color.Black;
            this.txbPass.Properties.AdvancedModeOptions.LabelAppearance.Options.UseFont = true;
            this.txbPass.Properties.AdvancedModeOptions.LabelAppearance.Options.UseForeColor = true;
            this.txbPass.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbPass.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbPass.Properties.Appearance.Options.UseFont = true;
            this.txbPass.Properties.Appearance.Options.UseForeColor = true;
            this.txbPass.Properties.UseSystemPasswordChar = true;
            this.txbPass.Size = new System.Drawing.Size(230, 50);
            this.txbPass.StyleController = this.layoutControl1;
            this.txbPass.TabIndex = 5;
            // 
            // txbID
            // 
            this.txbID.Location = new System.Drawing.Point(8, 7);
            this.txbID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txbID.Name = "txbID";
            this.txbID.Properties.AdvancedModeOptions.Label = "Account";
            this.txbID.Properties.AdvancedModeOptions.LabelAppearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.txbID.Properties.AdvancedModeOptions.LabelAppearance.ForeColor = System.Drawing.Color.Black;
            this.txbID.Properties.AdvancedModeOptions.LabelAppearance.Options.UseFont = true;
            this.txbID.Properties.AdvancedModeOptions.LabelAppearance.Options.UseForeColor = true;
            this.txbID.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbID.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbID.Properties.Appearance.Options.UseFont = true;
            this.txbID.Properties.Appearance.Options.UseForeColor = true;
            this.txbID.Size = new System.Drawing.Size(230, 50);
            this.txbID.StyleController = this.layoutControl1;
            this.txbID.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 6, 5, 5);
            this.Root.Size = new System.Drawing.Size(246, 118);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txbID;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(234, 54);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txbPass;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 54);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(234, 54);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // uc00_AdminLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "uc00_AdminLogin";
            this.Size = new System.Drawing.Size(246, 118);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbPass.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txbPass;
        private DevExpress.XtraEditors.TextEdit txbID;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
