using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System.Net.Mail;
using System.IO;

namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    public partial class uc308_ExportReport : DevExpress.XtraEditors.XtraUserControl
    {
        public uc308_ExportReport()
        {
            InitializeComponent();

            cbbYear.DataBindings.Add("Text", this, "Year", false, DataSourceUpdateMode.OnPropertyChanged);
            SickFiles = new List<string>();
        }

        public int Year { get; set; }
        public List<string> SickFiles { get; set; }

        private void uc308_ExportReport_Load(object sender, EventArgs e)
        {
            int currentYear = DateTime.Now.Year;

            // Thêm năm hiện tại và 5 năm trước
            cbbYear.Properties.Items.Clear();
            cbbYear.Properties.Items.AddRange(Enumerable.Range(currentYear - 5, 6).Reverse().ToArray());

            // Đặt năm mặc định là năm hiện tại
            cbbYear.SelectedItem = currentYear;
        }

        private void txbFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            switch (e.Button.Caption)
            {
                case "Paste":
                    if (Clipboard.ContainsFileDropList())
                    {
                        SickFiles = Clipboard.GetFileDropList()
                                                .Cast<string>()
                                                .Where(file => file.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) && File.Exists(file))
                                                .ToList();

                        if (!SickFiles.Any())
                        {
                            XtraMessageBox.Show("請選擇PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Hiển thị danh sách tên file (không có phần mở rộng)
                        txbFilePath.EditValue = string.Join("; ", SickFiles.Select(Path.GetFileNameWithoutExtension));
                    }
                    else
                    {
                        XtraMessageBox.Show("請選擇PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    break;

                default:
                    using (OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "PDF Files|*.pdf", Multiselect = true })
                    {
                        if (openFileDialog.ShowDialog() != DialogResult.OK)
                            return;

                        SickFiles = openFileDialog.FileNames.ToList();

                        // Lấy danh sách tên file mà không có phần mở rộng
                        txbFilePath.EditValue = string.Join("; ", SickFiles.Select(Path.GetFileNameWithoutExtension));
                    }
                    break;
            }
        }
    }
}
