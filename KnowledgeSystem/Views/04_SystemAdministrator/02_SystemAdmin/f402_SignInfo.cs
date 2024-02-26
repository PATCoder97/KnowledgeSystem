using BusinessLayer;
using DataAccessLayer;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
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

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class f402_SignInfo : DevExpress.XtraEditors.XtraForm
    {
        public f402_SignInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string formName = string.Empty;
        public EventFormInfo eventInfo = EventFormInfo.Create;
        public dm_Sign signInfo = null;

        List<string> types = new List<string> { "簽名", "密封" };
        string imgSignPath = "";
        Image ImageSign = null;

        string letter = DateTime.Today.ToString("yyyy.MM.dd");
        Font font = new Font("Times New Roman", 12, FontStyle.Regular);
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

        private void f402_SignInfo_Load(object sender, EventArgs e)
        {
            cbbType.Properties.Items.AddRange(types);

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    signInfo = new dm_Sign();
                    cbbType.SelectedIndex = 0;
                    colorFont.Color = dateTimeColor;
                    break;
                case EventFormInfo.View:

                    string source = Path.Combine(TPConfigs.FolderSign, signInfo.ImgName);
                    string dest = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyMMddhhmmss} Sign.png");
                    if (!Directory.Exists(TPConfigs.TempFolderData))
                        Directory.CreateDirectory(TPConfigs.TempFolderData);

                    File.Copy(source, dest, true);

                    ImageSign = Image.FromFile(dest);
                    imgWid = ImageSign.Width;
                    imgHgt = ImageSign.Height;

                    txbDisplayName.EditValue = signInfo.DisplayName;

                    var fontName = signInfo.FontName;
                    var fontSize = (byte)signInfo.FontSize;
                    var fontStyle = signInfo.FontType;
                    font = new Font(fontName, fontSize, FontStyle.Regular);
                    dateTimeColor = ColorTranslator.FromHtml(signInfo.FontColor);
                    colorFont.Color = dateTimeColor;

                    txbX.EditValue = signInfo.X;
                    txbY.EditValue = signInfo.Y;

                    txbWid.EditValue = signInfo.WidImg;
                    txbHgt.EditValue = signInfo.HgtImg;

                    // Cho xuống cuối cùng để xác định kiểu chữ ký
                    int indexType = signInfo.ImgType;
                    cbbType.SelectedIndex = indexType;
                    break;
            }

            txbFont.EditValue = $"{font.Name}, {font.Size}, {font.Style}";
            lbInfo.Text = $"WxH: {imgWid} x {imgHgt}";
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
            var displayName = txbDisplayName.EditValue?.ToString();
            int idType = cbbType.SelectedIndex;
            var imgName = EncryptionHelper.EncryptionFileName(Path.GetFileName(imgSignPath));
            var widImg = Convert.ToInt16(txbWid.EditValue?.ToString().Trim());
            var hgtImg = Convert.ToInt16(txbHgt.EditValue?.ToString().Trim());
            var pointX = Convert.ToInt16(txbX.EditValue?.ToString().Trim());
            var pointY = Convert.ToInt16(txbY.EditValue?.ToString().Trim());
            var fontName = font.Name;
            var fontSize = font.Size;
            var fontType = font.Style.ToString();
            var fontColor = "#" + dateTimeColor.R.ToString("X2") + dateTimeColor.G.ToString("X2") + dateTimeColor.B.ToString("X2");

            if (!Directory.Exists(TPConfigs.FolderSign)) Directory.CreateDirectory(TPConfigs.FolderSign);

            File.Copy(imgSignPath, Path.Combine(TPConfigs.FolderSign, imgName));

            signInfo = new dm_Sign()
            {
                ImgName = imgName,
                DisplayName = displayName,
                ImgType = (byte)idType,
                WidImg = widImg,
                HgtImg = hgtImg,
                X = pointX,
                Y = pointY,
                FontName = fontName,
                FontSize = fontSize,
                FontType = fontType,
                FontColor = fontColor
            };

            dm_SignBUS.Instance.Add(signInfo);

            MessageBox.Show("OK");
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