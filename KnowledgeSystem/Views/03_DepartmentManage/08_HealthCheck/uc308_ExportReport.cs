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
            txbFilePath.DataBindings.Add("EditValue", this, "SickFile", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public int Year { get; set; }
        public string SickFile { get; set; }

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
                        var files = Clipboard.GetFileDropList();
                        var pdfFiles = new List<string>();

                        foreach (var file in files)
                        {
                            if (file.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) && File.Exists(file))
                            {
                                pdfFiles.Add(file);
                            }
                        }

                        if (pdfFiles.Count != 1)
                        {
                            XtraMessageBox.Show("請選擇一個PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        txbFilePath.EditValue = pdfFiles.First();
                    }
                    else
                    {
                        XtraMessageBox.Show("請選擇一個PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    break;
                default:
                    OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Pdf|*.pdf" };

                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    txbFilePath.EditValue = openFileDialog.FileName;
                    break;
            }
        }
    }
}
