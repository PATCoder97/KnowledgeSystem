using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal sealed class AssetPhotoManagerForm : XtraForm
    {
        private readonly dt313_FixedAsset asset;
        private List<dt313_FixedAssetPhoto> photos;

        private TextEdit txtCloseUp;
        private TextEdit txtOverview;
        private TextEdit txtInUse;

        public AssetPhotoManagerForm(dt313_FixedAsset asset, List<dt313_FixedAssetPhoto> photos)
        {
            this.asset = asset;
            this.photos = photos ?? new List<dt313_FixedAssetPhoto>();
            InitializeUi();
            RefreshDisplay();
        }

        private void InitializeUi()
        {
            Text = $"照片管理 - {asset.AssetCode}";
            StartPosition = FormStartPosition.CenterParent;
            Size = new System.Drawing.Size(760, 280);
            MinimizeBox = false;
            MaximizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 3,
                Padding = new Padding(16)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));

            txtCloseUp = CreateReadonlyText();
            txtOverview = CreateReadonlyText();
            txtInUse = CreateReadonlyText();

            AddPhotoRow(panel, 0, "近拍照片", txtCloseUp, "CloseUp");
            AddPhotoRow(panel, 1, "宏觀照片", txtOverview, "Overview");
            AddPhotoRow(panel, 2, "使用中照片", txtInUse, "InUse");

            var btnClose = new SimpleButton { Text = "關閉", Dock = DockStyle.Bottom, Height = 36 };
            btnClose.Click += (s, e) => Close();

            Controls.Add(panel);
            Controls.Add(btnClose);
        }

        private TextEdit CreateReadonlyText()
        {
            return new TextEdit
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Properties = { Appearance = { Font = TPConfigs.fontUI14 } }
            };
        }

        private void AddPhotoRow(TableLayoutPanel panel, int rowIndex, string title, TextEdit editor, string photoType)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var lbl = new LabelControl
            {
                Text = title,
                Appearance = { Font = TPConfigs.fontUI14 },
                Padding = new Padding(0, 8, 0, 0)
            };

            var btnUpload = new SimpleButton { Text = "上傳", Width = 80 };
            var btnView = new SimpleButton { Text = "查看", Width = 80 };
            var btnDisable = new SimpleButton { Text = "停用", Width = 80 };

            btnUpload.Click += (s, e) => UploadPhoto(photoType);
            btnView.Click += (s, e) => ViewPhoto(photoType);
            btnDisable.Click += (s, e) => DisablePhoto(photoType);

            editor.Margin = new Padding(0, 0, 8, 12);
            btnUpload.Margin = new Padding(0, 0, 8, 12);
            btnView.Margin = new Padding(0, 0, 8, 12);
            btnDisable.Margin = new Padding(0, 0, 0, 12);

            panel.Controls.Add(lbl, 0, rowIndex);
            panel.Controls.Add(editor, 1, rowIndex);
            panel.Controls.Add(btnUpload, 2, rowIndex);
            panel.Controls.Add(btnView, 3, rowIndex);
            panel.Controls.Add(btnDisable, 4, rowIndex);
        }

        private void UploadPhoto(string photoType)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                if (dialog.ShowDialog() != DialogResult.OK) return;

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

            photos = dt313_FixedAssetPhotoBUS.Instance.GetListByAssetId(asset.Id);
            RefreshDisplay();
        }

        private void ViewPhoto(string photoType)
        {
            var photo = photos.Find(r => r.PhotoType == photoType && r.IsActive);
            if (photo == null)
            {
                XtraMessageBox.Show("尚未上傳照片。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new f00_VIewFile(FixedAsset313Helper.CopyToTemp(FixedAsset313Helper.GetFixedAssetPhotoPath(photo), photo.ActualName), true, false))
            {
                form.ShowDialog();
            }
        }

        private void DisablePhoto(string photoType)
        {
            var photo = photos.Find(r => r.PhotoType == photoType && r.IsActive);
            if (photo == null) return;
            dt313_FixedAssetPhotoBUS.Instance.DeactivateById(photo.Id);
            photos = dt313_FixedAssetPhotoBUS.Instance.GetListByAssetId(asset.Id);
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            txtCloseUp.Text = photos.Find(r => r.PhotoType == "CloseUp" && r.IsActive)?.ActualName ?? "";
            txtOverview.Text = photos.Find(r => r.PhotoType == "Overview" && r.IsActive)?.ActualName ?? "";
            txtInUse.Text = photos.Find(r => r.PhotoType == "InUse" && r.IsActive)?.ActualName ?? "";
        }
    }
}
