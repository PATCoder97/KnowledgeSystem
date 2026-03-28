using DataAccessLayer;
using System;
using System.IO;

namespace KnowledgeSystem.Helpers
{
    public static class FixedAsset313Helper
    {
        private const string AssetPhotoFolder = "AssetPhotos";
        private const string InspectionPhotoFolder = "InspectionPhotos";

        public static string EnsureBaseFolder()
        {
            if (!Directory.Exists(TPConfigs.Folder313))
            {
                Directory.CreateDirectory(TPConfigs.Folder313);
            }

            return TPConfigs.Folder313;
        }

        public static string EnsureAssetPhotoFolder(int fixedAssetId)
        {
            string folder = Path.Combine(EnsureBaseFolder(), AssetPhotoFolder, fixedAssetId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static string EnsureInspectionPhotoFolder(int batchAssetId)
        {
            string folder = Path.Combine(EnsureBaseFolder(), InspectionPhotoFolder, batchAssetId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static (string encryptionName, string actualName) SaveFixedAssetPhoto(int fixedAssetId, string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = EncryptionHelper.EncryptionFileName(sourceFilePath);
            string folder = EnsureAssetPhotoFolder(fixedAssetId);
            File.Copy(sourceFilePath, Path.Combine(folder, encryptionName), true);
            return (encryptionName, actualName);
        }

        public static (string encryptionName, string actualName) SaveInspectionPhoto(int batchAssetId, string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = EncryptionHelper.EncryptionFileName(sourceFilePath);
            string folder = EnsureInspectionPhotoFolder(batchAssetId);
            File.Copy(sourceFilePath, Path.Combine(folder, encryptionName), true);
            return (encryptionName, actualName);
        }

        public static string GetFixedAssetPhotoPath(dt313_FixedAssetPhoto photo)
        {
            return Path.Combine(EnsureAssetPhotoFolder(photo.FixedAssetId), photo.EncryptionName);
        }

        public static string GetInspectionPhotoPath(dt313_InspectionPhoto photo)
        {
            return Path.Combine(EnsureInspectionPhotoFolder(photo.BatchAssetId), photo.EncryptionName);
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
    }
}
