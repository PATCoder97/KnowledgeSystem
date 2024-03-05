namespace KnowledgeSystem.Views._03_DepartmentManage._04_BorrVehicle
{
    partial class uc304_BorrVehicleMain
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gcInfo = new DevExpress.XtraGrid.GridControl();
            this.gvInfo = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcVehicleStatus = new DevExpress.XtraGrid.GridControl();
            this.gvVehicleStatus = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cbbTypeVehicle = new DevExpress.XtraEditors.ComboBoxEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcVehicleStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvVehicleStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbTypeVehicle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Controls.Add(this.gcInfo);
            this.layoutControl1.Controls.Add(this.gcVehicleStatus);
            this.layoutControl1.Controls.Add(this.cbbTypeVehicle);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1145, 212, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(812, 504);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gcInfo
            // 
            this.gcInfo.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcInfo.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcInfo.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcInfo.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcInfo.EmbeddedNavigator.Buttons.Remove.Visible = false;
            this.gcInfo.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.gcInfo.Location = new System.Drawing.Point(12, 301);
            this.gcInfo.MainView = this.gvInfo;
            this.gcInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gcInfo.Name = "gcInfo";
            this.gcInfo.Size = new System.Drawing.Size(788, 191);
            this.gcInfo.TabIndex = 7;
            this.gcInfo.UseEmbeddedNavigator = true;
            this.gcInfo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvInfo});
            // 
            // gvInfo
            // 
            this.gvInfo.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvInfo.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvInfo.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvInfo.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvInfo.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvInfo.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvInfo.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvInfo.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvInfo.Appearance.Row.Options.UseFont = true;
            this.gvInfo.Appearance.Row.Options.UseForeColor = true;
            this.gvInfo.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn6,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13,
            this.gridColumn14,
            this.gridColumn15});
            this.gvInfo.DetailHeight = 377;
            this.gvInfo.GridControl = this.gcInfo;
            this.gvInfo.Name = "gvInfo";
            this.gvInfo.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvInfo.OptionsView.ColumnAutoWidth = false;
            this.gvInfo.OptionsView.EnableAppearanceOddRow = true;
            this.gvInfo.OptionsView.ShowAutoFilterRow = true;
            this.gvInfo.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Người mượn";
            this.gridColumn3.FieldName = "IdUserBorr";
            this.gridColumn3.MinWidth = 23;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            this.gridColumn3.Width = 87;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Tên người mượn";
            this.gridColumn4.FieldName = "NameUserBorr";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 1;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Thời gian mượn";
            this.gridColumn6.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
            this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn6.FieldName = "BorrTime";
            this.gridColumn6.MinWidth = 23;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 2;
            this.gridColumn6.Width = 87;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Thời gian trả";
            this.gridColumn10.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
            this.gridColumn10.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn10.FieldName = "BackTime";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 3;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Địa điểm";
            this.gridColumn11.FieldName = "Place";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 4;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "Mục đích";
            this.gridColumn12.FieldName = "Uses";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 5;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "Km bắt đầu";
            this.gridColumn13.FieldName = "StartKm";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 6;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "Km kết thúc";
            this.gridColumn14.FieldName = "EndKm";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 7;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "Tổng Km";
            this.gridColumn15.FieldName = "TotalKm";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 8;
            // 
            // gcVehicleStatus
            // 
            this.gcVehicleStatus.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcVehicleStatus.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcVehicleStatus.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcVehicleStatus.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcVehicleStatus.EmbeddedNavigator.Buttons.Remove.Visible = false;
            this.gcVehicleStatus.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.gcVehicleStatus.Location = new System.Drawing.Point(12, 48);
            this.gcVehicleStatus.MainView = this.gvVehicleStatus;
            this.gcVehicleStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gcVehicleStatus.Name = "gcVehicleStatus";
            this.gcVehicleStatus.Size = new System.Drawing.Size(788, 249);
            this.gcVehicleStatus.TabIndex = 6;
            this.gcVehicleStatus.UseEmbeddedNavigator = true;
            this.gcVehicleStatus.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvVehicleStatus});
            // 
            // gvVehicleStatus
            // 
            this.gvVehicleStatus.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvVehicleStatus.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvVehicleStatus.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvVehicleStatus.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvVehicleStatus.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvVehicleStatus.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvVehicleStatus.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvVehicleStatus.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvVehicleStatus.Appearance.Row.Options.UseFont = true;
            this.gvVehicleStatus.Appearance.Row.Options.UseForeColor = true;
            this.gvVehicleStatus.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColId,
            this.gridColumn7,
            this.gridColumn5,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn1});
            this.gvVehicleStatus.DetailHeight = 377;
            this.gvVehicleStatus.GridControl = this.gcVehicleStatus;
            this.gvVehicleStatus.Name = "gvVehicleStatus";
            this.gvVehicleStatus.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvVehicleStatus.OptionsView.ColumnAutoWidth = false;
            this.gvVehicleStatus.OptionsView.EnableAppearanceOddRow = true;
            this.gvVehicleStatus.OptionsView.ShowAutoFilterRow = true;
            this.gvVehicleStatus.OptionsView.ShowGroupPanel = false;
            this.gvVehicleStatus.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvVehicleStatus_PopupMenuShowing);
            // 
            // gColId
            // 
            this.gColId.Caption = "Biển số xe";
            this.gColId.FieldName = "Name";
            this.gColId.MinWidth = 23;
            this.gColId.Name = "gColId";
            this.gColId.Visible = true;
            this.gColId.VisibleIndex = 0;
            this.gColId.Width = 87;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Bộ phận quản lý";
            this.gridColumn7.FieldName = "Dept";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 1;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Trạng thái";
            this.gridColumn5.FieldName = "Status";
            this.gridColumn5.MinWidth = 23;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 2;
            this.gridColumn5.Width = 87;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Người mượn";
            this.gridColumn8.FieldName = "IdUserBorr";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 3;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Tên người mượn";
            this.gridColumn9.FieldName = "NameUserBorr";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 4;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Thời gian mượn";
            this.gridColumn1.FieldName = "BorrTime";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 5;
            // 
            // cbbTypeVehicle
            // 
            this.cbbTypeVehicle.Location = new System.Drawing.Point(134, 12);
            this.cbbTypeVehicle.Name = "cbbTypeVehicle";
            this.cbbTypeVehicle.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbTypeVehicle.Properties.Appearance.Options.UseFont = true;
            this.cbbTypeVehicle.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbTypeVehicle.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbTypeVehicle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbTypeVehicle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbbTypeVehicle.Size = new System.Drawing.Size(182, 32);
            this.cbbTypeVehicle.StyleController = this.layoutControl1;
            this.cbbTypeVehicle.TabIndex = 4;
            this.cbbTypeVehicle.SelectedIndexChanged += new System.EventHandler(this.cbbTypeVehicle_SelectedIndexChanged);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem4});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(812, 504);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.Control = this.cbbTypeVehicle;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(308, 36);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(308, 36);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(308, 36);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.Text = "Chọn loại xe";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(110, 24);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gcVehicleStatus;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 36);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(0, 253);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(104, 253);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(792, 253);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(419, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(373, 36);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gcInfo;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 289);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(792, 195);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Appearance.Options.UseFont = true;
            this.btnRefresh.Location = new System.Drawing.Point(328, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(99, 32);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem4.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem4.Control = this.btnRefresh;
            this.layoutControlItem4.Location = new System.Drawing.Point(308, 0);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(111, 0);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(111, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
            this.layoutControlItem4.Size = new System.Drawing.Size(111, 36);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // uc304_BorrVehicleMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "uc304_BorrVehicleMain";
            this.Size = new System.Drawing.Size(812, 504);
            this.Load += new System.EventHandler(this.uc304_BorrVehicleMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcVehicleStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvVehicleStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbTypeVehicle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.ComboBoxEdit cbbTypeVehicle;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl gcVehicleStatus;
        private DevExpress.XtraGrid.Views.Grid.GridView gvVehicleStatus;
        private DevExpress.XtraGrid.Columns.GridColumn gColId;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.GridControl gcInfo;
        private DevExpress.XtraGrid.Views.Grid.GridView gvInfo;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
    }
}
