namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    partial class f313_AbnormalCatalog_Info
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
            this.memoRemarks = new DevExpress.XtraEditors.MemoEdit();
            this.chkActive = new DevExpress.XtraEditors.CheckEdit();
            this.spinSort = new DevExpress.XtraEditors.SpinEdit();
            this.txbName = new DevExpress.XtraEditors.TextEdit();
            this.txbCode = new DevExpress.XtraEditors.TextEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSort = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcActive = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcRemarks = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoRemarks.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRemarks)).BeginInit();
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
            this.barDockControlTop.Size = new System.Drawing.Size(480, 49);
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Manager = this.barManagerTP;
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Controls.Add(this.txbCode);
            this.layoutControl1.Controls.Add(this.txbName);
            this.layoutControl1.Controls.Add(this.spinSort);
            this.layoutControl1.Controls.Add(this.chkActive);
            this.layoutControl1.Controls.Add(this.memoRemarks);
            this.layoutControl1.Root = this.Root;
            this.txbCode.StyleController = this.layoutControl1;
            this.txbName.StyleController = this.layoutControl1;
            this.spinSort.StyleController = this.layoutControl1;
            this.chkActive.StyleController = this.layoutControl1;
            this.memoRemarks.StyleController = this.layoutControl1;
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { this.lcCode, this.lcName, this.lcSort, this.lcActive, this.lcRemarks });
            this.Root.TextVisible = false;
            this.Root.Size = new System.Drawing.Size(480, 197);
            this.lcCode.Control = this.txbCode;
            this.lcCode.Location = new System.Drawing.Point(0, 0);
            this.lcCode.Size = new System.Drawing.Size(460, 36);
            this.lcCode.Text = "代碼";
            this.lcName.Control = this.txbName;
            this.lcName.Location = new System.Drawing.Point(0, 36);
            this.lcName.Size = new System.Drawing.Size(460, 36);
            this.lcName.Text = "名稱";
            this.lcSort.Control = this.spinSort;
            this.lcSort.Location = new System.Drawing.Point(0, 72);
            this.lcSort.Size = new System.Drawing.Size(460, 36);
            this.lcSort.Text = "排序";
            this.lcActive.Control = this.chkActive;
            this.lcActive.Location = new System.Drawing.Point(0, 108);
            this.lcActive.Size = new System.Drawing.Size(460, 25);
            this.lcActive.TextVisible = false;
            this.lcRemarks.Control = this.memoRemarks;
            this.lcRemarks.Location = new System.Drawing.Point(0, 133);
            this.lcRemarks.Size = new System.Drawing.Size(460, 44);
            this.lcRemarks.Text = "備註";
            this.ClientSize = new System.Drawing.Size(480, 246);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Abnormal Catalog";
            this.Load += new System.EventHandler(this.f313_AbnormalCatalog_Info_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoRemarks.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcRemarks)).EndInit();
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
        private DevExpress.XtraEditors.TextEdit txbCode;
        private DevExpress.XtraEditors.TextEdit txbName;
        private DevExpress.XtraEditors.SpinEdit spinSort;
        private DevExpress.XtraEditors.CheckEdit chkActive;
        private DevExpress.XtraEditors.MemoEdit memoRemarks;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcCode;
        private DevExpress.XtraLayout.LayoutControlItem lcName;
        private DevExpress.XtraLayout.LayoutControlItem lcSort;
        private DevExpress.XtraLayout.LayoutControlItem lcActive;
        private DevExpress.XtraLayout.LayoutControlItem lcRemarks;
    }
}
