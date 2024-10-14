using BusinessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_AuditDoc_Info : DevExpress.XtraEditors.XtraForm
    {
        public f201_AuditDoc_Info()
        {
            InitializeComponent();
        }

        public int idBase = -1;

        private void LoadData()
        {
            var formByBaseDatas = dt201_FormsBUS.Instance.GetListByIdBase(idBase);
            gcData.DataSource = formByBaseDatas;
            gvData.BestFitColumns();
        }

        private void f201_AuditDoc_Info_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
        }
    }
}