namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension
{
    partial class f403_ProcessFilesTool
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.progressBar = new DevExpress.XtraEditors.ProgressBarControl();
            this.btnStart = new DevExpress.XtraEditors.SimpleButton();
            this.cbbFunction = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lsFileComplete = new DevExpress.XtraEditors.ListBoxControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutStatus = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbFunction.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsFileComplete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.progressBar);
            this.layoutControl1.Controls.Add(this.btnStart);
            this.layoutControl1.Controls.Add(this.cbbFunction);
            this.layoutControl1.Controls.Add(this.lsFileComplete);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(480, 343);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 76);
            this.progressBar.MinimumSize = new System.Drawing.Size(0, 15);
            this.progressBar.Name = "progressBar";
            this.progressBar.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressBar.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.progressBar.Properties.ShowTitle = true;
            this.progressBar.Size = new System.Drawing.Size(456, 24);
            this.progressBar.StyleController = this.layoutControl1;
            this.progressBar.TabIndex = 11;
            // 
            // btnStart
            // 
            this.btnStart.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
            this.btnStart.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnStart.Appearance.Options.UseFont = true;
            this.btnStart.Location = new System.Drawing.Point(315, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(153, 32);
            this.btnStart.StyleController = this.layoutControl1;
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "執行<color=red>*</color>檔案";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // cbbFunction
            // 
            this.cbbFunction.Location = new System.Drawing.Point(55, 12);
            this.cbbFunction.Name = "cbbFunction";
            this.cbbFunction.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbFunction.Properties.Appearance.Options.UseFont = true;
            this.cbbFunction.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbFunction.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbFunction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbFunction.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbbFunction.Size = new System.Drawing.Size(256, 32);
            this.cbbFunction.StyleController = this.layoutControl1;
            this.cbbFunction.TabIndex = 5;
            // 
            // lsFileComplete
            // 
            this.lsFileComplete.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsFileComplete.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lsFileComplete.Appearance.Options.UseFont = true;
            this.lsFileComplete.Appearance.Options.UseForeColor = true;
            this.lsFileComplete.Location = new System.Drawing.Point(12, 104);
            this.lsFileComplete.Name = "lsFileComplete";
            this.lsFileComplete.Size = new System.Drawing.Size(456, 227);
            this.lsFileComplete.StyleController = this.layoutControl1;
            this.lsFileComplete.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutStatus});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(480, 343);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lsFileComplete;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 92);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(460, 231);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem2.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.layoutControlItem2.Control = this.cbbFunction;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(303, 36);
            this.layoutControlItem2.Text = "功能";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(38, 24);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnStart;
            this.layoutControlItem3.Location = new System.Drawing.Point(303, 0);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(49, 33);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(157, 36);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutStatus
            // 
            this.layoutStatus.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutStatus.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutStatus.AppearanceItemCaption.Options.UseFont = true;
            this.layoutStatus.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutStatus.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.layoutStatus.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.layoutStatus.Control = this.progressBar;
            this.layoutStatus.Location = new System.Drawing.Point(0, 36);
            this.layoutStatus.Name = "layoutStatus";
            this.layoutStatus.Size = new System.Drawing.Size(460, 56);
            this.layoutStatus.Text = "...";
            this.layoutStatus.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutStatus.TextSize = new System.Drawing.Size(12, 24);
            // 
            // f403_ProcessFilesTool
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 343);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f403_ProcessFilesTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "處理文件工具";
            this.Load += new System.EventHandler(this.f403_ProcessFilesTool_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbFunction.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lsFileComplete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.ListBoxControl lsFileComplete;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnStart;
        private DevExpress.XtraEditors.ComboBoxEdit cbbFunction;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.ProgressBarControl progressBar;
        private DevExpress.XtraLayout.LayoutControlItem layoutStatus;
    }
}