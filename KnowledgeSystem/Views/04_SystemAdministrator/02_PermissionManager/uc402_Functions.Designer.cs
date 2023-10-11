namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    partial class uc402_Functions
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
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc402_Functions));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnUpdateRole = new DevExpress.XtraEditors.SimpleButton();
            this.txbNameRole = new DevExpress.XtraEditors.ButtonEdit();
            this.gcRoles = new DevExpress.XtraGrid.GridControl();
            this.gvRoles = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.treeFunctions = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn5 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn6 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn7 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnNewRole = new DevExpress.XtraBars.BarButtonItem();
            this.btnNewFunc = new DevExpress.XtraBars.BarButtonItem();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbNameRole.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcRoles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRoles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeFunctions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.layoutControl1.Controls.Add(this.btnUpdateRole);
            this.layoutControl1.Controls.Add(this.txbNameRole);
            this.layoutControl1.Controls.Add(this.gcRoles);
            this.layoutControl1.Controls.Add(this.treeFunctions);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1114, 424);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnUpdateRole
            // 
            this.btnUpdateRole.Appearance.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateRole.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnUpdateRole.Appearance.Options.UseFont = true;
            this.btnUpdateRole.Appearance.Options.UseForeColor = true;
            this.btnUpdateRole.Location = new System.Drawing.Point(989, 12);
            this.btnUpdateRole.Name = "btnUpdateRole";
            this.btnUpdateRole.Size = new System.Drawing.Size(113, 28);
            this.btnUpdateRole.StyleController = this.layoutControl1;
            this.btnUpdateRole.TabIndex = 7;
            this.btnUpdateRole.Text = "更新權限";
            this.btnUpdateRole.Click += new System.EventHandler(this.btnUpdateRole_Click);
            // 
            // txbNameRole
            // 
            this.txbNameRole.Location = new System.Drawing.Point(457, 12);
            this.txbNameRole.Name = "txbNameRole";
            this.txbNameRole.Properties.Appearance.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbNameRole.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbNameRole.Properties.Appearance.Options.UseFont = true;
            this.txbNameRole.Properties.Appearance.Options.UseForeColor = true;
            editorButtonImageOptions1.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.txbNameRole.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Close, "", -1, true, true, true, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.txbNameRole.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.txbNameRole.Size = new System.Drawing.Size(528, 28);
            this.txbNameRole.StyleController = this.layoutControl1;
            this.txbNameRole.TabIndex = 6;
            this.txbNameRole.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbNameRole_ButtonClick);
            // 
            // gcRoles
            // 
            this.gcRoles.Location = new System.Drawing.Point(12, 12);
            this.gcRoles.MainView = this.gvRoles;
            this.gcRoles.Name = "gcRoles";
            this.gcRoles.Size = new System.Drawing.Size(381, 400);
            this.gcRoles.TabIndex = 5;
            this.gcRoles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvRoles});
            // 
            // gvRoles
            // 
            this.gvRoles.Appearance.HeaderPanel.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvRoles.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvRoles.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvRoles.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvRoles.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvRoles.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvRoles.Appearance.Row.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvRoles.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvRoles.Appearance.Row.Options.UseFont = true;
            this.gvRoles.Appearance.Row.Options.UseForeColor = true;
            this.gvRoles.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2,
            this.gridColumn1});
            this.gvRoles.GridControl = this.gcRoles;
            this.gvRoles.Name = "gvRoles";
            this.gvRoles.OptionsView.ColumnAutoWidth = false;
            this.gvRoles.OptionsView.EnableAppearanceOddRow = true;
            this.gvRoles.OptionsView.ShowAutoFilterRow = true;
            this.gvRoles.OptionsView.ShowGroupPanel = false;
            this.gvRoles.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvRoles_PopupMenuShowing);
            this.gvRoles.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gvRoles_RowUpdated);
            this.gvRoles.DoubleClick += new System.EventHandler(this.gvRoles_DoubleClick);
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "名稱";
            this.gridColumn2.FieldName = "DisplayName";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "說明";
            this.gridColumn1.FieldName = "Describe";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            // 
            // treeFunctions
            // 
            this.treeFunctions.Appearance.HeaderPanel.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeFunctions.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.treeFunctions.Appearance.HeaderPanel.Options.UseFont = true;
            this.treeFunctions.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.treeFunctions.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.treeFunctions.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeFunctions.Appearance.Row.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeFunctions.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.treeFunctions.Appearance.Row.Options.UseFont = true;
            this.treeFunctions.Appearance.Row.Options.UseForeColor = true;
            this.treeFunctions.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2,
            this.treeListColumn3,
            this.treeListColumn4,
            this.treeListColumn5,
            this.treeListColumn6,
            this.treeListColumn7});
            this.treeFunctions.Location = new System.Drawing.Point(405, 44);
            this.treeFunctions.Name = "treeFunctions";
            this.treeFunctions.OptionsCustomization.AllowFilter = false;
            this.treeFunctions.OptionsCustomization.AllowSort = false;
            this.treeFunctions.OptionsView.AutoWidth = false;
            this.treeFunctions.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
            this.treeFunctions.OptionsView.EnableAppearanceOddRow = true;
            this.treeFunctions.OptionsView.ShowAutoFilterRow = true;
            this.treeFunctions.OptionsView.ShowIndicator = false;
            this.treeFunctions.Size = new System.Drawing.Size(697, 368);
            this.treeFunctions.TabIndex = 4;
            this.treeFunctions.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeFunctions_NodeCellStyle);
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "功能";
            this.treeListColumn1.FieldName = "DisplayName";
            this.treeListColumn1.MaxWidth = 250;
            this.treeListColumn1.MinWidth = 150;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.Width = 192;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "照片";
            this.treeListColumn2.FieldName = "ImageLive";
            this.treeListColumn2.MaxWidth = 60;
            this.treeListColumn2.MinWidth = 60;
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.OptionsColumn.AllowEdit = false;
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 2;
            this.treeListColumn2.Width = 60;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.AppearanceCell.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeListColumn3.AppearanceCell.Options.UseFont = true;
            this.treeListColumn3.Caption = "模組、頁面";
            this.treeListColumn3.FieldName = "ControlName";
            this.treeListColumn3.Name = "treeListColumn3";
            this.treeListColumn3.OptionsColumn.AllowEdit = false;
            this.treeListColumn3.Visible = true;
            this.treeListColumn3.VisibleIndex = 5;
            this.treeListColumn3.Width = 190;
            // 
            // treeListColumn4
            // 
            this.treeListColumn4.AppearanceCell.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeListColumn4.AppearanceCell.Options.UseFont = true;
            this.treeListColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn4.Caption = "優先級";
            this.treeListColumn4.FieldName = "Prioritize";
            this.treeListColumn4.MaxWidth = 75;
            this.treeListColumn4.MinWidth = 75;
            this.treeListColumn4.Name = "treeListColumn4";
            this.treeListColumn4.OptionsColumn.AllowEdit = false;
            this.treeListColumn4.UnboundDataType = typeof(short);
            this.treeListColumn4.Visible = true;
            this.treeListColumn4.VisibleIndex = 4;
            // 
            // treeListColumn5
            // 
            this.treeListColumn5.Caption = "ID";
            this.treeListColumn5.FieldName = "Id";
            this.treeListColumn5.Name = "treeListColumn5";
            this.treeListColumn5.UnboundDataType = typeof(short);
            this.treeListColumn5.Width = 95;
            // 
            // treeListColumn6
            // 
            this.treeListColumn6.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn6.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn6.Caption = "編號";
            this.treeListColumn6.FieldName = "IdChild";
            this.treeListColumn6.MaxWidth = 60;
            this.treeListColumn6.MinWidth = 60;
            this.treeListColumn6.Name = "treeListColumn6";
            this.treeListColumn6.OptionsColumn.AllowEdit = false;
            this.treeListColumn6.UnboundDataType = typeof(short);
            this.treeListColumn6.UnboundExpression = "[Id]";
            this.treeListColumn6.Visible = true;
            this.treeListColumn6.VisibleIndex = 3;
            this.treeListColumn6.Width = 60;
            // 
            // treeListColumn7
            // 
            this.treeListColumn7.Caption = "權限";
            this.treeListColumn7.FieldName = "SetPermission";
            this.treeListColumn7.MaxWidth = 60;
            this.treeListColumn7.MinWidth = 60;
            this.treeListColumn7.Name = "treeListColumn7";
            this.treeListColumn7.Visible = true;
            this.treeListColumn7.VisibleIndex = 1;
            this.treeListColumn7.Width = 60;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1114, 424);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.treeFunctions;
            this.layoutControlItem1.Location = new System.Drawing.Point(393, 32);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(701, 372);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gcRoles;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 10, 2, 2);
            this.layoutControlItem2.Size = new System.Drawing.Size(393, 404);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem3.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem3.Control = this.txbNameRole;
            this.layoutControlItem3.Location = new System.Drawing.Point(393, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(584, 32);
            this.layoutControlItem3.Text = "權限";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(40, 19);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnUpdateRole;
            this.layoutControlItem4.Location = new System.Drawing.Point(977, 0);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(117, 0);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(117, 28);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(117, 32);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // barManager1
            // 
            this.barManager1.AllowMoveBarOnToolbar = false;
            this.barManager1.AllowQuickCustomization = false;
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnNewRole,
            this.btnReload,
            this.btnNewFunc});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 5;
            // 
            // bar2
            // 
            this.bar2.BarAppearance.Hovered.Font = new System.Drawing.Font("DFKai-SB", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bar2.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Hovered.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Options.UseForeColor = true;
            this.bar2.BarAppearance.Normal.Font = new System.Drawing.Font("DFKai-SB", 14.25F);
            this.bar2.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Normal.Options.UseFont = true;
            this.bar2.BarAppearance.Normal.Options.UseForeColor = true;
            this.bar2.BarAppearance.Pressed.Font = new System.Drawing.Font("DFKai-SB", 14.25F);
            this.bar2.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Pressed.Options.UseFont = true;
            this.bar2.BarAppearance.Pressed.Options.UseForeColor = true;
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnNewRole),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnNewFunc, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnReload, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawBorder = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnNewRole
            // 
            this.btnNewRole.Caption = "新增權限";
            this.btnNewRole.Id = 1;
            this.btnNewRole.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnNewRole.ImageOptions.SvgImage")));
            this.btnNewRole.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnNewRole.Name = "btnNewRole";
            this.btnNewRole.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnNewRole.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnNewRole_ItemClick);
            // 
            // btnNewFunc
            // 
            this.btnNewFunc.Caption = "新增功能";
            this.btnNewFunc.Id = 4;
            this.btnNewFunc.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnNewFunc.Name = "btnNewFunc";
            // 
            // btnReload
            // 
            this.btnReload.Caption = "重新整理";
            this.btnReload.Id = 3;
            this.btnReload.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnReload.Name = "btnReload";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.barDockControlTop.Size = new System.Drawing.Size(1114, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 473);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.barDockControlBottom.Size = new System.Drawing.Size(1114, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 424);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1114, 49);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 424);
            // 
            // uc402_Functions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "uc402_Functions";
            this.Size = new System.Drawing.Size(1114, 473);
            this.Load += new System.EventHandler(this.uc402_Functions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbNameRole.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcRoles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRoles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeFunctions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraTreeList.TreeList treeFunctions;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn4;
        private DevExpress.XtraGrid.GridControl gcRoles;
        private DevExpress.XtraGrid.Views.Grid.GridView gvRoles;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn5;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.ButtonEdit txbNameRole;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SimpleButton btnUpdateRole;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn7;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnNewRole;
        private DevExpress.XtraBars.BarButtonItem btnReload;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem btnNewFunc;
    }
}
