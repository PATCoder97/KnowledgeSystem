using BusinessLayer;
using DataAccessLayer;
using DevExpress.Charts.Native;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
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

        Dictionary<int, string> signTypes = TPConfigs.signTypes;
        string imgSignPath = "";
        Image ImageSign = null;

        string letter = DateTime.Today.ToString("yyyy.MM.dd");
        Font font = new Font("Times New Roman", 12, FontStyle.Regular);
        Font fontInfo = new Font("Times New Roman", 12, FontStyle.Regular);
        Color dateTimeColor = Color.FromArgb(210, 14, 18);

        int imgWid, imgHgt, px, py = 0;

        List<dm_Sign> infosMore = new List<dm_Sign>();
        BindingSource moreInfosSource = new BindingSource();
        string displayNameMoreInfo = "";

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void LockLetterInfo()
        {
            txbFont.Enabled = false;
            colorFont.Enabled = false;
            txbWid.Enabled = false;
            txbHgt.Enabled = false;
            txbX.Enabled = false;
            txbY.Enabled = false;

            txbFont1.Enabled = false;
            colorFont1.Enabled = false;
            txbX1.Enabled = false;
            txbY1.Enabled = false;

            btnAddInfo.Enabled = false;
            btnEditInfo.Enabled = false;

            switch (cbbType.SelectedIndex)
            {
                case 1:
                    if (eventInfo == EventFormInfo.View)
                        break;

                    txbFont.Enabled = true;
                    colorFont.Enabled = true;
                    txbWid.Enabled = true;
                    txbHgt.Enabled = true;
                    txbX.Enabled = true;
                    txbY.Enabled = true;

                    txbFont1.Enabled = true;
                    colorFont1.Enabled = true;
                    txbX1.Enabled = true;
                    txbY1.Enabled = true;

                    btnAddInfo.Enabled = true;
                    btnEditInfo.Enabled = true;

                    DrawStamp();
                    break;
                default:
                    txbWid.EditValue = 0;
                    txbHgt.EditValue = 0;
                    txbX.EditValue = 0;
                    txbY.EditValue = 0;
                    txbX1.EditValue = 0;
                    txbY1.EditValue = 0;

                    picSign.Image = ImageSign;
                    break;
            }
        }

        private void LockControl()
        {
            txbDisplayName.Enabled = false;
            txbPrioritize.Enabled = false;
            cbbType.Enabled = false;
            txbFont.Enabled = false;
            colorFont.Enabled = false;
            txbWid.Enabled = false;
            txbHgt.Enabled = false;
            txbX.Enabled = false;
            txbY.Enabled = false;

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    txbDisplayName.Enabled = true;
                    cbbType.Enabled = true;
                    txbPrioritize.Enabled = true;
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    txbDisplayName.Enabled = true;
                    cbbType.Enabled = true;
                    txbPrioritize.Enabled = true;

                    LockLetterInfo();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    LockLetterInfo();
                    break;
                default:
                    break;
            }
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

            var imageOut = MergeTwoImages(img, bit);

            foreach (var item in infosMore)
            {
                string textTest = item.DisplayName;
                var fontInfoName = item.FontName;
                var fontInfoSize = (byte)item.FontSize;
                var fontInfoStyle = FontStyle.Regular;
                switch (item.FontType)
                {
                    case "Bold":
                        fontInfoStyle = FontStyle.Bold;
                        break;
                    case "Italic":
                        fontInfoStyle = FontStyle.Italic;
                        break;
                    default:
                        fontInfoStyle = FontStyle.Regular;
                        break;
                }
                var fontInfo = new Font(fontInfoName, fontInfoSize, fontInfoStyle);

                size = g.MeasureString(textTest.ToString(), fontInfo);
                bit = new Bitmap(img.Width, (int)Math.Ceiling(size.Height));

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.InterpolationMode = InterpolationMode.High;

                sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;

                // Top/Left
                sf.Alignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Near;

                rect = new Rectangle(item.X ?? 0, item.Y ?? 0, bit.Width, bit.Height);
                g.DrawString(textTest, fontInfo, new SolidBrush(ColorTranslator.FromHtml(item.FontColor)), rect, sf);

                imageOut = MergeTwoImages(img, bit);
            }

            picSign.Image = imageOut;
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
            LockControl();
            cbbType.Properties.Items.AddRange(signTypes.Values);

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    signInfo = new dm_Sign();
                    colorFont.Color = dateTimeColor;
                    cbbType.SelectedIndex = 0;
                    break;
                case EventFormInfo.View:
                    string source = Path.Combine(TPConfigs.FolderSign, signInfo.ImgName);
                    ImageSign = File.Exists(source) ? new Bitmap(source) : TPSvgimages.NoImage;

                    imgWid = ImageSign.Width;
                    imgHgt = ImageSign.Height;

                    txbDisplayName.EditValue = signInfo.DisplayName;
                    txbPrioritize.EditValue = signInfo.Prioritize;

                    var fontName = signInfo.FontName;
                    var fontSize = (byte)signInfo.FontSize;

                    var fontStyle = FontStyle.Regular;
                    switch (signInfo.FontType)
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
                    font = new Font(fontName, fontSize, fontStyle);
                    dateTimeColor = ColorTranslator.FromHtml(signInfo.FontColor);
                    colorFont.Color = dateTimeColor;

                    txbX.EditValue = signInfo.X;
                    txbY.EditValue = signInfo.Y;

                    txbWid.EditValue = signInfo.WidImg;
                    txbHgt.EditValue = signInfo.HgtImg;

                    // Cho xuống cuối cùng để xác định kiểu chữ ký
                    int indexType = signInfo.ImgType;
                    cbbType.SelectedIndex = indexType;

                    if (!string.IsNullOrEmpty(signInfo.MoreInfo))
                    {
                        infosMore = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dm_Sign>>(signInfo.MoreInfo);
                    }
                    break;
            }

            gvInfoMore.ReadOnlyGridView();
            gvInfoMore.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            moreInfosSource.DataSource = infosMore;
            gcInfoMore.DataSource = moreInfosSource;

            txbFont.EditValue = $"{font.Name}, {font.Size}, {font.Style}";
            lbInfo.Text = $"寬度×高度：{imgWid}×{imgHgt}";

            switch (cbbType.SelectedIndex)
            {
                case 0:
                    infosMore.Clear();
                    gvInfoMore.RefreshData();
                    break;
                case 1:
                    DrawStamp();
                    break;
            }
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
            switch (cbbType.SelectedIndex)
            {
                case 0:
                    infosMore.Clear();
                    gvInfoMore.RefreshData();
                    break;
                case 1:
                    DrawStamp();
                    break;
            }

            LockLetterInfo();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnAddInfo_Click(object sender, EventArgs e)
        {
            dm_Sign signMoreInfo = new dm_Sign()
            {
                FontName = fontInfo.Name,
                FontSize = fontInfo.Size,
                FontType = fontInfo.Style.ToString(),
                FontColor = $"#{colorFont1.Color.R.ToString("X2")}{colorFont1.Color.G.ToString("X2")}{colorFont1.Color.B.ToString("X2")}",
                X = Convert.ToInt16(txbX1.Text.Trim()),
                Y = Convert.ToInt16(txbY1.Text.Trim()),
                DisplayName = Guid.NewGuid().ToString("N").Substring(10)
            };

            infosMore.Add(signMoreInfo);
            gvInfoMore.RefreshData();

            DrawStamp();
        }

        private void txbFont1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = fontInfo;
            fontDialog.ShowDialog();

            fontInfo = fontDialog.Font;
            txbFont1.EditValue = $"{fontInfo.Name}, {fontInfo.Size}, {fontInfo.Style}";
        }

        private void gvInfoMore_DoubleClick(object sender, EventArgs e)
        {
            var signMoreInfo = gvInfoMore.GetRow(gvInfoMore.FocusedRowHandle) as dm_Sign;

            txbFont1.EditValue = $"{signMoreInfo.FontName}, {signMoreInfo.FontSize}, {signMoreInfo.FontType}";
            colorFont1.Color = ColorTranslator.FromHtml(signMoreInfo.FontColor);
            txbX1.EditValue = signMoreInfo.X;
            txbY1.EditValue = signMoreInfo.Y;
            displayNameMoreInfo = signMoreInfo.DisplayName;

            string[] fontData = txbFont1.Text.Split(',');
            var fontInfoName = fontData[0].Trim();
            var fontInfoSize = Convert.ToByte(Convert.ToDouble(fontData[1].Trim()));
            var fontInfoStyle = FontStyle.Regular;
            switch (fontData[2].Trim())
            {
                case "Bold":
                    fontInfoStyle = FontStyle.Bold;
                    break;
                case "Italic":
                    fontInfoStyle = FontStyle.Italic;
                    break;
                default:
                    fontInfoStyle = FontStyle.Regular;
                    break;
            }
            fontInfo = new Font(fontInfoName, fontInfoSize, fontInfoStyle);
        }

        private void btnEditInfo_Click(object sender, EventArgs e)
        {
            var signEdit = infosMore.First(r => r.DisplayName == displayNameMoreInfo);
            signEdit.FontName = fontInfo.Name;
            signEdit.FontSize = fontInfo.Size;
            signEdit.FontType = fontInfo.Style.ToString();
            signEdit.FontColor = $"#{colorFont1.Color.R.ToString("X2")}{colorFont1.Color.G.ToString("X2")}{colorFont1.Color.B.ToString("X2")}";
            signEdit.X = Convert.ToInt16(txbX1.Text.Trim());
            signEdit.Y = Convert.ToInt16(txbY1.Text.Trim());

            gvInfoMore.RefreshData();

            DrawStamp();
        }

        private void ibtnDel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            GridView view = gvInfoMore;
            dm_Sign steps = view.GetRow(view.FocusedRowHandle) as dm_Sign;

            infosMore.Remove(steps);

            int rowIndex = view.FocusedRowHandle;
            view.RefreshData();
            view.FocusedRowHandle = rowIndex;

            DrawStamp();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = false;
            string msg = "";

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                signInfo.DisplayName = txbDisplayName.EditValue?.ToString();
                signInfo.ImgType = (byte)cbbType.SelectedIndex;
                signInfo.WidImg = Convert.ToInt16(txbWid.EditValue?.ToString().Trim());
                signInfo.HgtImg = Convert.ToInt16(txbHgt.EditValue?.ToString().Trim());
                signInfo.X = Convert.ToInt16(txbX.EditValue?.ToString().Trim());
                signInfo.Y = Convert.ToInt16(txbY.EditValue?.ToString().Trim());
                signInfo.FontName = font.Name;
                signInfo.FontSize = font.Size;
                signInfo.FontType = font.Style.ToString();
                signInfo.FontColor = $"#{dateTimeColor.R.ToString("X2")}{dateTimeColor.G.ToString("X2")}{dateTimeColor.B.ToString("X2")}";
                signInfo.Prioritize = Convert.ToInt16(txbPrioritize.EditValue?.ToString().Trim());

                string moreInfoJson = Newtonsoft.Json.JsonConvert.SerializeObject(infosMore);
                signInfo.MoreInfo = moreInfoJson;

                if (!string.IsNullOrEmpty(imgSignPath))
                {
                    signInfo.ImgName = EncryptionHelper.EncryptionFileName(Path.GetFileName(imgSignPath));
                    if (!Directory.Exists(TPConfigs.FolderSign)) Directory.CreateDirectory(TPConfigs.FolderSign);
                    File.Copy(imgSignPath, Path.Combine(TPConfigs.FolderSign, signInfo.ImgName));
                }

                msg = $"{signInfo.Id} {signInfo.DisplayName}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        result = dm_SignBUS.Instance.Add(signInfo);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        result = dm_SignBUS.Instance.AddOrUpdate(signInfo);
                        break;
                    case EventFormInfo.Delete:
                        result = dm_SignBUS.Instance.Remove(signInfo.Id);
                        break;
                }
            }

            if (result)
            {
                //switch (_eventInfo)
                //{
                //    case EventFormInfo.Update:
                //        logger.Info(_eventInfo.ToString(), msg);
                //        break;
                //    case EventFormInfo.Delete:
                //        logger.Warning(_eventInfo.ToString(), msg);
                //        break;
                //}
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
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