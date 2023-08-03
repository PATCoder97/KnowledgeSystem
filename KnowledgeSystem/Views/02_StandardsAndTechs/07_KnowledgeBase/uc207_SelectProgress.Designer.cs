namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    partial class uc207_SelectProgress
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
            this.cbbProgress = new DevExpress.XtraEditors.LookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbProgress.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cbbProgress
            // 
            this.cbbProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbbProgress.Location = new System.Drawing.Point(5, 0);
            this.cbbProgress.Name = "cbbProgress";
            this.cbbProgress.Properties.Appearance.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbProgress.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbProgress.Properties.Appearance.Options.UseFont = true;
            this.cbbProgress.Properties.Appearance.Options.UseForeColor = true;
            this.cbbProgress.Properties.AppearanceDropDown.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbProgress.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbProgress.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbProgress.Properties.NullText = "";
            this.cbbProgress.Properties.PopupSizeable = false;
            this.cbbProgress.Properties.ShowHeader = false;
            this.cbbProgress.Size = new System.Drawing.Size(590, 28);
            this.cbbProgress.TabIndex = 0;
            this.cbbProgress.EditValueChanged += new System.EventHandler(this.cbbProgress_EditValueChanged);
            // 
            // uc207_SelectProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbbProgress);
            this.Name = "uc207_SelectProgress";
            this.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Size = new System.Drawing.Size(600, 28);
            this.Load += new System.EventHandler(this.uc207_SelectProgress_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cbbProgress.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LookUpEdit cbbProgress;
    }
}
