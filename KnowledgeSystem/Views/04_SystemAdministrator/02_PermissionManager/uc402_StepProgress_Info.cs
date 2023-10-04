using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class uc402_StepProgress_Info : DevExpress.XtraEditors.XtraUserControl
    {
        public List<dm_GroupProgressM> lsGroupProgress { get; set; }

        public uc402_StepProgress_Info()
        {
            InitializeComponent();
        }

        BindingSource sourceStep = new BindingSource();

        private void uc207_StepProgress_Info_Load(object sender, EventArgs e)
        {
            sourceStep.DataSource = lsGroupProgress;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var query = db.dm_Group.ToList();
                cbbGroup.DataSource = query;
                cbbGroup.ValueMember = "Id";
                cbbGroup.DisplayMember = "DisplayName";
                cbbGroup.Columns.AddRange(new[] { new LookUpColumnInfo { FieldName = "DisplayName", Caption = "名稱" } });
            }

            gcStep.DataSource = sourceStep;
        }

        private void btnNewStep_Click(object sender, EventArgs e)
        {
            if (lsGroupProgress == null)
            {
                lsGroupProgress = new List<dm_GroupProgressM>();
                lsGroupProgress.Add(new dm_GroupProgressM() { IndexStep = 1 });
                sourceStep.DataSource = lsGroupProgress;
            }
            else
            {
                lsGroupProgress.Add(new dm_GroupProgressM() { IndexStep = lsGroupProgress.Max(r => r.IndexStep + 1) });
            }

            gcStep.RefreshDataSource();
        }
    }
}
