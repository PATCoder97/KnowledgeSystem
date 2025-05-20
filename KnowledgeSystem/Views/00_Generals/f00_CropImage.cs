using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        int indexImage = 0;
        int countImage = 0;

        List<string> imagePaths = new List<string>();

        private void f00_CropImage_Load(object sender, EventArgs e)
        {
            cbbSize.Properties.Items.AddRange(Enum.GetValues(typeof(CropPictureBox.CropBoxSelectionInitialMode)));
            cbbSize.SelectedIndex = 0;

            if (imagePaths.Count() == 0)
                Close();

            countImage = imagePaths.Count();

            picImg.Image = Image.FromFile(imagePaths[0]);
            Text = Path.GetFileNameWithoutExtension(imagePaths[indexImage]);
            btnCrop.Text = $"裁剪({indexImage + 1}/{countImage})";
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            Image croppedImage = picImg.GetCroppedImage();

            if (croppedImage != null)
            {
                try
                {
                    // Lấy đường dẫn và tên file gốc
                    string sourceDir = System.IO.Path.GetDirectoryName(imagePaths[indexImage]);
                    string fileName = System.IO.Path.GetFileName(imagePaths[indexImage]);

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

                    if (indexImage + 1 == countImage)
                    {
                        XtraMessageBox.Show("所有圖片已裁剪完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                        return;
                    }

                    indexImage++;
                    picImg.Image = Image.FromFile(imagePaths[indexImage]);

                    Text = Path.GetFileNameWithoutExtension(imagePaths[indexImage]);
                    btnCrop.Text = $"裁剪({indexImage + 1}/{countImage})";

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

        private void cbbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            CropPictureBox.CropBoxSelectionInitialMode selectedStatus = (CropPictureBox.CropBoxSelectionInitialMode)cbbSize.SelectedItem;
            picImg.SelectionInitialMode = selectedStatus;
        }
    }
}
