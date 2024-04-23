namespace KnowledgeSystem.Views._00_Generals
{
    partial class f00_Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(f00_Login));
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnLogin = new DevExpress.XtraEditors.SimpleButton();
            this.txbUserID = new DevExpress.XtraEditors.TextEdit();
            this.lbNameSoft = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.txbPassword = new DevExpress.XtraEditors.ButtonEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txbUserID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPassword.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.BorderColor = System.Drawing.Color.Black;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancel.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Appearance.Options.UseBorderColor = true;
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Appearance.Options.UseForeColor = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCancel.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnCancel.ImageOptions.SvgImage")));
            this.btnCancel.Location = new System.Drawing.Point(602, 340);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 42);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            // 
            // btnLogin
            // 
            this.btnLogin.Appearance.BorderColor = System.Drawing.Color.Black;
            this.btnLogin.Appearance.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLogin.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnLogin.Appearance.Options.UseBorderColor = true;
            this.btnLogin.Appearance.Options.UseFont = true;
            this.btnLogin.Appearance.Options.UseForeColor = true;
            this.btnLogin.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnLogin.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnLogin.ImageOptions.SvgImage")));
            this.btnLogin.Location = new System.Drawing.Point(485, 340);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(110, 42);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "登錄";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txbUserID
            // 
            this.txbUserID.Location = new System.Drawing.Point(500, 213);
            this.txbUserID.Name = "txbUserID";
            this.txbUserID.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.txbUserID.Properties.Appearance.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbUserID.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(78)))), ((int)(((byte)(162)))));
            this.txbUserID.Properties.Appearance.Options.UseBackColor = true;
            this.txbUserID.Properties.Appearance.Options.UseFont = true;
            this.txbUserID.Properties.Appearance.Options.UseForeColor = true;
            this.txbUserID.Properties.Appearance.Options.UseImage = true;
            this.txbUserID.Properties.Appearance.Options.UseTextOptions = true;
            this.txbUserID.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.txbUserID.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.txbUserID.Properties.MaxLength = 10;
            this.txbUserID.Size = new System.Drawing.Size(197, 30);
            this.txbUserID.TabIndex = 0;
            // 
            // lbNameSoft
            // 
            this.lbNameSoft.BackColor = System.Drawing.Color.Transparent;
            this.lbNameSoft.Font = new System.Drawing.Font("DFKai-SB", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbNameSoft.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.lbNameSoft.Location = new System.Drawing.Point(12, 35);
            this.lbNameSoft.Name = "lbNameSoft";
            this.lbNameSoft.Size = new System.Drawing.Size(443, 51);
            this.lbNameSoft.TabIndex = 1;
            this.lbNameSoft.Text = " 軟體名稱";
            this.lbNameSoft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbVersion
            // 
            this.lbVersion.BackColor = System.Drawing.Color.Transparent;
            this.lbVersion.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.lbVersion.Location = new System.Drawing.Point(302, 86);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(153, 24);
            this.lbVersion.TabIndex = 2;
            this.lbVersion.Text = "Version: 0.0.0";
            this.lbVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txbPassword
            // 
            this.txbPassword.EditValue = "";
            this.txbPassword.Location = new System.Drawing.Point(500, 278);
            this.txbPassword.Name = "txbPassword";
            this.txbPassword.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.txbPassword.Properties.Appearance.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbPassword.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(78)))), ((int)(((byte)(162)))));
            this.txbPassword.Properties.Appearance.Options.UseBackColor = true;
            this.txbPassword.Properties.Appearance.Options.UseFont = true;
            this.txbPassword.Properties.Appearance.Options.UseForeColor = true;
            this.txbPassword.Properties.Appearance.Options.UseTextOptions = true;
            this.txbPassword.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.txbPassword.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            editorButtonImageOptions1.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("editorButtonImageOptions1.SvgImage")));
            editorButtonImageOptions1.SvgImageSize = new System.Drawing.Size(16, 16);
            this.txbPassword.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.txbPassword.Properties.UseSystemPasswordChar = true;
            this.txbPassword.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbPassword_Properties_ButtonClick);
            this.txbPassword.Size = new System.Drawing.Size(197, 30);
            this.txbPassword.TabIndex = 1;
            // 
            // f00_Login
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Stretch;
            this.BackgroundImageStore = global::KnowledgeSystem.Properties.Resources.loginscreen;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(750, 450);
            this.Controls.Add(this.lbVersion);
            this.Controls.Add(this.lbNameSoft);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txbUserID);
            this.Controls.Add(this.txbPassword);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.MaximumSize = new System.Drawing.Size(750, 450);
            this.Name = "f00_Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập";
            this.Load += new System.EventHandler(this.fLogin_Load);
            this.Shown += new System.EventHandler(this.fLogin_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.f00_Login_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txbUserID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPassword.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnLogin;
        private DevExpress.XtraEditors.TextEdit txbUserID;
        private System.Windows.Forms.Label lbNameSoft;
        private System.Windows.Forms.Label lbVersion;
        private DevExpress.XtraEditors.ButtonEdit txbPassword;
    }
}