namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    partial class f313_BatchCreate
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
            this.cbbTarget = new DevExpress.XtraEditors.ComboBoxEdit();
            this.datePeriod = new DevExpress.XtraEditors.DateEdit();
            this.spinRate = new DevExpress.XtraEditors.SpinEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcTarget = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPeriod = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSampleRate = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbbTarget.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datePeriod.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datePeriod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinRate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSampleRate)).BeginInit();
            this.SuspendLayout();
            // 
            // barManagerTP
            // 
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnConfirm});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 1;
            // 
            // bar2
            // 
            this.bar2.BarName = "Custom 2";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnConfirm)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Custom 2";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Caption = "確認";
            this.btnConfirm.Id = 0;
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(520, 30);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 166);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(520, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 30);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 136);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(520, 30);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 136);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cbbTarget);
            this.layoutControl1.Controls.Add(this.datePeriod);
            this.layoutControl1.Controls.Add(this.spinRate);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 30);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(520, 136);
            this.layoutControl1.TabIndex = 0;
            // 
            // cbbTarget
            // 
            this.cbbTarget.Location = new System.Drawing.Point(67, 12);
            this.cbbTarget.Name = "cbbTarget";
            this.cbbTarget.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbTarget.Size = new System.Drawing.Size(441, 22);
            this.cbbTarget.StyleController = this.layoutControl1;
            this.cbbTarget.TabIndex = 4;
            // 
            // datePeriod
            // 
            this.datePeriod.EditValue = new System.DateTime(2026, 3, 28, 0, 0, 0, 0);
            this.datePeriod.Location = new System.Drawing.Point(67, 40);
            this.datePeriod.Name = "datePeriod";
            this.datePeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.datePeriod.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.datePeriod.Size = new System.Drawing.Size(441, 22);
            this.datePeriod.StyleController = this.layoutControl1;
            this.datePeriod.TabIndex = 5;
            // 
            // spinRate
            // 
            this.spinRate.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinRate.Location = new System.Drawing.Point(67, 66);
            this.spinRate.Name = "spinRate";
            this.spinRate.Size = new System.Drawing.Size(441, 22);
            this.spinRate.StyleController = this.layoutControl1;
            this.spinRate.TabIndex = 6;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcTarget,
            this.lcPeriod,
            this.lcSampleRate});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(520, 136);
            this.Root.TextVisible = false;
            // 
            // lcTarget
            // 
            this.lcTarget.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcTarget.AppearanceItemCaption.Options.UseFont = true;
            this.lcTarget.Control = this.cbbTarget;
            this.lcTarget.Location = new System.Drawing.Point(0, 0);
            this.lcTarget.Name = "lcTarget";
            this.lcTarget.Size = new System.Drawing.Size(500, 28);
            this.lcTarget.Text = "目標";
            this.lcTarget.TextSize = new System.Drawing.Size(43, 24);
            // 
            // lcPeriod
            // 
            this.lcPeriod.Control = this.datePeriod;
            this.lcPeriod.Location = new System.Drawing.Point(0, 28);
            this.lcPeriod.Name = "lcPeriod";
            this.lcPeriod.Size = new System.Drawing.Size(500, 26);
            this.lcPeriod.Text = "期間";
            this.lcPeriod.TextSize = new System.Drawing.Size(43, 14);
            // 
            // lcSampleRate
            // 
            this.lcSampleRate.Control = this.spinRate;
            this.lcSampleRate.Location = new System.Drawing.Point(0, 54);
            this.lcSampleRate.Name = "lcSampleRate";
            this.lcSampleRate.Size = new System.Drawing.Size(500, 62);
            this.lcSampleRate.Text = "抽樣率%";
            this.lcSampleRate.TextSize = new System.Drawing.Size(43, 14);
            // 
            // f313_BatchCreate
            // 
            this.ClientSize = new System.Drawing.Size(520, 166);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "f313_BatchCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batch Create";
            this.Load += new System.EventHandler(this.f313_BatchCreate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbbTarget.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datePeriod.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datePeriod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinRate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSampleRate)).EndInit();
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
        private DevExpress.XtraEditors.ComboBoxEdit cbbTarget;
        private DevExpress.XtraEditors.DateEdit datePeriod;
        private DevExpress.XtraEditors.SpinEdit spinRate;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcTarget;
        private DevExpress.XtraLayout.LayoutControlItem lcPeriod;
        private DevExpress.XtraLayout.LayoutControlItem lcSampleRate;
    }
}
