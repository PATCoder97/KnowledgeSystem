using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.IO;
using System.Windows.Forms;

namespace KnowledgeSystem.Helpers
{
    public static class Material309Helper
    {
        private const string MaterialPhotoFolder = "MaterialPhotos";

        public static string EnsureBaseFolder()
        {
            if (!Directory.Exists(TPConfigs.Folder309))
            {
                Directory.CreateDirectory(TPConfigs.Folder309);
            }

            return TPConfigs.Folder309;
        }

        public static string EnsureMaterialPhotoFolder(int materialId)
        {
            string folder = Path.Combine(EnsureBaseFolder(), MaterialPhotoFolder, materialId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static (string encryptionName, string actualName) SaveMaterialPhoto(int materialId, string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = EncryptionHelper.EncryptionFileName(sourceFilePath);
            string folder = EnsureMaterialPhotoFolder(materialId);
            File.Copy(sourceFilePath, Path.Combine(folder, encryptionName), true);
            return (encryptionName, actualName);
        }

        public static string GetMaterialPhotoPath(dt309_MaterialPhoto photo)
        {
            return Path.Combine(EnsureMaterialPhotoFolder(photo.MaterialId), photo.EncryptionName);
        }

        public static string CopyToTemp(string sourcePath, string actualName)
        {
            if (!Directory.Exists(TPConfigs.TempFolderData))
            {
                Directory.CreateDirectory(TPConfigs.TempFolderData);
            }

            string tempFile = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyyyMMddHHmmss}-{actualName}");
            File.Copy(sourcePath, tempFile, true);
            return tempFile;
        }

        public static void OpenPhotoFile(string physicalPath, string actualName)
        {
            if (!File.Exists(physicalPath))
            {
                XtraMessageBox.Show("找不到對應的照片檔案。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tempFile = CopyToTemp(physicalPath, actualName);
            using (var form = new f00_VIewFile(tempFile, true, false))
            {
                form.ShowDialog();
            }
        }
    }
}
