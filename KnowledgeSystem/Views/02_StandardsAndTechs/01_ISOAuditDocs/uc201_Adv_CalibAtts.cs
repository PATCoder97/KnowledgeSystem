using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class uc201_Adv_CalibAtts : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_Adv_CalibAtts()
        {
            InitializeComponent();

            string[] types = new string[] { "校正記錄表/Biên bản hiệu chuẩn", "校正證書/Giấy chứng nhận hiệu chuẩn", "委託單/Đơn uỷ thác", "遊校確認單/Bảng xác nhận hiệu chuẩn hiện trường" };
            cbbType.Properties.Items.AddRange(types);

            var watermarks = dm_WatermarkBUS.Instance.GetList();
            cbbWatermark.Properties.DataSource = watermarks;
            cbbWatermark.Properties.DisplayMember = "DisplayName";
            cbbWatermark.Properties.ValueMember = "ID";

            cbbType.DataBindings.Add("Text", this, "DisplayName", false, DataSourceUpdateMode.OnPropertyChanged);
            cbbWatermark.DataBindings.Add("EditValue", this, "IdWatermark", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public string DisplayName { get; set; }
        public int IdWatermark { get; set; }
    }
}
