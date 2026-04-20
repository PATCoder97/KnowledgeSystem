using DataAccessLayer;
using System.IO;

namespace KnowledgeSystem.Helpers
{
    public static class Material309CheckPhotoHelper
    {
        private const string InspectionCheckPhotoFolder = "InspectionCheckPhotos";

        public static string GetThread(int batchMaterialId)
        {
            return $"309CHECK_{batchMaterialId}";
        }

        public static string EnsureInspectionPhotoFolder(int batchMaterialId)
        {
            string folder = Path.Combine(Material309Helper.EnsureBaseFolder(), InspectionCheckPhotoFolder, batchMaterialId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static (string encryptionName, string actualName) SavePhoto(int batchMaterialId, string sourceFilePath)
        {
            string actualName = Path.GetFileName(sourceFilePath);
            string encryptionName = EncryptionHelper.EncryptionFileName(sourceFilePath);
            string folder = EnsureInspectionPhotoFolder(batchMaterialId);
            File.Copy(sourceFilePath, Path.Combine(folder, encryptionName), true);
            return (encryptionName, actualName);
        }

        public static string GetPhotoPath(int batchMaterialId, dm_Attachment attachment)
        {
            if (attachment == null || string.IsNullOrWhiteSpace(attachment.EncryptionName))
            {
                return string.Empty;
            }

            return Path.Combine(EnsureInspectionPhotoFolder(batchMaterialId), attachment.EncryptionName);
        }

        public static void DeletePhotoFile(int batchMaterialId, dm_Attachment attachment)
        {
            string path = GetPhotoPath(batchMaterialId, attachment);
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void OpenPhotoFile(int batchMaterialId, dm_Attachment attachment)
        {
            if (attachment == null)
            {
                return;
            }

            Material309Helper.OpenPhotoFile(GetPhotoPath(batchMaterialId, attachment), attachment.ActualName);
        }
    }
}
