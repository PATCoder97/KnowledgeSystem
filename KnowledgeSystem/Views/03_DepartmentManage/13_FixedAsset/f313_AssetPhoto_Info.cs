using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal partial class f313_AssetPhoto_Info : XtraForm
    {
        private readonly FixedAsset313Context module;
        private readonly dt313_FixedAsset asset;
        private List<dt313_FixedAssetPhoto> photos = new List<dt313_FixedAssetPhoto>();
        private bool hasChanged;
        private string currentPhotoType = FixedAsset313Const.PhotoTypeCloseUp;

        public f313_AssetPhoto_Info(FixedAsset313Context module, dt313_FixedAsset asset)
        {
            InitializeComponent();
            this.module = module;
            this.asset = asset;
        }

        private void f313_AssetPhoto_Info_Load(object sender, EventArgs e)
        {
            AdjustFormToWorkingArea();
            Text = $"照片管理 - {asset.AssetCode}";
            DevExpress.Utils.AppearanceObject.DefaultMenuFont =
                new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ReloadPhotos();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearPreviewImages();
            base.OnFormClosed(e);
        }

        private void AdjustFormToWorkingArea()
        {
            Rectangle workingArea = Screen.FromControl(this).WorkingArea;
            int targetWidth = Math.Min(Width, workingArea.Width - 24);
            int targetHeight = Math.Min(Height, workingArea.Height - 24);

            if (targetWidth <= 0 || targetHeight <= 0)
            {
                return;
            }

            Size = new Size(targetWidth, targetHeight);
            Location = new Point(
                workingArea.Left + Math.Max((workingArea.Width - Width) / 2, 0),
                workingArea.Top + Math.Max((workingArea.Height - Height) / 2, 0));
        }

        private void ReloadPhotos()
        {
            photos = dt313_FixedAssetPhotoBUS.Instance.GetListByAssetId(asset.Id);
            BindPhoto(FixedAsset313Const.PhotoTypeCloseUp, picCloseUp);
            BindPhoto(FixedAsset313Const.PhotoTypeOverview, picOverview);
            BindPhoto(FixedAsset313Const.PhotoTypeInUse, picInUse);
        }

        private void BindPhoto(string photoType, PictureBox pictureBox)
        {
            var photo = GetActivePhoto(photoType);
            string physicalPath = photo == null ? string.Empty : FixedAsset313Helper.GetFixedAssetPhotoPath(photo);
            Image preview = photo == null ? CreatePlaceholderImage() : LoadPreviewImage(physicalPath);
            ReplacePreviewImage(pictureBox, preview);
        }

        private dt313_FixedAssetPhoto GetActivePhoto(string photoType)
        {
            return photos.Find(r => r.PhotoType == photoType && r.IsActive);
        }

        private Image LoadPreviewImage(string physicalPath)
        {
            if (string.IsNullOrWhiteSpace(physicalPath) || !File.Exists(physicalPath))
            {
                return CreatePlaceholderImage();
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(physicalPath);
                using (var ms = new MemoryStream(bytes))
                using (var image = Image.FromStream(ms))
                {
                    return new Bitmap(image);
                }
            }
            catch
            {
                return CreatePlaceholderImage();
            }
        }

        private Image CreatePlaceholderImage()
        {
            if (TPSvgimages.NoImage != null)
            {
                return new Bitmap(TPSvgimages.NoImage);
            }

            Bitmap bitmap = new Bitmap(480, 360);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Brush background = new SolidBrush(Color.WhiteSmoke))
            using (Brush foreground = new SolidBrush(Color.Gray))
            using (Pen borderPen = new Pen(Color.Gainsboro))
            using (Font font = new Font("Microsoft JhengHei UI", 16F))
            {
                graphics.Clear(Color.WhiteSmoke);
                graphics.DrawRectangle(borderPen, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                graphics.DrawString("尚未上傳照片", font, foreground, new RectangleF(0, 0, bitmap.Width, bitmap.Height), format);
            }

            return bitmap;
        }

        private void ReplacePreviewImage(PictureBox pictureBox, Image image)
        {
            if (pictureBox == null)
            {
                return;
            }

            var oldImage = pictureBox.Image;
            pictureBox.Image = image;
            oldImage?.Dispose();
        }

        private void ClearPreviewImages()
        {
            ReplacePreviewImage(picCloseUp, null);
            ReplacePreviewImage(picOverview, null);
            ReplacePreviewImage(picInUse, null);
        }

        private void UploadPhoto(string photoType)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    var saved = FixedAsset313Helper.SaveFixedAssetPhoto(asset.Id, dialog.FileName);
                    int id = dt313_FixedAssetPhotoBUS.Instance.AddOrReplace(new dt313_FixedAssetPhoto
                    {
                        FixedAssetId = asset.Id,
                        PhotoType = photoType,
                        EncryptionName = saved.encryptionName,
                        ActualName = saved.actualName,
                        UploadedBy = TPConfigs.LoginUser.Id,
                        UploadedDate = DateTime.Now,
                        IsActive = true
                    });

                    if (id <= 0)
                    {
                        MsgTP.MsgErrorDB();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            hasChanged = true;
            ReloadPhotos();
        }

        private void ViewPhoto(string photoType)
        {
            var photo = GetActivePhoto(photoType);
            if (photo == null)
            {
                XtraMessageBox.Show("尚未上傳照片。", TPConfigs.SoftNameTW);
                return;
            }

            FixedAsset313Context.OpenPhotoFile(FixedAsset313Helper.GetFixedAssetPhotoPath(photo), photo.ActualName);
        }

        private void DisablePhoto(string photoType)
        {
            var photo = GetActivePhoto(photoType);
            if (photo == null)
            {
                return;
            }

            dt313_FixedAssetPhotoBUS.Instance.DeactivateById(photo.Id);
            hasChanged = true;
            ReloadPhotos();
        }

        private string GetPhotoTypeFromControl(Control control)
        {
            if (control == picCloseUp)
            {
                return FixedAsset313Const.PhotoTypeCloseUp;
            }

            if (control == picOverview)
            {
                return FixedAsset313Const.PhotoTypeOverview;
            }

            return FixedAsset313Const.PhotoTypeInUse;
        }

        private void ShowPhotoMenu(Control control)
        {
            currentPhotoType = GetPhotoTypeFromControl(control);
            bool hasPhoto = GetActivePhoto(currentPhotoType) != null;
            btnViewPhoto.Enabled = hasPhoto;
            btnDisablePhoto.Enabled = hasPhoto;
            popupMenuPhoto.ShowPopup(Control.MousePosition);
        }

        private void picPhoto_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ShowPhotoMenu(sender as Control);
            }
        }

        private void picPhoto_DoubleClick(object sender, EventArgs e)
        {
            ViewPhoto(GetPhotoTypeFromControl(sender as Control));
        }

        private void btnUploadPhoto_ItemClick(object sender, ItemClickEventArgs e)
        {
            UploadPhoto(currentPhotoType);
        }

        private void btnViewPhoto_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewPhoto(currentPhotoType);
        }

        private void btnDisablePhoto_ItemClick(object sender, ItemClickEventArgs e)
        {
            DisablePhoto(currentPhotoType);
        }

        private void btnReload_ItemClick(object sender, ItemClickEventArgs e)
        {
            ReloadPhotos();
        }

        private void btnConfirm_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult = hasChanged ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }
    }
}
