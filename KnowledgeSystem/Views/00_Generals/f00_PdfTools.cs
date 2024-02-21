using DevExpress.Pdf;
using DevExpress.XtraEditors;
using DevExpress.XtraPdfViewer;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_PdfTools : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public f00_PdfTools()
        {
            InitializeComponent();
            pdfViewer.MouseDown += PdfViewer_MouseDown;
            pdfViewer.MouseUp += PdfViewer_MouseUp;
            pdfViewer.MouseMove += PdfViewer_MouseMove;
            pdfViewer.Paint += PdfViewer_Paint;

            pdfViewer.NavigationPanePageVisibility = PdfNavigationPanePageVisibility.None;
        }


        #region parameters

        class GraphicsCoordinates
        {
            public GraphicsCoordinates(int pageIndex, PdfPoint point1, PdfPoint point2, Image imageSign)
            {
                PageIndex = pageIndex;
                Point1 = point1;
                Point2 = point2;
                ImageSign = imageSign;
            }

            public Image ImageSign { get; }
            public int PageIndex { get; }
            public PdfPoint Point1 { get; }
            public PdfPoint Point2 { get; }
            public bool IsEmpty => Point1 == Point2;
        }

        List<GraphicsCoordinates> rectangleCoordinateList = new List<GraphicsCoordinates>();
        GraphicsCoordinates currentCoordinates;

        Image imageSign = null;

        // This variable indicates whether the Drawing button is activated
        bool ActivateDrawing = false;

        #endregion

        #region methods

        void DrawImageRectangle(Graphics graphics, GraphicsCoordinates rect)
        {
            var image = rect.ImageSign;

            PointF start = pdfViewer.GetClientPoint(new PdfDocumentPosition(rect.PageIndex + 1, rect.Point1));
            PointF end = pdfViewer.GetClientPoint(new PdfDocumentPosition(rect.PageIndex + 1, rect.Point2));
            // Create a rectangle where graphics should be drawn
            var r = Rectangle.FromLTRB((int)Math.Min(start.X, end.X), (int)Math.Min(start.Y, end.Y), (int)Math.Max(start.X, end.X), (int)Math.Max(start.Y, end.Y));
            var r1 = Rectangle.FromLTRB((int)Math.Min(start.X, end.X), (int)Math.Min(start.Y, end.Y), (int)Math.Max(start.X, end.X), (int)Math.Max(start.Y, end.Y) - 20);

            // Draw a rectangle in the created area
            graphics.DrawRectangle(new Pen(Color.Red), r);

            graphics.DrawImage(image, r1);

            PointF point = new PointF(Math.Max(start.X, end.X) - 95, Math.Max(start.Y, end.Y) - 20);
            SolidBrush mybrush = new SolidBrush(Color.Black);
            //graph.DrawString("Here my Text", myfont, mybrush, point);
            Font font = new Font("Times New Roman", 14, FontStyle.Regular);
            graphics.DrawString(DateTime.Now.ToString("yyyy.MM.dd"), font, mybrush, point);
        }

        void UpdateCurrentRect(Point location)
        {
            if (rectangleCoordinateList != null && currentCoordinates != null)
            {
                var documentPosition = pdfViewer.GetDocumentPosition(location, true);

                var widthImage = imageSign.Width;
                var heightImage = imageSign.Height + 25;

                // Tính sự thay đổi của tọa độ Y
                var deltaY = Math.Abs(documentPosition.Point.Y - currentCoordinates.Point1.Y);

                // Tính tỷ lệ thay đổi theo chiều dọc (Y)
                var scaleY = (float)deltaY / heightImage;

                // Tính toạ độ X mới không thay đổi
                var YNew = documentPosition.Point.Y;

                // Tính toạ độ Y mới dựa trên tỷ lệ thay đổi theo chiều dọc (scaleY)
                var XNew = documentPosition.Point.X - currentCoordinates.Point1.X < 0 ? currentCoordinates.Point1.X - (int)(widthImage * scaleY) : currentCoordinates.Point1.X + (int)(widthImage * scaleY);

                // Tạo điểm mới
                PdfPoint newPoint = new PdfPoint(XNew, YNew);

                if (currentCoordinates.PageIndex == documentPosition.PageNumber - 1)
                    currentCoordinates = new GraphicsCoordinates(currentCoordinates.PageIndex, currentCoordinates.Point1, newPoint, imageSign);
            }
        }

        private void SaveDrawingAndReload()
        {
            string fileName = pdfViewer.DocumentFilePath;
            string fileNameSave = System.IO.Path.GetDirectoryName(pdfViewer.DocumentFilePath) + "\\" + DateTime.Now.ToString("mmhhss") + ".pdf";
            pdfViewer.CloseDocument();
            using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
            {
                // Load a document to the PdfDocumentProcessor instance
                processor.LoadDocument(fileName);
                foreach (var rect in rectangleCoordinateList)
                {
                    // Create a PdfGraphics object
                    using (PdfGraphics graph = processor.CreateGraphics())
                    {
                        PdfPage page = processor.Document.Pages[rect.PageIndex];
                        PdfRectangle pageCropBox = page.CropBox;
                        PdfPoint p1 = new PdfPoint(rect.Point1.X, pageCropBox.Height - rect.Point1.Y);
                        PdfPoint p2 = new PdfPoint(rect.Point2.X, pageCropBox.Height - rect.Point2.Y);

                        // Create a rectangle where graphics should be drawn
                        RectangleF bounds = RectangleF.FromLTRB(
                            (float)Math.Min(p1.X, p2.X), (float)Math.Min(p1.Y, p2.Y),
                            (float)Math.Max(p1.X, p2.X), (float)Math.Max(p1.Y, p2.Y));
                        RectangleF bounds2 = RectangleF.FromLTRB(
                           (float)Math.Min(p1.X, p2.X), (float)Math.Min(p1.Y, p2.Y),
                           (float)Math.Max(p1.X, p2.X), (float)Math.Max(p1.Y, p2.Y) - 15);
                        // Draw a rectangle in the created area
                        //  graph.DrawRectangle(new Pen(Color.Red), bounds);
                        var image = rect.ImageSign;
                        graph.DrawImage(image, bounds2);

                        PointF point = new PointF((float)Math.Max(p1.X, p2.X) - 65, (float)Math.Max(p1.Y, p2.Y) - 15);
                        SolidBrush mybrush = new SolidBrush(Color.Black);
                        //graph.DrawString("Here my Text", myfont, mybrush, point);
                        Font font = new Font("Times New Roman", 14, FontStyle.Regular);
                        graph.DrawString(DateTime.Now.ToString("yyyy.MM.dd"), font, mybrush, point);

                        // Draw graphics content into a file
                        graph.AddToPageForeground(page, 72, 72);
                    }
                }
                // Save the document

                processor.SaveDocument(fileNameSave);
            }
            rectangleCoordinateList.Clear();
            ActivateDrawing = false;

            // Open the document in the PDF Viewer
            pdfViewer.LoadDocument(fileNameSave);
        }

        #endregion

        private void f00_PdfTools_Load(object sender, EventArgs e)
        {
            imageSign = Image.FromFile(@"E:\01. Softwares Programming\24. Knowledge System\02. Images\sign.png");
        }

        private void PdfViewer_Paint(object sender, PaintEventArgs e)
        {
            if (ActivateDrawing)
            {
                foreach (var r in rectangleCoordinateList)
                    DrawImageRectangle(e.Graphics, r);
                if (currentCoordinates != null)
                    DrawImageRectangle(e.Graphics, currentCoordinates);
            }
        }

        private void PdfViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentCoordinates != null)
            {
                UpdateCurrentRect(e.Location);
                pdfViewer.Invalidate();
            }
        }

        private void PdfViewer_MouseUp(object sender, MouseEventArgs e)
        {
            // Convert the retrieved coordinates to the page coordinates
            UpdateCurrentRect(e.Location);
            if (currentCoordinates != null)
            {
                if (!currentCoordinates.IsEmpty && ActivateDrawing)
                    // Add coordinates to the list
                    rectangleCoordinateList.Add(currentCoordinates);
                currentCoordinates = null;
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
            currentCoordinates = new GraphicsCoordinates(position.PageNumber - 1, position.Point, position.Point, imageSign);
        }

        private void btnSignDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Change the activation indicator
            ActivateDrawing = !ActivateDrawing;
            pdfViewer.Invalidate();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveDrawingAndReload();
        }

        private void btnClearSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            rectangleCoordinateList.Clear();
            pdfViewer.Invalidate();
        }
    }
}