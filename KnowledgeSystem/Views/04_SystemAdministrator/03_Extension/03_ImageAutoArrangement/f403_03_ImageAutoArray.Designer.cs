namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._03_ImageAutoArrangement
{
    partial class f403_03_ImageAutoArray
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(f403_03_ImageAutoArray));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSubmit = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.cbbZoom = new DevExpress.XtraEditors.ComboBoxEdit();
            this.txbPath = new DevExpress.XtraEditors.TextEdit();
            this.cbbTypeFile = new DevExpress.XtraEditors.ComboBoxEdit();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbZoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbTypeFile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.btnSubmit);
            this.layoutControl1.Controls.Add(this.cbbZoom);
            this.layoutControl1.Controls.Add(this.txbPath);
            this.layoutControl1.Controls.Add(this.cbbTypeFile);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(415, 132);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Appearance.Options.UseForeColor = true;
            this.btnSubmit.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSubmit.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnSubmit.ImageOptions.SvgImage")));
            this.btnSubmit.Location = new System.Drawing.Point(235, 84);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(168, 36);
            this.btnSubmit.StyleController = this.layoutControl1;
            this.btnSubmit.TabIndex = 6;
            this.btnSubmit.Text = "开始排列照片";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(415, 132);
            this.Root.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 72);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(223, 40);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSubmit;
            this.layoutControlItem3.Location = new System.Drawing.Point(223, 72);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(172, 40);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // cbbZoom
            // 
            this.cbbZoom.EditValue = "100X";
            this.cbbZoom.Location = new System.Drawing.Point(323, 48);
            this.cbbZoom.Name = "cbbZoom";
            this.cbbZoom.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbZoom.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbZoom.Properties.Appearance.Options.UseFont = true;
            this.cbbZoom.Properties.Appearance.Options.UseForeColor = true;
            this.cbbZoom.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbZoom.Properties.AppearanceDropDown.ForeColor = System.Drawing.Color.Black;
            this.cbbZoom.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbZoom.Properties.AppearanceDropDown.Options.UseForeColor = true;
            this.cbbZoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbZoom.Properties.DropDownRows = 3;
            this.cbbZoom.Properties.Items.AddRange(new object[] {
            "100X",
            "200X"});
            this.cbbZoom.Properties.Sorted = true;
            this.cbbZoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbbZoom.Size = new System.Drawing.Size(80, 32);
            this.cbbZoom.StyleController = this.layoutControl1;
            this.cbbZoom.TabIndex = 5;
            // 
            // txbPath
            // 
            this.txbPath.Location = new System.Drawing.Point(100, 12);
            this.txbPath.Name = "txbPath";
            this.txbPath.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbPath.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbPath.Properties.Appearance.Options.UseFont = true;
            this.txbPath.Properties.Appearance.Options.UseForeColor = true;
            this.txbPath.Size = new System.Drawing.Size(303, 32);
            this.txbPath.StyleController = this.layoutControl1;
            this.txbPath.TabIndex = 4;
            // 
            // cbbTypeFile
            // 
            this.cbbTypeFile.EditValue = "PowerPoint";
            this.cbbTypeFile.Location = new System.Drawing.Point(100, 48);
            this.cbbTypeFile.Name = "cbbTypeFile";
            this.cbbTypeFile.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbTypeFile.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbTypeFile.Properties.Appearance.Options.UseFont = true;
            this.cbbTypeFile.Properties.Appearance.Options.UseForeColor = true;
            this.cbbTypeFile.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbTypeFile.Properties.AppearanceDropDown.ForeColor = System.Drawing.Color.Black;
            this.cbbTypeFile.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbTypeFile.Properties.AppearanceDropDown.Options.UseForeColor = true;
            this.cbbTypeFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbTypeFile.Properties.DropDownRows = 3;
            this.cbbTypeFile.Properties.Items.AddRange(new object[] {
            "Excel",
            "PowerPoint"});
            this.cbbTypeFile.Properties.Sorted = true;
            this.cbbTypeFile.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbbTypeFile.Size = new System.Drawing.Size(131, 32);
            this.cbbTypeFile.StyleController = this.layoutControl1;
            this.cbbTypeFile.TabIndex = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.Control = this.txbPath;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(395, 36);
            this.layoutControlItem1.Text = "資料夾";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(76, 24);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.Control = this.cbbZoom;
            this.layoutControlItem2.Location = new System.Drawing.Point(223, 36);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(172, 36);
            this.layoutControlItem2.Text = "放大倍率";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(76, 24);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutControlItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem4.Control = this.cbbTypeFile;
            this.layoutControlItem4.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.layoutControlItem4.CustomizationFormText = "檔案";
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 36);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(223, 36);
            this.layoutControlItem4.Text = "文件類型";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(76, 24);
            // 
            // f403_03_ImageAutoArray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 132);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f403_03_ImageAutoArray";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "金相照片自動排布";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbZoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbTypeFile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnSubmit;
        private DevExpress.XtraEditors.ComboBoxEdit cbbZoom;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.TextEdit txbPath;
        private DevExpress.XtraEditors.ComboBoxEdit cbbTypeFile;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
    }
}