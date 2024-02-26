using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_CreateSign : DevExpress.XtraEditors.XtraForm
    {
        public f00_CreateSign()
        {
            InitializeComponent();
            InitializeIcon();
        }

        List<string> types = new List<string> { "簽名", "密封" };
        string imgSignPath = "";
        Image ImageSign = null;

        string letter = DateTime.Today.ToString("yyyy.MM.dd");
        Font font = new Font("Times New Roman", 12, FontStyle.Bold);
        Color dateTimeColor = Color.FromArgb(210, 14, 18);

        int imgWid, imgHgt, px, py = 0;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void DrawStamp()
        {
            if (ImageSign == null) return;

            var img = new Bitmap(ImageSign);
            Graphics g = Graphics.FromImage(img);

            SizeF size = g.MeasureString(letter.ToString(), font);
            var bit = new Bitmap(img.Width, (int)Math.Ceiling(size.Height));

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.InterpolationMode = InterpolationMode.High;

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;

            // Top/Left
            sf.Alignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Near;

            Rectangle rect = new Rectangle(px, py, bit.Width, bit.Height);
            g.DrawString(letter, font, new SolidBrush(dateTimeColor), rect, sf);
            //g.DrawRectangle(new Pen(Color.Black), rect);

            var imageOut = MergeTwoImages(img, bit);
            picSign.Image = imageOut;
        }

        private int SizeLabelFont(string text, int wid, int hgt)
        {
            string txt = text;
            int best_size = 100;
            if (txt.Length > 0)
            {
                using (Graphics gr = this.CreateGraphics())
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        using (Font test_font = new Font(this.Font.FontFamily, i))
                        {
                            SizeF text_size = gr.MeasureString(txt, test_font);
                            if ((text_size.Width > wid) ||
                                (text_size.Height > hgt))
                            {
                                best_size = i - 1;
                                return best_size;

                            }
                        }
                    }
                }
            }
            return best_size;
        }

        public Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            if (firstImage == null)
            {
                throw new ArgumentNullException("firstImage");
            }

            if (secondImage == null)
            {
                throw new ArgumentNullException("secondImage");
            }

            int outputImageWidth = firstImage.Width > secondImage.Width ? firstImage.Width : secondImage.Width;

            int outputImageHeight = firstImage.Height;// + secondImage.Height + 1;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(firstImage, new Rectangle(new Point(), firstImage.Size),
                    new Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(secondImage, new Rectangle(new Point(0, firstImage.Height + 1), secondImage.Size),
                    new Rectangle(new Point(), secondImage.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }

        private void txbFont_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = font;
            fontDialog.ShowDialog();

            font = fontDialog.Font;
            txbFont.EditValue = $"{font.Name}, {font.Size}, {font.Style}";

            DrawStamp();
        }

        private void f00_CreateSign_Load(object sender, EventArgs e)
        {
            cbbType.Properties.Items.AddRange(types);
            txbFont.EditValue = $"{font.Name}, {font.Size}, {font.Style}";
            colorFont.Color = dateTimeColor;
        }

        private void txbX_EditValueChanged(object sender, EventArgs e)
        {
            int.TryParse(txbX.EditValue?.ToString(), out px);
            DrawStamp();
        }

        private void txbY_EditValueChanged(object sender, EventArgs e)
        {
            int.TryParse(txbY.EditValue?.ToString(), out py);
            DrawStamp();
        }

        private void colorFont_EditValueChanged(object sender, EventArgs e)
        {
            dateTimeColor = colorFont.Color;
            DrawStamp();
        }

        private void txbWid_EditValueChanged(object sender, EventArgs e)
        {
            int newWid = 0;
            int.TryParse(txbWid.EditValue?.ToString(), out newWid);

            txbHgt.EditValue = (int)((newWid * imgHgt) / (imgWid * 1.0));
        }

        private void cbbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txbFont.Enabled = false;
            colorFont.Enabled = false;
            txbWid.Enabled = false;
            txbHgt.Enabled = false;
            txbX.Enabled = false;
            txbY.Enabled = false;

            switch (cbbType.EditValue)
            {
                case "密封":
                    txbFont.Enabled = true;
                    colorFont.Enabled = true;
                    txbWid.Enabled = true;
                    txbHgt.Enabled = true;
                    txbX.Enabled = true;
                    txbY.Enabled = true;

                    DrawStamp();
                    break;
                default:

                    picSign.Image = ImageSign;

                    break;
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            var sign = new dm_Sign()
            {

            };

            dm_SignBUS.Instance.Add(sign);
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void picSign_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG|*.png";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            imgSignPath = ofd.FileName;
            ImageSign = Image.FromFile(imgSignPath);
            picSign.Image = ImageSign;

            imgWid = ImageSign.Width;
            imgHgt = ImageSign.Height;

            lbInfo.Text = $"WxH: {imgWid} x {imgHgt}";
        }
    }
}