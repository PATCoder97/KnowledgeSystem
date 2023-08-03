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

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_StepProgress : DevExpress.XtraEditors.XtraUserControl
    {
        public uc207_StepProgress()
        {
            InitializeComponent();
        }

        #region parameters

        BindingSource sourceProgress = new BindingSource();

        List<dm_Progress> lsProgresses = new List<dm_Progress>();

        #endregion

        #region methods

        private void LoadData()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsProgresses = db.dm_Progress.ToList();

                sourceProgress.DataSource = lsProgresses;
            }
        }

        #endregion

        private void uc207_StepProgress_Load(object sender, EventArgs e)
        {
            gcProgress.DataSource = sourceProgress;

            LoadData();

        }

        private void btnAddProgress_Click(object sender, EventArgs e)
        {
            lsProgresses.Add(new dm_Progress() { DisplayName = "New" });

            gcProgress.RefreshDataSource();
        }
    }
}
