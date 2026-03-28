namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    partial class f313_InspectionResult_Info
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblInfo = new DevExpress.XtraEditors.LabelControl();
            this.panelTop = new System.Windows.Forms.TableLayoutPanel();
            this.cbbResult = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cbbAbnormal = new DevExpress.XtraEditors.ComboBoxEdit();
            this.chkCloseCorrection = new DevExpress.XtraEditors.CheckEdit();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblAbnormal = new System.Windows.Forms.Label();
            this.groupAbnormal = new DevExpress.XtraEditors.GroupControl();
            this.memoAbnormal = new DevExpress.XtraEditors.MemoEdit();
            this.groupCorrection = new DevExpress.XtraEditors.GroupControl();
            this.memoCorrection = new DevExpress.XtraEditors.MemoEdit();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelAbnormal = new System.Windows.Forms.TableLayoutPanel();
            this.lbAbnormal = new System.Windows.Forms.ListBox();
            this.btnAddAbnormal = new DevExpress.XtraEditors.SimpleButton();
            this.btnViewAbnormal = new DevExpress.XtraEditors.SimpleButton();
            this.btnRemoveAbnormal = new DevExpress.XtraEditors.SimpleButton();
            this.panelCorrection = new System.Windows.Forms.TableLayoutPanel();
            this.lbCorrection = new System.Windows.Forms.ListBox();
            this.btnAddCorrection = new DevExpress.XtraEditors.SimpleButton();
            this.btnViewCorrection = new DevExpress.XtraEditors.SimpleButton();
            this.btnRemoveCorrection = new DevExpress.XtraEditors.SimpleButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbbResult.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbAbnormal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCloseCorrection.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupAbnormal)).BeginInit();
            this.groupAbnormal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoAbnormal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupCorrection)).BeginInit();
            this.groupCorrection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoCorrection.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelAbnormal.SuspendLayout();
            this.panelCorrection.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblInfo, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelTop, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupAbnormal, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.groupCorrection, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(920, 700);
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.Appearance.Font = KnowledgeSystem.Helpers.TPConfigs.fontUI14;
            this.panelTop.ColumnCount = 3;
            this.panelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.panelTop.Controls.Add(this.lblResult, 0, 0);
            this.panelTop.Controls.Add(this.lblAbnormal, 1, 0);
            this.panelTop.Controls.Add(this.cbbResult, 0, 1);
            this.panelTop.Controls.Add(this.cbbAbnormal, 1, 1);
            this.panelTop.Controls.Add(this.chkCloseCorrection, 2, 1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblResult.Text = "檢查結果";
            this.lblAbnormal.Text = "異常項目";
            this.lblResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAbnormal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbbResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbbAbnormal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkCloseCorrection.Text = "管理端關閉改善";
            this.chkCloseCorrection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupAbnormal.Text = "異常說明";
            this.groupAbnormal.Controls.Add(this.memoAbnormal);
            this.groupAbnormal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoAbnormal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupCorrection.Text = "改善說明";
            this.groupCorrection.Controls.Add(this.memoCorrection);
            this.groupCorrection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoCorrection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Panel1.Controls.Add(this.panelAbnormal);
            this.splitContainer1.Panel2.Controls.Add(this.panelCorrection);
            this.panelAbnormal.ColumnCount = 2;
            this.panelAbnormal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelAbnormal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.panelAbnormal.Controls.Add(this.lbAbnormal, 0, 0);
            this.panelAbnormal.Controls.Add(this.btnAddAbnormal, 1, 0);
            this.panelAbnormal.Controls.Add(this.btnViewAbnormal, 1, 1);
            this.panelAbnormal.Controls.Add(this.btnRemoveAbnormal, 1, 2);
            this.panelAbnormal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAbnormal.RowCount = 3;
            this.panelAbnormal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.panelAbnormal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.panelAbnormal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.lbAbnormal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCorrection.ColumnCount = 2;
            this.panelCorrection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelCorrection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.panelCorrection.Controls.Add(this.lbCorrection, 0, 0);
            this.panelCorrection.Controls.Add(this.btnAddCorrection, 1, 0);
            this.panelCorrection.Controls.Add(this.btnViewCorrection, 1, 1);
            this.panelCorrection.Controls.Add(this.btnRemoveCorrection, 1, 2);
            this.panelCorrection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCorrection.RowCount = 3;
            this.panelCorrection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.panelCorrection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.panelCorrection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.lbCorrection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddAbnormal.Text = "新增";
            this.btnViewAbnormal.Text = "查看";
            this.btnRemoveAbnormal.Text = "移除";
            this.btnAddCorrection.Text = "新增";
            this.btnViewCorrection.Text = "查看";
            this.btnRemoveCorrection.Text = "移除";
            this.btnAddAbnormal.Click += new System.EventHandler(this.btnAddAbnormal_Click);
            this.btnViewAbnormal.Click += new System.EventHandler(this.btnViewAbnormal_Click);
            this.btnRemoveAbnormal.Click += new System.EventHandler(this.btnRemoveAbnormal_Click);
            this.btnAddCorrection.Click += new System.EventHandler(this.btnAddCorrection_Click);
            this.btnViewCorrection.Click += new System.EventHandler(this.btnViewCorrection_Click);
            this.btnRemoveCorrection.Click += new System.EventHandler(this.btnRemoveCorrection_Click);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.btnCancel.Text = "取消";
            this.btnSave.Text = "儲存";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.ClientSize = new System.Drawing.Size(920, 700);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "f313_InspectionResult_Info";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Inspection Result";
            this.Load += new System.EventHandler(this.f313_InspectionResult_Info_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbbResult.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbAbnormal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCloseCorrection.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupAbnormal)).EndInit();
            this.groupAbnormal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoAbnormal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupCorrection)).EndInit();
            this.groupCorrection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoCorrection.Properties)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelAbnormal.ResumeLayout(false);
            this.panelCorrection.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.LabelControl lblInfo;
        private System.Windows.Forms.TableLayoutPanel panelTop;
        private DevExpress.XtraEditors.ComboBoxEdit cbbResult;
        private DevExpress.XtraEditors.ComboBoxEdit cbbAbnormal;
        private DevExpress.XtraEditors.CheckEdit chkCloseCorrection;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblAbnormal;
        private DevExpress.XtraEditors.GroupControl groupAbnormal;
        private DevExpress.XtraEditors.MemoEdit memoAbnormal;
        private DevExpress.XtraEditors.GroupControl groupCorrection;
        private DevExpress.XtraEditors.MemoEdit memoCorrection;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel panelAbnormal;
        private System.Windows.Forms.ListBox lbAbnormal;
        private DevExpress.XtraEditors.SimpleButton btnAddAbnormal;
        private DevExpress.XtraEditors.SimpleButton btnViewAbnormal;
        private DevExpress.XtraEditors.SimpleButton btnRemoveAbnormal;
        private System.Windows.Forms.TableLayoutPanel panelCorrection;
        private System.Windows.Forms.ListBox lbCorrection;
        private DevExpress.XtraEditors.SimpleButton btnAddCorrection;
        private DevExpress.XtraEditors.SimpleButton btnViewCorrection;
        private DevExpress.XtraEditors.SimpleButton btnRemoveCorrection;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnSave;
    }
}
