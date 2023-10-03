using DataAccessLayer;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_SelectProgress : DevExpress.XtraEditors.XtraUserControl
    {
        public dm_Progress ProgressSelect { get; set; }

        public uc207_SelectProgress()
        {
            InitializeComponent();
        }

        private void uc207_SelectProgress_Load(object sender, EventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsProgress = db.dm_Progress.ToList();

                cbbProgress.Properties.DataSource = lsProgress;
                cbbProgress.Properties.ValueMember = "Id";
                cbbProgress.Properties.DisplayMember = "DisplayName";
                cbbProgress.Properties.Columns.AddRange(new[] { new LookUpColumnInfo { FieldName = "DisplayName", Caption = "名稱" } });

                cbbProgress.EditValue = lsProgress.FirstOrDefault().Id;
            }
        }

        private void cbbProgress_EditValueChanged(object sender, EventArgs e)
        {
            ProgressSelect = cbbProgress.GetSelectedDataRow() as dm_Progress;
        }
    }
}
