namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    partial class f207_ViewFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(f207_ViewFile));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lbCanntView = new System.Windows.Forms.Label();
            this.viewExcel = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.viewWord = new DevExpress.XtraRichEdit.RichEditControl();
            this.viewPDF = new DevExpress.XtraPdfViewer.PdfViewer();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcPDF = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcExcel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcWord = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCanntView = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPDF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExcel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcWord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCanntView)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.lbCanntView);
            this.layoutControl1.Controls.Add(this.viewExcel);
            this.layoutControl1.Controls.Add(this.viewWord);
            this.layoutControl1.Controls.Add(this.viewPDF);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(606, 241, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(895, 643);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lbCanntView
            // 
            this.lbCanntView.Font = new System.Drawing.Font("DFKai-SB", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCanntView.Location = new System.Drawing.Point(449, 314);
            this.lbCanntView.Name = "lbCanntView";
            this.lbCanntView.Size = new System.Drawing.Size(434, 317);
            this.lbCanntView.TabIndex = 7;
            this.lbCanntView.Text = "謝謝";
            this.lbCanntView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // viewExcel
            // 
            this.viewExcel.Location = new System.Drawing.Point(12, 314);
            this.viewExcel.MenuManager = this.barManager1;
            this.viewExcel.Name = "viewExcel";
            this.viewExcel.ReadOnly = true;
            this.viewExcel.Size = new System.Drawing.Size(433, 317);
            this.viewExcel.TabIndex = 6;
            this.viewExcel.Text = "spreadsheetControl1";
            this.viewExcel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Viewer_KeyDown);
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
            this.btnSave});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 3;
            // 
            // bar2
            // 
            this.bar2.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Hovered.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Options.UseForeColor = true;
            this.bar2.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Normal.Options.UseFont = true;
            this.bar2.BarAppearance.Normal.Options.UseForeColor = true;
            this.bar2.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Pressed.Options.UseFont = true;
            this.bar2.BarAppearance.Pressed.Options.UseForeColor = true;
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnSave, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawBorder = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnSave
            // 
            this.btnSave.Caption = "下載";
            this.btnSave.Id = 0;
            this.btnSave.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnSave.ImageOptions.SvgImage")));
            this.btnSave.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnSave.Name = "btnSave";
            this.btnSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.barDockControlTop.Size = new System.Drawing.Size(895, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 692);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.barDockControlBottom.Size = new System.Drawing.Size(895, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 643);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(895, 49);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 643);
            // 
            // viewWord
            // 
            this.viewWord.Location = new System.Drawing.Point(449, 12);
            this.viewWord.MenuManager = this.barManager1;
            this.viewWord.Name = "viewWord";
            this.viewWord.ReadOnly = true;
            this.viewWord.Size = new System.Drawing.Size(434, 298);
            this.viewWord.TabIndex = 5;
            this.viewWord.Text = "richEditControl1";
            this.viewWord.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Viewer_KeyDown);
            // 
            // viewPDF
            // 
            this.viewPDF.Location = new System.Drawing.Point(12, 12);
            this.viewPDF.MenuManager = this.barManager1;
            this.viewPDF.Name = "viewPDF";
            this.viewPDF.NavigationPanePageVisibility = DevExpress.XtraPdfViewer.PdfNavigationPanePageVisibility.None;
            this.viewPDF.Size = new System.Drawing.Size(433, 298);
            this.viewPDF.TabIndex = 4;
            this.viewPDF.PopupMenuShowing += new DevExpress.XtraPdfViewer.PdfPopupMenuShowingEventHandler(this.pdfViewerData_PopupMenuShowing);
            this.viewPDF.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Viewer_KeyDown);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcPDF,
            this.lcExcel,
            this.lcWord,
            this.lcCanntView});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(895, 643);
            this.Root.TextVisible = false;
            // 
            // lcPDF
            // 
            this.lcPDF.Control = this.viewPDF;
            this.lcPDF.Location = new System.Drawing.Point(0, 0);
            this.lcPDF.Name = "lcPDF";
            this.lcPDF.Size = new System.Drawing.Size(437, 302);
            this.lcPDF.TextSize = new System.Drawing.Size(0, 0);
            this.lcPDF.TextVisible = false;
            // 
            // lcExcel
            // 
            this.lcExcel.Control = this.viewExcel;
            this.lcExcel.Location = new System.Drawing.Point(0, 302);
            this.lcExcel.Name = "lcExcel";
            this.lcExcel.Size = new System.Drawing.Size(437, 321);
            this.lcExcel.TextSize = new System.Drawing.Size(0, 0);
            this.lcExcel.TextVisible = false;
            // 
            // lcWord
            // 
            this.lcWord.Control = this.viewWord;
            this.lcWord.Location = new System.Drawing.Point(437, 0);
            this.lcWord.Name = "lcWord";
            this.lcWord.Size = new System.Drawing.Size(438, 302);
            this.lcWord.TextSize = new System.Drawing.Size(0, 0);
            this.lcWord.TextVisible = false;
            // 
            // lcCanntView
            // 
            this.lcCanntView.Control = this.lbCanntView;
            this.lcCanntView.Location = new System.Drawing.Point(437, 302);
            this.lcCanntView.Name = "lcCanntView";
            this.lcCanntView.Size = new System.Drawing.Size(438, 321);
            this.lcCanntView.TextSize = new System.Drawing.Size(0, 0);
            this.lcCanntView.TextVisible = false;
            // 
            // f207_ViewFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 692);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f207_ViewFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "f207_ViewPdf";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.f207_ViewFile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPDF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExcel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcWord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCanntView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraPdfViewer.PdfViewer viewPDF;
        private DevExpress.XtraLayout.LayoutControlItem lcPDF;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraRichEdit.RichEditControl viewWord;
        private DevExpress.XtraLayout.LayoutControlItem lcWord;
        private DevExpress.XtraSpreadsheet.SpreadsheetControl viewExcel;
        private DevExpress.XtraLayout.LayoutControlItem lcExcel;
        private System.Windows.Forms.Label lbCanntView;
        private DevExpress.XtraLayout.LayoutControlItem lcCanntView;
    }
}