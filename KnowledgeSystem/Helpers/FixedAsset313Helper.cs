using DataAccessLayer;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace KnowledgeSystem.Helpers
{
    public static class FixedAsset313Helper
    {
        private const string AssetPhotoFolder = "AssetPhotos";
        private const string InspectionPhotoFolder = "InspectionPhotos";
        private const int MaxPhotoLength = 1600;
        private const long JpegQuality = 82L;
        private const string JpegExtension = ".jpg";
        private const int EncryptionNameMaxLength = 64;

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
            return SaveCompressedPhoto(EnsureAssetPhotoFolder(fixedAssetId), sourceFilePath);
        }

        public static (string encryptionName, string actualName) SaveInspectionPhoto(int batchAssetId, string sourceFilePath)
        {
            return SaveCompressedPhoto(EnsureInspectionPhotoFolder(batchAssetId), sourceFilePath);
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

        private static (string encryptionName, string actualName) SaveCompressedPhoto(string folder, string sourceFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("Unable to find the selected image file.", sourceFilePath);
            }

            Directory.CreateDirectory(folder);

            string actualName = BuildCompressedActualName(sourceFilePath);
            string encryptionName = CreateJpegEncryptionName();
            string destinationPath = Path.Combine(folder, encryptionName);

            using (var sourceImage = Image.FromFile(sourceFilePath))
            {
                Size targetSize = GetTargetSize(sourceImage.Width, sourceImage.Height);
                using (var bitmap = new Bitmap(targetSize.Width, targetSize.Height, PixelFormat.Format24bppRgb))
                {
                    if (sourceImage.HorizontalResolution > 0 && sourceImage.VerticalResolution > 0)
                    {
                        bitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                    }

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.Clear(Color.White);
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.DrawImage(sourceImage, new Rectangle(Point.Empty, targetSize));
                    }

                    SaveBitmapAsJpeg(bitmap, destinationPath);
                }
            }

            return (encryptionName, actualName);
        }

        private static string BuildCompressedActualName(string sourceFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "photo";
            }

            return $"{fileName}{JpegExtension}";
        }

        private static string CreateJpegEncryptionName()
        {
            int encryptedBaseLength = EncryptionNameMaxLength - JpegExtension.Length;
            return $"{EncryptionHelper.EncryptionFileName($"compressed{JpegExtension}", encryptedBaseLength)}{JpegExtension}";
        }

        private static Size GetTargetSize(int sourceWidth, int sourceHeight)
        {
            int longestSide = Math.Max(sourceWidth, sourceHeight);
            if (longestSide <= MaxPhotoLength)
            {
                return new Size(sourceWidth, sourceHeight);
            }

            double ratio = (double)MaxPhotoLength / longestSide;
            int targetWidth = Math.Max(1, (int)Math.Round(sourceWidth * ratio));
            int targetHeight = Math.Max(1, (int)Math.Round(sourceHeight * ratio));
            return new Size(targetWidth, targetHeight);
        }

        private static void SaveBitmapAsJpeg(Image image, string destinationPath)
        {
            ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

            if (jpegCodec == null)
            {
                throw new InvalidOperationException("JPEG encoder is not available on this machine.");
            }

            using (var encoderParameters = new EncoderParameters(1))
            {
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, JpegQuality);
                image.Save(destinationPath, jpegCodec, encoderParameters);
            }
        }
    }
}
