using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_ModeratorStaff
{
    public partial class f401_AppFormManage : DevExpress.XtraEditors.XtraForm
    {
        public f401_AppFormManage()
        {
            InitializeComponent();
        }

        List<AppForm> lsAppForms = new List<AppForm>();

        private void f401_AppFormManage_Load(object sender, EventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsAppForms = db.AppForms.Select(r => r).OrderBy(r => r.IndexRow).ToList();
            }

            tlAppForm.DataSource = lsAppForms;
            tlAppForm.ParentFieldName = "ParentId";
            tlAppForm.KeyFieldName = "Id";
        }
    }
}