namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    partial class uc301_SelectOutputFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc301_SelectOutputFile));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnUploadFile51 = new DevExpress.XtraEditors.SimpleButton();
            this.gcData51 = new DevExpress.XtraGrid.GridControl();
            this.gvData51 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cbbQuarter = new DevExpress.XtraEditors.ComboBoxEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcData51)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData51)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbQuarter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.btnUploadFile51);
            this.layoutControl1.Controls.Add(this.gcData51);
            this.layoutControl1.Controls.Add(this.cbbQuarter);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.ForeColor = System.Drawing.Color.White;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(425, 0, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(796, 446);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnUploadFile51
            // 
            this.btnUploadFile51.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUploadFile51.Appearance.Options.UseFont = true;
            this.btnUploadFile51.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnUploadFile51.ImageOptions.SvgImage")));
            this.btnUploadFile51.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnUploadFile51.Location = new System.Drawing.Point(662, 12);
            this.btnUploadFile51.Name = "btnUploadFile51";
            this.btnUploadFile51.Size = new System.Drawing.Size(122, 32);
            this.btnUploadFile51.StyleController = this.layoutControl1;
            this.btnUploadFile51.TabIndex = 13;
            this.btnUploadFile51.Text = "附件05.1";
            this.btnUploadFile51.Click += new System.EventHandler(this.btnUploadFile51_Click);
            // 
            // gcData51
            // 
            this.gcData51.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcData51.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcData51.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcData51.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcData51.EmbeddedNavigator.Buttons.Remove.Visible = false;
            this.gcData51.Location = new System.Drawing.Point(12, 76);
            this.gcData51.MainView = this.gvData51;
            this.gcData51.Name = "gcData51";
            this.gcData51.Size = new System.Drawing.Size(772, 358);
            this.gcData51.TabIndex = 12;
            this.gcData51.UseEmbeddedNavigator = true;
            this.gcData51.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvData51});
            // 
            // gvData51
            // 
            this.gvData51.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvData51.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvData51.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvData51.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvData51.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvData51.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvData51.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvData51.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvData51.Appearance.Row.Options.UseFont = true;
            this.gvData51.Appearance.Row.Options.UseForeColor = true;
            this.gvData51.GridControl = this.gcData51;
            this.gvData51.Name = "gvData51";
            this.gvData51.OptionsView.EnableAppearanceOddRow = true;
            this.gvData51.OptionsView.ShowGroupPanel = false;
            // 
            // cbbQuarter
            // 
            this.cbbQuarter.Location = new System.Drawing.Point(36, 12);
            this.cbbQuarter.Name = "cbbQuarter";
            this.cbbQuarter.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbQuarter.Properties.Appearance.Options.UseFont = true;
            this.cbbQuarter.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbQuarter.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbQuarter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbQuarter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbbQuarter.Size = new System.Drawing.Size(79, 32);
            this.cbbQuarter.StyleController = this.layoutControl1;
            this.cbbQuarter.TabIndex = 11;
            this.cbbQuarter.SelectedIndexChanged += new System.EventHandler(this.cbbQuarter_SelectedIndexChanged);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem8,
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(796, 446);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem8.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.layoutControlItem8.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem8.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem8.Control = this.cbbQuarter;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(107, 36);
            this.layoutControlItem8.Text = "季";
            this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(19, 24);
            this.layoutControlItem8.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(107, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(543, 36);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.Control = this.gcData51;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 36);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(776, 390);
            this.layoutControlItem2.Text = "附件05.1：初訓之提報需求人員名單";
            this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(303, 24);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnUploadFile51;
            this.layoutControlItem1.Location = new System.Drawing.Point(650, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(126, 0);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(126, 33);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(126, 36);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // uc301_SelectOutputFile
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc301_SelectOutputFile";
            this.Size = new System.Drawing.Size(796, 446);
            this.Load += new System.EventHandler(this.uc301_SelectOutputFile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcData51)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData51)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbQuarter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.ComboBoxEdit cbbQuarter;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.GridControl gcData51;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData51;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton btnUploadFile51;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
