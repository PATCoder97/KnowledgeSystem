using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
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

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension
{
    public partial class uc403_SoftManual_Info : DevExpress.XtraEditors.XtraUserControl
    {
        public uc403_SoftManual_Info(string softName = "")
        {
            InitializeComponent();
            SoftName = softName;

            txbSoftName.DataBindings.Add("Text", this, "SoftName", false, DataSourceUpdateMode.OnPropertyChanged);
            //txbSOP.DataBindings.Add("Text", this, "SOPName", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public string SoftName { get; set; }
        public string SOPName { get; set; }
        public string FilePath { get; set; }

        private void txbFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = TPConfigs.FilterFile,
                FilterIndex = 1
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;
            FilePath = dialog.FileName;
            SOPName = Path.GetFileName(FilePath);

            txbFilePath.Text = SOPName;
        }
    }
}
