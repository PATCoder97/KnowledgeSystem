using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal partial class f313_AssetPhoto_Info : XtraForm
    {
        private readonly FixedAsset313Context module;
        private readonly dt313_FixedAsset asset;
        private List<dt313_FixedAssetPhoto> photos = new List<dt313_FixedAssetPhoto>();
        private bool hasChanged;

        public f313_AssetPhoto_Info(FixedAsset313Context module, dt313_FixedAsset asset)
        {
            InitializeComponent();
            this.module = module;
            this.asset = asset;
            InitializeIcon();
        }

        private void f313_AssetPhoto_Info_Load(object sender, EventArgs e)
        {
            FixedAsset313UIHelper.ApplyFreeFormStyle(this);
            Text = $"照片管理 - {asset.AssetCode}";
            ReloadPhotos();
        }

        private void ReloadPhotos()
        {
            photos = dt313_FixedAssetPhotoBUS.Instance.GetListByAssetId(asset.Id);
            txtCloseUp.Text = photos.Find(r => r.PhotoType == FixedAsset313Const.PhotoTypeCloseUp && r.IsActive)?.ActualName ?? string.Empty;
            txtOverview.Text = photos.Find(r => r.PhotoType == FixedAsset313Const.PhotoTypeOverview && r.IsActive)?.ActualName ?? string.Empty;
            txtInUse.Text = photos.Find(r => r.PhotoType == FixedAsset313Const.PhotoTypeInUse && r.IsActive)?.ActualName ?? string.Empty;
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

            hasChanged = true;
            ReloadPhotos();
        }

        private void ViewPhoto(string photoType)
        {
            var photo = photos.Find(r => r.PhotoType == photoType && r.IsActive);
            if (photo == null)
            {
                XtraMessageBox.Show("尚未上傳照片。", TPConfigs.SoftNameTW);
                return;
            }

            FixedAsset313Context.OpenPhotoFile(FixedAsset313Helper.GetFixedAssetPhotoPath(photo), photo.ActualName);
        }

        private void DisablePhoto(string photoType)
        {
            var photo = photos.Find(r => r.PhotoType == photoType && r.IsActive);
            if (photo == null)
            {
                return;
            }

            dt313_FixedAssetPhotoBUS.Instance.DeactivateById(photo.Id);
            hasChanged = true;
            ReloadPhotos();
        }

        private void btnUploadCloseUp_Click(object sender, EventArgs e) => UploadPhoto(FixedAsset313Const.PhotoTypeCloseUp);
        private void btnViewCloseUp_Click(object sender, EventArgs e) => ViewPhoto(FixedAsset313Const.PhotoTypeCloseUp);
        private void btnDisableCloseUp_Click(object sender, EventArgs e) => DisablePhoto(FixedAsset313Const.PhotoTypeCloseUp);
        private void btnUploadOverview_Click(object sender, EventArgs e) => UploadPhoto(FixedAsset313Const.PhotoTypeOverview);
        private void btnViewOverview_Click(object sender, EventArgs e) => ViewPhoto(FixedAsset313Const.PhotoTypeOverview);
        private void btnDisableOverview_Click(object sender, EventArgs e) => DisablePhoto(FixedAsset313Const.PhotoTypeOverview);
        private void btnUploadInUse_Click(object sender, EventArgs e) => UploadPhoto(FixedAsset313Const.PhotoTypeInUse);
        private void btnViewInUse_Click(object sender, EventArgs e) => ViewPhoto(FixedAsset313Const.PhotoTypeInUse);
        private void btnDisableInUse_Click(object sender, EventArgs e) => DisablePhoto(FixedAsset313Const.PhotoTypeInUse);

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = hasChanged ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        private void InitializeIcon()
        {
            btnUploadCloseUp.ImageOptions.SvgImage = TPSvgimages.UploadFile;
            btnViewCloseUp.ImageOptions.SvgImage = TPSvgimages.View;
            btnDisableCloseUp.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnUploadOverview.ImageOptions.SvgImage = TPSvgimages.UploadFile;
            btnViewOverview.ImageOptions.SvgImage = TPSvgimages.View;
            btnDisableOverview.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnUploadInUse.ImageOptions.SvgImage = TPSvgimages.UploadFile;
            btnViewInUse.ImageOptions.SvgImage = TPSvgimages.View;
            btnDisableInUse.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnClose.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }
    }
}
