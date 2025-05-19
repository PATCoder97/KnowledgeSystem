using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_CropImage : DevExpress.XtraEditors.XtraForm
    {
        public f00_CropImage()
        {
            InitializeComponent();
            imagePaths = new List<string> { @"C:\Users\Dell Alpha\Desktop\RÁC 1\RAC\2766883200\00-NC-2-1.jpg" };
        }

        public f00_CropImage(string _imagePath)
        {
            InitializeComponent();
            imagePaths = new List<string> { _imagePath };
        }

        public f00_CropImage(List<string> _imagePaths)
        {
            InitializeComponent();
            imagePaths = _imagePaths;
        }

        List<string> imagePaths = new List<string>();

        private void f00_CropImage_Load(object sender, EventArgs e)
        {
            if (imagePaths.Count() == 0)
                Close();

            picImg.Image = Image.FromFile(imagePaths[0]);
            picImg.SelectionInitialMode = CropPictureBox.CropBoxSelectionInitialMode.Size2x1;
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            Image croppedImage = picImg.GetCroppedImage();

            if (croppedImage != null)
            {
                try
                {
                    // Lấy đường dẫn và tên file gốc
                    string sourceDir = System.IO.Path.GetDirectoryName(imagePaths[0]);
                    string fileName = System.IO.Path.GetFileName(imagePaths[0]);

                    // Tạo thư mục mới với hậu tố "-Crop"
                    string cropDir = System.IO.Path.Combine(sourceDir, System.IO.Path.GetFileName(sourceDir) + "-Crop");
                    if (!System.IO.Directory.Exists(cropDir))
                    {
                        System.IO.Directory.CreateDirectory(cropDir);
                    }

                    // Đường dẫn file mới
                    string newFilePath = System.IO.Path.Combine(cropDir, fileName);

                    // Lưu ảnh đã cắt vào file mới
                    croppedImage.Save(newFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                    // Hiển thị thông báo thành công
                    XtraMessageBox.Show("Ảnh đã được cắt và lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Thông báo nếu có lỗi trong quá trình lưu
                    XtraMessageBox.Show("Lỗi khi lưu ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("Không có vùng nào được chọn để cắt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
