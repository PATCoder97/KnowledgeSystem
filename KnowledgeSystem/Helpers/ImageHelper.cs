using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    class ImageHelper
    {
        public static string ConvertImageToBase64DataUri(string imagePath)
        {
            // Load the image
            using (Image image = Image.FromFile(imagePath))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Determine the image format and MIME type
                    ImageFormat imageFormat = GetImageFormat(image, out string mimeType);

                    // Save the image to the memory stream
                    image.Save(memoryStream, imageFormat);

                    // Convert byte array to Base64 string
                    string base64String = Convert.ToBase64String(memoryStream.ToArray());

                    // Construct the full data URI
                    string dataUri = $"data:{mimeType};base64,{base64String}";

                    return dataUri;
                }
            }
        }

        private static ImageFormat GetImageFormat(Image image, out string mimeType)
        {
            if (ImageFormat.Jpeg.Equals(image.RawFormat))
            {
                mimeType = "image/jpeg";
                return ImageFormat.Jpeg;
            }
            if (ImageFormat.Png.Equals(image.RawFormat))
            {
                mimeType = "image/png";
                return ImageFormat.Png;
            }
            if (ImageFormat.Gif.Equals(image.RawFormat))
            {
                mimeType = "image/gif";
                return ImageFormat.Gif;
            }
            if (ImageFormat.Bmp.Equals(image.RawFormat))
            {
                mimeType = "image/bmp";
                return ImageFormat.Bmp;
            }
            if (ImageFormat.Tiff.Equals(image.RawFormat))
            {
                mimeType = "image/tiff";
                return ImageFormat.Tiff;
            }
            if (ImageFormat.Icon.Equals(image.RawFormat))
            {
                mimeType = "image/x-icon";
                return ImageFormat.Icon;
            }

            // Default to PNG if format is unknown
            mimeType = "image/png";
            return ImageFormat.Png;
        }
    }
}
