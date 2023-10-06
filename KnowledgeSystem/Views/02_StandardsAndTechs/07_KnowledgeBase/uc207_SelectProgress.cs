using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors.Controls;
using System;
using System.Linq;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_SelectProgress : DevExpress.XtraEditors.XtraUserControl
    {
        public dm_Progress ProgressSelect { get; set; }

        public uc207_SelectProgress()
        {
            InitializeComponent();
        }

        dm_ProgressBUS _dm_ProgressBUS = new dm_ProgressBUS();

        private void uc207_SelectProgress_Load(object sender, EventArgs e)
        {
            var lsProgress = _dm_ProgressBUS.GetList();

            cbbProgress.Properties.DataSource = lsProgress;
            cbbProgress.Properties.ValueMember = "Id";
            cbbProgress.Properties.DisplayMember = "DisplayName";
            cbbProgress.Properties.Columns.AddRange(new[] { new LookUpColumnInfo { FieldName = "DisplayName", Caption = "名稱" } });

            cbbProgress.EditValue = lsProgress.FirstOrDefault().Id;
        }

        private void cbbProgress_EditValueChanged(object sender, EventArgs e)
        {
            ProgressSelect = cbbProgress.GetSelectedDataRow() as dm_Progress;
        }
    }
}
