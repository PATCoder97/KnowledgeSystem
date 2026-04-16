using DataAccessLayer;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public static class Material309RecoveryHelper
    {
        private const string RecoveryGuideFolder = "RecoveryGuides";
        private const string RecoveryEvidenceFolder = "RecoveryEvidence";

        public static string EnsureBaseFolder()
        {
            if (!Directory.Exists(TPConfigs.Folder309))
            {
                Directory.CreateDirectory(TPConfigs.Folder309);
            }

            return TPConfigs.Folder309;
        }

        public static string EnsureGuideFolder()
        {
            string folder = Path.Combine(EnsureBaseFolder(), RecoveryGuideFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static string EnsureEvidenceFolder(int ticketId)
        {
            string folder = Path.Combine(EnsureBaseFolder(), RecoveryEvidenceFolder, ticketId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static (string encryptionName, string actualName, string extension) SaveGuideFile(string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = EncryptionHelper.EncryptionFileName(sourceFilePath);
            string extension = Path.GetExtension(sourceFilePath) ?? string.Empty;
            File.Copy(sourceFilePath, Path.Combine(EnsureGuideFolder(), encryptionName), true);
            return (encryptionName, actualName, extension);
        }

        public static (string encryptionName, string actualName, string extension) SaveEvidenceFile(int ticketId, string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = EncryptionHelper.EncryptionFileName(sourceFilePath);
            string extension = Path.GetExtension(sourceFilePath) ?? string.Empty;
            File.Copy(sourceFilePath, Path.Combine(EnsureEvidenceFolder(ticketId), encryptionName), true);
            return (encryptionName, actualName, extension);
        }

        public static string GetGuidePath(dt309_RecoveryGuides guide)
        {
            return Path.Combine(EnsureGuideFolder(), guide.EncryptionName);
        }

        public static string GetEvidencePath(dt309_RecoveryEvidence evidence)
        {
            return Path.Combine(EnsureEvidenceFolder(evidence.RecoveryTicketId), evidence.EncryptionName);
        }

        public static void OpenGuideFiles(IEnumerable<dt309_RecoveryGuides> guides)
        {
            if (guides == null)
            {
                return;
            }

            OpenStoredFiles(guides.Select(guide => (guide.ActualName, GetGuidePath(guide))));
        }

        public static void OpenEvidenceFiles(IEnumerable<dt309_RecoveryEvidence> evidences)
        {
            if (evidences == null)
            {
                return;
            }

            OpenStoredFiles(evidences.Select(evidence => (evidence.ActualName, GetEvidencePath(evidence))));
        }

        public static void OpenLocalFiles(IEnumerable<string> files)
        {
            if (files == null)
            {
                return;
            }

            OpenStoredFiles(files
                .Where(File.Exists)
                .Select(file => (Path.GetFileName(file), file)));
        }

        private static void OpenStoredFiles(IEnumerable<(string actualName, string physicalPath)> files)
        {
            var validFiles = files
                .Where(item => !string.IsNullOrWhiteSpace(item.actualName) && !string.IsNullOrWhiteSpace(item.physicalPath))
                .Where(item => File.Exists(item.physicalPath))
                .ToList();

            if (validFiles.Count == 0)
            {
                MessageBox.Show("找不到對應檔案。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var mainForm = f00_ViewMultiFile.Instance;
            if (!mainForm.Visible)
            {
                mainForm.Show();
            }

            foreach (var item in validFiles)
            {
                string safeName = Regex.Replace(item.actualName, @"[\\/:*?""<>|]", "");
                string tempFile = Path.Combine(
                    TPConfigs.TempFolderData,
                    $"{safeName}_{DateTime.Now:yyyyMMddHHmmssfff}{Path.GetExtension(item.actualName)}");

                if (!Directory.Exists(TPConfigs.TempFolderData))
                {
                    Directory.CreateDirectory(TPConfigs.TempFolderData);
                }

                File.Copy(item.physicalPath, tempFile, true);
                mainForm.OpenFormInDocumentManager(tempFile);
            }
        }
    }
}
