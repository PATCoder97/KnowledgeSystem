namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    partial class f201_SignProg_Detail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(f201_SignProg_Detail));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gcHistoryProcess = new DevExpress.XtraGrid.GridControl();
            this.gvHistoryProcess = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.stepProgressDoc = new DevExpress.XtraEditors.StepProgressBar();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcHistoryProcess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistoryProcess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stepProgressDoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.gcHistoryProcess);
            this.layoutControl1.Controls.Add(this.stepProgressDoc);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(908, 238, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(885, 501);
            this.layoutControl1.TabIndex = 6;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gcHistoryProcess
            // 
            this.gcHistoryProcess.Location = new System.Drawing.Point(12, 184);
            this.gcHistoryProcess.MainView = this.gvHistoryProcess;
            this.gcHistoryProcess.Name = "gcHistoryProcess";
            this.gcHistoryProcess.Size = new System.Drawing.Size(861, 305);
            this.gcHistoryProcess.TabIndex = 22;
            this.gcHistoryProcess.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvHistoryProcess});
            // 
            // gvHistoryProcess
            // 
            this.gvHistoryProcess.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvHistoryProcess.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvHistoryProcess.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvHistoryProcess.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvHistoryProcess.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvHistoryProcess.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvHistoryProcess.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvHistoryProcess.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvHistoryProcess.Appearance.Row.Options.UseFont = true;
            this.gvHistoryProcess.Appearance.Row.Options.UseForeColor = true;
            this.gvHistoryProcess.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn7,
            this.gridColumn1,
            this.gridColumn8,
            this.gridColumn9});
            this.gvHistoryProcess.GridControl = this.gcHistoryProcess;
            this.gvHistoryProcess.Name = "gvHistoryProcess";
            this.gvHistoryProcess.OptionsCustomization.AllowColumnMoving = false;
            this.gvHistoryProcess.OptionsCustomization.AllowFilter = false;
            this.gvHistoryProcess.OptionsCustomization.AllowGroup = false;
            this.gvHistoryProcess.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvHistoryProcess.OptionsCustomization.AllowSort = false;
            this.gvHistoryProcess.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvHistoryProcess.OptionsView.EnableAppearanceOddRow = true;
            this.gvHistoryProcess.OptionsView.ShowGroupPanel = false;
            this.gvHistoryProcess.OptionsView.ShowIndicator = false;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.Caption = "時期";
            this.gridColumn7.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
            this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn7.FieldName = "data.RespTime";
            this.gridColumn7.MaxWidth = 150;
            this.gridColumn7.MinWidth = 150;
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 0;
            this.gridColumn7.Width = 150;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "職務";
            this.gridColumn1.FieldName = "job.DisplayName";
            this.gridColumn1.MaxWidth = 200;
            this.gridColumn1.MinWidth = 150;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 150;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "處理人";
            this.gridColumn8.FieldName = "DisplayName";
            this.gridColumn8.MaxWidth = 250;
            this.gridColumn8.MinWidth = 250;
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 2;
            this.gridColumn8.Width = 250;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "說明";
            this.gridColumn9.FieldName = "data.Desc";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 3;
            this.gridColumn9.Width = 409;
            // 
            // stepProgressDoc
            // 
            this.stepProgressDoc.Appearances.ItemAppearance.ContentBlockAppearance.Caption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepProgressDoc.Appearances.ItemAppearance.ContentBlockAppearance.Caption.ForeColor = System.Drawing.Color.Black;
            this.stepProgressDoc.Appearances.ItemAppearance.ContentBlockAppearance.Caption.Options.UseFont = true;
            this.stepProgressDoc.Appearances.ItemAppearance.ContentBlockAppearance.Caption.Options.UseForeColor = true;
            this.stepProgressDoc.Appearances.ItemAppearance.ContentBlockAppearance.Description.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepProgressDoc.Appearances.ItemAppearance.ContentBlockAppearance.Description.ForeColor = System.Drawing.Color.Blue;
            this.stepProgressDoc.Appearances.ItemAppearance.ContentBlockAppearance.Description.Options.UseFont = true;
            this.stepProgressDoc.Appearances.ItemAppearance.ContentBlockAppearance.Description.Options.UseForeColor = true;
            this.stepProgressDoc.Appearances.SecondContentBlockAppearance.Caption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepProgressDoc.Appearances.SecondContentBlockAppearance.Caption.ForeColor = System.Drawing.Color.Red;
            this.stepProgressDoc.Appearances.SecondContentBlockAppearance.Caption.Options.UseFont = true;
            this.stepProgressDoc.Appearances.SecondContentBlockAppearance.Caption.Options.UseForeColor = true;
            this.stepProgressDoc.ItemOptions.Indicator.ActiveStateImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("stepProgressDoc.ItemOptions.Indicator.ActiveStateImageOptions.SvgImage")));
            this.stepProgressDoc.ItemOptions.Indicator.ActiveStateImageOptions.SvgImageSize = new System.Drawing.Size(48, 48);
            this.stepProgressDoc.Location = new System.Drawing.Point(12, 12);
            this.stepProgressDoc.Name = "stepProgressDoc";
            this.stepProgressDoc.Size = new System.Drawing.Size(861, 168);
            this.stepProgressDoc.StyleController = this.layoutControl1;
            this.stepProgressDoc.TabIndex = 21;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(885, 501);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gcHistoryProcess;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 172);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(865, 309);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.stepProgressDoc;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 172);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(54, 172);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(865, 172);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.Text = "核簽文件";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // f201_SignProg_Detail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 501);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f201_SignProg_Detail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "f201_SignProg_Detail";
            this.Load += new System.EventHandler(this.f201_SignProg_Detail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcHistoryProcess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistoryProcess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stepProgressDoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gcHistoryProcess;
        private DevExpress.XtraGrid.Views.Grid.GridView gvHistoryProcess;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraEditors.StepProgressBar stepProgressDoc;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}