﻿namespace KnowledgeSystem.Views._00_Generals
{
    partial class uc00_ChangePassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc00_ChangePassword));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.txbNewPass2 = new DevExpress.XtraEditors.TextEdit();
            this.txbNewPass1 = new DevExpress.XtraEditors.TextEdit();
            this.txbOldPass = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNewPass2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNewPass1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbOldPass.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.btnConfirm);
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Controls.Add(this.txbNewPass2);
            this.layoutControl1.Controls.Add(this.txbNewPass1);
            this.layoutControl1.Controls.Add(this.txbOldPass);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(366, 0, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(302, 212);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnConfirm.Appearance.Options.UseFont = true;
            this.btnConfirm.Appearance.Options.UseForeColor = true;
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnConfirm.ImageOptions.SvgImage")));
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.btnConfirm.Location = new System.Drawing.Point(215, 169);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(80, 36);
            this.btnConfirm.StyleController = this.layoutControl1;
            this.btnConfirm.TabIndex = 8;
            this.btnConfirm.Text = "確認";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Appearance.Options.UseForeColor = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnCancel.ImageOptions.SvgImage")));
            this.btnCancel.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.btnCancel.Location = new System.Drawing.Point(131, 169);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 36);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取消";
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
            this.layoutControlItem5,
            this.emptySpaceItem1});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.Root.Size = new System.Drawing.Size(302, 212);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnCancel;
            this.layoutControlItem4.Location = new System.Drawing.Point(124, 162);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(100, 0);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(80, 28);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(84, 40);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnConfirm;
            this.layoutControlItem5.Location = new System.Drawing.Point(208, 162);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(100, 0);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(80, 28);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(84, 40);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 162);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(124, 40);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // txbNewPass2
            // 
            this.txbNewPass2.Location = new System.Drawing.Point(7, 115);
            this.txbNewPass2.Name = "txbNewPass2";
            this.txbNewPass2.Properties.AdvancedModeOptions.Label = "確認密碼";
            this.txbNewPass2.Properties.AdvancedModeOptions.LabelAppearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.txbNewPass2.Properties.AdvancedModeOptions.LabelAppearance.ForeColor = System.Drawing.Color.Black;
            this.txbNewPass2.Properties.AdvancedModeOptions.LabelAppearance.Options.UseFont = true;
            this.txbNewPass2.Properties.AdvancedModeOptions.LabelAppearance.Options.UseForeColor = true;
            this.txbNewPass2.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbNewPass2.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbNewPass2.Properties.Appearance.Options.UseFont = true;
            this.txbNewPass2.Properties.Appearance.Options.UseForeColor = true;
            this.txbNewPass2.Properties.UseSystemPasswordChar = true;
            this.txbNewPass2.Size = new System.Drawing.Size(288, 50);
            this.txbNewPass2.StyleController = this.layoutControl1;
            this.txbNewPass2.TabIndex = 6;
            // 
            // txbNewPass1
            // 
            this.txbNewPass1.Location = new System.Drawing.Point(7, 61);
            this.txbNewPass1.Name = "txbNewPass1";
            this.txbNewPass1.Properties.AdvancedModeOptions.Label = "新密碼";
            this.txbNewPass1.Properties.AdvancedModeOptions.LabelAppearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbNewPass1.Properties.AdvancedModeOptions.LabelAppearance.ForeColor = System.Drawing.Color.Black;
            this.txbNewPass1.Properties.AdvancedModeOptions.LabelAppearance.Options.UseFont = true;
            this.txbNewPass1.Properties.AdvancedModeOptions.LabelAppearance.Options.UseForeColor = true;
            this.txbNewPass1.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbNewPass1.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbNewPass1.Properties.Appearance.Options.UseFont = true;
            this.txbNewPass1.Properties.Appearance.Options.UseForeColor = true;
            this.txbNewPass1.Properties.UseSystemPasswordChar = true;
            this.txbNewPass1.Size = new System.Drawing.Size(288, 50);
            this.txbNewPass1.StyleController = this.layoutControl1;
            this.txbNewPass1.TabIndex = 5;
            // 
            // txbOldPass
            // 
            this.txbOldPass.Location = new System.Drawing.Point(7, 7);
            this.txbOldPass.Name = "txbOldPass";
            this.txbOldPass.Properties.AdvancedModeOptions.Label = "舊密碼";
            this.txbOldPass.Properties.AdvancedModeOptions.LabelAppearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.txbOldPass.Properties.AdvancedModeOptions.LabelAppearance.ForeColor = System.Drawing.Color.Black;
            this.txbOldPass.Properties.AdvancedModeOptions.LabelAppearance.Options.UseFont = true;
            this.txbOldPass.Properties.AdvancedModeOptions.LabelAppearance.Options.UseForeColor = true;
            this.txbOldPass.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbOldPass.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbOldPass.Properties.Appearance.Options.UseFont = true;
            this.txbOldPass.Properties.Appearance.Options.UseForeColor = true;
            this.txbOldPass.Properties.UseSystemPasswordChar = true;
            this.txbOldPass.Size = new System.Drawing.Size(288, 50);
            this.txbOldPass.StyleController = this.layoutControl1;
            this.txbOldPass.TabIndex = 4;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txbOldPass;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(292, 54);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txbNewPass1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 54);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(292, 54);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.txbNewPass2;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 108);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(292, 54);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // uc00_ChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc00_ChangePassword";
            this.Size = new System.Drawing.Size(302, 212);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNewPass2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNewPass1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbOldPass.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txbOldPass;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.TextEdit txbNewPass2;
        private DevExpress.XtraEditors.TextEdit txbNewPass1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SimpleButton btnConfirm;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
