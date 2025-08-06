using BusinessLayer;
using DataAccessLayer;
using DevExpress.Pdf;
using DevExpress.XtraEditors;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSpreadsheet.Model;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_PdfTools : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public string OutFileName { get; private set; }
        public string Describe { get; private set; }

        public f00_PdfTools(string FilePath, string OutDic, bool FullSign = false)
        {
            InitializeComponent();
            InitializeIcon();
            filePath = FilePath;
            outDic = OutDic;
            fullSign = FullSign;

            pdfViewer.MouseDown += PdfViewer_MouseDown;
            pdfViewer.MouseUp += PdfViewer_MouseUp;
            pdfViewer.MouseMove += PdfViewer_MouseMove;
            pdfViewer.Paint += PdfViewer_Paint;

            pdfViewer.PopupMenuShowing += PdfViewer_PopupMenuShowing;
            pdfViewer.KeyDown += PdfViewer_KeyDown;
            ribbonControl1.KeyDown += PdfViewer_KeyDown;

            pdfViewer.NavigationPanePageVisibility = PdfNavigationPanePageVisibility.None;
            KeyPreview = true;
        }

        #region parameters

        private void InitializeIcon()
        {
            btnStamp.ImageOptions.SvgImage = TPSvgimages.Stamp;
            btnSignDefault.ImageOptions.SvgImage = TPSvgimages.Approval;
        }

        class GraphicsCoordinates
        {
            public GraphicsCoordinates(int pageIndex, PdfPoint point1, PdfPoint point2, Image imageSign, string descrip)
            {
                PageIndex = pageIndex;
                Point1 = point1;
                Point2 = point2;
                ImageSign = imageSign;
                Descrip = descrip;
            }

            public int PageIndex { get; }
            public PdfPoint Point1 { get; }
            public PdfPoint Point2 { get; }
            public bool IsEmpty => Point1 == Point2;
            public Image ImageSign { get; }
            public string Descrip { get; }
        }

        SignInfo signInfo = SignInfo.Sign;
        List<dm_Sign> dmSigns;
        dm_Sign signSelect = new dm_Sign();

        List<GraphicsCoordinates> signs = new List<GraphicsCoordinates>();
        GraphicsCoordinates currentSign;

        bool fullSign = false;
        string filePath = "";
        string outDic = "";

        Image imageSign = null;
        string descrip = "";
        Font font = new Font("Times New Roman", 12, FontStyle.Regular);
        SizeF sizeFont = new SizeF();

        // This variable indicates whether the Drawing button is activated
        bool ActivateDrawing = false;

        #endregion

        #region methods

        private void DefaultSign()
        {
            if (!dmSigns.Any(r => r.ImgType == 0)) return;

            signSelect = dmSigns.FirstOrDefault(r => r.ImgType == 0);
            string signPath = Path.Combine(TPConfigs.FolderSign, signSelect.ImgName);
            imageSign = File.Exists(signPath) ? new Bitmap(signPath) : TPSvgimages.NoImage;

            descrip = DateTime.Now.ToString("yyyyMMdd");
        }

        void DrawImageRectangle(Graphics graphics, GraphicsCoordinates rect)
        {
            var image = rect.ImageSign;
            string descripSign = rect.Descrip;
            sizeFont = graphics.MeasureString(descripSign, font);

            var desHeight = (int)(sizeFont.Height);
            var desWidth = (int)(sizeFont.Width);

            PointF start = pdfViewer.GetClientPoint(new PdfDocumentPosition(rect.PageIndex + 1, rect.Point1));
            PointF end = pdfViewer.GetClientPoint(new PdfDocumentPosition(rect.PageIndex + 1, rect.Point2));
            // Create a rectangle where graphics should be drawn
            var recRectangle = Rectangle.FromLTRB((int)Math.Min(start.X, end.X), (int)Math.Min(start.Y, end.Y), (int)Math.Max(start.X, end.X), (int)Math.Max(start.Y, end.Y));
            var recSignImage = Rectangle.FromLTRB((int)Math.Min(start.X, end.X), (int)Math.Min(start.Y, end.Y), (int)Math.Max(start.X, end.X), (int)Math.Max(start.Y, end.Y) - desHeight);

            // Draw a rectangle in the created area
            graphics.DrawRectangle(new Pen(Color.Blue), recRectangle);

            // Vẽ chữ ký
            recSignImage = string.IsNullOrWhiteSpace(rect.Descrip) ? recRectangle : recSignImage;
            graphics.DrawImage(image, recSignImage);

            // Vẽ mô tả (Ngày tháng)
            if (string.IsNullOrWhiteSpace(rect.Descrip)) return;
            PointF point = new PointF(Math.Max(start.X, end.X) - desWidth, Math.Max(start.Y, end.Y) - desHeight);
            SolidBrush mybrush = new SolidBrush(Color.Black);
            graphics.DrawString(descripSign, font, mybrush, point);
        }

        void UpdateCurrentRect(Point location)
        {
            if (signs != null && currentSign != null && imageSign != null)
            {
                var documentPosition = pdfViewer.GetDocumentPosition(location, true);

                var desHeight = string.IsNullOrWhiteSpace(descrip) ? 0 : sizeFont.Height;
                var widthImage = imageSign.Width;
                var heightImage = imageSign.Height + desHeight;

                PdfPoint point1 = new PdfPoint();
                PdfPoint point2 = new PdfPoint();

                // Tạo toạ độ theo Chữ ký hoặc con dấu
                switch (signInfo)
                {
                    case SignInfo.Sign:
                        // Tính sự thay đổi của tọa độ Y
                        var deltaY = Math.Abs(documentPosition.Point.Y - currentSign.Point1.Y);
                        // Tính tỷ lệ thay đổi theo chiều dọc (Y)
                        var scaleY = (float)deltaY / heightImage;
                        // Tính toạ độ X mới không thay đổi
                        var YNew = documentPosition.Point.Y;
                        // Tính toạ độ Y mới dựa trên tỷ lệ thay đổi theo chiều dọc (scaleY)
                        var XNew = documentPosition.Point.X - currentSign.Point1.X < 0 ? currentSign.Point1.X - (int)(widthImage * scaleY) : currentSign.Point1.X + (int)(widthImage * scaleY);
                        point2 = new PdfPoint(XNew, YNew);
                        point1 = currentSign.Point1;
                        break;
                    case SignInfo.Stamp:
                        widthImage = signSelect.WidImg ?? 10;
                        heightImage = signSelect.HgtImg ?? 2;

                        XNew = documentPosition.Point.X - widthImage / 2;
                        YNew = documentPosition.Point.Y - heightImage / 2;
                        point1 = new PdfPoint(XNew, YNew);
                        XNew = documentPosition.Point.X + widthImage / 2;
                        YNew = documentPosition.Point.Y + heightImage / 2;
                        point2 = new PdfPoint(XNew, YNew);
                        break;
                    default:
                        break;
                }

                if (currentSign.PageIndex == documentPosition.PageNumber - 1)
                    currentSign = new GraphicsCoordinates(currentSign.PageIndex, point1, point2, imageSign, descrip);
            }
        }

        private void SaveDrawingAndReload()
        {
            string fileName = pdfViewer.DocumentFilePath;
            OutFileName = EncryptionHelper.EncryptionFileName(fileName);

            string fileNameSave = Path.Combine(outDic, OutFileName);
            pdfViewer.CloseDocument();
            using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
            {
                // Load a document to the PdfDocumentProcessor instance
                processor.LoadDocument(fileName);
                foreach (var rect in signs)
                {
                    // Create a PdfGraphics object
                    using (PdfGraphics graph = processor.CreateGraphics())
                    {
                        PdfPage page = processor.Document.Pages[rect.PageIndex];
                        PdfRectangle pageCropBox = page.CropBox;
                        PdfPoint p1 = new PdfPoint(rect.Point1.X, pageCropBox.Height - rect.Point1.Y);
                        PdfPoint p2 = new PdfPoint(rect.Point2.X, pageCropBox.Height - rect.Point2.Y);

                        var image = rect.ImageSign;
                        string desSign = rect.Descrip;
                        SizeF sizeFont = graph.MeasureString(desSign, font);

                        var desHeight = (float)(sizeFont.Height * 0.7);
                        var desWidth = (float)(sizeFont.Width * 0.75);

                        // Tạo khung vẽ vòng đỏ
                        RectangleF recRectangle = RectangleF.FromLTRB(
                            (float)Math.Min(p1.X, p2.X), (float)Math.Min(p1.Y, p2.Y),
                            (float)Math.Max(p1.X, p2.X), (float)Math.Max(p1.Y, p2.Y));

                        // Tạo khung vẽ chữ ký
                        RectangleF recSignImage = RectangleF.FromLTRB(
                           (float)Math.Min(p1.X, p2.X), (float)Math.Min(p1.Y, p2.Y),
                           (float)Math.Max(p1.X, p2.X), (float)Math.Max(p1.Y, p2.Y) - desHeight);

                        // Draw a rectangle in the created area
                        // graph.DrawRectangle(new Pen(Color.Red), recRectangle);

                        // Vẽ chữ ký
                        recSignImage = string.IsNullOrWhiteSpace(rect.Descrip) ? recRectangle : recSignImage;
                        graph.DrawImage(image, recSignImage);

                        // Vẽ phần mô tả chữ ký (Ngày tháng)
                        if (!string.IsNullOrWhiteSpace(rect.Descrip))
                        {
                            PointF point = new PointF((float)recRectangle.Right - desWidth, (float)recRectangle.Bottom - desHeight);
                            SolidBrush mybrush = new SolidBrush(Color.Black);
                            graph.DrawString(desSign, font, mybrush, point);
                        }
                        graph.AddToPageForeground(page, 72, 72);
                    }
                }
                // Save the document
                if (!Directory.Exists(outDic))
                    Directory.CreateDirectory(outDic);
                processor.SaveDocument(fileNameSave);
            }
            signs.Clear();
            ActivateDrawing = false;

            // Open the document in the PDF Viewer
            pdfViewer.LoadDocument(fileNameSave);
        }

        #endregion

        private void f00_PdfTools_Load(object sender, EventArgs e)
        {
            Text = "PDF工具";
            pdfViewer.LoadDocument(filePath);
            //pdfViewer.CursorMode = PdfCursorMode.HandTool;

            // Load các chữ ký, con dấu
            var signUsrs = dm_SignUsersBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id).ToList();
            var idSigns = signUsrs.Select(r => r.IdSign).ToList();

            dmSigns = dm_SignBUS.Instance.GetListByIdSigns(idSigns).OrderBy(r => r.Prioritize).ToList();

            DefaultSign();
        }

        private void PdfViewer_Paint(object sender, PaintEventArgs e)
        {
            if (ActivateDrawing)
            {
                foreach (var r in signs)
                    DrawImageRectangle(e.Graphics, r);
                if (currentSign != null)
                    DrawImageRectangle(e.Graphics, currentSign);
            }
        }

        private void PdfViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentSign != null)
            {
                UpdateCurrentRect(e.Location);
                pdfViewer.Invalidate();
            }
        }

        private void PdfViewer_MouseUp(object sender, MouseEventArgs e)
        {
            // Convert the retrieved coordinates to the page coordinates
            UpdateCurrentRect(e.Location);
            if (currentSign != null)
            {
                if (!currentSign.IsEmpty && ActivateDrawing)
                    // Add coordinates to the list
                    signs.Add(currentSign);
                currentSign = null;
            }
        }

        private void PdfViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (!pdfViewer.IsDocumentOpened)
            {
                Console.WriteLine("---------- No document loaded ----------");
                return;
            }

            var position = pdfViewer.GetDocumentPosition(e.Location, true);
            currentSign = new GraphicsCoordinates(position.PageNumber - 1, position.Point, position.Point, imageSign, descrip);
        }

        private void PdfViewer_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine($"{e.Control} {e.KeyCode}");

            if (e.Control && (e.KeyCode == Keys.P || e.KeyCode == Keys.S || e.KeyCode == Keys.O))
            {
                e.SuppressKeyPress = true;
                return;
            }
        }

        private void PdfViewer_PopupMenuShowing(object sender, PdfPopupMenuShowingEventArgs e)
        {
            e.ItemLinks.Clear();
        }

        private void btnSignDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!dmSigns.Any(r => r.ImgType == 0))
            {
                MsgTP.MsgError("你沒有簽名！");
                return;
            }

            DefaultSign();

            // Change the activation indicator
            ActivateDrawing = true;
            pdfViewer.Invalidate();
            signInfo = SignInfo.Sign;
        }

        private void btnClearSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            signs.Clear();
            pdfViewer.Invalidate();
        }

        private void btnAdvanced_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!dmSigns.Any(r => r.ImgType == 0))
            {
                MsgTP.MsgError("你沒有簽名！");
                return;
            }

            ActivateDrawing = true;
            pdfViewer.Invalidate();

            uc00_AdvancedSign ucAdvanced = new uc00_AdvancedSign(FullSign: fullSign);
            ucAdvanced.signInfo = SignInfo.Sign;
            if (XtraDialog.Show(ucAdvanced, "修改簽名", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                DefaultSign();
                return;
            }

            signInfo = SignInfo.Sign;
            imageSign = ucAdvanced.ImageSign;
            signSelect = ucAdvanced.SignSelect;
            descrip = ucAdvanced.DescripSign;
        }

        private void btnStamp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!dmSigns.Any(r => r.ImgType == 1))
            {
                MsgTP.MsgError("你沒有蓋章！");
                return;
            }

            ActivateDrawing = true;
            pdfViewer.Invalidate();

            uc00_AdvancedSign ucAdvanced = new uc00_AdvancedSign(FullSign: fullSign);
            ucAdvanced.signInfo = SignInfo.Stamp;
            if (XtraDialog.Show(ucAdvanced, "修改簽名", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                DefaultSign();
                return;
            }

            signInfo = SignInfo.Stamp;
            imageSign = ucAdvanced.ImageSign;
            signSelect = ucAdvanced.SignSelect;
            descrip = "";
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (signs.Count() == 0)
            {
                MsgTP.MsgError("你還沒簽名！");
                return;
            }

            SaveDrawingAndReload();

            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;
            Describe = result?.ToString() ?? "";

            Close();
        }
    }
}