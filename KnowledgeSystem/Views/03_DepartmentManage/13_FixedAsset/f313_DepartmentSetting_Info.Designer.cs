namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    partial class f313_DepartmentSetting_Info
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.chkActive = new DevExpress.XtraEditors.CheckEdit();
            this.spinRate = new DevExpress.XtraEditors.SpinEdit();
            this.cbbDept = new DevExpress.XtraEditors.ComboBoxEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcDept = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcRate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcActive = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinRate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcActive)).BeginInit();
            this.SuspendLayout();
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] { this.bar2 });
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] { this.btnConfirm });
            this.barManagerTP.MainMenu = this.bar2;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(this.btnConfirm));
            this.btnConfirm.Caption = "確認";
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(420, 49);
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Manager = this.barManagerTP;
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Controls.Add(this.cbbDept);
            this.layoutControl1.Controls.Add(this.spinRate);
            this.layoutControl1.Controls.Add(this.chkActive);
            this.layoutControl1.Root = this.Root;
            this.cbbDept.StyleController = this.layoutControl1;
            this.spinRate.StyleController = this.layoutControl1;
            this.chkActive.StyleController = this.layoutControl1;
            this.cbbDept.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { this.lcDept, this.lcRate, this.lcActive });
            this.Root.TextVisible = false;
            this.Root.Size = new System.Drawing.Size(420, 117);
            this.lcDept.Control = this.cbbDept;
            this.lcDept.Location = new System.Drawing.Point(0, 0);
            this.lcDept.Size = new System.Drawing.Size(400, 36);
            this.lcDept.Text = "部門";
            this.lcRate.Control = this.spinRate;
            this.lcRate.Location = new System.Drawing.Point(0, 36);
            this.lcRate.Size = new System.Drawing.Size(400, 36);
            this.lcRate.Text = "抽樣率%";
            this.lcActive.Control = this.chkActive;
            this.lcActive.Location = new System.Drawing.Point(0, 72);
            this.lcActive.Size = new System.Drawing.Size(400, 25);
            this.lcActive.TextVisible = false;
            this.ClientSize = new System.Drawing.Size(420, 166);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Department Setting";
            this.Load += new System.EventHandler(this.f313_DepartmentSetting_Info_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinRate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcActive)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.ComboBoxEdit cbbDept;
        private DevExpress.XtraEditors.SpinEdit spinRate;
        private DevExpress.XtraEditors.CheckEdit chkActive;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcDept;
        private DevExpress.XtraLayout.LayoutControlItem lcRate;
        private DevExpress.XtraLayout.LayoutControlItem lcActive;
    }
}
