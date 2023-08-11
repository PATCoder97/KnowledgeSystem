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
    public partial class uc207_DataStatistics : DevExpress.XtraEditors.XtraUserControl
    {
        public uc207_DataStatistics()
        {
            InitializeComponent();
        }

        #region methods

        private void LoadData()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsGrade = db.dm_Departments.Where(r => r.IdParent == -1).ToList();

                cbbGrade.Properties.DataSource = lsGrade;
                cbbGrade.Properties.DisplayMember = "DisplayName"; ;
                cbbGrade.Properties.ValueMember = "Id";
            }
        }

        #endregion

        private void uc207_DataStatistics_Load(object sender, EventArgs e)
        {
            LoadData();

            btnExcel.Text = "導出\r\nExcel";
            btnStatistics.Text = "資料\r\n統計";
        }

        private void cbbGrade_EditValueChanged(object sender, EventArgs e)
        {
            string idGrade = cbbGrade.EditValue.ToString();

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var idParent = db.dm_Departments.First(r => r.Id == idGrade).IdChild;

                var lsGrade = db.dm_Departments.Where(r => r.IdParent == idParent).ToList();

                cbbClass.Properties.DataSource = lsGrade;
                cbbClass.Properties.DisplayMember = "DisplayName"; ;
                cbbClass.Properties.ValueMember = "Id";
            }
        }
    }
}
