namespace KnowledgeSystem.Views._00_Generals
{
    partial class f00_Frame
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
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.MdiManager = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
            this.treeAppForm = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.btnShowForm = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            ((System.ComponentModel.ISupportInitialize)(this.MdiManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeAppForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnShowForm)).BeginInit();
            this.SuspendLayout();
            // 
            // MdiManager
            // 
            this.MdiManager.AppearancePage.Header.Font = new System.Drawing.Font("DFKai-SB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MdiManager.AppearancePage.Header.ForeColor = System.Drawing.Color.Black;
            this.MdiManager.AppearancePage.Header.Options.UseFont = true;
            this.MdiManager.AppearancePage.Header.Options.UseForeColor = true;
            this.MdiManager.AppearancePage.HeaderActive.Font = new System.Drawing.Font("DFKai-SB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MdiManager.AppearancePage.HeaderActive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.MdiManager.AppearancePage.HeaderActive.Options.UseFont = true;
            this.MdiManager.AppearancePage.HeaderActive.Options.UseForeColor = true;
            this.MdiManager.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeader;
            this.MdiManager.MdiParent = this;
            // 
            // treeAppForm
            // 
            this.treeAppForm.Appearance.Row.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeAppForm.Appearance.Row.Options.UseFont = true;
            this.treeAppForm.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2});
            this.treeAppForm.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeAppForm.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeAppForm.Location = new System.Drawing.Point(0, 0);
            this.treeAppForm.Name = "treeAppForm";
            this.treeAppForm.OptionsBehavior.ReadOnly = true;
            this.treeAppForm.OptionsView.EnableAppearanceOddRow = true;
            this.treeAppForm.OptionsView.ShowAutoFilterRow = true;
            this.treeAppForm.OptionsView.ShowColumns = false;
            this.treeAppForm.OptionsView.ShowIndicator = false;
            this.treeAppForm.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnShowForm});
            this.treeAppForm.Size = new System.Drawing.Size(215, 571);
            this.treeAppForm.TabIndex = 3;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "treeListColumn1";
            this.treeListColumn1.FieldName = "DisplayName";
            this.treeListColumn1.MinWidth = 16;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.Width = 213;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "treeListColumn2";
            this.treeListColumn2.ColumnEdit = this.btnShowForm;
            this.treeListColumn2.FieldName = "treeListColumn2";
            this.treeListColumn2.MaxWidth = 40;
            this.treeListColumn2.MinWidth = 40;
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 1;
            this.treeListColumn2.Width = 40;
            // 
            // btnShowForm
            // 
            this.btnShowForm.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", 0, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.btnShowForm.Name = "btnShowForm";
            this.btnShowForm.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnShowForm.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnShowForm_ButtonClick);
            // 
            // fFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1331, 571);
            this.Controls.Add(this.treeAppForm);
            this.IsMdiContainer = true;
            this.Name = "fFrame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "fFrame";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.fFrame_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MdiManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeAppForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnShowForm)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraTabbedMdi.XtraTabbedMdiManager MdiManager;
        private DevExpress.XtraTreeList.TreeList treeAppForm;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnShowForm;
    }
}