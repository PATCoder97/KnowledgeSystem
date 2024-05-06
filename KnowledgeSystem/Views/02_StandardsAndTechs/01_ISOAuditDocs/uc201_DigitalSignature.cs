using BusinessLayer;
using DataAccessLayer;
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
    public partial class uc201_DigitalSignature : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_DigitalSignature()
        {
            InitializeComponent();
        }

        List<dt201_Forms> baseForm;
        List<dm_User> users;

        BindingSource sourceForm = new BindingSource();

        private void LoadData()
        {
            baseForm = dt201_FormsBUS.Instance.GetListProcessing();
            users = dm_UserBUS.Instance.GetList();

            var dataInfo = (from data in baseForm
                            join usr in users on data.UploadUser equals usr.Id
                            select new
                            {
                                data,
                                usr
                            }).ToList();

            sourceForm.DataSource = dataInfo;

            gvData.BestFitColumns();
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
        }

        private void uc201_DigitalSignature_Load(object sender, EventArgs e)
        {
            gcData.DataSource = sourceForm;
            LoadData();
        }
    }
}
