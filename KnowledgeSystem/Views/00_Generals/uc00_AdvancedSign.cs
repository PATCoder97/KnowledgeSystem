using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Commands;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class uc00_AdvancedSign : DevExpress.XtraEditors.XtraUserControl
    {
        public uc00_AdvancedSign()
        {
            InitializeComponent();
        }

        public SignInfo signInfo = SignInfo.Sign;
        List<dm_Sign> signs;

        public dm_Sign SignSelect { get; set; }
        public Image ImageSign { get; set; }
        public string DescripSign { get; set; }

        #region methods

        private void ShowSign()
        {
            int indexSign = Convert.ToInt16(cbbSign.EditValue?.ToString() ?? "0");
            SignSelect = signs.FirstOrDefault(r => r.Id == indexSign);

            string signPath = Path.Combine(TPConfigs.FolderSign, SignSelect.ImgName);
            Image imageSign = new Bitmap(signPath);

            string letter = txbDate.EditValue == null ? string.Empty : txbDate.DateTime.ToString("yyyy.MM.dd");

            switch (signInfo)
            {
                case SignInfo.Stamp:
                    letter = string.IsNullOrEmpty(letter) ? DateTime.Today.ToString("yyyy.MM.dd") : letter;

                    DrawStamp(letter, imageSign);
                    break;
                default:
                    DrawSign(letter, imageSign);
                    break;
            }
        }

        private void DrawSign(string letter, Image image)
        {
            if (string.IsNullOrWhiteSpace(letter))
            {
                DescripSign = letter;
                ImageSign = image;
                picSign.Image = image;
                return;
            }

            var img = (Bitmap)image;
            var bit = new Bitmap(img.Width, 20);
            Graphics g = Graphics.FromImage(bit);

            float width = ((float)bit.Width);
            float height = ((float)bit.Height);

            float emSize = height;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.InterpolationMode = InterpolationMode.High;

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Near;

            // Top/Left.
            sf.Alignment = StringAlignment.Far;

            Font font = new Font(FontFamily.GenericSansSerif, emSize, FontStyle.Regular);
            Rectangle rect = new Rectangle(5, 5, bit.Width - 10, bit.Height);

            var fontsize = SizeLabelFont(letter, bit.Width, 20);

            font = new Font("Times New Roman", 12, FontStyle.Bold);

            SizeF size = g.MeasureString(letter.ToString(), font);
            g.DrawString(letter, font, new SolidBrush(Color.Black), rect, sf);

            DescripSign = letter;
            ImageSign = image;
            picSign.Image = MergeTwoImages(img, bit);
        }

        private void DrawStamp(string letter, Image image)
        {
            var img = (Bitmap)image;
            Graphics g = Graphics.FromImage(img);

            var fontName = SignSelect.FontName;
            var fontSize = (byte)SignSelect.FontSize;

            var fontStyle = FontStyle.Regular;
            switch (SignSelect.FontType)
            {
                case "Bold":
                    fontStyle = FontStyle.Bold;
                    break;
                case "Italic":
                    fontStyle = FontStyle.Italic;
                    break;
                default:
                    fontStyle = FontStyle.Regular;
                    break;
            }
            Font font = new Font(fontName, fontSize, fontStyle);
            SizeF size = g.MeasureString(letter.ToString(), font);

            Color desColor = ColorTranslator.FromHtml(SignSelect.FontColor);

            var bit = new Bitmap(img.Width, (int)Math.Ceiling(size.Height));

            float width = ((float)bit.Width);
            float height = ((float)bit.Height);

            float emSize = height;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.InterpolationMode = InterpolationMode.High;

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;

            // Top/Left.
            sf.Alignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Near;

            Rectangle rect = new Rectangle(SignSelect.X ?? 0, SignSelect.X ?? 0, bit.Width, bit.Height);
            g.DrawString(letter, font, new SolidBrush(desColor), rect, sf);

            var imageOut = MergeTwoImages(img, bit);
            ImageSign = imageOut;
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

        #endregion

        private void cbbSign_EditValueChanged(object sender, EventArgs e)
        {
            ShowSign();
        }

        private void txbDate_EditValueChanged(object sender, EventArgs e)
        {
            ShowSign();
        }

        private void uc00_AdvancedSign_Load(object sender, EventArgs e)
        {
            signs = dm_SignBUS.Instance.GetList();
            switch (signInfo)
            {
                case SignInfo.Sign:
                    signs = signs.Where(r => r.ImgType == 0).ToList();
                    break;
                case SignInfo.Stamp:
                    signs = signs.Where(r => r.ImgType == 1).ToList();
                    break;
                default:
                    break;
            }

            cbbSign.Properties.DataSource = signs;
            cbbSign.Properties.DisplayMember = "DisplayName";
            cbbSign.Properties.ValueMember = "Id";

            if (signs.Count != 0)
                cbbSign.EditValue = signs.FirstOrDefault().Id;
        }
    }
}
