using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    public partial class uc313_FixedAssetMain
    {
        private void BtnAssetReload_Click(object sender, EventArgs e) => ReloadAllData();
        private void BtnBatchReload_Click(object sender, EventArgs e) => ReloadAllData();
        private void BtnAbnormalReload_Click(object sender, EventArgs e) => ReloadAllData();
        private void BtnSettingReload_Click(object sender, EventArgs e) => ReloadAllData();

        private void BtnAssetAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AssetEditForm(GetDepartmentLookupItems(), GetUserLookupItems(), null))
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                int id = dt313_FixedAssetBUS.Instance.Add(form.Asset);
                if (id <= 0)
                {
                    MsgTP.MsgErrorDB();
                    return;
                }
            }

            ReloadAllData();
        }

        private void BtnAssetEdit_Click(object sender, EventArgs e)
        {
            EditFocusedAsset();
        }

        private void BtnAssetDelete_Click(object sender, EventArgs e)
        {
            var row = GetFocusedAsset();
            if (row == null) return;

            if (XtraMessageBox.Show($"確定要刪除資產 {row.AssetCode} ?", TPConfigs.SoftNameTW,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            dt313_FixedAssetBUS.Instance.RemoveById(row.Entity.Id, TPConfigs.LoginUser.Id);
            ReloadAllData();
        }

        private void BtnAssetImport_Click(object sender, EventArgs e)
        {
            ImportAssetsFromExcel();
        }

        private void BtnAssetExport_Click(object sender, EventArgs e)
        {
            ExportGrid(gcAssets, "FixedAssetList");
        }

        private void BtnAssetPhotos_Click(object sender, EventArgs e)
        {
            OpenPhotoManager();
        }

        private void GvAssets_DoubleClick(object sender, EventArgs e)
        {
            EditFocusedAsset();
        }

        private void BtnCreateMonthlyBatch_Click(object sender, EventArgs e)
        {
            CreateMonthlyBatch();
        }

        private void BtnCreateQuarterlyBatch_Click(object sender, EventArgs e)
        {
            CreateQuarterlyBatch();
        }

        private void BtnEditBatchResult_Click(object sender, EventArgs e)
        {
            EditFocusedBatchDetail();
        }

        private void BtnCloseBatch_Click(object sender, EventArgs e)
        {
            CloseFocusedBatch();
        }

        private void BtnBatchExport_Click(object sender, EventArgs e)
        {
            ExportGrid(gcBatchDetails, "FixedAssetBatchDetail");
        }

        private void GvBatches_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            BindBatchDetailGrid(GetFocusedBatch());
        }

        private void GvBatches_DoubleClick(object sender, EventArgs e)
        {
            if (batchDetailSource.Count > 0)
            {
                gvBatchDetails.FocusedRowHandle = 0;
            }
        }

        private void GvBatchDetails_DoubleClick(object sender, EventArgs e)
        {
            EditFocusedBatchDetail();
        }

        private void BtnAbnormalHandle_Click(object sender, EventArgs e)
        {
            EditFocusedAbnormal();
        }

        private void BtnAbnormalExport_Click(object sender, EventArgs e)
        {
            ExportGrid(gcAbnormals, "FixedAssetAbnormalList");
        }

        private void GvAbnormals_DoubleClick(object sender, EventArgs e)
        {
            EditFocusedAbnormal();
        }

        private void BtnDeptSettingAdd_Click(object sender, EventArgs e)
        {
            EditDepartmentSetting(null);
        }

        private void BtnDeptSettingEdit_Click(object sender, EventArgs e)
        {
            EditDepartmentSetting(GetFocusedDeptSetting()?.Entity);
        }

        private void BtnCatalogAdd_Click(object sender, EventArgs e)
        {
            EditAbnormalCatalog(null);
        }

        private void BtnCatalogEdit_Click(object sender, EventArgs e)
        {
            EditAbnormalCatalog(GetFocusedAbnormalCatalog()?.Entity);
        }

        private void GvDeptSettings_DoubleClick(object sender, EventArgs e)
        {
            EditDepartmentSetting(GetFocusedDeptSetting()?.Entity);
        }

        private void GvAbnormalCatalogs_DoubleClick(object sender, EventArgs e)
        {
            EditAbnormalCatalog(GetFocusedAbnormalCatalog()?.Entity);
        }

        private void EditFocusedAsset()
        {
            var row = GetFocusedAsset();
            if (row == null) return;
            if (!CanEditAsset(row.Entity))
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new AssetEditForm(GetDepartmentLookupItems(), GetUserLookupItems(), CloneAsset(row.Entity)))
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                if (!dt313_FixedAssetBUS.Instance.AddOrUpdate(form.Asset))
                {
                    MsgTP.MsgErrorDB();
                    return;
                }
            }

            ReloadAllData();
        }

        private void OpenPhotoManager()
        {
            var row = GetFocusedAsset();
            if (row == null) return;
            if (!CanEditAsset(row.Entity))
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new AssetPhotoManagerForm(row.Entity, fixedAssetPhotos.Where(r => r.FixedAssetId == row.Entity.Id).ToList()))
            {
                form.ShowDialog();
            }

            ReloadAllData();
        }

        private void CreateMonthlyBatch()
        {
            using (var form = BatchCreateForm.CreateMonthly(GetUserLookupItems(true)))
            {
                if (form.ShowDialog() != DialogResult.OK) return;

                string periodKey = form.Result.TargetId == null ? string.Empty : form.Result.PeriodKey;
                string userId = form.Result.TargetId;
                var targetAssets = fixedAssets
                    .Where(r => !r.IsDeleted && string.Equals(r.IdManager, userId, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(r => r.AssetCode)
                    .ToList();

                if (targetAssets.Count == 0)
                {
                    XtraMessageBox.Show("該經辦名下沒有可建立月檢批次的資產。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var context = new DBDocumentManagementSystemEntities())
                using (var tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        bool duplicated = context.dt313_InspectionBatch.Any(r => r.BatchType == BatchTypeMonthly
                            && r.PeriodKey == periodKey
                            && r.AssignedUserId == userId);
                        if (duplicated)
                        {
                            throw new Exception("同一期間與經辦的月檢批次已存在。");
                        }

                        var user = users.FirstOrDefault(r => r.Id == userId);
                        string deptId = NormalizeDeptId(user?.IdDepartment);
                        var batch = new dt313_InspectionBatch
                        {
                            BatchName = $"【每月自檢】{periodKey}-{form.Result.TargetDisplay}",
                            BatchType = BatchTypeMonthly,
                            PeriodKey = periodKey,
                            IdDept = deptId,
                            AssignedUserId = userId,
                            TargetQty = targetAssets.Count,
                            Status = "Open",
                            CreatedBy = TPConfigs.LoginUser.Id,
                            CreatedDate = DateTime.Now
                        };

                        context.dt313_InspectionBatch.Add(batch);
                        context.SaveChanges();

                        context.dt313_InspectionBatchAsset.AddRange(targetAssets.Select(asset => new dt313_InspectionBatchAsset
                        {
                            BatchId = batch.Id,
                            FixedAssetId = asset.Id,
                            Result = ResultPending,
                            CreatedDate = DateTime.Now
                        }));
                        context.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            ReloadAllData();
        }

        private void CreateQuarterlyBatch()
        {
            using (var form = BatchCreateForm.CreateQuarterly(GetDepartmentLookupItems(true), departmentSettings))
            {
                if (form.ShowDialog() != DialogResult.OK) return;

                string periodKey = form.Result.PeriodKey;
                string deptId = form.Result.TargetId;
                int sampleRate = Math.Max(1, form.Result.SampleRate);

                var targetAssets = fixedAssets
                    .Where(r => !r.IsDeleted && !string.IsNullOrWhiteSpace(r.IdDept) && r.IdDept.StartsWith(deptId, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(r => r.LastQuarterlyAuditDate ?? DateTime.MinValue)
                    .ThenBy(r => r.AssetCode)
                    .ToList();

                if (targetAssets.Count == 0)
                {
                    XtraMessageBox.Show("該部門沒有可建立季檢批次的資產。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int sampleCount = Math.Max(1, (int)Math.Ceiling(targetAssets.Count * sampleRate / 100D));
                var selectedAssets = targetAssets.Take(sampleCount).ToList();

                using (var context = new DBDocumentManagementSystemEntities())
                using (var tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        bool duplicated = context.dt313_InspectionBatch.Any(r => r.BatchType == BatchTypeQuarterly
                            && r.PeriodKey == periodKey
                            && r.IdDept == deptId);
                        if (duplicated)
                        {
                            throw new Exception("同一季度與部門的稽核批次已存在。");
                        }

                        var batch = new dt313_InspectionBatch
                        {
                            BatchName = $"【季度稽核】{periodKey}-{form.Result.TargetDisplay}",
                            BatchType = BatchTypeQuarterly,
                            PeriodKey = periodKey,
                            IdDept = deptId,
                            AssignedUserId = null,
                            SampleRate = sampleRate,
                            TargetQty = selectedAssets.Count,
                            Status = "Open",
                            CreatedBy = TPConfigs.LoginUser.Id,
                            CreatedDate = DateTime.Now
                        };

                        context.dt313_InspectionBatch.Add(batch);
                        context.SaveChanges();

                        context.dt313_InspectionBatchAsset.AddRange(selectedAssets.Select(asset => new dt313_InspectionBatchAsset
                        {
                            BatchId = batch.Id,
                            FixedAssetId = asset.Id,
                            Result = ResultPending,
                            CreatedDate = DateTime.Now
                        }));
                        context.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            ReloadAllData();
        }

        private void EditFocusedBatchDetail()
        {
            var row = GetFocusedBatchDetail();
            if (row == null) return;

            bool allowResultEdit = CanEditBatchResult(row);
            bool allowCorrectionEdit = CanUpdateCorrection(row);
            if (!allowResultEdit && !allowCorrectionEdit)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new InspectionResultForm(row, abnormalCatalogs.Where(r => r.IsActive).ToList(),
                inspectionPhotos.Where(r => r.BatchAssetId == row.Entity.Id).ToList(), allowResultEdit, allowCorrectionEdit, isManager313))
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                SaveInspectionDetail(row.Entity.Id, form.ResultData, allowResultEdit, allowCorrectionEdit);
            }

            ReloadAllData();
        }

        private void EditFocusedAbnormal()
        {
            var row = GetFocusedAbnormal();
            if (row == null) return;

            var detailRow = new BatchDetailGridRow
            {
                Entity = row.Entity,
                Batch = row.Batch,
                Asset = row.Asset
            };

            bool allowResultEdit = row.Batch.Status != "Closed" && CanEditBatchResult(detailRow);
            bool allowCorrectionEdit = CanUpdateCorrection(row);

            if (!allowResultEdit && !allowCorrectionEdit)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new InspectionResultForm(detailRow, abnormalCatalogs.Where(r => r.IsActive).ToList(),
                inspectionPhotos.Where(r => r.BatchAssetId == row.Entity.Id).ToList(), allowResultEdit, allowCorrectionEdit, isManager313))
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                SaveInspectionDetail(row.Entity.Id, form.ResultData, allowResultEdit, allowCorrectionEdit);
            }

            ReloadAllData();
        }

        private void SaveInspectionDetail(int batchAssetId, InspectionResultDialogResult dialogResult, bool allowResultEdit, bool allowCorrectionEdit)
        {
            using (var context = new DBDocumentManagementSystemEntities())
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    var item = context.dt313_InspectionBatchAsset.First(r => r.Id == batchAssetId);
                    var batch = context.dt313_InspectionBatch.First(r => r.Id == item.BatchId);
                    var asset = context.dt313_FixedAsset.First(r => r.Id == item.FixedAssetId);
                    var photoList = context.dt313_InspectionPhoto.Where(r => r.BatchAssetId == batchAssetId).ToList();

                    if (allowResultEdit)
                    {
                        if (dialogResult.Result == ResultPending)
                        {
                            item.Result = ResultPending;
                            item.AbnormalId = null;
                            item.AbnormalNote = null;
                            item.CheckedBy = null;
                            item.CheckedDate = null;
                            item.CorrectionDueDate = null;
                            item.CorrectionStatus = null;
                            item.CorrectionNote = null;
                            item.ClosedBy = null;
                            item.ClosedDate = null;
                            context.dt313_InspectionPhoto.RemoveRange(photoList);
                        }
                        else if (dialogResult.Result == ResultNormal)
                        {
                            DateTime checkDate = item.CheckedDate ?? DateTime.Now;
                            item.Result = ResultNormal;
                            item.AbnormalId = null;
                            item.AbnormalNote = null;
                            item.CheckedBy = item.CheckedBy ?? TPConfigs.LoginUser.Id;
                            item.CheckedDate = checkDate;
                            item.CorrectionDueDate = null;
                            item.CorrectionStatus = null;
                            item.CorrectionNote = null;
                            item.ClosedBy = null;
                            item.ClosedDate = null;
                            context.dt313_InspectionPhoto.RemoveRange(photoList);
                            UpdateAssetLastCheck(asset, batch, checkDate);
                        }
                        else
                        {
                            DateTime checkDate = item.CheckedDate ?? DateTime.Now;
                            item.Result = ResultAbnormal;
                            item.AbnormalId = dialogResult.AbnormalId;
                            item.AbnormalNote = dialogResult.AbnormalNote?.Trim();
                            item.CheckedBy = item.CheckedBy ?? TPConfigs.LoginUser.Id;
                            item.CheckedDate = checkDate;
                            item.CorrectionDueDate = item.CorrectionDueDate ?? checkDate.Date.AddDays(5);
                            UpdateAssetLastCheck(asset, batch, checkDate);
                        }
                    }

                    foreach (int deleteId in dialogResult.DeletedPhotoIds.Distinct())
                    {
                        var photoDel = photoList.FirstOrDefault(r => r.Id == deleteId);
                        if (photoDel != null)
                        {
                            context.dt313_InspectionPhoto.Remove(photoDel);
                        }
                    }

                    context.SaveChanges();

                    if (allowResultEdit && dialogResult.Result == ResultAbnormal)
                    {
                        int abnormalDisplayOrder = context.dt313_InspectionPhoto
                            .Where(r => r.BatchAssetId == batchAssetId && r.PhotoPurpose == InspectionPhotoPurposeAbnormal)
                            .Select(r => (int?)r.DisplayOrder).Max() ?? 0;
                        foreach (string file in dialogResult.NewAbnormalPhotoFiles)
                        {
                            var saved = FixedAsset313Helper.SaveInspectionPhoto(batchAssetId, file);
                            abnormalDisplayOrder++;
                            context.dt313_InspectionPhoto.Add(new dt313_InspectionPhoto
                            {
                                BatchAssetId = batchAssetId,
                                PhotoPurpose = InspectionPhotoPurposeAbnormal,
                                EncryptionName = saved.encryptionName,
                                ActualName = saved.actualName,
                                UploadedBy = TPConfigs.LoginUser.Id,
                                UploadedDate = DateTime.Now,
                                DisplayOrder = abnormalDisplayOrder
                            });
                        }
                    }

                    if (allowCorrectionEdit && item.Result == ResultAbnormal)
                    {
                        int correctionDisplayOrder = context.dt313_InspectionPhoto
                            .Where(r => r.BatchAssetId == batchAssetId && r.PhotoPurpose == InspectionPhotoPurposeCorrection)
                            .Select(r => (int?)r.DisplayOrder).Max() ?? 0;
                        foreach (string file in dialogResult.NewCorrectionPhotoFiles)
                        {
                            var saved = FixedAsset313Helper.SaveInspectionPhoto(batchAssetId, file);
                            correctionDisplayOrder++;
                            context.dt313_InspectionPhoto.Add(new dt313_InspectionPhoto
                            {
                                BatchAssetId = batchAssetId,
                                PhotoPurpose = InspectionPhotoPurposeCorrection,
                                EncryptionName = saved.encryptionName,
                                ActualName = saved.actualName,
                                UploadedBy = TPConfigs.LoginUser.Id,
                                UploadedDate = DateTime.Now,
                                DisplayOrder = correctionDisplayOrder
                            });
                        }

                        item.CorrectionNote = dialogResult.CorrectionNote?.Trim();
                        DateTime dueDate = (item.CorrectionDueDate ?? DateTime.Today).Date;
                        if (isManager313 && dialogResult.MarkCorrectionClosed)
                        {
                            item.CorrectionStatus = CorrectionClosed;
                            item.ClosedBy = TPConfigs.LoginUser.Id;
                            item.ClosedDate = DateTime.Now;
                        }
                        else
                        {
                            item.CorrectionStatus = DateTime.Today > dueDate ? CorrectionOverdue : CorrectionOpen;
                            item.ClosedBy = null;
                            item.ClosedDate = null;
                        }
                    }

                    if (allowResultEdit && item.Result == ResultAbnormal)
                    {
                        int abnormalPhotoCount = context.dt313_InspectionPhoto.Count(r => r.BatchAssetId == batchAssetId && r.PhotoPurpose == InspectionPhotoPurposeAbnormal);
                        if (abnormalPhotoCount == 0)
                        {
                            throw new Exception("異常結果至少需要一張異常照片。");
                        }
                    }

                    context.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void CloseFocusedBatch()
        {
            var row = GetFocusedBatch();
            if (row == null) return;

            using (var context = new DBDocumentManagementSystemEntities())
            {
                var batch = context.dt313_InspectionBatch.First(r => r.Id == row.Entity.Id);
                if (batch.Status == "Closed")
                {
                    XtraMessageBox.Show("此批次已結案。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool hasPending = context.dt313_InspectionBatchAsset.Any(r => r.BatchId == batch.Id && r.Result == ResultPending);
                if (hasPending)
                {
                    XtraMessageBox.Show("仍有待處理項目，無法結案。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                batch.Status = "Closed";
                batch.ClosedBy = TPConfigs.LoginUser.Id;
                batch.ClosedDate = DateTime.Now;
                context.SaveChanges();
            }

            ReloadAllData();
        }

        private void EditDepartmentSetting(dt313_DepartmentSetting entity)
        {
            if (!isManager313)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new DepartmentSettingForm(GetDepartmentLookupItems(true), entity == null ? null : CloneDepartmentSetting(entity)))
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                if (!dt313_DepartmentSettingBUS.Instance.AddOrUpdate(form.Setting))
                {
                    MsgTP.MsgErrorDB();
                    return;
                }
            }

            ReloadAllData();
        }

        private void EditAbnormalCatalog(dt313_AbnormalCatalog entity)
        {
            if (!isManager313)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new AbnormalCatalogForm(entity == null ? null : CloneAbnormalCatalog(entity)))
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                if (!dt313_AbnormalCatalogBUS.Instance.AddOrUpdate(form.Catalog))
                {
                    MsgTP.MsgErrorDB();
                    return;
                }
            }

            ReloadAllData();
        }
    }
}
