using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal sealed class InspectionResultForm : XtraForm
    {
        private readonly BatchDetailGridRow row;
        private readonly List<dt313_AbnormalCatalog> abnormalCatalogs;
        private readonly List<dt313_InspectionPhoto> existingPhotos;
        private readonly bool allowResultEdit;
        private readonly bool allowCorrectionEdit;
        private readonly bool allowCloseCorrection;

        private ComboBoxEdit cbbResult;
        private ComboBoxEdit cbbAbnormal;
        private MemoEdit memoAbnormal;
        private MemoEdit memoCorrection;
        private CheckEdit chkCloseCorrection;
        private ListBox lbAbnormalPhotos;
        private ListBox lbCorrectionPhotos;
        private List<PhotoSelectionRow> abnormalPhotoRows = new List<PhotoSelectionRow>();
        private List<PhotoSelectionRow> correctionPhotoRows = new List<PhotoSelectionRow>();
        private HashSet<int> deletedPhotoIds = new HashSet<int>();

        public InspectionResultDialogResult ResultData { get; private set; }

        public InspectionResultForm(BatchDetailGridRow row, List<dt313_AbnormalCatalog> abnormalCatalogs, List<dt313_InspectionPhoto> existingPhotos,
            bool allowResultEdit, bool allowCorrectionEdit, bool allowCloseCorrection)
        {
            this.row = row;
            this.abnormalCatalogs = abnormalCatalogs ?? new List<dt313_AbnormalCatalog>();
            this.existingPhotos = existingPhotos ?? new List<dt313_InspectionPhoto>();
            this.allowResultEdit = allowResultEdit;
            this.allowCorrectionEdit = allowCorrectionEdit;
            this.allowCloseCorrection = allowCloseCorrection;
            InitializeUi();
            BindData();
        }

        private void InitializeUi()
        {
            Text = $"檢查結果 - {row.Asset?.AssetCode}";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(940, 720);
            MinimizeBox = false;
            MaximizeBox = false;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblInfo = new LabelControl
            {
                Text = $"批次: {row.Batch.BatchName}\r\n資產: {row.Asset.AssetCode} {row.Asset.AssetNameTW}\r\n部門: {row.Asset.IdDept}    經辦: {row.Asset.IdManager}",
                Appearance = { Font = TPConfigs.fontUI14 },
                AutoSizeMode = LabelAutoSizeMode.Vertical,
                Dock = DockStyle.Fill
            };

            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            cbbResult = new ComboBoxEdit { Dock = DockStyle.Fill };
            cbbResult.Properties.Appearance.Font = TPConfigs.fontUI14;
            cbbResult.Properties.AppearanceDropDown.Font = TPConfigs.fontUI14;
            cbbResult.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cbbResult.Properties.Items.AddRange(new object[] { "Pending", "Normal", "Abnormal" });

            cbbAbnormal = new ComboBoxEdit { Dock = DockStyle.Fill };
            cbbAbnormal.Properties.Appearance.Font = TPConfigs.fontUI14;
            cbbAbnormal.Properties.AppearanceDropDown.Font = TPConfigs.fontUI14;
            cbbAbnormal.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cbbAbnormal.Properties.Items.AddRange(abnormalCatalogs.Select(r => new LookupItem(r.Id.ToString(), $"{r.Code} - {r.DisplayName}")).ToArray());

            chkCloseCorrection = new CheckEdit
            {
                Text = "管理端關閉改善",
                Properties = { Appearance = { Font = TPConfigs.fontUI14 } }
            };

            AddHeaderRow(header, 0, "檢查結果", cbbResult, "異常項目", cbbAbnormal);
            AddHeaderRow(header, 1, "改善關閉", chkCloseCorrection, "", new Panel());

            memoAbnormal = new MemoEdit { Dock = DockStyle.Fill, Properties = { Appearance = { Font = TPConfigs.fontUI14 } } };
            memoCorrection = new MemoEdit { Dock = DockStyle.Fill, Properties = { Appearance = { Font = TPConfigs.fontUI14 } } };

            var abnormalGroup = CreatePhotoGroup("異常照片", out lbAbnormalPhotos,
                (s, e) => AddPhotos(abnormalPhotoRows, lbAbnormalPhotos),
                (s, e) => ViewSelectedPhoto(lbAbnormalPhotos),
                (s, e) => RemoveSelectedPhoto(abnormalPhotoRows, lbAbnormalPhotos));
            var correctionGroup = CreatePhotoGroup("改善照片", out lbCorrectionPhotos,
                (s, e) => AddPhotos(correctionPhotoRows, lbCorrectionPhotos),
                (s, e) => ViewSelectedPhoto(lbCorrectionPhotos),
                (s, e) => RemoveSelectedPhoto(correctionPhotoRows, lbCorrectionPhotos));

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 450 };
            split.Panel1.Controls.Add(abnormalGroup);
            split.Panel2.Controls.Add(correctionGroup);

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                FlowDirection = FlowDirection.RightToLeft
            };
            var btnCancel = new SimpleButton { Text = "取消", DialogResult = DialogResult.Cancel, Width = 90 };
            var btnSave = new SimpleButton { Text = "儲存", Width = 90 };
            btnSave.Click += BtnSave_Click;
            bottom.Controls.Add(btnCancel);
            bottom.Controls.Add(btnSave);

            root.Controls.Add(lblInfo, 0, 0);
            root.Controls.Add(header, 0, 1);
            root.Controls.Add(CreateMemoGroup("異常說明", memoAbnormal), 0, 2);
            root.Controls.Add(split, 0, 3);
            root.Controls.Add(CreateMemoGroup("改善說明", memoCorrection), 0, 4);

            Controls.Add(root);
            Controls.Add(bottom);
            cbbResult.SelectedIndexChanged += (s, e) => UpdateState();
        }

        private void AddHeaderRow(TableLayoutPanel panel, int rowIndex, string leftLabel, Control leftControl, string rightLabel, Control rightControl)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var lblLeft = new LabelControl { Text = leftLabel, Appearance = { Font = TPConfigs.fontUI14 }, Padding = new Padding(0, 8, 0, 0) };
            var lblRight = new LabelControl { Text = rightLabel, Appearance = { Font = TPConfigs.fontUI14 }, Padding = new Padding(0, 8, 0, 0) };
            leftControl.Margin = new Padding(0, 0, 12, 8);
            rightControl.Margin = new Padding(0, 0, 0, 8);
            panel.Controls.Add(lblLeft, 0, rowIndex);
            panel.Controls.Add(leftControl, 1, rowIndex);
            panel.Controls.Add(lblRight, 2, rowIndex);
            panel.Controls.Add(rightControl, 3, rowIndex);
        }

        private GroupControl CreateMemoGroup(string caption, MemoEdit memo)
        {
            var group = new GroupControl { Text = caption, Dock = DockStyle.Fill, Height = 120 };
            memo.Height = 90;
            group.Controls.Add(memo);
            return group;
        }

        private GroupControl CreatePhotoGroup(string caption, out ListBox listBox, EventHandler addEvent, EventHandler viewEvent, EventHandler removeEvent)
        {
            var group = new GroupControl { Text = caption, Dock = DockStyle.Fill };
            listBox = new ListBox { Dock = DockStyle.Fill, Font = new Font("Microsoft JhengHei UI", 10.5F) };

            var right = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 110, FlowDirection = FlowDirection.TopDown };
            var btnAdd = new SimpleButton { Text = "新增", Width = 96 };
            var btnView = new SimpleButton { Text = "查看", Width = 96 };
            var btnRemove = new SimpleButton { Text = "移除", Width = 96 };
            btnAdd.Click += addEvent;
            btnView.Click += viewEvent;
            btnRemove.Click += removeEvent;
            right.Controls.Add(btnAdd);
            right.Controls.Add(btnView);
            right.Controls.Add(btnRemove);

            group.Controls.Add(listBox);
            group.Controls.Add(right);
            return group;
        }

        private void BindData()
        {
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

            memoAbnormal.Text = row.Entity.AbnormalNote;
            memoCorrection.Text = row.Entity.CorrectionNote;
            chkCloseCorrection.Checked = row.Entity.CorrectionStatus == "Closed";

            abnormalPhotoRows = existingPhotos.Where(r => r.PhotoPurpose == "Abnormal")
                .Select(r => new PhotoSelectionRow
                {
                    IsExisting = true,
                    ExistingId = r.Id,
                    PhysicalPath = FixedAsset313Helper.GetInspectionPhotoPath(r),
                    ActualName = r.ActualName
                }).ToList();

            correctionPhotoRows = existingPhotos.Where(r => r.PhotoPurpose == "Correction")
                .Select(r => new PhotoSelectionRow
                {
                    IsExisting = true,
                    ExistingId = r.Id,
                    PhysicalPath = FixedAsset313Helper.GetInspectionPhotoPath(r),
                    ActualName = r.ActualName
                }).ToList();

            RefreshPhotoLists();
            UpdateState();
        }

        private void UpdateState()
        {
            bool isAbnormal = string.Equals(cbbResult.Text, "Abnormal", StringComparison.OrdinalIgnoreCase);
            cbbResult.Enabled = allowResultEdit;
            cbbAbnormal.Enabled = allowResultEdit && isAbnormal;
            memoAbnormal.Enabled = allowResultEdit && isAbnormal;
            memoCorrection.Enabled = allowCorrectionEdit && isAbnormal;
            chkCloseCorrection.Enabled = allowCloseCorrection && allowCorrectionEdit && isAbnormal;
            chkCloseCorrection.Visible = allowCloseCorrection;
        }

        private void RefreshPhotoLists()
        {
            lbAbnormalPhotos.DataSource = null;
            lbAbnormalPhotos.DataSource = abnormalPhotoRows.ToList();
            lbCorrectionPhotos.DataSource = null;
            lbCorrectionPhotos.DataSource = correctionPhotoRows.ToList();
        }

        private void AddPhotos(List<PhotoSelectionRow> target, ListBox listBox)
        {
            bool abnormalPurpose = ReferenceEquals(target, abnormalPhotoRows);
            if ((abnormalPurpose && !allowResultEdit && !allowCorrectionEdit) || (!abnormalPurpose && !allowCorrectionEdit))
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                dialog.Multiselect = true;
                if (dialog.ShowDialog() != DialogResult.OK) return;
                foreach (string file in dialog.FileNames.Where(System.IO.File.Exists))
                {
                    target.Add(new PhotoSelectionRow { IsExisting = false, PhysicalPath = file, ActualName = System.IO.Path.GetFileName(file) });
                }
            }

            RefreshPhotoLists();
            listBox.SelectedIndex = listBox.Items.Count - 1;
        }

        private void ViewSelectedPhoto(ListBox listBox)
        {
            if (!(listBox.SelectedItem is PhotoSelectionRow rowPhoto)) return;
            using (var form = new f00_VIewFile(rowPhoto.IsExisting
                ? FixedAsset313Helper.CopyToTemp(rowPhoto.PhysicalPath, rowPhoto.ActualName)
                : rowPhoto.PhysicalPath, true, false))
            {
                form.ShowDialog();
            }
        }

        private void RemoveSelectedPhoto(List<PhotoSelectionRow> target, ListBox listBox)
        {
            if (!(listBox.SelectedItem is PhotoSelectionRow rowPhoto)) return;
            bool abnormalPurpose = ReferenceEquals(target, abnormalPhotoRows);
            if ((abnormalPurpose && !allowResultEdit && !allowCorrectionEdit) || (!abnormalPurpose && !allowCorrectionEdit))
            {
                MsgTP.MsgNoPermission();
                return;
            }

            if (rowPhoto.IsExisting)
            {
                deletedPhotoIds.Add(rowPhoto.ExistingId);
            }

            target.Remove(rowPhoto);
            RefreshPhotoLists();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string result = cbbResult.Text;
            if (allowResultEdit && string.IsNullOrWhiteSpace(result))
            {
                XtraMessageBox.Show("請選擇檢查結果。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isAbnormal = string.Equals(result, "Abnormal", StringComparison.OrdinalIgnoreCase);
            int? abnormalId = null;
            if (isAbnormal)
            {
                if (!(cbbAbnormal.SelectedItem is LookupItem abnormalLookup) || string.IsNullOrWhiteSpace(abnormalLookup.Value))
                {
                    XtraMessageBox.Show("異常結果必須選擇異常項目。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                abnormalId = Convert.ToInt32(abnormalLookup.Value);
                if (abnormalPhotoRows.Count == 0)
                {
                    XtraMessageBox.Show("異常結果至少要保留一張異常照片。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            ResultData = new InspectionResultDialogResult
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

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
