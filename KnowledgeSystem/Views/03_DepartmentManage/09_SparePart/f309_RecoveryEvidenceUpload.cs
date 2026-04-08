using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class f309_RecoveryEvidenceUpload : XtraForm
    {
        private readonly List<LocalFileRow> selectedFiles = new List<LocalFileRow>();
        private readonly string ticketNo;

        public f309_RecoveryEvidenceUpload(string ticketNo, DateTime? actualDisposeDate = null, string resultNote = "")
        {
            this.ticketNo = ticketNo ?? string.Empty;

            InitializeComponent();
            InitializeIcon();
            InitializeForm(actualDisposeDate, resultNote);
        }

        public DateTime ActualDisposeDate => deActualDisposeDate.DateTime;

        public string ResultNote => memoResultNote.Text.Trim();

        public List<string> SelectedFiles => selectedFiles.Select(r => r.FullPath).ToList();

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Cancel;

            btnAddFile.ImageOptions.SvgImage = TPSvgimages.Add;
            btnViewFile.ImageOptions.SvgImage = TPSvgimages.View;
            btnRemoveFile.ImageOptions.SvgImage = TPSvgimages.Remove;
        }

        private void InitializeForm(DateTime? actualDisposeDate, string resultNote)
        {
            Text = string.IsNullOrWhiteSpace(ticketNo)
                ? "\u4e0a\u50b3\u5831\u5ee2\u8b49\u660e"
                : $"\u4e0a\u50b3\u5831\u5ee2\u8b49\u660e - {ticketNo}";

            deActualDisposeDate.EditValue = actualDisposeDate;
            memoResultNote.EditValue = resultNote ?? string.Empty;
            RefreshFileList();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = TPConfigs.FilterFile;
                dialog.Multiselect = true;
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                foreach (string file in dialog.FileNames)
                {
                    if (selectedFiles.Any(r => string.Equals(r.FullPath, file, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    selectedFiles.Add(new LocalFileRow(file));
                }
            }

            RefreshFileList();
        }

        private void btnViewFile_Click(object sender, EventArgs e)
        {
            if (lbFiles.SelectedItem is LocalFileRow row)
            {
                Material309RecoveryHelper.OpenLocalFiles(new[] { row.FullPath });
            }
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            if (lbFiles.SelectedItem is LocalFileRow row)
            {
                selectedFiles.Remove(row);
                RefreshFileList();
            }
        }

        private void btnConfirm_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (deActualDisposeDate.EditValue == null)
            {
                MsgTP.MsgError("\u8acb\u586b\u5beb\u5be6\u969b\u5831\u5ee2\u6642\u9593\u3002");
                return;
            }

            if (selectedFiles.Count == 0)
            {
                MsgTP.MsgError("\u8acb\u81f3\u5c11\u9078\u64c7\u4e00\u4efd\u8b49\u660e\u6587\u4ef6\u3002");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void RefreshFileList()
        {
            lbFiles.DataSource = null;
            lbFiles.DataSource = selectedFiles.ToList();
        }

        private class LocalFileRow
        {
            public LocalFileRow(string fullPath)
            {
                FullPath = fullPath;
            }

            public string FullPath { get; }

            public override string ToString()
            {
                return Path.GetFileName(FullPath);
            }
        }
    }
}
