using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal partial class f313_InspectionResult_Info : XtraForm
    {
        private readonly FixedAsset313Context module;
        private readonly BatchDetailGridRow row;
        private readonly bool allowResultEdit;
        private readonly bool allowCorrectionEdit;
        private readonly bool allowCloseCorrection;
        private readonly List<dt313_InspectionPhoto> existingPhotos;
        private readonly List<dt313_AbnormalCatalog> abnormalCatalogs;

        private readonly List<PhotoSelectionRow> abnormalPhotoRows = new List<PhotoSelectionRow>();
        private readonly List<PhotoSelectionRow> correctionPhotoRows = new List<PhotoSelectionRow>();
        private readonly HashSet<int> deletedPhotoIds = new HashSet<int>();

        public f313_InspectionResult_Info(FixedAsset313Context module, BatchDetailGridRow row, bool allowResultEdit, bool allowCorrectionEdit)
        {
            InitializeComponent();
            this.module = module;
            this.row = row;
            this.allowResultEdit = allowResultEdit;
            this.allowCorrectionEdit = allowCorrectionEdit;
            allowCloseCorrection = module.IsManager313;
            existingPhotos = module.GetInspectionPhotos(row.Entity.Id);
            abnormalCatalogs = module.GetActiveAbnormalCatalogs();
        }

        private void f313_InspectionResult_Info_Load(object sender, EventArgs e)
        {
            Text = $"檢查結果 - {row.Asset?.AssetCode}";
            lblInfo.Text = $"批次: {row.Batch?.BatchName}\r\n資產: {row.Asset?.AssetCode} {row.Asset?.AssetNameTW}\r\n部門: {row.Asset?.IdDept}    經辦: {row.Asset?.IdManager}";

            cbbResult.Properties.Items.AddRange(new object[]
            {
                FixedAsset313Const.ResultPending,
                FixedAsset313Const.ResultNormal,
                FixedAsset313Const.ResultAbnormal
            });
            cbbResult.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

            cbbAbnormal.Properties.Items.AddRange(abnormalCatalogs.Select(r => new LookupItem(r.Id.ToString(), $"{r.Code} - {r.DisplayName}")).ToArray());
            cbbAbnormal.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

            cbbResult.EditValue = row.Entity.Result;
            if (row.Entity.AbnormalId.HasValue)
            {
                foreach (var item in cbbAbnormal.Properties.Items)
                {
                    if (item is LookupItem lookup && lookup.Value == row.Entity.AbnormalId.Value.ToString())
                    {
                        cbbAbnormal.SelectedItem = item;
                        break;
                    }
                }
            }

            memoAbnormal.EditValue = row.Entity.AbnormalNote;
            memoCorrection.EditValue = row.Entity.CorrectionNote;
            chkCloseCorrection.Checked = row.Entity.CorrectionStatus == FixedAsset313Const.CorrectionClosed;

            abnormalPhotoRows.AddRange(existingPhotos.Where(r => r.PhotoPurpose == FixedAsset313Const.InspectionPhotoPurposeAbnormal)
                .Select(r => new PhotoSelectionRow
                {
                    IsExisting = true,
                    ExistingId = r.Id,
                    PhysicalPath = FixedAsset313Helper.GetInspectionPhotoPath(r),
                    ActualName = r.ActualName
                }));

            correctionPhotoRows.AddRange(existingPhotos.Where(r => r.PhotoPurpose == FixedAsset313Const.InspectionPhotoPurposeCorrection)
                .Select(r => new PhotoSelectionRow
                {
                    IsExisting = true,
                    ExistingId = r.Id,
                    PhysicalPath = FixedAsset313Helper.GetInspectionPhotoPath(r),
                    ActualName = r.ActualName
                }));

            RefreshPhotoLists();
            UpdateState();
            cbbResult.SelectedIndexChanged += cbbResult_SelectedIndexChanged;
        }

        private void cbbResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            bool isAbnormal = string.Equals(cbbResult.Text, FixedAsset313Const.ResultAbnormal, StringComparison.OrdinalIgnoreCase);
            cbbResult.Enabled = allowResultEdit;
            cbbAbnormal.Enabled = allowResultEdit && isAbnormal;
            memoAbnormal.Enabled = allowResultEdit && isAbnormal;
            memoCorrection.Enabled = allowCorrectionEdit && isAbnormal;
            chkCloseCorrection.Enabled = allowCloseCorrection && allowCorrectionEdit && isAbnormal;
            chkCloseCorrection.Visible = allowCloseCorrection;
            btnAddAbnormal.Enabled = allowResultEdit && isAbnormal;
            btnRemoveAbnormal.Enabled = allowResultEdit && isAbnormal;
            btnAddCorrection.Enabled = allowCorrectionEdit && isAbnormal;
            btnRemoveCorrection.Enabled = allowCorrectionEdit && isAbnormal;
        }

        private void RefreshPhotoLists()
        {
            lbAbnormal.DataSource = null;
            lbAbnormal.DataSource = abnormalPhotoRows.ToList();
            lbCorrection.DataSource = null;
            lbCorrection.DataSource = correctionPhotoRows.ToList();
        }

        private void AddPhotos(List<PhotoSelectionRow> target)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                dialog.Multiselect = true;
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                foreach (string file in dialog.FileNames)
                {
                    target.Add(new PhotoSelectionRow
                    {
                        IsExisting = false,
                        PhysicalPath = file,
                        ActualName = System.IO.Path.GetFileName(file)
                    });
                }
            }

            RefreshPhotoLists();
        }

        private void ViewPhoto(ListBox listBox)
        {
            if (!(listBox.SelectedItem is PhotoSelectionRow item))
            {
                return;
            }

            FixedAsset313Context.OpenPhotoFile(item.PhysicalPath, item.ActualName);
        }

        private void RemovePhoto(List<PhotoSelectionRow> target, ListBox listBox)
        {
            if (!(listBox.SelectedItem is PhotoSelectionRow item))
            {
                return;
            }

            if (item.IsExisting)
            {
                deletedPhotoIds.Add(item.ExistingId);
            }

            target.Remove(item);
            RefreshPhotoLists();
        }

        private void btnAddAbnormal_Click(object sender, EventArgs e) => AddPhotos(abnormalPhotoRows);
        private void btnViewAbnormal_Click(object sender, EventArgs e) => ViewPhoto(lbAbnormal);
        private void btnRemoveAbnormal_Click(object sender, EventArgs e) => RemovePhoto(abnormalPhotoRows, lbAbnormal);
        private void btnAddCorrection_Click(object sender, EventArgs e) => AddPhotos(correctionPhotoRows);
        private void btnViewCorrection_Click(object sender, EventArgs e) => ViewPhoto(lbCorrection);
        private void btnRemoveCorrection_Click(object sender, EventArgs e) => RemovePhoto(correctionPhotoRows, lbCorrection);

        private void btnSave_Click(object sender, EventArgs e)
        {
            string result = cbbResult.Text;
            bool isAbnormal = string.Equals(result, FixedAsset313Const.ResultAbnormal, StringComparison.OrdinalIgnoreCase);
            int? abnormalId = null;

            if (allowResultEdit && string.IsNullOrWhiteSpace(result))
            {
                MsgTP.MsgError("請選擇檢查結果");
                return;
            }

            if (isAbnormal)
            {
                if (!(cbbAbnormal.SelectedItem is LookupItem abnormalLookup) || string.IsNullOrWhiteSpace(abnormalLookup.Value))
                {
                    MsgTP.MsgError("異常結果必須選擇異常項目");
                    return;
                }

                abnormalId = Convert.ToInt32(abnormalLookup.Value);
                if (abnormalPhotoRows.Count == 0)
                {
                    MsgTP.MsgError("異常結果至少要保留一張異常照片");
                    return;
                }
            }

            var dialogResult = new InspectionResultDialogResult
            {
                Result = result,
                AbnormalId = abnormalId,
                AbnormalNote = memoAbnormal.Text.Trim(),
                CorrectionNote = memoCorrection.Text.Trim(),
                MarkCorrectionClosed = chkCloseCorrection.Checked,
                DeletedPhotoIds = deletedPhotoIds.ToList(),
                NewAbnormalPhotoFiles = abnormalPhotoRows.Where(r => !r.IsExisting).Select(r => r.PhysicalPath).ToList(),
                NewCorrectionPhotoFiles = correctionPhotoRows.Where(r => !r.IsExisting).Select(r => r.PhysicalPath).ToList()
            };

            if (!module.SaveInspectionDetail(row.Entity.Id, dialogResult, allowResultEdit, allowCorrectionEdit))
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
