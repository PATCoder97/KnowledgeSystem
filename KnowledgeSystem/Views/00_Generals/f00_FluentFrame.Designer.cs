namespace KnowledgeSystem.Views._00_Generals
{
    partial class f00_FluentFrame
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
            this.div_container = new DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer();
            this.fluentControl = new DevExpress.XtraBars.Navigation.AccordionControl();
            this.fluentDesignFormControl1 = new DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl();
            ((System.ComponentModel.ISupportInitialize)(this.fluentControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentDesignFormControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // div_container
            // 
            this.div_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.div_container.Location = new System.Drawing.Point(200, 29);
            this.div_container.Name = "div_container";
            this.div_container.Size = new System.Drawing.Size(709, 574);
            this.div_container.TabIndex = 0;
            // 
            // fluentControl
            // 
            this.fluentControl.AllowItemSelection = true;
            this.fluentControl.ContextButtonsOptions.AnimationType = DevExpress.Utils.ContextAnimationType.OpacityAnimation;
            this.fluentControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.fluentControl.Location = new System.Drawing.Point(0, 29);
            this.fluentControl.Name = "fluentControl";
            this.fluentControl.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Touch;
            this.fluentControl.Size = new System.Drawing.Size(200, 574);
            this.fluentControl.TabIndex = 1;
            this.fluentControl.ViewType = DevExpress.XtraBars.Navigation.AccordionControlViewType.HamburgerMenu;
            // 
            // fluentDesignFormControl1
            // 
            this.fluentDesignFormControl1.FluentDesignForm = this;
            this.fluentDesignFormControl1.Location = new System.Drawing.Point(0, 0);
            this.fluentDesignFormControl1.Name = "fluentDesignFormControl1";
            this.fluentDesignFormControl1.Size = new System.Drawing.Size(909, 29);
            this.fluentDesignFormControl1.TabIndex = 2;
            this.fluentDesignFormControl1.TabStop = false;
            // 
            // f00_FluentFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 603);
            this.ControlContainer = this.div_container;
            this.Controls.Add(this.div_container);
            this.Controls.Add(this.fluentControl);
            this.Controls.Add(this.fluentDesignFormControl1);
            this.FluentDesignFormControl = this.fluentDesignFormControl1;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.DocumentSystem;
            this.Name = "f00_FluentFrame";
            this.NavigationControl = this.fluentControl;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "f00_FluentFrame";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.f00_FluentFrame_Load);
            this.Shown += new System.EventHandler(this.f00_FluentFrame_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.fluentControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentDesignFormControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer div_container;
        private DevExpress.XtraBars.Navigation.AccordionControl fluentControl;
        private DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl fluentDesignFormControl1;
    }
}