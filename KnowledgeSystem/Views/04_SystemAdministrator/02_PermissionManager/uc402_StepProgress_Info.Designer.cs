namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    partial class uc402_StepProgress_Info
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
            this.gcStep = new DevExpress.XtraGrid.GridControl();
            this.gvStep = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.步驟 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.群組 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cbbGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnNewStep = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.gcStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // gcStep
            // 
            this.gcStep.Location = new System.Drawing.Point(4, 32);
            this.gcStep.MainView = this.gvStep;
            this.gcStep.Name = "gcStep";
            this.gcStep.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.cbbGroup});
            this.gcStep.Size = new System.Drawing.Size(300, 369);
            this.gcStep.TabIndex = 0;
            this.gcStep.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvStep});
            // 
            // gvStep
            // 
            this.gvStep.Appearance.HeaderPanel.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvStep.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvStep.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvStep.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvStep.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvStep.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvStep.Appearance.Row.Font = new System.Drawing.Font("DFKai-SB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvStep.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvStep.Appearance.Row.Options.UseFont = true;
            this.gvStep.Appearance.Row.Options.UseForeColor = true;
            this.gvStep.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.步驟,
            this.群組});
            this.gvStep.GridControl = this.gcStep;
            this.gvStep.Name = "gvStep";
            this.gvStep.OptionsCustomization.AllowFilter = false;
            this.gvStep.OptionsCustomization.AllowSort = false;
            this.gvStep.OptionsView.EnableAppearanceOddRow = true;
            this.gvStep.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            this.gvStep.OptionsView.ShowGroupPanel = false;
            this.gvStep.OptionsView.ShowIndicator = false;
            // 
            // 步驟
            // 
            this.步驟.AppearanceCell.Options.UseTextOptions = true;
            this.步驟.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.步驟.Caption = "步驟";
            this.步驟.FieldName = "IndexStep";
            this.步驟.MaxWidth = 50;
            this.步驟.MinWidth = 70;
            this.步驟.Name = "步驟";
            this.步驟.Visible = true;
            this.步驟.VisibleIndex = 0;
            this.步驟.Width = 70;
            // 
            // 群組
            // 
            this.群組.Caption = "群組";
            this.群組.ColumnEdit = this.cbbGroup;
            this.群組.FieldName = "Id";
            this.群組.Name = "群組";
            this.群組.Visible = true;
            this.群組.VisibleIndex = 1;
            // 
            // cbbGroup
            // 
            this.cbbGroup.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.True;
            this.cbbGroup.AppearanceDropDown.Font = new System.Drawing.Font("DFKai-SB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbGroup.AppearanceDropDown.ForeColor = System.Drawing.Color.Black;
            this.cbbGroup.AppearanceDropDown.Options.UseFont = true;
            this.cbbGroup.AppearanceDropDown.Options.UseForeColor = true;
            this.cbbGroup.AutoHeight = false;
            this.cbbGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbGroup.Name = "cbbGroup";
            this.cbbGroup.ShowHeader = false;
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.btnNewStep);
            this.layoutControl1.Controls.Add(this.gcStep);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(330, 113, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(308, 405);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnNewStep
            // 
            this.btnNewStep.Appearance.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewStep.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnNewStep.Appearance.Options.UseFont = true;
            this.btnNewStep.Appearance.Options.UseForeColor = true;
            this.btnNewStep.Location = new System.Drawing.Point(4, 4);
            this.btnNewStep.Name = "btnNewStep";
            this.btnNewStep.Size = new System.Drawing.Size(102, 24);
            this.btnNewStep.StyleController = this.layoutControl1;
            this.btnNewStep.TabIndex = 3;
            this.btnNewStep.Text = "新增步驟";
            this.btnNewStep.Click += new System.EventHandler(this.btnNewStep_Click);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.Root.Size = new System.Drawing.Size(308, 405);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcStep;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 28);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(304, 373);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnNewStep;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 200, 2, 2);
            this.layoutControlItem2.Size = new System.Drawing.Size(304, 28);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // uc207_StepProgress_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc207_StepProgress_Info";
            this.Size = new System.Drawing.Size(308, 405);
            this.Load += new System.EventHandler(this.uc207_StepProgress_Info_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gcStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcStep;
        private DevExpress.XtraGrid.Views.Grid.GridView gvStep;
        private DevExpress.XtraGrid.Columns.GridColumn 步驟;
        private DevExpress.XtraGrid.Columns.GridColumn 群組;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnNewStep;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbbGroup;
    }
}
